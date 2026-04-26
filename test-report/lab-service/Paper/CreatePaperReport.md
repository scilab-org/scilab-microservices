# Unit Test Report for CreatePaper

# 1. General Information

| Field | Value |
|------|------|
| className | CreatePaperCommandHandler |
| functionName | CreatePaper |
| testClass | CreatePaperCommandsIntegrationTests |
| feature | Paper |

# 2. Test Case List

| UTCID | Description | Type |
|------|-------------|------|
| UTCID01 | WithValidData -> ShouldStoreAndReturnId | N |
| UTCID02 | WithSections -> ShouldStoreSectionsAlso | N |
| UTCID03 | WithEmptyProjectId -> ShouldSkipPostSaveManagementCalls | A |
| UTCID04 | WithEmptyGapTypeIds -> ShouldSucceed | A |
| UTCID05 | WithNonExistentJournal -> ShouldThrowNotFoundException | A |
| UTCID06 | WithMissingGapType -> ShouldThrowNotFoundException | A |
| UTCID07 | WithUnauthorizedRole -> ShouldThrowNoPermissionException | A |
| UTCID08 | WhenAddProjectJournalFails -> ShouldThrowNotFoundException | A |

# 3. Header Information

| Function Code | | Function Name | CreatePaper |
|---------------|---|---------------|-----------------|
| Created By | Auto-Generator | Executed By | Auto-Generator |
| Lines of code | N/A | Lack of test cases | N/A |
| Test requirement | Auto-generated from CreatePaperCommandsIntegrationTests | | |

| Passed | Failed | Untested | N/A/B | Total Test Cases |
|--------|--------|----------|-------|------------------|
| 8 | 0 | 0 | 2 0 6 | 8 |

# 4. Decision Table Matrix

### CONDITION Section

| Condition | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 | UTCID06 | UTCID07 | UTCID08 |
|-----------|-------|---|---|---|---|---|---|---|---|
| **Input** | |  |  |  |  |  |  |  |  |
| | WithValidData | O |   |   |   |   |   |   |   |
| | WithSections |   | O |   |   |   |   |   |   |
| | WithEmptyProjectId |   |   | O |   |   |   |   |   |
| | WithEmptyGapTypeIds |   |   |   | O |   |   |   |   |
| | WithNonExistentJournal |   |   |   |   | O |   |   |   |
| | WithMissingGapType |   |   |   |   |   | O |   |   |
| | WithUnauthorizedRole |   |   |   |   |   |   | O |   |
| | WhenAddProjectJournalFails |   |   |   |   |   |   |   | O |

### CONFIRM Section

| Confirm | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 | UTCID06 | UTCID07 | UTCID08 |
|---------|-------|---|---|---|---|---|---|---|---|
| **Return/Exception** | |  |  |  |  |  |  |  |  |
| | ShouldStoreAndReturnId | O |   |   |   |   |   |   |   |
| | ShouldStoreSectionsAlso |   | O |   |   |   |   |   |   |
| | ShouldSkipPostSaveManagementCalls |   |   | O |   |   |   |   |   |
| | ShouldSucceed |   |   |   | O |   |   |   |   |
| | ShouldThrowNotFoundException |   |   |   |   | O |   |   |   |
| | ShouldThrowNotFoundException |   |   |   |   |   | O |   |   |
| | ShouldThrowNoPermissionException |   |   |   |   |   |   | O |   |
| | ShouldThrowNotFoundException |   |   |   |   |   |   |   | O |

### RESULT Section

| Result Field | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 | UTCID06 | UTCID07 | UTCID08 |
|--------------|---|---|---|---|---|---|---|---|
| **Type (N/B/A)** | N | N | A | A | A | A | A | A |
| **Pass/Fail (P/F)** | P | P | P | P | P | P | P | P |
| **Executed Date** | 2026-04-26 | 2026-04-26 | 2026-04-26 | 2026-04-26 | 2026-04-26 | 2026-04-26 | 2026-04-26 | 2026-04-26 |
| **Defect ID** | - | - | - | - | - | - | - | - |

### Summary

| Field | Value |
|------|------|
| Passed | 8 |
| Failed | 0 |
| Untested | 0 |
| Total Test Cases | 8 |
