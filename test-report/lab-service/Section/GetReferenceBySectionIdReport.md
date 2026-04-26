# Unit Test Report for GetReferenceBySectionId

# 1. General Information

| Field | Value |
|------|------|
| className | GetReferenceBySectionIdHandler |
| functionName | GetReferenceBySectionId |
| testClass | SectionIntegrationTests |
| feature | Section |

# 2. Test Case List

| UTCID | Description | Type |
|------|-------------|------|
| UTCID01 | WithEmptyRefs -> ShouldReturnEmptyInUse | A |
| UTCID02 | WithNonExistentSection -> ShouldThrowNotFoundException | A |
| UTCID03 | WithNonExistentSection -> ShouldThrowNotFoundException | A |
| UTCID04 | WhenPaperNotFound -> ShouldThrowNotFoundException | A |
| UTCID05 | WithSectionAndPaper NoReferences -> ShouldReturnEmpty | N |
| UTCID06 | WithInUsePaperBankReferences -> ShouldReturnInUse | N |
| UTCID07 | WithOtherPaperReferences -> ShouldReturnOtherReference | N |

# 3. Header Information

| Function Code | | Function Name | GetReferenceBySectionId |
|---------------|---|---------------|-----------------|
| Created By | Auto-Generator | Executed By | Auto-Generator |
| Lines of code | N/A | Lack of test cases | N/A |
| Test requirement | Auto-generated from SectionIntegrationTests | | |

| Passed | Failed | Untested | N/A/B | Total Test Cases |
|--------|--------|----------|-------|------------------|
| 7 | 0 | 0 | 3 0 4 | 7 |

# 4. Decision Table Matrix

### CONDITION Section

| Condition | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 | UTCID06 | UTCID07 |
|-----------|-------|---|---|---|---|---|---|---|
| **Input** | |  |  |  |  |  |  |  |
| | WithEmptyRefs | O |   |   |   |   |   |   |
| | WithNonExistentSection |   | O |   |   |   |   |   |
| | WithNonExistentSection |   |   | O |   |   |   |   |
| | WhenPaperNotFound |   |   |   | O |   |   |   |
| | WithSectionAndPaper NoReferences |   |   |   |   | O |   |   |
| | WithInUsePaperBankReferences |   |   |   |   |   | O |   |
| | WithOtherPaperReferences |   |   |   |   |   |   | O |

### CONFIRM Section

| Confirm | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 | UTCID06 | UTCID07 |
|---------|-------|---|---|---|---|---|---|---|
| **Return/Exception** | |  |  |  |  |  |  |  |
| | ShouldReturnEmptyInUse | O |   |   |   |   |   |   |
| | ShouldThrowNotFoundException |   | O |   |   |   |   |   |
| | ShouldThrowNotFoundException |   |   | O |   |   |   |   |
| | ShouldThrowNotFoundException |   |   |   | O |   |   |   |
| | ShouldReturnEmpty |   |   |   |   | O |   |   |
| | ShouldReturnInUse |   |   |   |   |   | O |   |
| | ShouldReturnOtherReference |   |   |   |   |   |   | O |

### RESULT Section

| Result Field | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 | UTCID06 | UTCID07 |
|--------------|---|---|---|---|---|---|---|
| **Type (N/B/A)** | A | A | A | A | N | N | N |
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
