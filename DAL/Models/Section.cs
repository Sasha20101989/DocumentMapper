using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace doc_mapper.nuget.DAL.Models;

[Table("tbd_Sections")]
public partial class Section
{
    [Key]
    [Column("Section_Id")]
    public int Id { get; set; }

    [Column("Section_Name")]
    public string SectionName { get; set; } = null!;

    [InverseProperty("Section")]
    public virtual ICollection<ProcessesStep> ProcessesSteps { get; set; } = [];
}