# Unit Test Report for GetUserAffiliations

# 1. General Information

| Field | Value |
|------|------|
| className | GetUserAffiliationsHandler |
| functionName | GetUserAffiliations |
| testClass | UserAffiliationIntegrationTests |
| feature | UserAffiliation |

# 2. Test Case List

| UTCID | Description | Type |
|------|-------------|------|
| UTCID01 | WithNoAffiliations -> ShouldReturnEmptyList | N |
| UTCID02 | WhenAffiliationEntityExists -> ShouldReturnMappedAffiliation | N |
| UTCID03 | WhenAffiliationEntityNotFound -> ShouldReturnNullAffiliation | N |
| UTCID04 | ShouldReturnOnlyForRequestedUser -> Default | N |

# 3. Header Information

| Function Code | | Function Name | GetUserAffiliations |
|---------------|---|---------------|-----------------|
| Created By | Auto-Generator | Executed By | Auto-Generator |
| Lines of code | N/A | Lack of test cases | N/A |
| Test requirement | Auto-generated from UserAffiliationIntegrationTests | | |

| Passed | Failed | Untested | N/A/B | Total Test Cases |
|--------|--------|----------|-------|------------------|
| 4 | 0 | 0 | 4 0 0 | 4 |

# 4. Decision Table Matrix

### CONDITION Section

| Condition | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|-------|---|---|---|---|
| **Input** | |  |  |  |  |
| | WithNoAffiliations | O |   |   |   |
| | WhenAffiliationEntityExists |   | O |   |   |
| | WhenAffiliationEntityNotFound |   |   | O |   |
| | ShouldReturnOnlyForRequestedUser |   |   |   | O |

### CONFIRM Section

| Confirm | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|---------|-------|---|---|---|---|
| **Return/Exception** | |  |  |  |  |
| | ShouldReturnEmptyList | O |   |   |   |
| | ShouldReturnMappedAffiliation |   | O |   |   |
| | ShouldReturnNullAffiliation |   |   | O |   |
| | Default |   |   |   | O |

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
