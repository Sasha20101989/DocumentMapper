using doc_mapper.nuget.DAL.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace doc_mapper.nuget.DAL.Configurations;

public partial class ProcessConfiguration : IEntityTypeConfiguration<Process>
{
    public void Configure(EntityTypeBuilder<Process> entity)
    {
        _ = entity.HasKey(e => e.Id).HasName("PK_Processes");

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<Process> entity);
}
