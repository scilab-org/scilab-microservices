# Unit Test Report

## 1. General Information

| Field | Value |
|-------|-------|
| className | GetUserByIdQueryHandler |
| functionName | Handle |
| testClass | GetUserByIdQueryHandlerTests |
| feature | user |

---

## 2. Test Case List

| UTCID | Description | Type |
|-------|-------------|------|
| UTCID01 | User exists — return result with correct user data | N |
| UTCID02 | User not found — propagate KeyNotFoundException | A |
| UTCID03 | Valid userId — KeycloakService called exactly once with correct id | N |

---

## 3. Header Information

| Function Code | | Function Name | Handle |
|---------------|---|---------------|--------|
| Created By | | Executed By | |
| Lines of code | | Lack of test cases | |
| Test requirement | GetUserByIdQueryHandler must retrieve user from Keycloak by ID, return the result, and propagate exceptions when user is not found. | | |

| Passed | Failed | Untested | N/A/B | Total Test Cases |
|--------|--------|----------|-------|------------------|
| 3 | 0 | 0 | N:2 B:0 A:1 | 3 |

---

## 4. Decision Table Matrix

### CONDITION Section

| Condition | Value | UTCID01 | UTCID02 | UTCID03 |
|-----------|-------|---------|---------|---------|
| **Precondition** | | | | |
| | Can connect server | O | O | O |
| **userId** | | | | |
| | existing user id | O | | O |
| | non-existent user id | | O | |
| **keycloakService.GetUserByIdAsync** | | | | |
| | returns UserDto | O | | O |
| | throws KeyNotFoundException | | O | |

### CONFIRM Section

| Confirm | Value | UTCID01 | UTCID02 | UTCID03 |
|---------|-------|---------|---------|---------|
| **Return** | | | | |
| | result.User with correct Id, Username, Email | O | | |
| | result (not checked) | | | O |
| **keycloakService.GetUserByIdAsync calls** | | | | |
| | Times.Once | | | O |
| **Exception** | | | | |
| | none | O | | O |
| | KeyNotFoundException propagated | | O | |

### RESULT Section

| Result Field | UTCID01 | UTCID02 | UTCID03 |
|--------------|---------|---------|---------|
| **Type (N/B/A)** | N | A | N |
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
