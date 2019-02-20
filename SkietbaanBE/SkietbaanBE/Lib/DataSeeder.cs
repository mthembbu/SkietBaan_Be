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
                        MemberID = null, EntryDate = DateTime.Now.Date, MemberExpiryDate = null, Admin = false,
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

            if (!context.Awards.Any()) {
                context.AddRange(new Award[] {
                    new Award {
                        User = context.Users.Where(u => u.Id == 1).First(), Description = "Reached 500pts",
                        IconURL = "https://www.google.com/imgres?imgurl=http%3A%2F%2Fchittagongit.com%2Fimages%2Fman-icon-png%2Fman-icon-png-7.jpg&imgrefurl=http%3A%2F%2Fchittagongit.com%2Ficon%2Fman-icon-png-7.html&docid=MnAwdjTycW9GmM&tbnid=BgHtgn1z8Z5mbM%3A&vet=10ahUKEwjthPvahsHgAhUYTxUIHZXmCNkQMwjDAShLMEs..i&w=252&h=592&bih=754&biw=1536&q=icons&ved=0ahUKEwjthPvahsHgAhUYTxUIHZXmCNkQMwjDAShLMEs&iact=mrc&uact=8",
                        Stat = 500
                    },
                    new Award {
                        User = context.Users.Where(u => u.Id == 1).First(), Description = "Reached 1200pts",
                        IconURL = "https://www.google.com/imgres?imgurl=https%3A%2F%2Ficon2.kisspng.com%2F20180201%2Fure%2Fkisspng-icon-silver-star-5a72ad85a54e49.8622101115174649656771.jpg&imgrefurl=https%3A%2F%2Fwww.kisspng.com%2Ffree%2Fsilver-star.html&docid=hsspl1AOoulcuM&tbnid=opFgZbfxp_3T-M%3A&vet=10ahUKEwjj-t_NnL3gAhWkSBUIHU_HD2sQMwg9KAAwAA..i&w=260&h=260&bih=706&biw=1536&q=silver%20star%20icon%20png&ved=0ahUKEwjj-t_NnL3gAhWkSBUIHU_HD2sQMwg9KAAwAA&iact=mrc&uact=8",
                        Stat = 1200
                    },
                    new Award {
                        User = context.Users.Where(u => u.Id == 1).First(), Description = "Reached Accuracy of 80%",
                        IconURL = "https://www.google.com/imgres?imgurl=https%3A%2F%2Ficon2.kisspng.com%2F20180201%2Fure%2Fkisspng-icon-silver-star-5a72ad85a54e49.8622101115174649656771.jpg&imgrefurl=https%3A%2F%2Fwww.kisspng.com%2Ffree%2Fsilver-star.html&docid=hsspl1AOoulcuM&tbnid=opFgZbfxp_3T-M%3A&vet=10ahUKEwjj-t_NnL3gAhWkSBUIHU_HD2sQMwg9KAAwAA..i&w=260&h=260&bih=706&biw=1536&q=silver%20star%20icon%20png&ved=0ahUKEwjj-t_NnL3gAhWkSBUIHU_HD2sQMwg9KAAwAA&iact=mrc&uact=8",
                        Stat = 80
                    }
                });
                context.SaveChanges();
            }

        }
    }
}
