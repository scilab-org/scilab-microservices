# Unit Test Report for GetDomains

# 1. General Information

| Field | Value |
|------|------|
| className | GetDomainsHandler |
| functionName | GetDomains |
| testClass | DomainIntegrationTests |
| feature | Domain |

# 2. Test Case List

| UTCID | Description | Type |
|------|-------------|------|
| UTCID01 | WithNoFilter -> ShouldReturnAllDomains | N |
| UTCID02 | WithNameFilter -> ShouldReturnMatchingDomains | N |
| UTCID03 | WithWhitespaceNameFilter -> ShouldReturnAllDomains | N |
| UTCID04 | WithPagination -> ShouldReturnPagedResults | N |

# 3. Header Information

| Function Code | | Function Name | GetDomains |
|---------------|---|---------------|-----------------|
| Created By | Auto-Generator | Executed By | Auto-Generator |
| Lines of code | N/A | Lack of test cases | N/A |
| Test requirement | Auto-generated from DomainIntegrationTests | | |

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
| | WithWhitespaceNameFilter |   |   | O |   |
| | WithPagination |   |   |   | O |

### CONFIRM Section

| Confirm | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|---------|-------|---|---|---|---|
| **Return/Exception** | |  |  |  |  |
| | ShouldReturnAllDomains | O |   |   |   |
| | ShouldReturnMatchingDomains |   | O |   |   |
| | ShouldReturnAllDomains |   |   | O |   |
| | ShouldReturnPagedResults |   |   |   | O |

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
