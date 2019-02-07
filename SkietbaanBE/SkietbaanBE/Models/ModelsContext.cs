using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SkietbaanBE.Models {
    public class ModelsContext : DbContext {
        public ModelsContext(DbContextOptions<ModelsContext> options)
           : base(options) { }

        //Tables
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Competition> Competition { get; set; }
    }
}
