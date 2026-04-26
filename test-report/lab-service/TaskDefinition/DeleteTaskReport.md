# Unit Test Report for DeleteTask

# 1. General Information

| Field | Value |
|------|------|
| className | DeleteTaskCommandHandler |
| functionName | DeleteTask |
| testClass | TaskDefinitionIntegrationTests |
| feature | TaskDefinition |

# 2. Test Case List

| UTCID | Description | Type |
|------|-------------|------|
| UTCID01 | WithNonExistentId -> ShouldThrowNotFoundException | A |
| UTCID02 | WhenNotCreator -> ShouldThrowNoPermissionException | A |
| UTCID03 | NonWritingType -> ShouldDeleteTask | N |
| UTCID04 | WritingType WithNoContributor -> ShouldDeleteTask | N |
| UTCID05 | WritingType WithContributor -> ShouldRemoveTaskFromContributorAndDelete | N |

# 3. Header Information

| Function Code | | Function Name | DeleteTask |
|---------------|---|---------------|-----------------|
| Created By | Auto-Generator | Executed By | Auto-Generator |
| Lines of code | N/A | Lack of test cases | N/A |
| Test requirement | Auto-generated from TaskDefinitionIntegrationTests | | |

| Passed | Failed | Untested | N/A/B | Total Test Cases |
|--------|--------|----------|-------|------------------|
| 5 | 0 | 0 | 3 0 2 | 5 |

# 4. Decision Table Matrix

### CONDITION Section

| Condition | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 |
|-----------|-------|---|---|---|---|---|
| **Input** | |  |  |  |  |  |
| | WithNonExistentId | O |   |   |   |   |
| | WhenNotCreator |   | O |   |   |   |
| | NonWritingType |   |   | O |   |   |
| | WritingType WithNoContributor |   |   |   | O |   |
| | WritingType WithContributor |   |   |   |   | O |

### CONFIRM Section

| Confirm | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 |
|---------|-------|---|---|---|---|---|
| **Return/Exception** | |  |  |  |  |  |
| | ShouldThrowNotFoundException | O |   |   |   |   |
| | ShouldThrowNoPermissionException |   | O |   |   |   |
| | ShouldDeleteTask |   |   | O |   |   |
| | ShouldDeleteTask |   |   |   | O |   |
| | ShouldRemoveTaskFromContributorAndDelete |   |   |   |   | O |

### RESULT Section

| Result Field | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 |
|--------------|---|---|---|---|---|
| **Type (N/B/A)** | A | A | N | N | N |
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
