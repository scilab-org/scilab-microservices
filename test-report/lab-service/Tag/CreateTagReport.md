# Unit Test Report for CreateTag

# 1. General Information

| Field | Value |
|------|------|
| className | CreateTagCommandHandler |
| functionName | CreateTag |
| testClass | TagIntegrationTests |
| feature | Tag |

# 2. Test Case List

| UTCID | Description | Type |
|------|-------------|------|
| UTCID01 | WithValidName -> ShouldStoreAndReturnId | N |
| UTCID02 | ShouldNormalizeName -> ToLowerCaseTrimmed | N |
| UTCID03 | WithDuplicateName -> ShouldThrowClientValidationException | A |
| UTCID04 | WithDuplicateNameDifferentCase -> ShouldThrowClientValidationException | A |

# 3. Header Information

| Function Code | | Function Name | CreateTag |
|---------------|---|---------------|-----------------|
| Created By | Auto-Generator | Executed By | Auto-Generator |
| Lines of code | N/A | Lack of test cases | N/A |
| Test requirement | Auto-generated from TagIntegrationTests | | |

| Passed | Failed | Untested | N/A/B | Total Test Cases |
|--------|--------|----------|-------|------------------|
| 4 | 0 | 0 | 2 0 2 | 4 |

# 4. Decision Table Matrix

### CONDITION Section

| Condition | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|-------|---|---|---|---|
| **Input** | |  |  |  |  |
| | WithValidName | O |   |   |   |
| | ShouldNormalizeName |   | O |   |   |
| | WithDuplicateName |   |   | O |   |
| | WithDuplicateNameDifferentCase |   |   |   | O |

### CONFIRM Section

| Confirm | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|---------|-------|---|---|---|---|
| **Return/Exception** | |  |  |  |  |
| | ShouldStoreAndReturnId | O |   |   |   |
| | ToLowerCaseTrimmed |   | O |   |   |
| | ShouldThrowClientValidationException |   |   | O |   |
| | ShouldThrowClientValidationException |   |   |   | O |

### RESULT Section

| Result Field | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|--------------|---|---|---|---|
| **Type (N/B/A)** | N | N | A | A |
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
