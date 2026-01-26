namespace PersonalFinance.Application.DTOs;

/// <summary>
/// Result of an import operation
/// </summary>
public class ImportResultDto
{
    public int TotalRows { get; set; }
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public List<ImportErrorDto> Errors { get; set; } = new();
    public bool IsSuccess => FailureCount == 0;
}

/// <summary>
/// Error details for failed imports
/// </summary>
public class ImportErrorDto
{
    public int RowNumber { get; set; }
    public string Error { get; set; } = string.Empty;
    public Dictionary<string, string> RowData { get; set; } = new();
}
