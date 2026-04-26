,
# Unit Test Report Guideline (Decision Table Format)

This guideline explains how to write a **Unit Test Report (.md)** so that it matches the **Excel Unit Test Sheet (Decision Table)** used in the project.

The goal is to make sure the Markdown report contains enough information so the Excel sheet can be filled directly.

---

# 1. General Information

Each report must include the following information.

| Field | Description |
|------|-------------|
| className | Class containing the function |
| functionName | Function being tested |
| testClass | Unit test class |
| feature | Feature or module name |

Example:

| Field | Value |
|------|------|
| className | JobServiceImpl |
| functionName | applyToJob |
| testClass | JobServiceImplTest |
| feature | job-application |

---

# 2. Test Case List

Each test case must have a **UTCID**.  
These IDs correspond to the **columns in the Excel sheet**.

Example:

| UTCID | Description | Type |
|------|-------------|------|
| UTCID01 | bookingAmount < 0 should return error | A |
| UTCID02 | bookingAmount = 0 with REGULAR customer | N |
| UTCID03 | bookingAmount = 1 with VIP customer | N |
| UTCID04 | bookingAmount = 1 with REGULAR customer | N |

### Test Case Type

| Type | Meaning |
|------|--------|
| N | Normal case |
| B | Boundary case |
| A | Abnormal case |

---

# 3. Header Information

| Function Code | | Function Name | <function_name> |
|---------------|---|---------------|-----------------|
| Created By | | Executed By | |
| Lines of code | | Lack of test cases | |
| Test requirement | <brief description about requirements> | | |

| Passed | Failed | Untested | N/A/B | Total Test Cases |
|--------|--------|----------|-------|------------------|
| <passed> | <failed> | <untested> | <N> <B> <A> | <total> |

# 4. Decision Table Matrix

Create a **matrix table** with UTCIDs as columns.

### CONDITION Section

| Condition | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|-------|---------|---------|---------|---------|
| **Precondition** | | | | | |
| | Can connect server | O | O | O | O |
| **bookingAmount** | | | | | |
| | -1 | O | | | |
| | 0 | | O | | |
| | 1 | | | O | O |
| **customerType** | | | | | |
| | VIP | O | | O | |
| | REGULAR | | O | | O |
| **connectServer** | | | | | |
| | true | O | O | O | O |
| **ownerContext** | | | | | |
| | RECRUITER | O | O | O | O |

**Important:** 
- First row of each condition group: write condition name, leave Value cell empty
- Following rows: leave Condition cell empty, write the value
- This creates a clear visual hierarchy in the Decision Table Matrix
- **ALL conditions must use 2-row format**, even if only 1 value (e.g., ownerContext with RECRUITER)

### CONFIRM Section

| Confirm | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|---------|-------|---------|---------|---------|---------|
| **Return** | | | | | |
| | error | O | | | |
| | success | | O | | O |
| | discount applied | | | O | |
| **Exception** | | | | | |
| | none | O | O | O | O |
| **Log** | | | | | |
| | none | O | O | O | O |

### RESULT Section

| Result Field | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|--------------|---------|---------|---------|---------|
| **Type (N/B/A)** | A | N | N | N |
| **Pass/Fail (P/F)** | P | P | P | P |
| **Executed Date** | 2025-03-15 | 2025-03-15 | 2025-03-15 | 2025-03-15 |
| **Defect ID** | - | - | - | - |

### Summary

| Field | Value |
|------|------|
| Passed | 4 |
| Failed | 0 |
| Untested | 0 |
| Total Test Cases | 4 |

---

# 5. Important Rules

1. Every test case must have a UTCID.
2. Condition names must use real variable names.
3. Each UTCID column must select exactly **one value per condition**.
4. Mark selected values using **O** in the Excel sheet.
5. The Markdown report must contain enough information so the Decision Table can be filled directly.
6. **Merge cells**: For conditions with multiple values, leave the Condition cell empty after the first row (only first row has condition name).
7. UTCIDs are columns, conditions/results are rows in the Decision Table Matrix.

---

# 6. Recommended Workflow

1. Write the test report (.md) with Decision Table Matrix format
2. Define UTCID test cases (columns)
3. Define conditions and expected results (rows)
4. Mark **O** for selected values in the matrix
5. Execute unit tests
6. Fill RESULT section with Pass/Fail status
7. Map to Excel Decision Table

End of guideline.
