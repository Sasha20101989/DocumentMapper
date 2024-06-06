using doc_mapper.nuget.DAL.Configurations;
using doc_mapper.nuget.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace doc_mapper.nuget.DAL.Context;

public partial class DocumentMapperContext(DbContextOptions<DocumentMapperContext> options) : DbContext(options)
{
    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Section> Sections { get; set; }

    public virtual DbSet<DocumentMapper> DocumentMappers { get; set; }

    public virtual DbSet<DocumentMapperColumn> DocumentMapperColumns { get; set; }

    public virtual DbSet<DocumentMapperContent> DocumentMapperContents { get; set; }

    public virtual DbSet<Process> Processes { get; set; }

    public virtual DbSet<ProcessesStep> ProcessesSteps { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.ApplyConfiguration(new SectionConfiguration());
        _ = modelBuilder.ApplyConfiguration(new UserConfiguration());
        _ = modelBuilder.ApplyConfiguration(new DocumentMapperConfiguration());
        _ = modelBuilder.ApplyConfiguration(new DocumentMapperContentConfiguration());
        _ = modelBuilder.ApplyConfiguration(new ProcessConfiguration());
        _ = modelBuilder.ApplyConfiguration(new ProcessesStepConfiguration());

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}