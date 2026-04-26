# Unit Test Report for MarkMainSection

# 1. General Information

| Field | Value |
|------|------|
| className | MarkMainSectionHandler |
| functionName | MarkMainSection |
| testClass | SectionCommandsIntegrationTests |
| feature | Section |

# 2. Test Case List

| UTCID | Description | Type |
|------|-------------|------|
| UTCID01 | WhenRoleNotPaperAuthor -> ShouldThrowUnauthorizedException | A |
| UTCID02 | WithNonExistentSection -> ShouldThrowClientValidationException | A |
| UTCID03 | WhenSectionAlreadyMain -> ShouldThrowClientValidationException | A |
| UTCID04 | WhenContributorNotFound -> ShouldThrowClientValidationException | A |
| UTCID05 | WithValidData -> ShouldPromoteChildToMain | N |

# 3. Header Information

| Function Code | | Function Name | MarkMainSection |
|---------------|---|---------------|-----------------|
| Created By | Auto-Generator | Executed By | Auto-Generator |
| Lines of code | N/A | Lack of test cases | N/A |
| Test requirement | Auto-generated from SectionCommandsIntegrationTests | | |

| Passed | Failed | Untested | N/A/B | Total Test Cases |
|--------|--------|----------|-------|------------------|
| 5 | 0 | 0 | 1 0 4 | 5 |

# 4. Decision Table Matrix

### CONDITION Section

| Condition | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 |
|-----------|-------|---|---|---|---|---|
| **Input** | |  |  |  |  |  |
| | WhenRoleNotPaperAuthor | O |   |   |   |   |
| | WithNonExistentSection |   | O |   |   |   |
| | WhenSectionAlreadyMain |   |   | O |   |   |
| | WhenContributorNotFound |   |   |   | O |   |
| | WithValidData |   |   |   |   | O |

### CONFIRM Section

| Confirm | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 |
|---------|-------|---|---|---|---|---|
| **Return/Exception** | |  |  |  |  |  |
| | ShouldThrowUnauthorizedException | O |   |   |   |   |
| | ShouldThrowClientValidationException |   | O |   |   |   |
| | ShouldThrowClientValidationException |   |   | O |   |   |
| | ShouldThrowClientValidationException |   |   |   | O |   |
| | ShouldPromoteChildToMain |   |   |   |   | O |

### RESULT Section

| Result Field | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 |
|--------------|---|---|---|---|---|
| **Type (N/B/A)** | A | A | A | A | N |
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
