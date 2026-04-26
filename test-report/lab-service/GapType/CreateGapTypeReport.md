# Unit Test Report for CreateGapType

# 1. General Information

| Field | Value |
|------|------|
| className | CreateGapTypeCommandHandler |
| functionName | CreateGapType |
| testClass | GapTypeIntegrationTests |
| feature | GapType |

# 2. Test Case List

| UTCID | Description | Type |
|------|-------------|------|
| UTCID01 | WithValidName -> ShouldStoreAndReturnId | N |
| UTCID02 | ShouldTrimName -> Default | N |

# 3. Header Information

| Function Code | | Function Name | CreateGapType |
|---------------|---|---------------|-----------------|
| Created By | Auto-Generator | Executed By | Auto-Generator |
| Lines of code | N/A | Lack of test cases | N/A |
| Test requirement | Auto-generated from GapTypeIntegrationTests | | |

| Passed | Failed | Untested | N/A/B | Total Test Cases |
|--------|--------|----------|-------|------------------|
| 2 | 0 | 0 | 2 0 0 | 2 |

# 4. Decision Table Matrix

### CONDITION Section

| Condition | Value | UTCID01 | UTCID02 |
|-----------|-------|---|---|
| **Input** | |  |  |
| | WithValidName | O |   |
| | ShouldTrimName |   | O |

### CONFIRM Section

| Confirm | Value | UTCID01 | UTCID02 |
|---------|-------|---|---|
| **Return/Exception** | |  |  |
| | ShouldStoreAndReturnId | O |   |
| | Default |   | O |

### RESULT Section

| Result Field | UTCID01 | UTCID02 |
|--------------|---|---|
| **Type (N/B/A)** | N | N |
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
