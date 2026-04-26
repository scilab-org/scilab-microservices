# Unit Test Report for UpdatePaper

# 1. General Information

| Field | Value |
|------|------|
| className | UpdatePaperCommandHandler |
| functionName | UpdatePaper |
| testClass | PaperCommandsIntegrationTests |
| feature | Paper |

# 2. Test Case List

| UTCID | Description | Type |
|------|-------------|------|
| UTCID01 | WithNonExistentId -> ShouldThrowNotFoundException | A |
| UTCID02 | WithMissingGapTypes -> ShouldThrowNotFoundException | A |
| UTCID03 | WithMissingJournal -> ShouldThrow | A |
| UTCID04 | WithValidData -> ShouldUpdateAndReturn | N |
| UTCID05 | WithGapTypes -> ShouldUpdateGapTypeIds | N |
| UTCID06 | WithSectionsInDb -> ShouldUpdateSectionsPaperRule | N |

# 3. Header Information

| Function Code | | Function Name | UpdatePaper |
|---------------|---|---------------|-----------------|
| Created By | Auto-Generator | Executed By | Auto-Generator |
| Lines of code | N/A | Lack of test cases | N/A |
| Test requirement | Auto-generated from PaperCommandsIntegrationTests | | |

| Passed | Failed | Untested | N/A/B | Total Test Cases |
|--------|--------|----------|-------|------------------|
| 6 | 0 | 0 | 3 0 3 | 6 |

# 4. Decision Table Matrix

### CONDITION Section

| Condition | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 | UTCID06 |
|-----------|-------|---|---|---|---|---|---|
| **Input** | |  |  |  |  |  |  |
| | WithNonExistentId | O |   |   |   |   |   |
| | WithMissingGapTypes |   | O |   |   |   |   |
| | WithMissingJournal |   |   | O |   |   |   |
| | WithValidData |   |   |   | O |   |   |
| | WithGapTypes |   |   |   |   | O |   |
| | WithSectionsInDb |   |   |   |   |   | O |

### CONFIRM Section

| Confirm | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 | UTCID06 |
|---------|-------|---|---|---|---|---|---|
| **Return/Exception** | |  |  |  |  |  |  |
| | ShouldThrowNotFoundException | O |   |   |   |   |   |
| | ShouldThrowNotFoundException |   | O |   |   |   |   |
| | ShouldThrow |   |   | O |   |   |   |
| | ShouldUpdateAndReturn |   |   |   | O |   |   |
| | ShouldUpdateGapTypeIds |   |   |   |   | O |   |
| | ShouldUpdateSectionsPaperRule |   |   |   |   |   | O |

### RESULT Section

| Result Field | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 | UTCID06 |
|--------------|---|---|---|---|---|---|
| **Type (N/B/A)** | A | A | A | N | N | N |
| **Pass/Fail (P/F)** | P | P | P | P | P | P |
| **Executed Date** | 2026-04-26 | 2026-04-26 | 2026-04-26 | 2026-04-26 | 2026-04-26 | 2026-04-26 |
| **Defect ID** | - | - | - | - | - | - |

### Summary

| Field | Value |
|------|------|
| Passed | 6 |
| Failed | 0 |
| Untested | 0 |
| Total Test Cases | 6 |
