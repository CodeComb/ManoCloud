using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using CodeComb.AspNet.Upload.Models;
using Mano.Parser.Models;

namespace Mano.Models
{
    public class ManoContext : IdentityDbContext<User, IdentityRole<long>, long>, IFileUploadDbContext
    {
        public DbSet<File> Files { get; set; }

        public DbSet<Domain> Domains { get; set; }

        public DbSet<Project> Projects { get; set; }

        public DbSet<Certification> Certifications { get; set; }

        public DbSet<Skill> Skills { get; set; }

        public DbSet<Experience> Experiences { get; set; }

        public DbSet<Education> Educations { get; set; }

        public DbSet<Commit> Commits { get; set; }

        public DbSet<Change> Changes { get; set; }

        public DbSet<Node> Nodes { get; set; }

        public DbSet<Extension> Extensions { get; set; }

        public DbSet<Email> Emails { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.SetupBlob();

            builder.Entity<Domain>(e =>
            {
                e.HasIndex(x => x.DomainName);
            });

            builder.Entity<Project>(e =>
            {
                e.HasIndex(x => x.Type);
                e.HasIndex(x => x.Begin);
                e.HasIndex(x => x.End);
            });

            builder.Entity<Change>(e =>
            {
                e.HasIndex(x => x.Additions);
                e.HasIndex(x => x.Deletions);
            });

            builder.Entity<Commit>(e =>
            {
                e.HasIndex(x => x.Time);
                e.HasIndex(x => x.Email);
                e.HasIndex(x => x.Author);
            });

            builder.Entity<Email>(e =>
            {
                e.HasIndex(x => x.Verified);
                e.HasIndex(x => x.EmailAddress);
            });

            builder.Entity<Skill>(e =>
            {
                e.HasIndex(x => x.Begin);
            });

            builder.Entity<Experience>(e =>
            {
                e.HasIndex(x => x.Begin);
                e.HasIndex(x => x.End);
            });

            builder.Entity<Education>(e =>
            {
                e.HasIndex(x => x.Begin);
                e.HasIndex(x => x.End);
            });

            builder.Entity<User>(e =>
            {
                e.HasIndex(x => x.RegisteryTime);
            });
        }
    }
}
