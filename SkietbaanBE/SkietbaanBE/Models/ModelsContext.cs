using Microsoft.EntityFrameworkCore;

namespace SkietbaanBE.Models {
    public class ModelsContext : DbContext {
        public ModelsContext(DbContextOptions<ModelsContext> options)
           : base(options) { }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Competition> Competitions { get; set; }
        public virtual DbSet<Score> Scores { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<UserGroup> UserGroups { get; set; }
        public virtual DbSet<UserCompStats> UserCompStats { get; set; }
    }
}
