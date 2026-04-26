# Unit Test Report

## 1. General Information

| Field | Value |
|-------|-------|
| className | CreateUserCommandHandler |
| functionName | Handle |
| testClass | CreateUserCommandHandlerTests |
| feature | user |

---

## 2. Test Case List

| UTCID | Description | Type |
|-------|-------------|------|
| UTCID01 | No avatar provided — use default avatar, skip upload | N |
| UTCID02 | Avatar provided — upload avatar and use returned URL | N |
| UTCID03 | Avatar upload fails — fall back to default avatar | A |

---

## 3. Header Information

| Function Code | | Function Name | Handle |
|---------------|---|---------------|--------|
| Created By | | Executed By | |
| Lines of code | | Lack of test cases | |
| Test requirement | CreateUserCommandHandler must create a Keycloak user with avatar URL. If no avatar provided, use default. If upload fails, fall back to default. | | |

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
| **avatarImage** | | | | |
| | null | O | | |
| | provided (bytes) | | O | O |
| **minIoCloudService.UploadFileAsync** | | | | |
| | not called | O | | |
| | returns PublicURL | | O | |
| | throws InvalidOperationException | | | O |

### CONFIRM Section

| Confirm | Value | UTCID01 | UTCID02 | UTCID03 |
|---------|-------|---------|---------|---------|
| **Return** | | | | |
| | keycloakUserId (string) | O | O | O |
| **avatarUrl passed to Keycloak** | | | | |
| | AppConstants.Bucket.DefaultAvatar | O | | O |
| | uploaded PublicURL | | O | |
| **minIoCloudService.UploadFileAsync calls** | | | | |
| | Times.Never | O | | |
| | Times.Once | | O | |
| **Exception** | | | | |
| | none | O | O | O |

### RESULT Section

| Result Field | UTCID01 | UTCID02 | UTCID03 |
|--------------|---------|---------|---------|
| **Type (N/B/A)** | N | N | A |
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
