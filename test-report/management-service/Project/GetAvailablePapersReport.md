# Unit Test Report for GetAvailablePapers

# 1. General Information

| Field | Value |
|------|------|
| className | GetAvailablePapersQueryHandler |
| functionName | GetAvailablePapers |
| testClass | GetAvailablePapersQueryHandlerTests |
| feature | Project |

# 2. Test Case List

| UTCID | Description | Type |
|------|-------------|------|
| UTCID01 | ProjectNotFound -> ThrowsNotFoundException | A |
| UTCID02 | ValidProject -> CallsLabServiceAndReturnsResult | N |
| UTCID03 | ProjectWithNoPapers -> PassesEmptyExistingIds | N |

# 3. Header Information

| Function Code | | Function Name | GetAvailablePapers |
|---------------|---|---------------|-----------------|
| Created By | Auto-Generator | Executed By | Auto-Generator |
| Lines of code | N/A | Lack of test cases | N/A |
| Test requirement | Auto-generated from GetAvailablePapersQueryHandlerTests | | |

| Passed | Failed | Untested | N/A/B | Total Test Cases |
|--------|--------|----------|-------|------------------|
| 3 | 0 | 0 | 2 0 1 | 3 |

# 4. Decision Table Matrix

### CONDITION Section

| Condition | Value | UTCID01 | UTCID02 | UTCID03 |
|-----------|-------|---|---|---|
| **Input** | |  |  |  |
| | ProjectNotFound | O |   |   |
| | ValidProject |   | O |   |
| | ProjectWithNoPapers |   |   | O |

### CONFIRM Section

| Confirm | Value | UTCID01 | UTCID02 | UTCID03 |
|---------|-------|---|---|---|
| **Return/Exception** | |  |  |  |
| | ThrowsNotFoundException | O |   |   |
| | CallsLabServiceAndReturnsResult |   | O |   |
| | PassesEmptyExistingIds |   |   | O |

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
