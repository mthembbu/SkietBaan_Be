using SkietbaanBE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkietbaanBE.Lib {
    public static class DataSeeder {
        public static void Seed(ModelsContext context, bool DebugMode) {
            if (!DebugMode) return;
            if (!context.Users.Any()) {
                context.AddRange(new User[] {
                    new User {
                        Id = 1, Username = "John", Password = "q@eg$24t58", Email = "John@gmail.com",
                        MemberID = null, EntryDate = DateTime.Now.Date, MemberExpiry = DateTime.MinValue, Admin = false
                    },

                    new User {
                        Id = 2, Username = "Sibusiso", Password = "Uk2^704F%g", Email = "Sibusiso@gmail.com",
                        MemberID = null, EntryDate = DateTime.Now.Date, MemberExpiry = DateTime.MinValue, Admin = false
                    },

                    new User {
                        Id = 3, Username = "Tu", Password = "3ir@4sfe!", Email = "tu@gmail.com",
                        MemberID = null, EntryDate = DateTime.Now.Date, MemberExpiry = DateTime.MinValue, Admin = true
                    }
                });
                context.SaveChanges();
            }

            if (!context.Competitions.Any()) {
                context.AddRange(new Competition[] {
                    new Competition {
                        Id = 1, Name = "Pistol 100m", Status = true
                    },

                    new Competition {
                        Id = 2, Name = "Rifle 150m", Status = true
                    },

                    new Competition {
                        Id = 3, Name = "Rifle 100m", Status = false
                    }
                });
                context.SaveChanges();
            }

            if (!context.Groups.Any()) {
                context.AddRange(new Group[] {
                    new Group {
                        Id = 1, Name = "Group1"
                    },

                    new Group {
                        Id = 2, Name = "Group2"
                    },

                    new Group {
                        Id = 3, Name = "Group3"
                    }
                });
                context.SaveChanges();
            }

            if (!context.Scores.Any()) {
                context.AddRange(new Score[] {
                    new Score {
                        Id = 1, Competition = context.Competitions.Where(c => c.Id == 1).First(), PictureURL = null,
                        User = context.Users.Where(u => u.Id == 1).First(), UploadDate = null, UserScore = 48
                    },

                    new Score {
                        Id = 2, Competition = context.Competitions.Where(c => c.Id == 2).First(), PictureURL = null,
                        User = context.Users.Where(u => u.Id == 2).First(), UploadDate = null, UserScore = 35
                    },

                    new Score {
                        Id = 3, Competition = context.Competitions.Where(c => c.Id == 3).First(), PictureURL = null,
                        User = context.Users.Where(u => u.Id == 3).First(), UploadDate = null, UserScore = 38
                    },

                    new Score {
                        Id = 4, Competition = context.Competitions.Where(c => c.Id == 2).First(), PictureURL = null,
                        User = context.Users.Where(u => u.Id == 1).First(), UploadDate = null, UserScore = 40
                    }
                });
            }

            if (!context.UserGroups.Any()) {
                context.AddRange(new UserGroup[] {
                    new UserGroup {
                        Id = 1, User = context.Users.Where(u => u.Id == 1).First(),
                        Group = context.Groups.Where(g => g.Id == 1).First()
                    },
                    new UserGroup {
                        Id = 2, User = context.Users.Where(u => u.Id == 2).First(),
                        Group = context.Groups.Where(g => g.Id == 2).First()
                    },

                    new UserGroup {
                        Id = 3, User = context.Users.Where(u => u.Id == 2).First(),
                        Group = context.Groups.Where(g => g.Id == 1).First()
                    }
                });
            }

            if (!context.UserCompStats.Any()) {
                context.AddRange(new UserCompStats[] {
                    new UserCompStats {
                        Id = 1, User = context.Users.Where(u => u.Id == 1).First(),
                        Competition = context.Competitions.Where(c => c.Id == 1).First(), BestScore = 50, CompScore = 40,
                        Year = 2019
                    },

                    new UserCompStats {
                        Id = 2, User = context.Users.Where(u => u.Id == 2).First(),
                        Competition = context.Competitions.Where(c => c.Id == 2).First(), BestScore = 49, CompScore = 45,
                        Year = 2019
                    }
                });
            }
        }
    }
}
