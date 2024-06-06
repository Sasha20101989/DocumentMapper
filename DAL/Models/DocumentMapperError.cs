namespace doc_mapper.nuget.DAL.Models;

public class DocumentMapperError
{
    public string? ErrorMessage { get; set; }

    public int Row { get; set; }

    public int Column { get; set; }
}
