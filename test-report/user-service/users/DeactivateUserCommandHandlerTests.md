# Unit Test Report

## 1. General Information

| Field | Value |
|-------|-------|
| className | DeactivateUserCommandHandler |
| functionName | Handle |
| testClass | DeactivateUserCommandHandlerTests |
| feature | user |

---

## 2. Test Case List

| UTCID | Description | Type |
|-------|-------------|------|
| UTCID01 | Keycloak deactivates user successfully — return true | N |
| UTCID02 | Keycloak throws exception — propagate exception | A |

---

## 3. Header Information

| Function Code | | Function Name | Handle |
|---------------|---|---------------|--------|
| Created By | | Executed By | |
| Lines of code | | Lack of test cases | |
| Test requirement | DeactivateUserCommandHandler must call KeycloakService.DeactivateUserAsync and return true on success, or propagate exceptions on failure. | | |

| Passed | Failed | Untested | N/A/B | Total Test Cases |
|--------|--------|----------|-------|------------------|
| 2 | 0 | 0 | N:1 B:0 A:1 | 2 |

---

## 4. Decision Table Matrix

### CONDITION Section

| Condition | Value | UTCID01 | UTCID02 |
|-----------|-------|---------|---------|
| **Precondition** | | | |
| | Can connect server | O | O |
| **userId** | | | |
| | valid non-empty string | O | O |
| **keycloakService.DeactivateUserAsync** | | | |
| | completes successfully | O | |
| | throws InvalidOperationException | | O |

### CONFIRM Section

| Confirm | Value | UTCID01 | UTCID02 |
|---------|-------|---------|---------|
| **Return** | | | |
| | true | O | |
| **keycloakService.DeactivateUserAsync calls** | | | |
| | Times.Once | O | |
| **Exception** | | | |
| | none | O | |
| | InvalidOperationException propagated | | O |

### RESULT Section

| Result Field | UTCID01 | UTCID02 |
|--------------|---------|---------|
| **Type (N/B/A)** | N | A |
| **Pass/Fail (P/F)** | P | P |
| **Executed Date** | 2026-04-26 | 2026-04-26 |
| **Defect ID** | - | - |

### Summary

| Field | Value |
|-------|-------|
| Passed | 2 |
| Failed | 0 |
| Untested | 0 |
| Total Test Cases | 2 |
