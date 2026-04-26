# Unit Test Report for CreateSubProject

# 1. General Information

| Field | Value |
|------|------|
| className | CreateSubProjectCommandHandler |
| functionName | CreateSubProject |
| testClass | CreateSubProjectCommandHandlerTests |
| feature | Project |

# 2. Test Case List

| UTCID | Description | Type |
|------|-------------|------|
| UTCID01 | ProjectNotFound -> ThrowsNotFoundException | A |
| UTCID02 | ValidProject -> CreatesSubProjectAndReturnsId | N |

# 3. Header Information

| Function Code | | Function Name | CreateSubProject |
|---------------|---|---------------|-----------------|
| Created By | Auto-Generator | Executed By | Auto-Generator |
| Lines of code | N/A | Lack of test cases | N/A |
| Test requirement | Auto-generated from CreateSubProjectCommandHandlerTests | | |

| Passed | Failed | Untested | N/A/B | Total Test Cases |
|--------|--------|----------|-------|------------------|
| 2 | 0 | 0 | 1 0 1 | 2 |

# 4. Decision Table Matrix

### CONDITION Section

| Condition | Value | UTCID01 | UTCID02 |
|-----------|-------|---|---|
| **Input** | |  |  |
| | ProjectNotFound | O |   |
| | ValidProject |   | O |

### CONFIRM Section

| Confirm | Value | UTCID01 | UTCID02 |
|---------|-------|---|---|
| **Return/Exception** | |  |  |
| | ThrowsNotFoundException | O |   |
| | CreatesSubProjectAndReturnsId |   | O |

### RESULT Section

| Result Field | UTCID01 | UTCID02 |
|--------------|---|---|
| **Type (N/B/A)** | A | N |
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
