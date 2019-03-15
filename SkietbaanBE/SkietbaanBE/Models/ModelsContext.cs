using Microsoft.EntityFrameworkCore;

namespace SkietbaanBE.Models {
    public class ModelsContext : DbContext {

        public ModelsContext(DbContextOptions<ModelsContext> options)
           : base(options) {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserCompetitionTotalScore>().HasKey(sc => new { sc.CompetitionId, sc.UserId });
            modelBuilder.Entity<UserGroup>().HasKey(sc => new { sc.GroupId, sc.UserId });
            modelBuilder.Entity<Competition>().HasIndex(c=>c.Name).IsUnique();
            modelBuilder.Entity<Group>().HasIndex(c => c.Name).IsUnique();
            modelBuilder.Entity<OTP>().HasIndex(o => o.UserId).IsUnique();
            modelBuilder.Entity<TimeSpent>().HasKey(t => new { t.CompetitionId, t.UserId });
        }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Competition> Competitions { get; set; }
        public virtual DbSet<Score> Scores { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<UserGroup> UserGroups { get; set; }
        public virtual DbSet<Notifications> Notifications { get; set; }
        public virtual DbSet<Award> Awards { get; set; }
        public virtual DbSet<UserCompetitionTotalScore> UserCompetitionTotalScores { get; set; }
        public virtual DbSet<OTP> OTPs { get; set; }
        public virtual DbSet<TimeSpent> TimeSpents { get; set; }
    }
}