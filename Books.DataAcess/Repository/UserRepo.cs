using Books.Data;
using Books.DataAcess.Repository.IRepository;
using Books.Model;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.DataAcess.Repository
{
    public class UserRepo : Repository<User>, IUserRepo
    {
        private readonly ApplicationDbContext _db;
        public UserRepo(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(User obj)
        {
            var objFromDba = base.GetFirstOrDefault(x => x.Id == obj.Id);
            if (objFromDba != null)
                {
                objFromDba.Name = obj.Name;
                objFromDba.StreetAddress = obj.StreetAddress;
                objFromDba.City = obj.City;
                objFromDba.State = obj.State;
                objFromDba.PostalCode = obj.PostalCode;
                objFromDba.PhoneNumber = obj.PhoneNumber;
            }
        }

        public IdentityRole GetRoleUserByUserId(string? userId)
        {
            var userRole = _db.UserRoles.FirstOrDefault(x => x.UserId == userId);
            if (userRole == null) return null;
            return _db.Roles.FirstOrDefault(x => x.Id == userRole.RoleId);
        }

    }
}
