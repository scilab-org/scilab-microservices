# Add `users` KPI to Admin Dashboard Endpoint

## Context

The frontend Admin Dashboard displays a KPI grid with 5 indicators: **Projects**, **Paper Bank**, **Journals & Conferences**, **Templates**, and **Users**. The first four are already returned by the backend. The **Users** indicator is currently missing from the API response, so it always shows `0`.

---

## Endpoint

```
GET /management-service/admin/dashboard
```

> Requires `admin` role — already enforced by the existing route guard.

---

## Required Change

Add a `users` object inside the `kpis` field of the response.

### Before

```json
{
  "kpis": {
    "projects": { "total": 12, "byStatus": { "active": 5, "draft": 3, "completed": 3, "archived": 1 } },
    "submissionStatus": { "counts": { "draft": 8, "submitted": 4, "accepted": 2 } },
    "paperBank": { "total": 30 },
    "journals": { "total": 7, "journalCount": 4, "conferenceCount": 3 },
    "templates": { "total": 5 }
  },
  "recentProjects": [...],
  "recentPapers": [...]
}
```

### After

```json
{
  "kpis": {
    "projects": { "total": 12, "byStatus": { "active": 5, "draft": 3, "completed": 3, "archived": 1 } },
    "submissionStatus": { "counts": { "draft": 8, "submitted": 4, "accepted": 2 } },
    "paperBank": { "total": 30 },
    "journals": { "total": 7, "journalCount": 4, "conferenceCount": 3 },
    "templates": { "total": 5 },
    "users": { "total": 42 }
  },
  "recentProjects": [...],
  "recentPapers": [...]
}
```

---

## Implementation

### C# DTO changes

```csharp
public class AdminKpisResult
{
    public AdminProjectKpisResult Projects { get; set; }
    public AdminSubmissionStatusKpisResult SubmissionStatus { get; set; }
    public AdminPaperBankKpisResult PaperBank { get; set; }
    public AdminJournalKpisResult Journals { get; set; }
    public AdminTemplateKpisResult Templates { get; set; }

    // Add this:
    public AdminUserKpisResult Users { get; set; }
}

public class AdminUserKpisResult
{
    public int Total { get; set; }
}
```

### Data source

| Scenario                                                   | How to get `total`                                                                                  |
| ---------------------------------------------------------- | --------------------------------------------------------------------------------------------------- |
| Users stored in a local `Users` / `ApplicationUsers` table | `SELECT COUNT(*) FROM Users` (or `_dbContext.Users.CountAsync()`)                                   |
| Users managed in **Keycloak**                              | Call Keycloak Admin REST API: `GET /admin/realms/{realm}/users/count` using a service account token |

If calling Keycloak, use the existing `IKeycloakAdminClient` (or equivalent HTTP client) already wired up in the service. Example:

```csharp
var userCount = await _keycloakAdminClient.GetUserCountAsync(cancellationToken);
```

### Query handler change (example)

```csharp
// Inside GetAdminDashboardQueryHandler

var userTotal = await _dbContext.Users.CountAsync(cancellationToken);
// or: var userTotal = await _keycloakAdminClient.GetUserCountAsync(cancellationToken);

var kpis = new AdminKpisResult
{
    Projects = ...,
    SubmissionStatus = ...,
    PaperBank = ...,
    Journals = ...,
    Templates = ...,
    Users = new AdminUserKpisResult { Total = userTotal },
};
```

---

## Frontend Contract (for reference)

The frontend TypeScript type expecting this field:

```typescript
export type AdminUserKpis = {
  total: number;
};

export type AdminKpis = {
  projects: AdminProjectKpis;
  submissionStatus: AdminSubmissionStatusKpis;
  paperBank: AdminPaperBankKpis;
  journals: AdminJournalKpis;
  templates: AdminTemplateKpis;
  users?: AdminUserKpis; // optional until backend ships this
};
```

The field is marked optional (`users?`) so the frontend degrades gracefully (shows `0`) until this is deployed.
