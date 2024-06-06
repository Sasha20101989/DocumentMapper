using doc_mapper.nuget.DAL.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace doc_mapper.nuget.DAL.Configurations;

public partial class ProcessesStepConfiguration : IEntityTypeConfiguration<ProcessesStep>
{
    public void Configure(EntityTypeBuilder<ProcessesStep> entity)
    {
        _ = entity.HasKey(e => e.Id).HasName("PK_Processes_Steps");

        _ = entity.HasOne(d => d.Process).WithMany(p => p.ProcessesSteps)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Processes_Steps_Processes");

        _ = entity.HasOne(d => d.Section).WithMany(p => p.ProcessesSteps)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Processes_Steps_Sections");

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<ProcessesStep> entity);
}
