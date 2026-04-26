# Unit Test Report for CreateTask

# 1. General Information

| Field | Value |
|------|------|
| className | CreateTaskCommandHandler |
| functionName | CreateTask |
| testClass | TaskDefinitionIntegrationTests |
| feature | TaskDefinition |

# 2. Test Case List

| UTCID | Description | Type |
|------|-------------|------|
| UTCID01 | WithNonExistentPaper -> ShouldThrowNotFoundException | A |
| UTCID02 | WhenUserNotMember -> ShouldThrowNotFoundException | A |
| UTCID03 | WhenNotPaperAuthorRole -> ShouldThrowNoPermissionException | A |
| UTCID04 | WhenRoleIsNull -> ShouldThrowNoPermissionException | A |
| UTCID05 | WhenAssignedMemberNotFound -> ShouldThrowNotFoundException | A |
| UTCID06 | WithValidData -> ShouldStoreAndReturnId | N |
| UTCID07 | WhenUserNotInUserService -> ShouldStoreWithNullUsername | N |
| UTCID08 | WithSectionId SectionNotInDb -> ShouldThrowNotFoundException | A |
| UTCID09 | WithSectionId SectionBelongsToDifferentPaper -> ShouldThrowNotFoundException | A |
| UTCID10 | WithSectionId ExistingContributor -> ShouldAddTaskToContributor | N |
| UTCID11 | WithSectionId NoExistingContributor -> ShouldCreateContributorAndLinkTask | N |

# 3. Header Information

| Function Code | | Function Name | CreateTask |
|---------------|---|---------------|-----------------|
| Created By | Auto-Generator | Executed By | Auto-Generator |
| Lines of code | N/A | Lack of test cases | N/A |
| Test requirement | Auto-generated from TaskDefinitionIntegrationTests | | |

| Passed | Failed | Untested | N/A/B | Total Test Cases |
|--------|--------|----------|-------|------------------|
| 11 | 0 | 0 | 4 0 7 | 11 |

# 4. Decision Table Matrix

### CONDITION Section

| Condition | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 | UTCID06 | UTCID07 | UTCID08 | UTCID09 | UTCID10 | UTCID11 |
|-----------|-------|---|---|---|---|---|---|---|---|---|---|---|
| **Input** | |  |  |  |  |  |  |  |  |  |  |  |
| | WithNonExistentPaper | O |   |   |   |   |   |   |   |   |   |   |
| | WhenUserNotMember |   | O |   |   |   |   |   |   |   |   |   |
| | WhenNotPaperAuthorRole |   |   | O |   |   |   |   |   |   |   |   |
| | WhenRoleIsNull |   |   |   | O |   |   |   |   |   |   |   |
| | WhenAssignedMemberNotFound |   |   |   |   | O |   |   |   |   |   |   |
| | WithValidData |   |   |   |   |   | O |   |   |   |   |   |
| | WhenUserNotInUserService |   |   |   |   |   |   | O |   |   |   |   |
| | WithSectionId SectionNotInDb |   |   |   |   |   |   |   | O |   |   |   |
| | WithSectionId SectionBelongsToDifferentPaper |   |   |   |   |   |   |   |   | O |   |   |
| | WithSectionId ExistingContributor |   |   |   |   |   |   |   |   |   | O |   |
| | WithSectionId NoExistingContributor |   |   |   |   |   |   |   |   |   |   | O |

### CONFIRM Section

| Confirm | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 | UTCID06 | UTCID07 | UTCID08 | UTCID09 | UTCID10 | UTCID11 |
|---------|-------|---|---|---|---|---|---|---|---|---|---|---|
| **Return/Exception** | |  |  |  |  |  |  |  |  |  |  |  |
| | ShouldThrowNotFoundException | O |   |   |   |   |   |   |   |   |   |   |
| | ShouldThrowNotFoundException |   | O |   |   |   |   |   |   |   |   |   |
| | ShouldThrowNoPermissionException |   |   | O |   |   |   |   |   |   |   |   |
| | ShouldThrowNoPermissionException |   |   |   | O |   |   |   |   |   |   |   |
| | ShouldThrowNotFoundException |   |   |   |   | O |   |   |   |   |   |   |
| | ShouldStoreAndReturnId |   |   |   |   |   | O |   |   |   |   |   |
| | ShouldStoreWithNullUsername |   |   |   |   |   |   | O |   |   |   |   |
| | ShouldThrowNotFoundException |   |   |   |   |   |   |   | O |   |   |   |
| | ShouldThrowNotFoundException |   |   |   |   |   |   |   |   | O |   |   |
| | ShouldAddTaskToContributor |   |   |   |   |   |   |   |   |   | O |   |
| | ShouldCreateContributorAndLinkTask |   |   |   |   |   |   |   |   |   |   | O |

### RESULT Section

| Result Field | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 | UTCID06 | UTCID07 | UTCID08 | UTCID09 | UTCID10 | UTCID11 |
|--------------|---|---|---|---|---|---|---|---|---|---|---|
| **Type (N/B/A)** | A | A | A | A | A | N | N | A | A | N | N |
| **Pass/Fail (P/F)** | P | P | P | P | P | P | P | P | P | P | P |
| **Executed Date** | 2026-04-26 | 2026-04-26 | 2026-04-26 | 2026-04-26 | 2026-04-26 | 2026-04-26 | 2026-04-26 | 2026-04-26 | 2026-04-26 | 2026-04-26 | 2026-04-26 |
| **Defect ID** | - | - | - | - | - | - | - | - | - | - | - |

### Summary

| Field | Value |
|------|------|
| Passed | 11 |
| Failed | 0 |
| Untested | 0 |
| Total Test Cases | 11 |
