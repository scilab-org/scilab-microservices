# Unit Test Report for UpsertSection

# 1. General Information

| Field | Value |
|------|------|
| className | UpsertSectionQueryHandler |
| functionName | UpsertSection |
| testClass | SectionQueriesIntegrationTests |
| feature | Section |

# 2. Test Case List

| UTCID | Description | Type |
|------|-------------|------|
| UTCID01 | MainSectionWithNoExistingVersion -> ShouldCreateNewVersion | N |
| UTCID02 | WithNonExistentSection -> ShouldThrowClientValidationException | A |
| UTCID03 | WhenContributorNotFound -> ShouldThrowUnauthorizedException | A |
| UTCID04 | WhenContributorHasSectionReadRole -> ShouldThrowUnauthorizedException | A |
| UTCID05 | MainSectionVersionInitial -> ShouldCreateNewDraftVersion | N |
| UTCID06 | MainSectionContributorAlreadyHasVersion -> ShouldThrowClientValidationException | A |
| UTCID07 | NonMainSection -> ShouldUpdateContentDirectly | N |

# 3. Header Information

| Function Code | | Function Name | UpsertSection |
|---------------|---|---------------|-----------------|
| Created By | Auto-Generator | Executed By | Auto-Generator |
| Lines of code | N/A | Lack of test cases | N/A |
| Test requirement | Auto-generated from SectionQueriesIntegrationTests | | |

| Passed | Failed | Untested | N/A/B | Total Test Cases |
|--------|--------|----------|-------|------------------|
| 7 | 0 | 0 | 3 0 4 | 7 |

# 4. Decision Table Matrix

### CONDITION Section

| Condition | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 | UTCID06 | UTCID07 |
|-----------|-------|---|---|---|---|---|---|---|
| **Input** | |  |  |  |  |  |  |  |
| | MainSectionWithNoExistingVersion | O |   |   |   |   |   |   |
| | WithNonExistentSection |   | O |   |   |   |   |   |
| | WhenContributorNotFound |   |   | O |   |   |   |   |
| | WhenContributorHasSectionReadRole |   |   |   | O |   |   |   |
| | MainSectionVersionInitial |   |   |   |   | O |   |   |
| | MainSectionContributorAlreadyHasVersion |   |   |   |   |   | O |   |
| | NonMainSection |   |   |   |   |   |   | O |

### CONFIRM Section

| Confirm | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 | UTCID06 | UTCID07 |
|---------|-------|---|---|---|---|---|---|---|
| **Return/Exception** | |  |  |  |  |  |  |  |
| | ShouldCreateNewVersion | O |   |   |   |   |   |   |
| | ShouldThrowClientValidationException |   | O |   |   |   |   |   |
| | ShouldThrowUnauthorizedException |   |   | O |   |   |   |   |
| | ShouldThrowUnauthorizedException |   |   |   | O |   |   |   |
| | ShouldCreateNewDraftVersion |   |   |   |   | O |   |   |
| | ShouldThrowClientValidationException |   |   |   |   |   | O |   |
| | ShouldUpdateContentDirectly |   |   |   |   |   |   | O |

### RESULT Section

| Result Field | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 | UTCID06 | UTCID07 |
|--------------|---|---|---|---|---|---|---|
| **Type (N/B/A)** | N | A | A | A | N | A | N |
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
