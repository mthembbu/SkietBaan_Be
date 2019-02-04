using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkietbaanBE.Models {
    public class ModelsContext : DbContext {
        public ModelsContext(DbContextOptions<ModelsContext> options)
           : base(options) { }

        public ModelsContext() : base() { }

        //Tables
        public virtual DbSet<User> Users { get; set; }

    }
}
