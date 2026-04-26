# Unit Test Report for UpdatePaperBank

# 1. General Information

| Field | Value |
|------|------|
| className | UpdatePaperBankCommandHandler |
| functionName | UpdatePaperBank |
| testClass | PaperBankCommandsIntegrationTests |
| feature | PaperBank |

# 2. Test Case List

| UTCID | Description | Type |
|------|-------------|------|
| UTCID01 | WithValidData -> ShouldUpdateAndReturnId | N |
| UTCID02 | WithNonExistentId -> ShouldThrowClientValidationException | A |
| UTCID03 | WithNonExistentJournal -> ShouldThrowClientValidationException | A |
| UTCID04 | WithInvalidGapTypeId -> ShouldThrowClientValidationException | A |

# 3. Header Information

| Function Code | | Function Name | UpdatePaperBank |
|---------------|---|---------------|-----------------|
| Created By | Auto-Generator | Executed By | Auto-Generator |
| Lines of code | N/A | Lack of test cases | N/A |
| Test requirement | Auto-generated from PaperBankCommandsIntegrationTests | | |

| Passed | Failed | Untested | N/A/B | Total Test Cases |
|--------|--------|----------|-------|------------------|
| 4 | 0 | 0 | 1 0 3 | 4 |

# 4. Decision Table Matrix

### CONDITION Section

| Condition | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|-------|---|---|---|---|
| **Input** | |  |  |  |  |
| | WithValidData | O |   |   |   |
| | WithNonExistentId |   | O |   |   |
| | WithNonExistentJournal |   |   | O |   |
| | WithInvalidGapTypeId |   |   |   | O |

### CONFIRM Section

| Confirm | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|---------|-------|---|---|---|---|
| **Return/Exception** | |  |  |  |  |
| | ShouldUpdateAndReturnId | O |   |   |   |
| | ShouldThrowClientValidationException |   | O |   |   |
| | ShouldThrowClientValidationException |   |   | O |   |
| | ShouldThrowClientValidationException |   |   |   | O |

### RESULT Section

| Result Field | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|--------------|---|---|---|---|
| **Type (N/B/A)** | N | A | A | A |
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
