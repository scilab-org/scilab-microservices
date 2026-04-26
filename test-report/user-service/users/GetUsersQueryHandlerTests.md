# Unit Test Report

## 1. General Information

| Field | Value |
|-------|-------|
| className | GetUsersQueryHandler |
| functionName | Handle |
| testClass | GetUsersQueryHandlerTests |
| feature | user |

---

## 2. Test Case List

| UTCID | Description | Type |
|-------|-------------|------|
| UTCID01 | Users exist — return paginated list with correct count | N |
| UTCID02 | No users match filter — return empty list | N |
| UTCID03 | Query with all filter parameters — forward all params to Keycloak exactly | N |

---

## 3. Header Information

| Function Code | | Function Name | Handle |
|---------------|---|---------------|--------|
| Created By | | Executed By | |
| Lines of code | | Lack of test cases | |
| Test requirement | GetUsersQueryHandler must retrieve users from Keycloak with pagination and filters, return paginated result. All filter and exclude parameters must be forwarded correctly. | | |

| Passed | Failed | Untested | N/A/B | Total Test Cases |
|--------|--------|----------|-------|------------------|
| 3 | 0 | 0 | N:3 B:0 A:0 | 3 |

---

## 4. Decision Table Matrix

### CONDITION Section

| Condition | Value | UTCID01 | UTCID02 | UTCID03 |
|-----------|-------|---------|---------|---------|
| **Precondition** | | | | |
| | Can connect server | O | O | O |
| **filter.SearchText** | | | | |
| | null | O | | |
| | "nonexistent" | | O | |
| | "john" | | | O |
| **filter.GroupName** | | | | |
| | null | O | O | |
| | "Researchers" | | | O |
| **filter.Enabled** | | | | |
| | null | O | O | |
| | true | | | O |
| **paging** | | | | |
| | PageNumber:1 PageSize:10 | O | O | |
| | PageNumber:2 PageSize:5 | | | O |
| **excludeUserId / excludeAdminGroup** | | | | |
| | both null | O | O | |
| | both provided | | | O |
| **keycloakService.GetUsersAsync result** | | | | |
| | 5 users returned | O | | |
| | 0 users returned | | O | O |

### CONFIRM Section

| Confirm | Value | UTCID01 | UTCID02 | UTCID03 |
|---------|-------|---------|---------|---------|
| **Return** | | | | |
| | result.Items count = 5 | O | | |
| | result.Items empty | | O | |
| | keycloakService called Times.Once with exact params | | | O |
| **result.Paging** | | | | |
| | not null | O | | |
| **Exception** | | | | |
| | none | O | O | O |

### RESULT Section

| Result Field | UTCID01 | UTCID02 | UTCID03 |
|--------------|---------|---------|---------|
| **Type (N/B/A)** | N | N | N |
| **Pass/Fail (P/F)** | P | P | P |
| **Executed Date** | 2026-04-26 | 2026-04-26 | 2026-04-26 |
| **Defect ID** | - | - | - |

### Summary

| Field | Value |
|-------|-------|
| Passed | 3 |
| Failed | 0 |
| Untested | 0 |
| Total Test Cases | 3 |
