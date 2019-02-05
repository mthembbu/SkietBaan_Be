using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkietbaanBE.Models {
    public interface IMockModelsContext {
        IEnumerable<User> Get();
        User Get(int id);
        string Post([FromBody] User user);
        void Put([FromBody] User user);
        void Delete(User user);
    }
}
