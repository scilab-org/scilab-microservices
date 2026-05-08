# Dashboard Data Layer Design

> Analyzed codebase: `src/Services/Lab`, `src/Services/Management`, `src/Services/User`  
> Date: 2026-05-05

---

## A. Domain Summary

### Entity Map

| Entity                     | Service    | Key Fields                                                                                  | Notes                                                                               |
| -------------------------- | ---------- | ------------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------- |
| `ProjectEntity`            | Management | `Status`, `ParentProjectId`, `PaperIds`, `ConferenceJournalIds`, `DomainIds`                | `ParentProjectId == null` = top-level project; sub-projects link to papers          |
| `MemberEntity`             | Management | `UserId`, `ProjectId`, `ProjectRole`                                                        | Joins user to a project; `ProjectRole` is a string (e.g. `project:project-manager`) |
| `PaperEntity`              | Lab        | `Status (PaperStatus)`, `ConferenceJournalId`, `ConferenceJournalStartAt/EndAt`             | Internal working paper                                                              |
| `PaperStatusHistoryEntity` | Lab        | `PaperId`, `Status (SubmissionStatus)`, `ActorId`                                           | Append-only audit trail; latest record = current submission state                   |
| `PaperBankEntity`          | Lab        | `IsIngested`, `IngestStatus`, `ConferenceJournalId`                                         | Reference paper library, separate from authored papers                              |
| `ConferenceJournalEntity`  | Lab        | `Type (Journal/Conference)`, `Ranking`, `PaperIds`, `TemplateIds`                           | Target venues                                                                       |
| `TemplateEntity`           | Lab        | `Code`, `Sections`                                                                          | Paper writing templates                                                             |
| `TaskEntity`               | Lab        | `Status (TaskDefineStatus)`, `TaskType`, `MemberId`, `AssignedToUserName`, `NextReviewDate` | Work items per contributor                                                          |
| `PaperContributorEntity`   | Lab        | `PaperId`, `MemberId`, `SectionRole`, `TaskIds`                                             | Links member to a paper's sections                                                  |
| `PaperAuthorEntity`        | Lab        | `PaperId`, `MemberId`, `AuthorRoleId`, `AffiliationId`                                      | Formal authorship record                                                            |

### Role Hierarchy

```
system:admin            → full system visibility
project:project-manager → project-scoped management
project:author          → papers they co-author
project:member          → papers they contribute to (section-level)
app:user                → baseline authenticated user
```

`AuthorizeConstants` is the source of truth. The dashboard distinguishes **Admin** (`system:admin`) from **User** (everyone else).

### Cross-Service Dependencies

Management service has `ILabApiService` (calls Lab API). Lab service has `IManagementApiService`. Dashboard aggregation is **owned by Management** since it is the topological center (projects, members, submission summaries).

---

## B. Dashboard Data Requirements

### Admin KPIs

| KPI                                  | Source                                              | Computed?                | Cache TTL |
| ------------------------------------ | --------------------------------------------------- | ------------------------ | --------- |
| Projects by `ProjectStatus`          | Management · `ProjectEntity`                        | Group-by count           | 5 min     |
| Papers by current `SubmissionStatus` | Lab · `PaperStatusHistoryEntity` (latest-per-paper) | Aggregated               | 5 min     |
| PaperBank total                      | Lab · `PaperBankEntity`                             | Count                    | 5 min     |
| Journals by `ConferenceJournalType`  | Lab · `ConferenceJournalEntity`                     | Group-by count           | 10 min    |
| Template count                       | Lab · `TemplateEntity`                              | Count                    | 10 min    |
| 5 most recent projects               | Management · `ProjectEntity`                        | Raw, `CreatedOnUtc DESC` | No cache  |
| 5 most recent papers                 | Lab · `PaperEntity`                                 | Raw, `CreatedOnUtc DESC` | No cache  |

### User KPIs

| KPI                                     | Source                                                      | Computed?                       | Cache TTL        |
| --------------------------------------- | ----------------------------------------------------------- | ------------------------------- | ---------------- |
| My active projects                      | Management · `MemberEntity` + `ProjectEntity`               | Join + filter `Status = Active` | 2 min (per user) |
| My tasks by `TaskDefineStatus`          | Lab · `TaskEntity` (by `AssignedToUserName`)                | Group-by count                  | 1 min (per user) |
| My papers by current `SubmissionStatus` | Lab · `PaperContributorEntity` → `PaperStatusHistoryEntity` | Resolved via membership         | 2 min (per user) |
| My 5 most recent tasks                  | Lab · `TaskEntity`                                          | Raw, `LastModifiedOnUtc DESC`   | No cache         |
| My 5 most recent papers                 | Lab via `PaperContributorEntity`                            | Raw, `CreatedOnUtc DESC`        | No cache         |

---

## C. API Design

### Endpoint

```
GET /dashboard
Authorization: Bearer <token>
```

A single endpoint in **Management.Api**. The handler reads the caller's group from the JWT, branches into admin or user path, and returns a discriminated response.

**Implementation path:**

```
Management.Api → GET /dashboard
  → GetDashboardQuery (Management.Application.Features.Dashboard)
      → IDistributedCache (Redis)
      → IDocumentSession (Marten)
      → ILabApiService (cross-service HTTP)
```

---

### Admin Response (`role = system:admin`)

```json
{
  "role": "admin",
  "kpis": {
    "projects": {
      "total": 28,
      "byStatus": {
        "draft": 4,
        "active": 16,
        "completed": 6,
        "archived": 2
      }
    },
    "submissionStatus": {
      "draft": 35,
      "submitted": 28,
      "revisionRequired": 12,
      "resubmitted": 6,
      "accepted": 22,
      "published": 27,
      "rejected": 3,
      "onHold": 1
    },
    "paperBank": {
      "total": 512
    },
    "journals": {
      "total": 38,
      "byType": {
        "journal": 22,
        "conference": 16
      }
    },
    "templates": {
      "total": 14
    }
  },
  "recentProjects": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "name": "AI Safety 2025",
      "code": "AIS-25",
      "status": 2,
      "createdAt": "2025-04-20T09:00:00Z"
    }
  ],
  "recentPapers": [
    {
      "id": "7b4e3f12-1a2b-4c5d-8e9f-0a1b2c3d4e5f",
      "title": "Transformer Efficiency Survey",
      "status": 2,
      "conferenceJournalName": "NeurIPS 2025",
      "conferenceJournalType": 2,
      "createdAt": "2025-04-28T14:00:00Z"
    }
  ]
}
```

**Field notes:**

- `submissionStatus` maps to `SubmissionStatus` enum: `Draft=1` … `OnHold=8`
- `paperBank.total` — raw count of all `PaperBankEntity` records
- `journals.byType` maps to `ConferenceJournalType` enum: `Journal=1`, `Conference=2`
- `recentProjects` — 5 items max, top-level only (`ParentProjectId == null`)
- `recentPapers` — 5 items max; no large text fields (title + status only)

---

### User Response (`role ≠ system:admin`)

```json
{
  "role": "user",
  "kpis": {
    "myProjects": {
      "total": 3,
      "active": 2
    },
    "myTasks": {
      "total": 15,
      "byStatus": {
        "todo": 4,
        "inProgress": 5,
        "inReview": 3,
        "completed": 2,
        "closed": 1
      }
    },
    "myPapers": {
      "total": 6,
      "bySubmissionStatus": {
        "draft": 1,
        "submitted": 2,
        "revisionRequired": 1,
        "accepted": 1,
        "published": 1
      }
    }
  },
  "myRecentTasks": [
    {
      "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
      "name": "Write Section 3: Methodology",
      "taskType": 2,
      "status": 2,
      "paperId": "7b4e3f12-1a2b-4c5d-8e9f-0a1b2c3d4e5f",
      "paperTitle": "Transformer Efficiency Survey",
      "nextReviewDate": "2025-05-10T00:00:00Z",
      "lastModifiedAt": "2025-05-04T18:30:00Z"
    }
  ],
  "myRecentPapers": [
    {
      "id": "7b4e3f12-1a2b-4c5d-8e9f-0a1b2c3d4e5f",
      "title": "Transformer Efficiency Survey",
      "paperStatus": 2,
      "submissionStatus": 2,
      "conferenceJournalName": "NeurIPS 2025",
      "conferenceJournalEndAt": "2025-05-31T23:59:59Z",
      "lastModifiedAt": "2025-05-04T18:30:00Z"
    }
  ]
}
```

**Field notes:**

- `myPapers` is the union of `PaperContributorEntity` (contributor) and `PaperAuthorEntity` (author), deduplicated by `PaperId`
- `submissionStatus` per paper = latest `PaperStatusHistoryEntity` record; papers with no history = `Draft=1`
- `myRecentTasks` — 5 items, `LastModifiedOnUtc DESC`
- `myRecentPapers` — 5 items, `LastModifiedOnUtc DESC`

---

## D. Role-Based Differences

| Field                         | Admin | User |
| ----------------------------- | ----- | ---- |
| System-wide project counts    | ✅    | ❌   |
| PaperBank total               | ✅    | ❌   |
| Journal / template totals     | ✅    | ❌   |
| Recent projects (all)         | ✅    | ❌   |
| Recent papers (all)           | ✅    | ❌   |
| My project counts             | ❌    | ✅   |
| My task breakdown             | ❌    | ✅   |
| My papers + submission status | ❌    | ✅   |
| My recent tasks               | ❌    | ✅   |

Admin never sees `parsedText`, `abstract`, `researchGap`, or other large text fields in dashboard responses — only identifiers, status values, and titles. Users never see other users' data.

> If an admin is also a project member, they always receive the admin-shaped response. Use project-scoped endpoints for member-level data.

---

## E. Performance & Caching Strategy

### Redis Cache Keys

| Key                            | TTL    | Content                   |
| ------------------------------ | ------ | ------------------------- |
| `dashboard:admin:kpis`         | 5 min  | Full admin KPI block      |
| `dashboard:admin:journals`     | 10 min | Journal + template counts |
| `dashboard:user:{userId}:kpis` | 2 min  | User task + paper counts  |

`recentProjects`, `recentPapers`, `myRecentTasks`, and `myRecentPapers` are **not cached** — they signal current activity.

### Admin Query Strategy

Management queries its own DB for project KPIs, then calls `ILabApiService.GetAdminKpisAsync()` which hits a single Lab endpoint.

```
Management side (sequential):
  1. session.Query<ProjectEntity>() × 4 status counts + top-5 recent projects

Lab side — GET /admin/dashboard/kpis (single call from Management):
  1. PaperBankEntity.CountAsync()
  2. ConferenceJournalEntity filtered by Journal type .CountAsync()
  3. ConferenceJournalEntity filtered by Conference type .CountAsync()
  4. TemplateEntity.CountAsync()
  5. PaperStatusHistoryEntity ordered DESC → latest-per-paper group-by status in memory
  6. PaperEntity.OrderByDescending(CreatedOnUtc).Take(5) → recentPapers
```

### User Query Strategy

```
1. Load MemberEntity[] for userId                       → resolve projectIds + memberIds
2. [parallel] Count active projects (MemberEntity data + one DB status filter)
3. [parallel] Load TaskEntity[] where AssignedToUserName = username → count by TaskDefineStatus + top-5 recent
4. [parallel] Load PaperContributorEntity[] + PaperAuthorEntity[] where MemberId IN memberIds → collect paperIds
5. Load latest PaperStatusHistoryEntity per paperIds    → existing summary logic + top-5 recent papers
```

Steps 2, 3, 4 can fire in parallel after username and memberIds are resolved.

### Avoid N+1

- Never query paper status inside a loop — batch by `PaperId IN (...)`
- Never resolve username per-task — resolve once from JWT `sub` (already done in `GetMyTaskQueryHandler`)
- Reuse the single-pass pattern in `GetSubmissionStatusSummaryQueryHandler`

---

## F. Risks & Edge Cases

**1. Papers with no `PaperStatusHistoryEntity` records**  
Implicitly `SubmissionStatus.Draft`. The existing `GetSubmissionStatusSummaryQueryHandler` already handles this (counts unmapped paper IDs as Draft). Dashboard query must replicate this logic.

**2. User belongs to many projects / large `memberIds`**  
`TaskEntity` is filtered by `AssignedToUserName` (string), so task loading is independent of member list size. Paper contributor lookups use `MemberId IN (...)` via Marten/PostgreSQL JSON `ANY`. Monitor if `memberIds` grows beyond ~100.

**3. Cross-service latency for Admin KPIs**  
Admin KPI block requires 3–4 HTTP calls to Lab. Without Redis cache, each load incurs those round-trips. Cache the aggregate result at the Management layer.

**4. `PaperStatus` vs `SubmissionStatus` confusion**  
These are two distinct axes:

- `PaperStatus` = internal workflow (Draft → Processing → Submitted → Released → Sampled)
- `SubmissionStatus` = external journal/conference response (Submitted → RevisionRequired → Accepted → Published …)

Admin KPIs expose both. User dashboard exposes only `SubmissionStatus` in KPIs (more actionable), with `PaperStatus` surfaced in `myRecentPapers`.

**5. Admin is also a project member**  
Always returns admin-shaped response. Intentional — admin uses project-scoped endpoints for member-level drill-down.

**6. Submission status summary over all papers (admin)**  
Loading all paper IDs into memory to pass to the existing summary endpoint is unsafe at scale. A dedicated `GET /papers/submission-status-counts` Lab endpoint (no ID list required, full-table group-by) is required before enabling the admin `submissionStatus` KPI block.

---

## G. Suggested Background Jobs

### `DashboardAdminKpiJob`

- **Schedule:** every 5 minutes
- **Action:** precompute the full admin KPI block and write to `dashboard:admin:kpis` in Redis
- **Benefit:** eliminates cold-cache latency on first admin load after TTL expiry
