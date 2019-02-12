using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SkietbaanBE.Controllers;
using SkietbaanBE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkietbaanBETest {
    class UserControllerTest {
        private IQueryable<User> mockData = new List<User> {
            new User{
                Id=1, Username="Superman", Password="Clark Kent", Email="DC@comics.com", MemberID=null,
                Admin=false, EntryDate=DateTime.Now.Date, MemberExpiry=DateTime.MinValue, Scores=null,
                UserCompStats=null, UserGroups=null
            },
            new User {
                Id=2, Username="Thanos", Password="Infinity Stone", Email="mavel@comics.com", MemberID="YES",
                Admin=false, EntryDate=DateTime.Now.Date, MemberExpiry=DateTime.MaxValue, Scores=null,
                UserCompStats=null, UserGroups=null
            }
        }.AsQueryable();

        [Test]
        public void TestGetUsers() {
            var mockSet = new Mock<DbSet<User>>();
            mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(mockData.GetEnumerator());
            var mockContext = new Mock<ModelsContext>();
            mockContext.Setup(c => c.Users).Returns(mockSet.Object);
            var controller = new UserController(mockContext.Object, null);
            var users = controller.GetUsers();

            Assert.IsNotEmpty(users);
            Assert.AreEqual(2, users.Count());
        }

        [Test]
        public async Task TestAddUser() {
            var mockSet = new Mock<DbSet<User>>();
            mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(mockData.GetEnumerator());
            var mockContext = new Mock<ModelsContext>();
            mockContext.Setup(c => c.Users).Returns(mockSet.Object);
            var controller = new UserController(mockContext.Object);

            /*
             *  number of invocations on the context after AddUser call
             * minus
             *  expected invocations inside the AddUser function
             * should equal
             *  number of invocations on the context before AddUser call
             * */

            int invocationCountBeforeCall = mockContext.Invocations.Count;
            await controller.AddUser(mockData.First());
            int invocationCountAfterCall = mockContext.Invocations.Count;
            int invocationInFunction = 3;
            Assert.AreEqual(invocationCountBeforeCall, invocationCountAfterCall - invocationInFunction);
        }

        [TestCase(2)]
        public async Task TestUpdateUser(int id) {
            var mockSet = new Mock<DbSet<User>>();
            mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(mockData.GetEnumerator());
            var mockContext = new Mock<ModelsContext>();
            mockContext.Setup(c => c.Users).Returns(mockSet.Object);
            var controller = new UserController(mockContext.Object);

            mockContext.Setup(x => x.Users.FindAsync(id))
                .Returns(Task.FromResult(mockData.Where(u => u.Id == id).First()));

            int invocationCountBeforeCall = mockContext.Invocations.Count;
            await controller.UpdateUser(id, mockData.Where(u => u.Id == id).First());
            int invocationCountAfterCall = mockContext.Invocations.Count;
            int invocationInFunction = 3;
            Assert.AreEqual(invocationCountBeforeCall, invocationCountAfterCall - invocationInFunction);
        }
    }
}
