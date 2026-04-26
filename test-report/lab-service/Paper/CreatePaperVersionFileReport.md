# Unit Test Report for CreatePaperVersionFile

# 1. General Information

| Field | Value |
|------|------|
| className | CreatePaperVersionFileCommandHandler |
| functionName | CreatePaperVersionFile |
| testClass | PaperCommandsIntegrationTests |
| feature | Paper |

# 2. Test Case List

| UTCID | Description | Type |
|------|-------------|------|
| UTCID01 | WithNonExistentPaper -> ShouldThrowNotFoundException | A |
| UTCID02 | WithNonExistentVersion -> ShouldThrowNotFoundException | A |
| UTCID03 | VersionBelongsToDifferentPaper -> ShouldThrowClientValidationException | A |
| UTCID04 | WithValidData -> ShouldStoreAndReturnId | N |

# 3. Header Information

| Function Code | | Function Name | CreatePaperVersionFile |
|---------------|---|---------------|-----------------|
| Created By | Auto-Generator | Executed By | Auto-Generator |
| Lines of code | N/A | Lack of test cases | N/A |
| Test requirement | Auto-generated from PaperCommandsIntegrationTests | | |

| Passed | Failed | Untested | N/A/B | Total Test Cases |
|--------|--------|----------|-------|------------------|
| 4 | 0 | 0 | 1 0 3 | 4 |

# 4. Decision Table Matrix

### CONDITION Section

| Condition | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|-------|---|---|---|---|
| **Input** | |  |  |  |  |
| | WithNonExistentPaper | O |   |   |   |
| | WithNonExistentVersion |   | O |   |   |
| | VersionBelongsToDifferentPaper |   |   | O |   |
| | WithValidData |   |   |   | O |

### CONFIRM Section

| Confirm | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|---------|-------|---|---|---|---|
| **Return/Exception** | |  |  |  |  |
| | ShouldThrowNotFoundException | O |   |   |   |
| | ShouldThrowNotFoundException |   | O |   |   |
| | ShouldThrowClientValidationException |   |   | O |   |
| | ShouldStoreAndReturnId |   |   |   | O |

### RESULT Section

| Result Field | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|--------------|---|---|---|---|
| **Type (N/B/A)** | A | A | A | N |
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
