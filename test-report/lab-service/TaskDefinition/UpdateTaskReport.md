# Unit Test Report for UpdateTask

# 1. General Information

| Field | Value |
|------|------|
| className | UpdateTaskCommandHandler |
| functionName | UpdateTask |
| testClass | TaskDefinitionIntegrationTests |
| feature | TaskDefinition |

# 2. Test Case List

| UTCID | Description | Type |
|------|-------------|------|
| UTCID01 | WithNonExistentId -> ShouldThrowNotFoundException | A |
| UTCID02 | WhenAssigneeUpdates -> ShouldUpdateWithoutRoleCheck | N |
| UTCID03 | WhenNotAssigneeAndIsAuthor -> ShouldUpdate | N |
| UTCID04 | WhenNotAssigneeAndMemberNotFound -> ShouldThrowNoPermissionException | A |
| UTCID05 | WhenNotAssigneeAndNotAuthorRole -> ShouldThrowNoPermissionException | A |
| UTCID06 | WhenCompletedStatus -> ShouldSetCompleteDate | N |
| UTCID07 | WhenStatusChangedFromCompleted -> ShouldClearCompleteDate | N |

# 3. Header Information

| Function Code | | Function Name | UpdateTask |
|---------------|---|---------------|-----------------|
| Created By | Auto-Generator | Executed By | Auto-Generator |
| Lines of code | N/A | Lack of test cases | N/A |
| Test requirement | Auto-generated from TaskDefinitionIntegrationTests | | |

| Passed | Failed | Untested | N/A/B | Total Test Cases |
|--------|--------|----------|-------|------------------|
| 7 | 0 | 0 | 4 0 3 | 7 |

# 4. Decision Table Matrix

### CONDITION Section

| Condition | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 | UTCID06 | UTCID07 |
|-----------|-------|---|---|---|---|---|---|---|
| **Input** | |  |  |  |  |  |  |  |
| | WithNonExistentId | O |   |   |   |   |   |   |
| | WhenAssigneeUpdates |   | O |   |   |   |   |   |
| | WhenNotAssigneeAndIsAuthor |   |   | O |   |   |   |   |
| | WhenNotAssigneeAndMemberNotFound |   |   |   | O |   |   |   |
| | WhenNotAssigneeAndNotAuthorRole |   |   |   |   | O |   |   |
| | WhenCompletedStatus |   |   |   |   |   | O |   |
| | WhenStatusChangedFromCompleted |   |   |   |   |   |   | O |

### CONFIRM Section

| Confirm | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 | UTCID06 | UTCID07 |
|---------|-------|---|---|---|---|---|---|---|
| **Return/Exception** | |  |  |  |  |  |  |  |
| | ShouldThrowNotFoundException | O |   |   |   |   |   |   |
| | ShouldUpdateWithoutRoleCheck |   | O |   |   |   |   |   |
| | ShouldUpdate |   |   | O |   |   |   |   |
| | ShouldThrowNoPermissionException |   |   |   | O |   |   |   |
| | ShouldThrowNoPermissionException |   |   |   |   | O |   |   |
| | ShouldSetCompleteDate |   |   |   |   |   | O |   |
| | ShouldClearCompleteDate |   |   |   |   |   |   | O |

### RESULT Section

| Result Field | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 | UTCID06 | UTCID07 |
|--------------|---|---|---|---|---|---|---|
| **Type (N/B/A)** | A | N | N | A | A | N | N |
| **Pass/Fail (P/F)** | P | P | P | P | P | P | P |
| **Executed Date** | 2026-04-26 | 2026-04-26 | 2026-04-26 | 2026-04-26 | 2026-04-26 | 2026-04-26 | 2026-04-26 |
| **Defect ID** | - | - | - | - | - | - | - |

### Summary

| Field | Value |
|------|------|
| Passed | 7 |
| Failed | 0 |
| Untested | 0 |
| Total Test Cases | 7 |
