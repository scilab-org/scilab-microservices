# Unit Test Report for GetAssignedPaperSectionsHistory

# 1. General Information

| Field | Value |
|------|------|
| className | GetAssignedPaperSectionsHistoryQueryHandler |
| functionName | GetAssignedPaperSectionsHistory |
| testClass | PaperContributorQueriesIntegrationTests |
| feature | PaperContributor |

# 2. Test Case List

| UTCID | Description | Type |
|------|-------------|------|
| UTCID01 | MemberNotFound -> ShouldThrowNotFoundException | A |
| UTCID02 | NoContributors -> ShouldReturnEmptyResult | N |
| UTCID03 | WithContributors NoOldMainSections -> ShouldReturnEmpty | N |
| UTCID04 | FilterBySectionRole -> ShouldFilterContributors | N |

# 3. Header Information

| Function Code | | Function Name | GetAssignedPaperSectionsHistory |
|---------------|---|---------------|-----------------|
| Created By | Auto-Generator | Executed By | Auto-Generator |
| Lines of code | N/A | Lack of test cases | N/A |
| Test requirement | Auto-generated from PaperContributorQueriesIntegrationTests | | |

| Passed | Failed | Untested | N/A/B | Total Test Cases |
|--------|--------|----------|-------|------------------|
| 4 | 0 | 0 | 3 0 1 | 4 |

# 4. Decision Table Matrix

### CONDITION Section

| Condition | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|-------|---|---|---|---|
| **Input** | |  |  |  |  |
| | MemberNotFound | O |   |   |   |
| | NoContributors |   | O |   |   |
| | WithContributors NoOldMainSections |   |   | O |   |
| | FilterBySectionRole |   |   |   | O |

### CONFIRM Section

| Confirm | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|---------|-------|---|---|---|---|
| **Return/Exception** | |  |  |  |  |
| | ShouldThrowNotFoundException | O |   |   |   |
| | ShouldReturnEmptyResult |   | O |   |   |
| | ShouldReturnEmpty |   |   | O |   |
| | ShouldFilterContributors |   |   |   | O |

### RESULT Section

| Result Field | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|--------------|---|---|---|---|
| **Type (N/B/A)** | A | N | N | N |
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
