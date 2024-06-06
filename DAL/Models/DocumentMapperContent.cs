using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace doc_mapper.nuget.DAL.Models;

[Table("tbd_Docmapper_Content", Schema = "Docmapper")]
public partial class DocumentMapperContent
{
    [Key]
    [Column("Docmapper_Content_Id")]
    public int Id { get; set; }

    [Column("Docmapper_Id")]
    public int DocumentMapperId { get; set; }

    [Column("Docmapper_Column_Id")]
    public int DocumentMapperColumnId { get; set; }

    [Column("Row_Nr")]
    public int? RowNr { get; set; }

    [Column("Column_Nr")]
    public int ColumnNr { get; set; }

    [ForeignKey("DocumentMapperColumnId")]
    [InverseProperty("DocumentMapperContents")]
    public virtual DocumentMapperColumn DocumentMapperColumn { get; set; } = null!;

    [ForeignKey("DocumentMapperId")]
    [InverseProperty("DocumentMapperContents")]
    public virtual DocumentMapper DocumentMapper { get; set; } = null!;
}