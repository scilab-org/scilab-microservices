# Unit Test Report for GetMyTask

# 1. General Information

| Field | Value |
|------|------|
| className | GetMyTaskHandler |
| functionName | GetMyTask |
| testClass | TaskDefinitionIntegrationTests |
| feature | TaskDefinition |

# 2. Test Case List

| UTCID | Description | Type |
|------|-------------|------|
| UTCID01 | WhenUserNotResolved -> ShouldReturnEmpty | N |
| UTCID02 | WithNoMatchingTasks -> ShouldReturnEmpty | N |
| UTCID03 | WithMatchingTasks -> ShouldReturnItems | N |
| UTCID04 | WithStatusFilter -> ShouldReturnFiltered | N |
| UTCID05 | WithDateFilters -> ShouldReturnFiltered | N |
| UTCID06 | WithPaperIdFilter -> ShouldReturnFiltered | N |

# 3. Header Information

| Function Code | | Function Name | GetMyTask |
|---------------|---|---------------|-----------------|
| Created By | Auto-Generator | Executed By | Auto-Generator |
| Lines of code | N/A | Lack of test cases | N/A |
| Test requirement | Auto-generated from TaskDefinitionIntegrationTests | | |

| Passed | Failed | Untested | N/A/B | Total Test Cases |
|--------|--------|----------|-------|------------------|
| 6 | 0 | 0 | 6 0 0 | 6 |

# 4. Decision Table Matrix

### CONDITION Section

| Condition | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 | UTCID06 |
|-----------|-------|---|---|---|---|---|---|
| **Input** | |  |  |  |  |  |  |
| | WhenUserNotResolved | O |   |   |   |   |   |
| | WithNoMatchingTasks |   | O |   |   |   |   |
| | WithMatchingTasks |   |   | O |   |   |   |
| | WithStatusFilter |   |   |   | O |   |   |
| | WithDateFilters |   |   |   |   | O |   |
| | WithPaperIdFilter |   |   |   |   |   | O |

### CONFIRM Section

| Confirm | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 | UTCID06 |
|---------|-------|---|---|---|---|---|---|
| **Return/Exception** | |  |  |  |  |  |  |
| | ShouldReturnEmpty | O |   |   |   |   |   |
| | ShouldReturnEmpty |   | O |   |   |   |   |
| | ShouldReturnItems |   |   | O |   |   |   |
| | ShouldReturnFiltered |   |   |   | O |   |   |
| | ShouldReturnFiltered |   |   |   |   | O |   |
| | ShouldReturnFiltered |   |   |   |   |   | O |

### RESULT Section

| Result Field | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 | UTCID06 |
|--------------|---|---|---|---|---|---|
| **Type (N/B/A)** | N | N | N | N | N | N |
| **Pass/Fail (P/F)** | P | P | P | P | P | P |
| **Executed Date** | 2026-04-26 | 2026-04-26 | 2026-04-26 | 2026-04-26 | 2026-04-26 | 2026-04-26 |
| **Defect ID** | - | - | - | - | - | - |

### Summary

| Field | Value |
|------|------|
| Passed | 6 |
| Failed | 0 |
| Untested | 0 |
| Total Test Cases | 6 |
