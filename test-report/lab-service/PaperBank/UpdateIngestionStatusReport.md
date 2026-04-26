# Unit Test Report for UpdateIngestionStatus

# 1. General Information

| Field | Value |
|------|------|
| className | UpdateIngestionStatusCommandHandler |
| functionName | UpdateIngestionStatus |
| testClass | PaperBankIntegrationTests |
| feature | PaperBank |

# 2. Test Case List

| UTCID | Description | Type |
|------|-------------|------|
| UTCID01 | Success -> ShouldUpdateEntity | N |
| UTCID02 | Failure -> ShouldSetFailedStatus | A |
| UTCID03 | WithNonExistentId -> ShouldThrow | A |

# 3. Header Information

| Function Code | | Function Name | UpdateIngestionStatus |
|---------------|---|---------------|-----------------|
| Created By | Auto-Generator | Executed By | Auto-Generator |
| Lines of code | N/A | Lack of test cases | N/A |
| Test requirement | Auto-generated from PaperBankIntegrationTests | | |

| Passed | Failed | Untested | N/A/B | Total Test Cases |
|--------|--------|----------|-------|------------------|
| 3 | 0 | 0 | 1 0 2 | 3 |

# 4. Decision Table Matrix

### CONDITION Section

| Condition | Value | UTCID01 | UTCID02 | UTCID03 |
|-----------|-------|---|---|---|
| **Input** | |  |  |  |
| | Success | O |   |   |
| | Failure |   | O |   |
| | WithNonExistentId |   |   | O |

### CONFIRM Section

| Confirm | Value | UTCID01 | UTCID02 | UTCID03 |
|---------|-------|---|---|---|
| **Return/Exception** | |  |  |  |
| | ShouldUpdateEntity | O |   |   |
| | ShouldSetFailedStatus |   | O |   |
| | ShouldThrow |   |   | O |

### RESULT Section

| Result Field | UTCID01 | UTCID02 | UTCID03 |
|--------------|---|---|---|
| **Type (N/B/A)** | N | A | A |
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
