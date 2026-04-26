# Unit Test Report for GetPaperStatusHistory

# 1. General Information

| Field | Value |
|------|------|
| className | GetPaperStatusHistoryHandler |
| functionName | GetPaperStatusHistory |
| testClass | PaperIntegrationTests |
| feature | Paper |

# 2. Test Case List

| UTCID | Description | Type |
|------|-------------|------|
| UTCID01 | WithHistory -> ShouldReturnOrderedResults | N |
| UTCID02 | WithNoHistory -> ShouldReturnDraftStatus | N |
| UTCID03 | WithNonExistentPaper -> ShouldThrowNotFoundException | A |

# 3. Header Information

| Function Code | | Function Name | GetPaperStatusHistory |
|---------------|---|---------------|-----------------|
| Created By | Auto-Generator | Executed By | Auto-Generator |
| Lines of code | N/A | Lack of test cases | N/A |
| Test requirement | Auto-generated from PaperIntegrationTests | | |

| Passed | Failed | Untested | N/A/B | Total Test Cases |
|--------|--------|----------|-------|------------------|
| 3 | 0 | 0 | 2 0 1 | 3 |

# 4. Decision Table Matrix

### CONDITION Section

| Condition | Value | UTCID01 | UTCID02 | UTCID03 |
|-----------|-------|---|---|---|
| **Input** | |  |  |  |
| | WithHistory | O |   |   |
| | WithNoHistory |   | O |   |
| | WithNonExistentPaper |   |   | O |

### CONFIRM Section

| Confirm | Value | UTCID01 | UTCID02 | UTCID03 |
|---------|-------|---|---|---|
| **Return/Exception** | |  |  |  |
| | ShouldReturnOrderedResults | O |   |   |
| | ShouldReturnDraftStatus |   | O |   |
| | ShouldThrowNotFoundException |   |   | O |

### RESULT Section

| Result Field | UTCID01 | UTCID02 | UTCID03 |
|--------------|---|---|---|
| **Type (N/B/A)** | N | N | A |
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
