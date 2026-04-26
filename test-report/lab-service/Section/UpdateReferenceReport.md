# Unit Test Report for UpdateReference

# 1. General Information

| Field | Value |
|------|------|
| className | UpdateReferenceCommandHandler |
| functionName | UpdateReference |
| testClass | SectionCommandsIntegrationTests |
| feature | Section |

# 2. Test Case List

| UTCID | Description | Type |
|------|-------------|------|
| UTCID01 | WhenMemberNotFound -> ShouldThrowUnauthorizedException | A |
| UTCID02 | WithNonExistentSection -> ShouldThrowNotFoundException | A |
| UTCID03 | WhenContributorMissingOrReadOnly -> ShouldThrowUnauthorizedException | A |
| UTCID04 | WhenReferenceMainSectionNotFound -> ShouldThrowNotFoundException | A |

# 3. Header Information

| Function Code | | Function Name | UpdateReference |
|---------------|---|---------------|-----------------|
| Created By | Auto-Generator | Executed By | Auto-Generator |
| Lines of code | N/A | Lack of test cases | N/A |
| Test requirement | Auto-generated from SectionCommandsIntegrationTests | | |

| Passed | Failed | Untested | N/A/B | Total Test Cases |
|--------|--------|----------|-------|------------------|
| 4 | 0 | 0 | 0 0 4 | 4 |

# 4. Decision Table Matrix

### CONDITION Section

| Condition | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|-------|---|---|---|---|
| **Input** | |  |  |  |  |
| | WhenMemberNotFound | O |   |   |   |
| | WithNonExistentSection |   | O |   |   |
| | WhenContributorMissingOrReadOnly |   |   | O |   |
| | WhenReferenceMainSectionNotFound |   |   |   | O |

### CONFIRM Section

| Confirm | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|---------|-------|---|---|---|---|
| **Return/Exception** | |  |  |  |  |
| | ShouldThrowUnauthorizedException | O |   |   |   |
| | ShouldThrowNotFoundException |   | O |   |   |
| | ShouldThrowUnauthorizedException |   |   | O |   |
| | ShouldThrowNotFoundException |   |   |   | O |

### RESULT Section

| Result Field | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|--------------|---|---|---|---|
| **Type (N/B/A)** | A | A | A | A |
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
