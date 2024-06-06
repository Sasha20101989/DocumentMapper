using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace doc_mapper.nuget.DAL.Models;

[Table("tbd_Docmapper", Schema = "Docmapper")]
public partial class DocumentMapper
{
    [Key]
    [Column("Docmapper_Id")]
    public int Id { get; set; }

    [Column("Docmapper_Name")]
    public string DocumentMapperName { get; set; } = null!;

    [Column("Default_Folder")]
    public string? DefaultFolder { get; set; }

    [Column("Sheet_Name")]
    public string SheetName { get; set; } = null!;

    [Column("First_Data_Row")]
    public int? FirstDataRow { get; set; }

    [Column("Is_Active")]
    public bool IsActive { get; set; }

    [InverseProperty("DocumentMapper")]
    public virtual ICollection<DocumentMapperContent> DocumentMapperContents { get; set; } = [];

    [NotMapped]
    public virtual string? Folder { get; set; }

    [NotMapped]
    public virtual string? NgFolder { get; set; }

    [NotMapped]
    public virtual string[,]? Data { get; set; }
}