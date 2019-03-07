using SkietbaanBE.Models;
using System;
using System.Linq;

namespace SkietbaanBE.Lib {
    public static class DataSeeder {

        private static string BuildToken() {
            string guid = Guid.NewGuid().ToString();
            int index = guid.LastIndexOf("-");
            return guid.Substring(index + 1);
        }
        public static void Seed(ModelsContext context, bool debugMode) {
            if (!debugMode) return;
            if (!context.Users.Any()) {
                context.AddRange(new User[] {
                    new User {
                        Username = "Zintle", Password = "q@eg$24t58", Email = "zintle@gmail.com",
                        MemberID = "2598724", EntryDate = DateTime.Now.Date, MemberStartDate = DateTime.Today,
                        MemberExpiryDate = DateTime.Today.AddYears(1).Date, Admin = false,
                        Token = BuildToken()
                    },
                    new User {
                        Username = "Sibusiso", Password = "Uk2^704F%g", Email = "Sibusiso@gmail.com",
                        MemberID = null, EntryDate = DateTime.Now.Date, MemberExpiryDate = null, Admin = false,
                        Token = BuildToken()
                    },
                    new User {
                        Username = "Tu", Password = "3ir@4sfe!", Email = "tu@gmail.com",
                        MemberID = null, EntryDate = DateTime.Now.Date, MemberExpiryDate = null, Admin = false,
                        Token = BuildToken()
                    },
                    new User {
                        Username = "Gideon", Password = "56soe&", Email = "gideon@gmail.com",
                        MemberID = null, EntryDate = DateTime.Now.Date, MemberExpiryDate = null, Admin = false,
                        Token = BuildToken()
                    },
                    new User {
                        Username = "Dino", Password = "f8w9#445", Email = "dino@gmail.com",
                        MemberID = null, EntryDate = DateTime.Now.Date, MemberExpiryDate = null, Admin = false,
                        Token = BuildToken()
                    },
                    new User {
                        Username = "Ilze", Password = "tw224!2", Email = "ilze@gmail.com",
                        MemberID = "1423", EntryDate = DateTime.Now.Date, MemberStartDate = DateTime.Today,
                        MemberExpiryDate = DateTime.Today.AddYears(1).Date, Admin = true, Token = BuildToken()
                    }
                });
                context.SaveChanges();
                new TestData().AddTestData(context);
            }
        }
    }
}
