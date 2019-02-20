using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace SkietbaanBE.Models {
    public class ModelsContext : DbContext {

        public ModelsContext(DbContextOptions<ModelsContext> options)
           : base(options) {
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Competition> Competitions { get; set; }
        public virtual DbSet<Score> Scores { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<UserGroup> UserGroups { get; set; }
        public virtual DbSet<UserCompStats> UserCompStats { get; set; }
        public virtual DbSet<Notifications> Notifications { get; set; }
        public virtual DbSet<Award> Awards { get; set; }
        public virtual DbSet<UserCompetitionTotalScore> UserCompetitionTotalScores { get; set; }
    }
}