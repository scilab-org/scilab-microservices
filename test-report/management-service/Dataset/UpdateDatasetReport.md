# Unit Test Report for UpdateDataset

# 1. General Information

| Field | Value |
|------|------|
| className | UpdateDatasetCommandHandler |
| functionName | UpdateDataset |
| testClass | UpdateDatasetCommandHandlerTests |
| feature | Dataset |

# 2. Test Case List

| UTCID | Description | Type |
|------|-------------|------|
| UTCID01 | DatasetNotFound -> ThrowsClientValidationException | A |
| UTCID02 | ValidDatasetNoFile -> UpdatesWithoutUpload | N |
| UTCID03 | ValidDatasetWithFile -> UploadsAndSetsFilePath | N |
| UTCID04 | UploadReturnsNoResult -> DoesNotSetFilePath | N |

# 3. Header Information

| Function Code | | Function Name | UpdateDataset |
|---------------|---|---------------|-----------------|
| Created By | Auto-Generator | Executed By | Auto-Generator |
| Lines of code | N/A | Lack of test cases | N/A |
| Test requirement | Auto-generated from UpdateDatasetCommandHandlerTests | | |

| Passed | Failed | Untested | N/A/B | Total Test Cases |
|--------|--------|----------|-------|------------------|
| 4 | 0 | 0 | 3 0 1 | 4 |

# 4. Decision Table Matrix

### CONDITION Section

| Condition | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|-------|---|---|---|---|
| **Input** | |  |  |  |  |
| | DatasetNotFound | O |   |   |   |
| | ValidDatasetNoFile |   | O |   |   |
| | ValidDatasetWithFile |   |   | O |   |
| | UploadReturnsNoResult |   |   |   | O |

### CONFIRM Section

| Confirm | Value | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|---------|-------|---|---|---|---|
| **Return/Exception** | |  |  |  |  |
| | ThrowsClientValidationException | O |   |   |   |
| | UpdatesWithoutUpload |   | O |   |   |
| | UploadsAndSetsFilePath |   |   | O |   |
| | DoesNotSetFilePath |   |   |   | O |

### RESULT Section

| Result Field | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|--------------|---|---|---|---|
| **Type (N/B/A)** | A | N | N | N |
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
