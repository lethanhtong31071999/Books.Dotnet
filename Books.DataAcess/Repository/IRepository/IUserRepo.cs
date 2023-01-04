using Books.Model;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.DataAcess.Repository.IRepository
{
    public interface IUserRepo : IRepository<User>
    {
        public void Update(User obj);
        public IdentityRole GetRoleUserByUserId(string? userId);
    }
}
