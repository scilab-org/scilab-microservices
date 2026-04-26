# Unit Test Report for MarkSectionToReview

# 1. General Information

| Field | Value |
|------|------|
| className | MarkSectionToReviewHandler |
| functionName | MarkSectionToReview |
| testClass | SectionCommandsIntegrationTests |
| feature | Section |

# 2. Test Case List

| UTCID | Description | Type |
|------|-------------|------|
| UTCID01 | WhenRoleIsEmpty -> ShouldThrowUnauthorizedException | A |
| UTCID02 | WithNonExistentSection -> ShouldThrowClientValidationException | A |
| UTCID03 | WhenContributorMissingOrReadOnly -> ShouldThrowUnauthorizedException | A |
| UTCID04 | WhenStatusIsNotInProgress -> ShouldThrowClientValidationException | A |
| UTCID05 | WithValidData -> ShouldSetStatusToInReview | N |

# 3. Header Information

| Function Code | | Function Name | MarkSectionToReview |
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
| | WhenRoleIsEmpty | O |   |   |   |   |
| | WithNonExistentSection |   | O |   |   |   |
| | WhenContributorMissingOrReadOnly |   |   | O |   |   |
| | WhenStatusIsNotInProgress |   |   |   | O |   |
| | WithValidData |   |   |   |   | O |

### CONFIRM Section

| Confirm | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 |
|---------|-------|---|---|---|---|---|
| **Return/Exception** | |  |  |  |  |  |
| | ShouldThrowUnauthorizedException | O |   |   |   |   |
| | ShouldThrowClientValidationException |   | O |   |   |   |
| | ShouldThrowUnauthorizedException |   |   | O |   |   |
| | ShouldThrowClientValidationException |   |   |   | O |   |
| | ShouldSetStatusToInReview |   |   |   |   | O |

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
