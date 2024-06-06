using doc_mapper.nuget.DAL.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace doc_mapper.nuget.DAL.Configurations;

public partial class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entity)
    {
        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<User> entity);
}
