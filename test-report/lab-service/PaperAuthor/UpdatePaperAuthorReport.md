# Unit Test Report for UpdatePaperAuthor

# 1. General Information

| Field | Value |
|------|------|
| className | UpdatePaperAuthorCommandHandler |
| functionName | UpdatePaperAuthor |
| testClass | PaperAuthorCommandsIntegrationTests |
| feature | PaperAuthor |

# 2. Test Case List

| UTCID | Description | Type |
|------|-------------|------|
| UTCID01 | WhenRoleIsNull -> ShouldThrowUnauthorizedException | A |
| UTCID02 | WhenRoleIsNotPaperAuthor -> ShouldThrowUnauthorizedException | A |
| UTCID03 | WithNonExistentId -> ShouldThrowNotFoundException | A |
| UTCID04 | WithValidData -> ShouldUpdateFieldsAndReturnUnit | N |

# 3. Header Information

| Function Code | | Function Name | UpdatePaperAuthor |
|---------------|---|---------------|-----------------|
| Created By | Auto-Generator | Executed By | Auto-Generator |
| Lines of code | N/A | Lack of test cases | N/A |
| Test requirement | Auto-generated from PaperAuthorCommandsIntegrationTests | | |

| Passed | Failed | Untested | N/A/B | Total Test Cases |
|--------|--------|----------|-------|------------------|
| 4 | 0 | 0 | 1 0 3 | 4 |

# 4. Decision Table Matrix

### CONDITION Section

| Condition | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|-------|---|---|---|---|
| **Input** | |  |  |  |  |
| | WhenRoleIsNull | O |   |   |   |
| | WhenRoleIsNotPaperAuthor |   | O |   |   |
| | WithNonExistentId |   |   | O |   |
| | WithValidData |   |   |   | O |

### CONFIRM Section

| Confirm | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|---------|-------|---|---|---|---|
| **Return/Exception** | |  |  |  |  |
| | ShouldThrowUnauthorizedException | O |   |   |   |
| | ShouldThrowUnauthorizedException |   | O |   |   |
| | ShouldThrowNotFoundException |   |   | O |   |
| | ShouldUpdateFieldsAndReturnUnit |   |   |   | O |

### RESULT Section

| Result Field | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|--------------|---|---|---|---|
| **Type (N/B/A)** | A | A | A | N |
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
