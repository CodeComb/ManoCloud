using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using CodeComb.AspNet.Upload.Models;

namespace Mano.Models
{
    public class ManoContext : IdentityDbContext<User, IdentityRole<long>, long>, IFileUploadDbContext
    {
        public DbSet<File> Files { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.SetupBlob();
        }
    }
}
