using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace doc_mapper.nuget.DAL.Models;

[Table("tbd_Processes_Steps", Schema = "Master")]
public partial class ProcessesStep
{
    [Key]
    [Column("Process_Step_Id")]
    public int Id { get; set; }

    [Column("Process_Id")]
    public int ProcessId { get; set; }

    [Column("Step")]
    public int Step { get; set; }

    [Column("Docmapper_Id")]
    public int DocmapperId { get; set; }

    [Column("Section_Id")]
    public int SectionId { get; set; }

    [Column("Step_Name")]
    public string? StepName { get; set; }

    [ForeignKey("DocmapperId")]
    public virtual DocumentMapper DocumentMapper { get; set; } = null!;

    [ForeignKey("ProcessId")]
    [InverseProperty("ProcessesSteps")]
    public virtual Process Process { get; set; } = null!;

    [ForeignKey("SectionId")]
    [InverseProperty("ProcessesSteps")]
    public virtual Section Section { get; set; } = null!;

    [NotMapped]
    public virtual Dictionary<string, CellInfo> ValidationErrors { get; set; } = [];
}