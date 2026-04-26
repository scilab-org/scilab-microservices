# Unit Test Report for GetTemplates

# 1. General Information

| Field | Value |
|------|------|
| className | GetTemplatesHandler |
| functionName | GetTemplates |
| testClass | TemplateIntegrationTests |
| feature | Template |

# 2. Test Case List

| UTCID | Description | Type |
|------|-------------|------|
| UTCID01 | WithNoFilter -> ShouldReturnAll | N |
| UTCID02 | WithCodeFilter -> ShouldReturnMatchingTemplates | N |
| UTCID03 | WithDescriptionFilter -> ShouldReturnMatchingTemplates | N |

# 3. Header Information

| Function Code | | Function Name | GetTemplates |
|---------------|---|---------------|-----------------|
| Created By | Auto-Generator | Executed By | Auto-Generator |
| Lines of code | N/A | Lack of test cases | N/A |
| Test requirement | Auto-generated from TemplateIntegrationTests | | |

| Passed | Failed | Untested | N/A/B | Total Test Cases |
|--------|--------|----------|-------|------------------|
| 3 | 0 | 0 | 3 0 0 | 3 |

# 4. Decision Table Matrix

### CONDITION Section

| Condition | Value | UTCID01 | UTCID02 | UTCID03 |
|-----------|-------|---|---|---|
| **Input** | |  |  |  |
| | WithNoFilter | O |   |   |
| | WithCodeFilter |   | O |   |
| | WithDescriptionFilter |   |   | O |

### CONFIRM Section

| Confirm | Value | UTCID01 | UTCID02 | UTCID03 |
|---------|-------|---|---|---|
| **Return/Exception** | |  |  |  |
| | ShouldReturnAll | O |   |   |
| | ShouldReturnMatchingTemplates |   | O |   |
| | ShouldReturnMatchingTemplates |   |   | O |

### RESULT Section

| Result Field | UTCID01 | UTCID02 | UTCID03 |
|--------------|---|---|---|
| **Type (N/B/A)** | N | N | N |
| **Pass/Fail (P/F)** | P | P | P |
| **Executed Date** | 2026-04-26 | 2026-04-26 | 2026-04-26 |
| **Defect ID** | - | - | - |

### Summary

| Field | Value |
|------|------|
| Passed | 3 |
| Failed | 0 |
| Untested | 0 |
| Total Test Cases | 3 |
