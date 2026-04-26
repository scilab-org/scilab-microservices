# Unit Test Report for GetPaperBankById

# 1. General Information

| Field | Value |
|------|------|
| className | GetPaperBankByIdHandler |
| functionName | GetPaperBankById |
| testClass | PaperBankIntegrationTests |
| feature | PaperBank |

# 2. Test Case List

| UTCID | Description | Type |
|------|-------------|------|
| UTCID01 | WithExisting -> ShouldReturnMappedResult | N |
| UTCID02 | WithJournal -> ShouldPopulateJournalName | N |
| UTCID03 | WithGapTypes -> ShouldPopulateGapTypes | N |
| UTCID04 | WithNonExistent -> ShouldThrowNotFoundException | A |

# 3. Header Information

| Function Code | | Function Name | GetPaperBankById |
|---------------|---|---------------|-----------------|
| Created By | Auto-Generator | Executed By | Auto-Generator |
| Lines of code | N/A | Lack of test cases | N/A |
| Test requirement | Auto-generated from PaperBankIntegrationTests | | |

| Passed | Failed | Untested | N/A/B | Total Test Cases |
|--------|--------|----------|-------|------------------|
| 4 | 0 | 0 | 3 0 1 | 4 |

# 4. Decision Table Matrix

### CONDITION Section

| Condition | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|-------|---|---|---|---|
| **Input** | |  |  |  |  |
| | WithExisting | O |   |   |   |
| | WithJournal |   | O |   |   |
| | WithGapTypes |   |   | O |   |
| | WithNonExistent |   |   |   | O |

### CONFIRM Section

| Confirm | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|---------|-------|---|---|---|---|
| **Return/Exception** | |  |  |  |  |
| | ShouldReturnMappedResult | O |   |   |   |
| | ShouldPopulateJournalName |   | O |   |   |
| | ShouldPopulateGapTypes |   |   | O |   |
| | ShouldThrowNotFoundException |   |   |   | O |

### RESULT Section

| Result Field | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|--------------|---|---|---|---|
| **Type (N/B/A)** | N | N | N | A |
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
