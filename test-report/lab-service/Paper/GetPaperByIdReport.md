# Unit Test Report for GetPaperById

# 1. General Information

| Field | Value |
|------|------|
| className | GetPaperByIdHandler |
| functionName | GetPaperById |
| testClass | PaperCommandsIntegrationTests |
| feature | Paper |

# 2. Test Case List

| UTCID | Description | Type |
|------|-------------|------|
| UTCID01 | WithNonExistentId -> ShouldThrowNotFoundException | A |
| UTCID02 | WithExistingPaper -> ShouldReturnMappedResult | N |
| UTCID03 | WithVersions -> ShouldReturnVersionsInResult | N |
| UTCID04 | WithMembers -> ShouldResolveSubProjectId | N |

# 3. Header Information

| Function Code | | Function Name | GetPaperById |
|---------------|---|---------------|-----------------|
| Created By | Auto-Generator | Executed By | Auto-Generator |
| Lines of code | N/A | Lack of test cases | N/A |
| Test requirement | Auto-generated from PaperCommandsIntegrationTests | | |

| Passed | Failed | Untested | N/A/B | Total Test Cases |
|--------|--------|----------|-------|------------------|
| 4 | 0 | 0 | 3 0 1 | 4 |

# 4. Decision Table Matrix

### CONDITION Section

| Condition | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|-------|---|---|---|---|
| **Input** | |  |  |  |  |
| | WithNonExistentId | O |   |   |   |
| | WithExistingPaper |   | O |   |   |
| | WithVersions |   |   | O |   |
| | WithMembers |   |   |   | O |

### CONFIRM Section

| Confirm | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|---------|-------|---|---|---|---|
| **Return/Exception** | |  |  |  |  |
| | ShouldThrowNotFoundException | O |   |   |   |
| | ShouldReturnMappedResult |   | O |   |   |
| | ShouldReturnVersionsInResult |   |   | O |   |
| | ShouldResolveSubProjectId |   |   |   | O |

### RESULT Section

| Result Field | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|--------------|---|---|---|---|
| **Type (N/B/A)** | A | N | N | N |
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
