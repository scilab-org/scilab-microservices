# Unit Test Report for TransitionPaperStatus

# 1. General Information

| Field | Value |
|------|------|
| className | TransitionPaperStatusHandler |
| functionName | TransitionPaperStatus |
| testClass | PaperCommandsIntegrationTests |
| feature | Paper |

# 2. Test Case List

| UTCID | Description | Type |
|------|-------------|------|
| UTCID01 | WithNonExistentPaper -> ShouldThrowNotFoundException | A |
| UTCID02 | DuplicateStatus -> ShouldThrowClientValidationException | A |
| UTCID03 | InvalidTransition -> ShouldThrowClientValidationException | A |
| UTCID04 | RequiresPdfButNoneProvided -> ShouldThrowClientValidationException | A |
| UTCID05 | RequiresPdfButFileNotFound -> ShouldThrowNotFoundException | A |
| UTCID06 | PdfBelongsToDifferentPaper -> ShouldThrowClientValidationException | A |
| UTCID07 | SubmittedWithValidPdf AsAuthor -> ShouldStoreHistory | N |
| UTCID08 | SubmittedButUserNotAuthor -> ShouldThrowNoPermissionException | A |
| UTCID09 | RevisionRequired AsProjectAuthor -> ShouldStoreHistory | N |
| UTCID10 | EditorTransition NotProjectRole -> ShouldThrowNoPermissionException | A |

# 3. Header Information

| Function Code | | Function Name | TransitionPaperStatus |
|---------------|---|---------------|-----------------|
| Created By | Auto-Generator | Executed By | Auto-Generator |
| Lines of code | N/A | Lack of test cases | N/A |
| Test requirement | Auto-generated from PaperCommandsIntegrationTests | | |

| Passed | Failed | Untested | N/A/B | Total Test Cases |
|--------|--------|----------|-------|------------------|
| 10 | 0 | 0 | 2 0 8 | 10 |

# 4. Decision Table Matrix

### CONDITION Section

| Condition | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 | UTCID06 | UTCID07 | UTCID08 | UTCID09 | UTCID10 |
|-----------|-------|---|---|---|---|---|---|---|---|---|---|
| **Input** | |  |  |  |  |  |  |  |  |  |  |
| | WithNonExistentPaper | O |   |   |   |   |   |   |   |   |   |
| | DuplicateStatus |   | O |   |   |   |   |   |   |   |   |
| | InvalidTransition |   |   | O |   |   |   |   |   |   |   |
| | RequiresPdfButNoneProvided |   |   |   | O |   |   |   |   |   |   |
| | RequiresPdfButFileNotFound |   |   |   |   | O |   |   |   |   |   |
| | PdfBelongsToDifferentPaper |   |   |   |   |   | O |   |   |   |   |
| | SubmittedWithValidPdf AsAuthor |   |   |   |   |   |   | O |   |   |   |
| | SubmittedButUserNotAuthor |   |   |   |   |   |   |   | O |   |   |
| | RevisionRequired AsProjectAuthor |   |   |   |   |   |   |   |   | O |   |
| | EditorTransition NotProjectRole |   |   |   |   |   |   |   |   |   | O |

### CONFIRM Section

| Confirm | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 | UTCID06 | UTCID07 | UTCID08 | UTCID09 | UTCID10 |
|---------|-------|---|---|---|---|---|---|---|---|---|---|
| **Return/Exception** | |  |  |  |  |  |  |  |  |  |  |
| | ShouldThrowNotFoundException | O |   |   |   |   |   |   |   |   |   |
| | ShouldThrowClientValidationException |   | O |   |   |   |   |   |   |   |   |
| | ShouldThrowClientValidationException |   |   | O |   |   |   |   |   |   |   |
| | ShouldThrowClientValidationException |   |   |   | O |   |   |   |   |   |   |
| | ShouldThrowNotFoundException |   |   |   |   | O |   |   |   |   |   |
| | ShouldThrowClientValidationException |   |   |   |   |   | O |   |   |   |   |
| | ShouldStoreHistory |   |   |   |   |   |   | O |   |   |   |
| | ShouldThrowNoPermissionException |   |   |   |   |   |   |   | O |   |   |
| | ShouldStoreHistory |   |   |   |   |   |   |   |   | O |   |
| | ShouldThrowNoPermissionException |   |   |   |   |   |   |   |   |   | O |

### RESULT Section

| Result Field | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 | UTCID06 | UTCID07 | UTCID08 | UTCID09 | UTCID10 |
|--------------|---|---|---|---|---|---|---|---|---|---|
| **Type (N/B/A)** | A | A | A | A | A | A | N | A | N | A |
| **Pass/Fail (P/F)** | P | P | P | P | P | P | P | P | P | P |
| **Executed Date** | 2026-04-26 | 2026-04-26 | 2026-04-26 | 2026-04-26 | 2026-04-26 | 2026-04-26 | 2026-04-26 | 2026-04-26 | 2026-04-26 | 2026-04-26 |
| **Defect ID** | - | - | - | - | - | - | - | - | - | - |

### Summary

| Field | Value |
|------|------|
| Passed | 10 |
| Failed | 0 |
| Untested | 0 |
| Total Test Cases | 10 |
