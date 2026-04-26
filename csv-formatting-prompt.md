# Convert Unit Test Report to CSV Format

This document contains the prompt template you can use with an LLM to convert the standardized Markdown unit test reports into your required CSV format.

## LLM Conversion Prompt

Copy the text below and append the content of your Markdown test report at the bottom.

---

**System Prompt / Instructions:**

You are an expert QA engineer. I will provide you with a Unit Test Report in Markdown format. I need you to convert it into a highly specific CSV format that matches our internal templates.

Please strictly follow these CSV structural rules:

### 1. General Header (Rows 1-8)
The header must be exactly formatted as follows. Extract the values from the Markdown file's "1. General Information" and "3. Header Information" sections to replace the bracketed placeholders. If a value is missing, leave it blank but keep the commas.

```csv
,,,,,,,,,,,,,,,,,,,
Function Code,,[feature],,,Function Name,,,,,,[functionName],,,,,,,,
Created By,,<Developer Name>,,,Executed By,,,,,,,,,,,,,,
Lines  of code,,<Lines of code>,,,Lack of test cases,,,,,,<Lack of test cases>,,,,,,,,
Test requirement,,[Test requirement],,,,,,,,,,,,,,,,,
Passed,,Failed,,,Untested,,,,,,N/A/B,,,Total Test Cases,,,,,
[Passed],,[Failed],,,[Untested],,,,,,[N/A/B],,,[Total Test Cases],,,,,
,,,,,,,,,,,,,,,,,,,
```
*(Note: Be sure to preserve the exact number of commas between fields. The N/A/B string should just be copied as is, e.g., "N:2 B:0 A:0")*

### 2. UTCID Headers (Row 9)
Row 9 contains the test case IDs extracted from the Markdown's "Test Case List". There must be exactly 5 commas before the first UTCID.
```csv
,,,,,UTCID01,UTCID02,UTCID03,...
```

### 3. CONDITION Section
Row 10 starts the Condition section. 
```csv
Condition,Precondition ,,,,,,,,,,,,,,,,,,
```
For each condition category (e.g., `userId`, `groupNames`) in the Markdown "CONDITION Section":
- The category name goes in the 2nd column: `,[Category Name],,,,,,,,,,,,,,,,,,`
- Each condition value goes in the 4th column.
- The `O` markers go in the corresponding UTCID columns starting from the 6th column (which corresponds to UTCID01).
```csv
,,,[Value 1],,[O for UTCID01],[O for UTCID02],...
,,,[Value 2],,[O for UTCID01],[O for UTCID02],...
```

### 4. CONFIRM Section
Follows the Condition section with the same column alignment. For example:
```csv
Confirm,Return,,,,,,,,,,,,,,,,,,
,,,[Return Value 1],,[O for UTCID01],[O for UTCID02],...
```
Continue to map all confirm categories (e.g. Exception, log messages, mock calls) with their values in the 4th column and `O` markers from the 6th column onwards.

### 5. RESULT Section
The result section has 4 specific rows at the bottom. The labels are in the 1st and 2nd columns, and the values start from the 6th column. Ensure you map N/B/A and P/F appropriately.
```csv
Result,"Type(N : Normal, A : Abnormal, B : Boundary)",,,,[Type 1],[Type 2],...
,Passed/Failed,,,,[P/F 1],[P/F 2],...
,Executed Date,,,,[Date 1],[Date 2],...
,Defect ID,,,,[Defect 1],[Defect 2],...
```

**Important Output Rules:**
1. Ensure all rows maintain commas to pad up to at least 15-20 columns, ensuring visual alignment in Excel.
2. If a field contains a comma internally, wrap it in double quotes (e.g., `"Times.Once, with empty list"`).
3. Output ONLY the raw CSV text. Do not wrap it in ```csv or ``` markdown code blocks. Do not add any conversational text.

---

**Markdown Test Report to Convert:**

[Paste the content of your Markdown test report here, e.g. UpdateUserGroupsCommandHandlerTests.md]
