using doc_mapper.nuget.DAL.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace doc_mapper.nuget.DAL.Configurations;

public partial class DocumentMapperContentConfiguration : IEntityTypeConfiguration<DocumentMapperContent>
{
    public void Configure(EntityTypeBuilder<DocumentMapperContent> entity)
    {
        _ = entity.HasOne(d => d.DocumentMapperColumn).WithMany(p => p.DocumentMapperContents)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_tbd_Docmapper_Content_tbd_Docmapper_Columns");

        _ = entity.HasOne(d => d.DocumentMapper).WithMany(p => p.DocumentMapperContents)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_tbd_Docmapper_Content_tbd_Docmapper");

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<DocumentMapperContent> entity);
}
