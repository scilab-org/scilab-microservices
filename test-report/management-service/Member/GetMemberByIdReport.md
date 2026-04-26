# Unit Test Report for GetMemberById

# 1. General Information

| Field | Value |
|------|------|
| className | GetMemberByIdQueryHandler |
| functionName | GetMemberById |
| testClass | GetMemberByIdQueryHandlerTests |
| feature | Member |

# 2. Test Case List

| UTCID | Description | Type |
|------|-------------|------|
| UTCID01 | MemberNotFound -> ReturnsEmptyMemberDto | N |
| UTCID02 | MemberFound -> ReturnsMappedDto | N |

# 3. Header Information

| Function Code | | Function Name | GetMemberById |
|---------------|---|---------------|-----------------|
| Created By | Auto-Generator | Executed By | Auto-Generator |
| Lines of code | N/A | Lack of test cases | N/A |
| Test requirement | Auto-generated from GetMemberByIdQueryHandlerTests | | |

| Passed | Failed | Untested | N/A/B | Total Test Cases |
|--------|--------|----------|-------|------------------|
| 2 | 0 | 0 | 2 0 0 | 2 |

# 4. Decision Table Matrix

### CONDITION Section

| Condition | Value | UTCID01 | UTCID02 |
|-----------|-------|---|---|
| **Input** | |  |  |
| | MemberNotFound | O |   |
| | MemberFound |   | O |

### CONFIRM Section

| Confirm | Value | UTCID01 | UTCID02 |
|---------|-------|---|---|
| **Return/Exception** | |  |  |
| | ReturnsEmptyMemberDto | O |   |
| | ReturnsMappedDto |   | O |

### RESULT Section

| Result Field | UTCID01 | UTCID02 |
|--------------|---|---|
| **Type (N/B/A)** | N | N |
| **Pass/Fail (P/F)** | P | P |
| **Executed Date** | 2026-04-26 | 2026-04-26 |
| **Defect ID** | - | - |

### Summary

| Field | Value |
|------|------|
| Passed | 2 |
| Failed | 0 |
| Untested | 0 |
| Total Test Cases | 2 |
