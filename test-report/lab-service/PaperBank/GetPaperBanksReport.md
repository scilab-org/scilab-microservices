# Unit Test Report for GetPaperBanks

# 1. General Information

| Field | Value |
|------|------|
| className | GetPaperBanksHandler |
| functionName | GetPaperBanks |
| testClass | PaperBankIntegrationTests |
| feature | PaperBank |

# 2. Test Case List

| UTCID | Description | Type |
|------|-------------|------|
| UTCID01 | WithNoFilter -> ShouldReturnAll | N |
| UTCID02 | WithTitleFilter -> ShouldReturnMatching | N |
| UTCID03 | WithAuthorFilter -> ShouldReturnMatching | N |
| UTCID04 | WithPublisherFilter -> ShouldReturnMatching | N |
| UTCID05 | WithDoiFilter -> ShouldReturnMatching | N |
| UTCID06 | WithFromPublicationDateFilter -> ShouldReturnMatching | N |
| UTCID07 | WithToPublicationDateFilter -> ShouldReturnMatching | N |
| UTCID08 | WithGapTypeIdFilter -> ShouldReturnMatching | N |
| UTCID09 | WithJournalIdFilter -> ShouldReturnMatching | N |
| UTCID10 | WithRankingFilter -> ShouldReturnMatching | N |
| UTCID11 | WithKeywordsFilter -> ShouldReturnMatching | N |
| UTCID12 | WithExistingPaperIdsFilter -> ShouldExcludeThose | N |
| UTCID13 | WithJournalEnrichment -> ShouldPopulateJournalName | N |
| UTCID14 | WithGapTypeEnrichment -> ShouldPopulateGapTypes | N |

# 3. Header Information

| Function Code | | Function Name | GetPaperBanks |
|---------------|---|---------------|-----------------|
| Created By | Auto-Generator | Executed By | Auto-Generator |
| Lines of code | N/A | Lack of test cases | N/A |
| Test requirement | Auto-generated from PaperBankIntegrationTests | | |

| Passed | Failed | Untested | N/A/B | Total Test Cases |
|--------|--------|----------|-------|------------------|
| 14 | 0 | 0 | 14 0 0 | 14 |

# 4. Decision Table Matrix

### CONDITION Section

| Condition | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 | UTCID06 | UTCID07 | UTCID08 | UTCID09 | UTCID10 | UTCID11 | UTCID12 | UTCID13 | UTCID14 |
|-----------|-------|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| **Input** | |  |  |  |  |  |  |  |  |  |  |  |  |  |  |
| | WithNoFilter | O |   |   |   |   |   |   |   |   |   |   |   |   |   |
| | WithTitleFilter |   | O |   |   |   |   |   |   |   |   |   |   |   |   |
| | WithAuthorFilter |   |   | O |   |   |   |   |   |   |   |   |   |   |   |
| | WithPublisherFilter |   |   |   | O |   |   |   |   |   |   |   |   |   |   |
| | WithDoiFilter |   |   |   |   | O |   |   |   |   |   |   |   |   |   |
| | WithFromPublicationDateFilter |   |   |   |   |   | O |   |   |   |   |   |   |   |   |
| | WithToPublicationDateFilter |   |   |   |   |   |   | O |   |   |   |   |   |   |   |
| | WithGapTypeIdFilter |   |   |   |   |   |   |   | O |   |   |   |   |   |   |
| | WithJournalIdFilter |   |   |   |   |   |   |   |   | O |   |   |   |   |   |
| | WithRankingFilter |   |   |   |   |   |   |   |   |   | O |   |   |   |   |
| | WithKeywordsFilter |   |   |   |   |   |   |   |   |   |   | O |   |   |   |
| | WithExistingPaperIdsFilter |   |   |   |   |   |   |   |   |   |   |   | O |   |   |
| | WithJournalEnrichment |   |   |   |   |   |   |   |   |   |   |   |   | O |   |
| | WithGapTypeEnrichment |   |   |   |   |   |   |   |   |   |   |   |   |   | O |

### CONFIRM Section

| Confirm | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 | UTCID06 | UTCID07 | UTCID08 | UTCID09 | UTCID10 | UTCID11 | UTCID12 | UTCID13 | UTCID14 |
|---------|-------|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| **Return/Exception** | |  |  |  |  |  |  |  |  |  |  |  |  |  |  |
| | ShouldReturnAll | O |   |   |   |   |   |   |   |   |   |   |   |   |   |
| | ShouldReturnMatching |   | O |   |   |   |   |   |   |   |   |   |   |   |   |
| | ShouldReturnMatching |   |   | O |   |   |   |   |   |   |   |   |   |   |   |
| | ShouldReturnMatching |   |   |   | O |   |   |   |   |   |   |   |   |   |   |
| | ShouldReturnMatching |   |   |   |   | O |   |   |   |   |   |   |   |   |   |
| | ShouldReturnMatching |   |   |   |   |   | O |   |   |   |   |   |   |   |   |
| | ShouldReturnMatching |   |   |   |   |   |   | O |   |   |   |   |   |   |   |
| | ShouldReturnMatching |   |   |   |   |   |   |   | O |   |   |   |   |   |   |
| | ShouldReturnMatching |   |   |   |   |   |   |   |   | O |   |   |   |   |   |
| | ShouldReturnMatching |   |   |   |   |   |   |   |   |   | O |   |   |   |   |
| | ShouldReturnMatching |   |   |   |   |   |   |   |   |   |   | O |   |   |   |
| | ShouldExcludeThose |   |   |   |   |   |   |   |   |   |   |   | O |   |   |
| | ShouldPopulateJournalName |   |   |   |   |   |   |   |   |   |   |   |   | O |   |
| | ShouldPopulateGapTypes |   |   |   |   |   |   |   |   |   |   |   |   |   | O |

### RESULT Section

| Result Field | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 | UTCID06 | UTCID07 | UTCID08 | UTCID09 | UTCID10 | UTCID11 | UTCID12 | UTCID13 | UTCID14 |
|--------------|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| **Type (N/B/A)** | N | N | N | N | N | N | N | N | N | N | N | N | N | N |
| **Pass/Fail (P/F)** | P | P | P | P | P | P | P | P | P | P | P | P | P | P |
| **Executed Date** | 2026-04-26 | 2026-04-26 | 2026-04-26 | 2026-04-26 | 2026-04-26 | 2026-04-26 | 2026-04-26 | 2026-04-26 | 2026-04-26 | 2026-04-26 | 2026-04-26 | 2026-04-26 | 2026-04-26 | 2026-04-26 |
| **Defect ID** | - | - | - | - | - | - | - | - | - | - | - | - | - | - |

### Summary

| Field | Value |
|------|------|
| Passed | 14 |
| Failed | 0 |
| Untested | 0 |
| Total Test Cases | 14 |
