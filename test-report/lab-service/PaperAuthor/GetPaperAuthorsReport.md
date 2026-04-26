# Unit Test Report for GetPaperAuthors

# 1. General Information

| Field | Value |
|------|------|
| className | GetPaperAuthorsHandler |
| functionName | GetPaperAuthors |
| testClass | PaperAuthorIntegrationTests |
| feature | PaperAuthor |

# 2. Test Case List

| UTCID | Description | Type |
|------|-------------|------|
| UTCID01 | WithNoFilter -> ShouldReturnAll | N |
| UTCID02 | WithNameFilter -> ShouldReturnMatching | N |
| UTCID03 | WithPaperIdFilter -> ShouldReturnMatching | N |
| UTCID04 | WithRoleNameFilter -> ShouldReturnMatching | N |

# 3. Header Information

| Function Code | | Function Name | GetPaperAuthors |
|---------------|---|---------------|-----------------|
| Created By | Auto-Generator | Executed By | Auto-Generator |
| Lines of code | N/A | Lack of test cases | N/A |
| Test requirement | Auto-generated from PaperAuthorIntegrationTests | | |

| Passed | Failed | Untested | N/A/B | Total Test Cases |
|--------|--------|----------|-------|------------------|
| 4 | 0 | 0 | 4 0 0 | 4 |

# 4. Decision Table Matrix

### CONDITION Section

| Condition | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|-------|---|---|---|---|
| **Input** | |  |  |  |  |
| | WithNoFilter | O |   |   |   |
| | WithNameFilter |   | O |   |   |
| | WithPaperIdFilter |   |   | O |   |
| | WithRoleNameFilter |   |   |   | O |

### CONFIRM Section

| Confirm | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|---------|-------|---|---|---|---|
| **Return/Exception** | |  |  |  |  |
| | ShouldReturnAll | O |   |   |   |
| | ShouldReturnMatching |   | O |   |   |
| | ShouldReturnMatching |   |   | O |   |
| | ShouldReturnMatching |   |   |   | O |

### RESULT Section

| Result Field | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|--------------|---|---|---|---|
| **Type (N/B/A)** | N | N | N | N |
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
