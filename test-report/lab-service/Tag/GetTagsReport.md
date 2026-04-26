# Unit Test Report for GetTags

# 1. General Information

| Field | Value |
|------|------|
| className | GetTagsHandler |
| functionName | GetTags |
| testClass | TagIntegrationTests |
| feature | Tag |

# 2. Test Case List

| UTCID | Description | Type |
|------|-------------|------|
| UTCID01 | WithNoFilter -> ShouldReturnAllTags | N |
| UTCID02 | WithNameFilter -> ShouldReturnMatchingTags | N |
| UTCID03 | WithPagination -> ShouldReturnPagedResults | N |
| UTCID04 | WithIsDeletedFilter -> ShouldReturnDeletedTags | N |

# 3. Header Information

| Function Code | | Function Name | GetTags |
|---------------|---|---------------|-----------------|
| Created By | Auto-Generator | Executed By | Auto-Generator |
| Lines of code | N/A | Lack of test cases | N/A |
| Test requirement | Auto-generated from TagIntegrationTests | | |

| Passed | Failed | Untested | N/A/B | Total Test Cases |
|--------|--------|----------|-------|------------------|
| 4 | 0 | 0 | 4 0 0 | 4 |

# 4. Decision Table Matrix

### CONDITION Section

| Condition | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|-------|---|---|---|---|
| **Input** | |  |  |  |  |
| | WithNoFilter | O |   |   |   |
| | WithNameFilter |   | O |   |   |
| | WithPagination |   |   | O |   |
| | WithIsDeletedFilter |   |   |   | O |

### CONFIRM Section

| Confirm | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|---------|-------|---|---|---|---|
| **Return/Exception** | |  |  |  |  |
| | ShouldReturnAllTags | O |   |   |   |
| | ShouldReturnMatchingTags |   | O |   |   |
| | ShouldReturnPagedResults |   |   | O |   |
| | ShouldReturnDeletedTags |   |   |   | O |

### RESULT Section

| Result Field | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|--------------|---|---|---|---|
| **Type (N/B/A)** | N | N | N | N |
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
