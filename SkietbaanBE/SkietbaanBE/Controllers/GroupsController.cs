using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkietbaanBE.Helper;
using SkietbaanBE.Models;
using SkietbaanBE.RequestModel;

namespace SkietbaanBE.Controllers
{
    [Produces("application/json")]
    [Route("api/Groups")]
    public class GroupsController : Controller
    {
        private readonly ModelsContext _context;
        private NotificationMessages _notificationMessages;

        public GroupsController(ModelsContext context, NotificationMessages notificationMessages)
        {
            _context = context;
            _notificationMessages = notificationMessages;
        }
        // GET: api/Groups
        [HttpGet]
        public IEnumerable<Group> GetGroups()
        {
            IEnumerable<Group> groups = _context.Groups.Where(g => g.IsActive.Equals(true));
            return groups;
        }
        // GET: api/Groups/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGroup([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var @group = await _context.Groups.SingleOrDefaultAsync(m => m.Id == id);
            if (@group == null)
            {
                return NotFound();
            }
            return Ok(@group);
        }
        // PUT: api/Groups/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGroup([FromRoute] int id, [FromBody] Group @group)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (id != @group.Id)
            {
                return BadRequest();
            }
            _context.Entry(@group).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GroupExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }
        // POST: api/Groups
        [HttpPost]
        public async Task<IActionResult> PostGroup(string groupName)
        {
            Group group = new Group();
            if (!ModelState.IsValid)
            {
                group.IsActive = true;
                group.Name = groupName;
                _context.Groups.Add(group);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetGroup", new { id = @group.Id }, @group);
            }
            group.IsActive = true;
            group.Name = groupName;
            _context.Groups.Add(group);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetGroup", new { id = @group.Id }, @group);
        }
        // DELETE: api/Groups/5
        [HttpPost("{id}")]
        public async Task<IActionResult> DeleteGroup([FromRoute] int id)

        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var group = await _context.Groups.SingleOrDefaultAsync(m => m.Id == id);
            group.IsActive = false;
            if (group == null)
            {
                return NotFound();
            }
            await _context.SaveChangesAsync();
            return Ok(group);
        }

        private bool GroupExists(int id)
        {
            return _context.Groups.Any(e => e.Id == id);
        }

        //creating groups and adding members to the groups
        [HttpPost]
        [Route("add")]
        public void AddListUsers([FromBody] CreateGroup createobj)
        {
            Group group = new Group();
            group.Name = createobj.name;
            group.IsActive = true;
            Group tempGroup = _context.Groups.Where(g => g.Name.Equals(group.Name)).FirstOrDefault<Group>();
            if (tempGroup == null)
            {
                _context.Groups.Add(group);
                _context.SaveChanges();
                group = _context.Groups.Where(g => g.Name.Equals(group.Name)).FirstOrDefault<Group>();
            }
            else
            {
                group = tempGroup;
            }

            List<UserGroup> userGroups = new List<UserGroup>();
            for (int i = 0; i < createobj.users.Length; i++)
            {
                UserGroup userGroup = new UserGroup();
                User dbUser = _context.Users.FirstOrDefault(x => x.Id == createobj.users.ElementAt(i).Id);

                userGroup.GroupId = group.Id;
                userGroup.UserId = dbUser.Id;
                userGroups.Add(userGroup);
                _notificationMessages.GroupNotification(_context, group, dbUser);
            }
            _context.UserGroups.AddRange(userGroups);
            _context.SaveChanges();
        }

        //get users that do not in the group
        [HttpGet]
        [Route("list")]
        public List<User> getUsersThatAreNotInTheGroup(int id)
        {
            List<User> users = new List<User>();
            var query = from Group in _context.Groups
                        join UserGroup in _context.UserGroups on Group.Id equals UserGroup.Group.Id
                        join User in _context.Users on UserGroup.User.Id equals User.Id
                        where (Group.Id == id )
                        select new
                        {
                            User
                        };
            var qry = _context.Users.Select(x => x).ToList();
            if (qry != null)
            {
                foreach (var item in query)
                {
                    User user = new User();
                    user = item.User;
                    users.Add(user);
                }
            }
            var result = (qry).Except(users);
            return result.ToList<User>();
        }

        //get existing members in a group
        [HttpGet]
        [Route("edit")]
        public List<User> getExistingMembers(int id)
        {
            List<User> users = new List<User>();
            var query = from Group in _context.Groups
                        join UserGroup in _context.UserGroups on Group.Id equals UserGroup.Group.Id
                        join User in _context.Users on UserGroup.User.Id equals User.Id
                        where (Group.Id == id)
                        select new
                        {

                            User
                        };
            foreach (var item in query)
            {
                User user = new User();
                user = item.User; users.Add(user);
            }
            return users;
        }
        //delete members in a group
        [HttpPost]
        [Route("deleteMember")]
        public void deleteUsersOnTheList([FromBody] Filter usersobj)
        {
            List<string> userss = new List<string>();
            for (int i = 0; i < usersobj.users.Length; i++)
            {
                userss.Add(usersobj.users.ElementAt(i).Token);
            }
            var query = from Group in _context.Groups
                        join UserGroup in _context.UserGroups on Group.Id equals UserGroup.Group.Id
                        join User in _context.Users on UserGroup.User.Id equals User.Id
                        where (Group.Id == usersobj.GroupIds)
                        select new
                        {
                            UserGroup,
                            User
                        };

            var d = query.ToList();
            if (d != null)
            {
                foreach (var item in d)
                {
                    if (userss.Contains(item.User.Token))
                    {
                        _context.UserGroups.Remove(item.UserGroup);
                        _context.SaveChanges();
                    }
                }
            }
        }
        [HttpPost]
        [Route("postMember")]
        public void addUsersOnTheList([FromBody] Filter usersobj)
        {
            List<string> userss = new List<string>();
            Group group = _context.Groups.FirstOrDefault(x => x.Id == usersobj.GroupIds);
            for (int i = 0; i < usersobj.users.Length; i++)
            {
                UserGroup userGroup = new UserGroup();
                User dbUser = _context.Users.FirstOrDefault(x => x.Token == usersobj.users.ElementAt(i).Token);
                userGroup.Group = group;
      
                userGroup.User = dbUser;
                _context.UserGroups.Add(userGroup);
                _context.SaveChanges();
            }
        }
    }
}