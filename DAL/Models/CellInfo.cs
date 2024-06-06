namespace doc_mapper.nuget.DAL.Models;

public class CellInfo
{
    public object? Value { get; set; }

    public bool HasError => Errors is not null && Errors.Count > 0;

    public List<DocumentMapperError> Errors { get; set; } = [];
}
