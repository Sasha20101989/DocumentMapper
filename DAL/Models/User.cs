using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace doc_mapper.nuget.DAL.Models;

[Table("tbd_Users", Schema = "Users")]
[Index("Account", Name = "UQ_tbd_Users_Account", IsUnique = true)]
public partial class User
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    public string Account { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string Patronymic { get; set; } = null!;

    [Column("Section_Id")]
    public int SectionId { get; set; }

    [ForeignKey("SectionId")]
    public virtual Section Section { get; set; } = null!;

    [NotMapped]
    public string? Photo { get; set; }
}