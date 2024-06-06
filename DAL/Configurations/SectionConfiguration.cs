using doc_mapper.nuget.DAL.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace doc_mapper.nuget.DAL.Configurations;

public partial class SectionConfiguration : IEntityTypeConfiguration<Section>
{
    public void Configure(EntityTypeBuilder<Section> entity)
    {
        entity.HasKey(e => e.Id).HasName("PK_Sections");

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<Section> entity);
}
