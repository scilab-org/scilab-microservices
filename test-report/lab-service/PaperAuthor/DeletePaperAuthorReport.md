# Unit Test Report for DeletePaperAuthor

# 1. General Information

| Field | Value |
|------|------|
| className | DeletePaperAuthorCommandHandler |
| functionName | DeletePaperAuthor |
| testClass | PaperAuthorIntegrationTests |
| feature | PaperAuthor |

# 2. Test Case List

| UTCID | Description | Type |
|------|-------------|------|
| UTCID01 | WithExisting -> ShouldRemoveFromStore | N |
| UTCID02 | WithNonExistentId -> ShouldThrowNotFoundException | A |

# 3. Header Information

| Function Code | | Function Name | DeletePaperAuthor |
|---------------|---|---------------|-----------------|
| Created By | Auto-Generator | Executed By | Auto-Generator |
| Lines of code | N/A | Lack of test cases | N/A |
| Test requirement | Auto-generated from PaperAuthorIntegrationTests | | |

| Passed | Failed | Untested | N/A/B | Total Test Cases |
|--------|--------|----------|-------|------------------|
| 2 | 0 | 0 | 1 0 1 | 2 |

# 4. Decision Table Matrix

### CONDITION Section

| Condition | Value | UTCID01 | UTCID02 |
|-----------|-------|---|---|
| **Input** | |  |  |
| | WithExisting | O |   |
| | WithNonExistentId |   | O |

### CONFIRM Section

| Confirm | Value | UTCID01 | UTCID02 |
|---------|-------|---|---|
| **Return/Exception** | |  |  |
| | ShouldRemoveFromStore | O |   |
| | ShouldThrowNotFoundException |   | O |

### RESULT Section

| Result Field | UTCID01 | UTCID02 |
|--------------|---|---|
| **Type (N/B/A)** | N | A |
| **Pass/Fail (P/F)** | P | P |
| **Executed Date** | 2026-04-26 | 2026-04-26 |
| **Defect ID** | - | - |

### Summary

| Field | Value |
|------|------|
| Passed | 2 |
| Failed | 0 |
| Untested | 0 |
| Total Test Cases | 2 |
