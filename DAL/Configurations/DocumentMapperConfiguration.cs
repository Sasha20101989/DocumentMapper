using doc_mapper.nuget.DAL.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace doc_mapper.nuget.DAL.Configurations;

public partial class DocumentMapperConfiguration : IEntityTypeConfiguration<DocumentMapper>
{
    public void Configure(EntityTypeBuilder<DocumentMapper> entity)
    {
        _ = entity.Property(e => e.FirstDataRow).HasDefaultValue(1);
        _ = entity.Property(e => e.IsActive).HasDefaultValue(true);

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<DocumentMapper> entity);
}
