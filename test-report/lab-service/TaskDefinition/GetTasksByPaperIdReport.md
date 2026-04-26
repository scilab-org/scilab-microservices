# Unit Test Report for GetTasksByPaperId

# 1. General Information

| Field | Value |
|------|------|
| className | GetTasksByPaperIdHandler |
| functionName | GetTasksByPaperId |
| testClass | TaskDefinitionIntegrationTests |
| feature | TaskDefinition |

# 2. Test Case List

| UTCID | Description | Type |
|------|-------------|------|
| UTCID01 | WhenNotMember -> ShouldThrowNoPermissionException | A |
| UTCID02 | WhenNoMembers -> ShouldReturnEmpty | N |
| UTCID03 | WithTasks -> ShouldReturnPagedResults | N |
| UTCID04 | WithStatusFilter -> ShouldReturnFiltered | N |
| UTCID05 | WithAssignedToUserNameFilter -> ShouldReturnFiltered | N |

# 3. Header Information

| Function Code | | Function Name | GetTasksByPaperId |
|---------------|---|---------------|-----------------|
| Created By | Auto-Generator | Executed By | Auto-Generator |
| Lines of code | N/A | Lack of test cases | N/A |
| Test requirement | Auto-generated from TaskDefinitionIntegrationTests | | |

| Passed | Failed | Untested | N/A/B | Total Test Cases |
|--------|--------|----------|-------|------------------|
| 5 | 0 | 0 | 4 0 1 | 5 |

# 4. Decision Table Matrix

### CONDITION Section

| Condition | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 |
|-----------|-------|---|---|---|---|---|
| **Input** | |  |  |  |  |  |
| | WhenNotMember | O |   |   |   |   |
| | WhenNoMembers |   | O |   |   |   |
| | WithTasks |   |   | O |   |   |
| | WithStatusFilter |   |   |   | O |   |
| | WithAssignedToUserNameFilter |   |   |   |   | O |

### CONFIRM Section

| Confirm | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 |
|---------|-------|---|---|---|---|---|
| **Return/Exception** | |  |  |  |  |  |
| | ShouldThrowNoPermissionException | O |   |   |   |   |
| | ShouldReturnEmpty |   | O |   |   |   |
| | ShouldReturnPagedResults |   |   | O |   |   |
| | ShouldReturnFiltered |   |   |   | O |   |
| | ShouldReturnFiltered |   |   |   |   | O |

### RESULT Section

| Result Field | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 |
|--------------|---|---|---|---|---|
| **Type (N/B/A)** | A | N | N | N | N |
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
