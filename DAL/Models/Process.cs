using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace doc_mapper.nuget.DAL.Models;

[Table("tbd_Processes", Schema = "Master")]
public partial class Process
{
    [Key]
    [Column("Process_Id")]
    public int Id { get; set; }

    [Column("Process_Name")]
    public string? ProcessName { get; set; }

    [InverseProperty("Process")]
    public virtual ICollection<ProcessesStep> ProcessesSteps { get; set; } = [];
}