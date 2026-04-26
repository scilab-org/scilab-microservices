# Unit Test Report for CreatePaperBank

# 1. General Information

| Field | Value |
|------|------|
| className | CreatePaperBankCommandHandler |
| functionName | CreatePaperBank |
| testClass | PaperBankCommandsIntegrationTests |
| feature | PaperBank |

# 2. Test Case List

| UTCID | Description | Type |
|------|-------------|------|
| UTCID01 | WithValidData -> ShouldStoreAndReturnId | N |
| UTCID02 | WithNonExistentJournal -> ShouldThrowNotFoundException | A |
| UTCID03 | WithKeywords -> ShouldNormalizeAndStore | N |

# 3. Header Information

| Function Code | | Function Name | CreatePaperBank |
|---------------|---|---------------|-----------------|
| Created By | Auto-Generator | Executed By | Auto-Generator |
| Lines of code | N/A | Lack of test cases | N/A |
| Test requirement | Auto-generated from PaperBankCommandsIntegrationTests | | |

| Passed | Failed | Untested | N/A/B | Total Test Cases |
|--------|--------|----------|-------|------------------|
| 3 | 0 | 0 | 2 0 1 | 3 |

# 4. Decision Table Matrix

### CONDITION Section

| Condition | Value | UTCID01 | UTCID02 | UTCID03 |
|-----------|-------|---|---|---|
| **Input** | |  |  |  |
| | WithValidData | O |   |   |
| | WithNonExistentJournal |   | O |   |
| | WithKeywords |   |   | O |

### CONFIRM Section

| Confirm | Value | UTCID01 | UTCID02 | UTCID03 |
|---------|-------|---|---|---|
| **Return/Exception** | |  |  |  |
| | ShouldStoreAndReturnId | O |   |   |
| | ShouldThrowNotFoundException |   | O |   |
| | ShouldNormalizeAndStore |   |   | O |

### RESULT Section

| Result Field | UTCID01 | UTCID02 | UTCID03 |
|--------------|---|---|---|
| **Type (N/B/A)** | N | A | N |
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
