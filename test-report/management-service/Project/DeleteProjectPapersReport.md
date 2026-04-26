# Unit Test Report for DeleteProjectPapers

# 1. General Information

| Field | Value |
|------|------|
| className | DeleteProjectPapersCommandHandler |
| functionName | DeleteProjectPapers |
| testClass | DeleteProjectPapersCommandHandlerTests |
| feature | Project |

# 2. Test Case List

| UTCID | Description | Type |
|------|-------------|------|
| UTCID01 | ProjectNotFound -> ThrowsNotFoundException | A |
| UTCID02 | AllPaperIdsEmpty -> ThrowsClientValidationException | A |
| UTCID03 | PapersNotFoundInProject -> ThrowsNotFoundException | A |
| UTCID04 | ValidPaperIds -> RemovesPapersAndReturnsRemovedIds | N |
| UTCID05 | DuplicatePaperIds -> DeduplicatesBeforeRemoval | N |

# 3. Header Information

| Function Code | | Function Name | DeleteProjectPapers |
|---------------|---|---------------|-----------------|
| Created By | Auto-Generator | Executed By | Auto-Generator |
| Lines of code | N/A | Lack of test cases | N/A |
| Test requirement | Auto-generated from DeleteProjectPapersCommandHandlerTests | | |

| Passed | Failed | Untested | N/A/B | Total Test Cases |
|--------|--------|----------|-------|------------------|
| 5 | 0 | 0 | 2 0 3 | 5 |

# 4. Decision Table Matrix

### CONDITION Section

| Condition | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 |
|-----------|-------|---|---|---|---|---|
| **Input** | |  |  |  |  |  |
| | ProjectNotFound | O |   |   |   |   |
| | AllPaperIdsEmpty |   | O |   |   |   |
| | PapersNotFoundInProject |   |   | O |   |   |
| | ValidPaperIds |   |   |   | O |   |
| | DuplicatePaperIds |   |   |   |   | O |

### CONFIRM Section

| Confirm | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 |
|---------|-------|---|---|---|---|---|
| **Return/Exception** | |  |  |  |  |  |
| | ThrowsNotFoundException | O |   |   |   |   |
| | ThrowsClientValidationException |   | O |   |   |   |
| | ThrowsNotFoundException |   |   | O |   |   |
| | RemovesPapersAndReturnsRemovedIds |   |   |   | O |   |
| | DeduplicatesBeforeRemoval |   |   |   |   | O |

### RESULT Section

| Result Field | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 |
|--------------|---|---|---|---|---|
| **Type (N/B/A)** | A | A | A | N | N |
| **Pass/Fail (P/F)** | P | P | P | P | P |
| **Executed Date** | 2026-04-26 | 2026-04-26 | 2026-04-26 | 2026-04-26 | 2026-04-26 |
| **Defect ID** | - | - | - | - | - |

### Summary

| Field | Value |
|------|------|
| Passed | 5 |
| Failed | 0 |
| Untested | 0 |
| Total Test Cases | 5 |
