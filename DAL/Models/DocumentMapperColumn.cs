using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace doc_mapper.nuget.DAL.Models;

[Table("tbd_Docmapper_Columns", Schema = "Docmapper")]
public partial class DocumentMapperColumn
{
    [Key]
    [Column("Docmapper_Column_Id")]
    public int Id { get; set; }

    [Column("Element_Name")]
    public string ElementName { get; set; } = null!;

    [Column("System_Column_Name")]
    public string SystemColumnName { get; set; } = null!;

    [InverseProperty("DocumentMapperColumn")]
    public virtual ICollection<DocumentMapperContent> DocumentMapperContents { get; set; } = [];
}