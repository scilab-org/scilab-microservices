# Unit Test Report for GetAssignedPaperSections

# 1. General Information

| Field | Value |
|------|------|
| className | GetAssignedPaperSectionsQueryHandler |
| functionName | GetAssignedPaperSections |
| testClass | PaperContributorQueriesIntegrationTests |
| feature | PaperContributor |

# 2. Test Case List

| UTCID | Description | Type |
|------|-------------|------|
| UTCID01 | MemberNotFound -> ShouldThrowNotFoundException | A |
| UTCID02 | NoContributors -> ShouldReturnEmptyResult | N |
| UTCID03 | WithContributorsAndSections -> ShouldReturnAssignedSections | N |

# 3. Header Information

| Function Code | | Function Name | GetAssignedPaperSections |
|---------------|---|---------------|-----------------|
| Created By | Auto-Generator | Executed By | Auto-Generator |
| Lines of code | N/A | Lack of test cases | N/A |
| Test requirement | Auto-generated from PaperContributorQueriesIntegrationTests | | |

| Passed | Failed | Untested | N/A/B | Total Test Cases |
|--------|--------|----------|-------|------------------|
| 3 | 0 | 0 | 2 0 1 | 3 |

# 4. Decision Table Matrix

### CONDITION Section

| Condition | Value | UTCID01 | UTCID02 | UTCID03 |
|-----------|-------|---|---|---|
| **Input** | |  |  |  |
| | MemberNotFound | O |   |   |
| | NoContributors |   | O |   |
| | WithContributorsAndSections |   |   | O |

### CONFIRM Section

| Confirm | Value | UTCID01 | UTCID02 | UTCID03 |
|---------|-------|---|---|---|
| **Return/Exception** | |  |  |  |
| | ShouldThrowNotFoundException | O |   |   |
| | ShouldReturnEmptyResult |   | O |   |
| | ShouldReturnAssignedSections |   |   | O |

### RESULT Section

| Result Field | UTCID01 | UTCID02 | UTCID03 |
|--------------|---|---|---|
| **Type (N/B/A)** | A | N | N |
| **Pass/Fail (P/F)** | P | P | P |
| **Executed Date** | 2026-04-26 | 2026-04-26 | 2026-04-26 |
| **Defect ID** | - | - | - |

### Summary

| Field | Value |
|------|------|
| Passed | 3 |
| Failed | 0 |
| Untested | 0 |
| Total Test Cases | 3 |
