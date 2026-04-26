# Unit Test Report for GetJournalById

# 1. General Information

| Field | Value |
|------|------|
| className | GetJournalByIdQueryHandler |
| functionName | GetJournalById |
| testClass | JournalQueriesIntegrationTests |
| feature | Journal |

# 2. Test Case List

| UTCID | Description | Type |
|------|-------------|------|
| UTCID01 | WithNonExistentId -> ShouldThrowNotFoundException | A |
| UTCID02 | WithExistingJournal NoAssociations -> ShouldReturnMappedResult | N |
| UTCID03 | WithTemplates -> ShouldPopulateTemplates | N |
| UTCID04 | WithProjects -> ShouldPopulateProjects | N |
| UTCID05 | WithProjectIds WhenProjectNotReturnedByService -> ShouldReturnEmpty | N |
| UTCID06 | WithPapers -> ShouldPopulatePapers | N |
| UTCID07 | WithPaperIdNotInDb -> ShouldReturnEmptyPapers | N |
| UTCID08 | WithExisting -> ShouldReturnFullResult | N |
| UTCID09 | WithNonExistent -> ShouldThrowNotFoundException | A |

# 3. Header Information

| Function Code | | Function Name | GetJournalById |
|---------------|---|---------------|-----------------|
| Created By | Auto-Generator | Executed By | Auto-Generator |
| Lines of code | N/A | Lack of test cases | N/A |
| Test requirement | Auto-generated from JournalQueriesIntegrationTests | | |

| Passed | Failed | Untested | N/A/B | Total Test Cases |
|--------|--------|----------|-------|------------------|
| 9 | 0 | 0 | 7 0 2 | 9 |

# 4. Decision Table Matrix

### CONDITION Section

| Condition | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 | UTCID06 | UTCID07 | UTCID08 | UTCID09 |
|-----------|-------|---|---|---|---|---|---|---|---|---|
| **Input** | |  |  |  |  |  |  |  |  |  |
| | WithNonExistentId | O |   |   |   |   |   |   |   |   |
| | WithExistingJournal NoAssociations |   | O |   |   |   |   |   |   |   |
| | WithTemplates |   |   | O |   |   |   |   |   |   |
| | WithProjects |   |   |   | O |   |   |   |   |   |
| | WithProjectIds WhenProjectNotReturnedByService |   |   |   |   | O |   |   |   |   |
| | WithPapers |   |   |   |   |   | O |   |   |   |
| | WithPaperIdNotInDb |   |   |   |   |   |   | O |   |   |
| | WithExisting |   |   |   |   |   |   |   | O |   |
| | WithNonExistent |   |   |   |   |   |   |   |   | O |

### CONFIRM Section

| Confirm | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 | UTCID06 | UTCID07 | UTCID08 | UTCID09 |
|---------|-------|---|---|---|---|---|---|---|---|---|
| **Return/Exception** | |  |  |  |  |  |  |  |  |  |
| | ShouldThrowNotFoundException | O |   |   |   |   |   |   |   |   |
| | ShouldReturnMappedResult |   | O |   |   |   |   |   |   |   |
| | ShouldPopulateTemplates |   |   | O |   |   |   |   |   |   |
| | ShouldPopulateProjects |   |   |   | O |   |   |   |   |   |
| | ShouldReturnEmpty |   |   |   |   | O |   |   |   |   |
| | ShouldPopulatePapers |   |   |   |   |   | O |   |   |   |
| | ShouldReturnEmptyPapers |   |   |   |   |   |   | O |   |   |
| | ShouldReturnFullResult |   |   |   |   |   |   |   | O |   |
| | ShouldThrowNotFoundException |   |   |   |   |   |   |   |   | O |

### RESULT Section

| Result Field | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 | UTCID06 | UTCID07 | UTCID08 | UTCID09 |
|--------------|---|---|---|---|---|---|---|---|---|
| **Type (N/B/A)** | A | N | N | N | N | N | N | N | A |
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
