# Unit Test Report for GetMemberAffiliations

# 1. General Information

| Field | Value |
|------|------|
| className | GetMemberAffiliationsHandler |
| functionName | GetMemberAffiliations |
| testClass | UserAffiliationIntegrationTests |
| feature | UserAffiliation |

# 2. Test Case List

| UTCID | Description | Type |
|------|-------------|------|
| UTCID01 | MemberNotFound -> ShouldThrowClientValidationException | A |
| UTCID02 | WithNoUserAffiliations -> ShouldReturnEmptyResult | N |
| UTCID03 | WithNoNameFilter -> ShouldReturnAllAffiliations | N |
| UTCID04 | WithNoNameFilter WhenAffiliationEntityMissing -> ShouldIncludeWithNullAffiliation | N |
| UTCID05 | WithNameFilterMatching -> ShouldReturnFilteredAffiliations | N |
| UTCID06 | WithNameFilterNotMatching -> ShouldExcludeEntry | N |
| UTCID07 | WithNameFilter WhenAffiliationEntityMissing -> ShouldSkipEntry | N |
| UTCID08 | WithNameFilter WhenAffiliationNameIsNull -> ShouldSkipEntry | A |
| UTCID09 | WithPagination -> ShouldReturnPagedResults | N |

# 3. Header Information

| Function Code | | Function Name | GetMemberAffiliations |
|---------------|---|---------------|-----------------|
| Created By | Auto-Generator | Executed By | Auto-Generator |
| Lines of code | N/A | Lack of test cases | N/A |
| Test requirement | Auto-generated from UserAffiliationIntegrationTests | | |

| Passed | Failed | Untested | N/A/B | Total Test Cases |
|--------|--------|----------|-------|------------------|
| 9 | 0 | 0 | 7 0 2 | 9 |

# 4. Decision Table Matrix

### CONDITION Section

| Condition | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 | UTCID06 | UTCID07 | UTCID08 | UTCID09 |
|-----------|-------|---|---|---|---|---|---|---|---|---|
| **Input** | |  |  |  |  |  |  |  |  |  |
| | MemberNotFound | O |   |   |   |   |   |   |   |   |
| | WithNoUserAffiliations |   | O |   |   |   |   |   |   |   |
| | WithNoNameFilter |   |   | O |   |   |   |   |   |   |
| | WithNoNameFilter WhenAffiliationEntityMissing |   |   |   | O |   |   |   |   |   |
| | WithNameFilterMatching |   |   |   |   | O |   |   |   |   |
| | WithNameFilterNotMatching |   |   |   |   |   | O |   |   |   |
| | WithNameFilter WhenAffiliationEntityMissing |   |   |   |   |   |   | O |   |   |
| | WithNameFilter WhenAffiliationNameIsNull |   |   |   |   |   |   |   | O |   |
| | WithPagination |   |   |   |   |   |   |   |   | O |

### CONFIRM Section

| Confirm | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 | UTCID06 | UTCID07 | UTCID08 | UTCID09 |
|---------|-------|---|---|---|---|---|---|---|---|---|
| **Return/Exception** | |  |  |  |  |  |  |  |  |  |
| | ShouldThrowClientValidationException | O |   |   |   |   |   |   |   |   |
| | ShouldReturnEmptyResult |   | O |   |   |   |   |   |   |   |
| | ShouldReturnAllAffiliations |   |   | O |   |   |   |   |   |   |
| | ShouldIncludeWithNullAffiliation |   |   |   | O |   |   |   |   |   |
| | ShouldReturnFilteredAffiliations |   |   |   |   | O |   |   |   |   |
| | ShouldExcludeEntry |   |   |   |   |   | O |   |   |   |
| | ShouldSkipEntry |   |   |   |   |   |   | O |   |   |
| | ShouldSkipEntry |   |   |   |   |   |   |   | O |   |
| | ShouldReturnPagedResults |   |   |   |   |   |   |   |   | O |

### RESULT Section

| Result Field | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 | UTCID06 | UTCID07 | UTCID08 | UTCID09 |
|--------------|---|---|---|---|---|---|---|---|---|
| **Type (N/B/A)** | A | N | N | N | N | N | N | A | N |
| **Pass/Fail (P/F)** | P | P | P | P | P | P | P | P | P |
| **Executed Date** | 2026-04-26 | 2026-04-26 | 2026-04-26 | 2026-04-26 | 2026-04-26 | 2026-04-26 | 2026-04-26 | 2026-04-26 | 2026-04-26 |
| **Defect ID** | - | - | - | - | - | - | - | - | - |

### Summary

| Field | Value |
|------|------|
| Passed | 9 |
| Failed | 0 |
| Untested | 0 |
| Total Test Cases | 9 |
