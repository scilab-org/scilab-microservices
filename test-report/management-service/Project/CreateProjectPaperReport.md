# Unit Test Report for CreateProjectPaper

# 1. General Information

| Field | Value |
|------|------|
| className | CreateProjectPaperCommandHandler |
| functionName | CreateProjectPaper |
| testClass | CreateProjectPaperCommandHandlerTests |
| feature | Project |

# 2. Test Case List

| UTCID | Description | Type |
|------|-------------|------|
| UTCID01 | ProjectNotFound -> ThrowsNotFoundException | A |
| UTCID02 | NoValidPaperIds -> ThrowsNotFoundException | A |
| UTCID03 | ValidPaperIds -> AddsPapersAndReturnsIds | N |

# 3. Header Information

| Function Code | | Function Name | CreateProjectPaper |
|---------------|---|---------------|-----------------|
| Created By | Auto-Generator | Executed By | Auto-Generator |
| Lines of code | N/A | Lack of test cases | N/A |
| Test requirement | Auto-generated from CreateProjectPaperCommandHandlerTests | | |

| Passed | Failed | Untested | N/A/B | Total Test Cases |
|--------|--------|----------|-------|------------------|
| 3 | 0 | 0 | 1 0 2 | 3 |

# 4. Decision Table Matrix

### CONDITION Section

| Condition | Value | UTCID01 | UTCID02 | UTCID03 |
|-----------|-------|---|---|---|
| **Input** | |  |  |  |
| | ProjectNotFound | O |   |   |
| | NoValidPaperIds |   | O |   |
| | ValidPaperIds |   |   | O |

### CONFIRM Section

| Confirm | Value | UTCID01 | UTCID02 | UTCID03 |
|---------|-------|---|---|---|
| **Return/Exception** | |  |  |  |
| | ThrowsNotFoundException | O |   |   |
| | ThrowsNotFoundException |   | O |   |
| | AddsPapersAndReturnsIds |   |   | O |

### RESULT Section

| Result Field | UTCID01 | UTCID02 | UTCID03 |
|--------------|---|---|---|
| **Type (N/B/A)** | A | A | N |
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
