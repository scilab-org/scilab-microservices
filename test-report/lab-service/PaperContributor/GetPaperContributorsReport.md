# Unit Test Report for GetPaperContributors

# 1. General Information

| Field | Value |
|------|------|
| className | GetPaperContributorsQueryHandler |
| functionName | GetPaperContributors |
| testClass | PaperContributorQueriesIntegrationTests |
| feature | PaperContributor |

# 2. Test Case List

| UTCID | Description | Type |
|------|-------------|------|
| UTCID01 | NoContributors -> ShouldReturnEmptyResult | N |
| UTCID02 | WithContributors -> ShouldReturnEnrichedList | N |
| UTCID03 | WithPaperAuthorRole -> ShouldBeExcluded | N |

# 3. Header Information

| Function Code | | Function Name | GetPaperContributors |
|---------------|---|---------------|-----------------|
| Created By | Auto-Generator | Executed By | Auto-Generator |
| Lines of code | N/A | Lack of test cases | N/A |
| Test requirement | Auto-generated from PaperContributorQueriesIntegrationTests | | |

| Passed | Failed | Untested | N/A/B | Total Test Cases |
|--------|--------|----------|-------|------------------|
| 3 | 0 | 0 | 3 0 0 | 3 |

# 4. Decision Table Matrix

### CONDITION Section

| Condition | Value | UTCID01 | UTCID02 | UTCID03 |
|-----------|-------|---|---|---|
| **Input** | |  |  |  |
| | NoContributors | O |   |   |
| | WithContributors |   | O |   |
| | WithPaperAuthorRole |   |   | O |

### CONFIRM Section

| Confirm | Value | UTCID01 | UTCID02 | UTCID03 |
|---------|-------|---|---|---|
| **Return/Exception** | |  |  |  |
| | ShouldReturnEmptyResult | O |   |   |
| | ShouldReturnEnrichedList |   | O |   |
| | ShouldBeExcluded |   |   | O |

### RESULT Section

| Result Field | UTCID01 | UTCID02 | UTCID03 |
|--------------|---|---|---|
| **Type (N/B/A)** | N | N | N |
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
