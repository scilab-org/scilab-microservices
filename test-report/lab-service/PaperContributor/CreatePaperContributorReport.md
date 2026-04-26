# Unit Test Report for CreatePaperContributor

# 1. General Information

| Field | Value |
|------|------|
| className | CreatePaperContributorCommandHandler |
| functionName | CreatePaperContributor |
| testClass | PaperContributorIntegrationTests |
| feature | PaperContributor |

# 2. Test Case List

| UTCID | Description | Type |
|------|-------------|------|
| UTCID01 | WithPaperAuthorRole -> ShouldStoreAndReturnIds | N |
| UTCID02 | WithMultipleMembers -> ShouldCreateOnePerMember | N |
| UTCID03 | WithNonPaperAuthorRole AndReferenceSection -> ShouldCreateRefContributor | N |
| UTCID04 | WithNonPaperAuthorRole AlreadyAssignedToReference -> ShouldNotDuplicate | N |

# 3. Header Information

| Function Code | | Function Name | CreatePaperContributor |
|---------------|---|---------------|-----------------|
| Created By | Auto-Generator | Executed By | Auto-Generator |
| Lines of code | N/A | Lack of test cases | N/A |
| Test requirement | Auto-generated from PaperContributorIntegrationTests | | |

| Passed | Failed | Untested | N/A/B | Total Test Cases |
|--------|--------|----------|-------|------------------|
| 4 | 0 | 0 | 4 0 0 | 4 |

# 4. Decision Table Matrix

### CONDITION Section

| Condition | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|-------|---|---|---|---|
| **Input** | |  |  |  |  |
| | WithPaperAuthorRole | O |   |   |   |
| | WithMultipleMembers |   | O |   |   |
| | WithNonPaperAuthorRole AndReferenceSection |   |   | O |   |
| | WithNonPaperAuthorRole AlreadyAssignedToReference |   |   |   | O |

### CONFIRM Section

| Confirm | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|---------|-------|---|---|---|---|
| **Return/Exception** | |  |  |  |  |
| | ShouldStoreAndReturnIds | O |   |   |   |
| | ShouldCreateOnePerMember |   | O |   |   |
| | ShouldCreateRefContributor |   |   | O |   |
| | ShouldNotDuplicate |   |   |   | O |

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
