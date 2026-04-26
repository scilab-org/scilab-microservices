# Unit Test Report for CreateAuthorRole

# 1. General Information

| Field | Value |
|------|------|
| className | CreateAuthorRoleCommandHandler |
| functionName | CreateAuthorRole |
| testClass | AuthorRoleIntegrationTests |
| feature | AuthorRole |

# 2. Test Case List

| UTCID | Description | Type |
|------|-------------|------|
| UTCID01 | WithValidData -> ShouldStoreAndReturnId | N |
| UTCID02 | ShouldTrimName -> Default | N |
| UTCID03 | WithDuplicateName -> ShouldThrowClientValidationException | A |
| UTCID04 | WithDuplicateNameDifferentCase -> ShouldThrowClientValidationException | A |

# 3. Header Information

| Function Code | | Function Name | CreateAuthorRole |
|---------------|---|---------------|-----------------|
| Created By | Auto-Generator | Executed By | Auto-Generator |
| Lines of code | N/A | Lack of test cases | N/A |
| Test requirement | Auto-generated from AuthorRoleIntegrationTests | | |

| Passed | Failed | Untested | N/A/B | Total Test Cases |
|--------|--------|----------|-------|------------------|
| 4 | 0 | 0 | 2 0 2 | 4 |

# 4. Decision Table Matrix

### CONDITION Section

| Condition | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|-------|---|---|---|---|
| **Input** | |  |  |  |  |
| | WithValidData | O |   |   |   |
| | ShouldTrimName |   | O |   |   |
| | WithDuplicateName |   |   | O |   |
| | WithDuplicateNameDifferentCase |   |   |   | O |

### CONFIRM Section

| Confirm | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|---------|-------|---|---|---|---|
| **Return/Exception** | |  |  |  |  |
| | ShouldStoreAndReturnId | O |   |   |   |
| | Default |   | O |   |   |
| | ShouldThrowClientValidationException |   |   | O |   |
| | ShouldThrowClientValidationException |   |   |   | O |

### RESULT Section

| Result Field | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|--------------|---|---|---|---|
| **Type (N/B/A)** | N | N | A | A |
| **Pass/Fail (P/F)** | P | P | P | P |
| **Executed Date** | 2026-04-26 | 2026-04-26 | 2026-04-26 | 2026-04-26 |
| **Defect ID** | - | - | - | - |

### Summary

| Field | Value |
|------|------|
| Passed | 4 |
| Failed | 0 |
| Untested | 0 |
| Total Test Cases | 4 |
