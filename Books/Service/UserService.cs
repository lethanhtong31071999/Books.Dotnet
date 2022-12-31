using Books.DataAcess.Repository;
using Books.Service.IService;

namespace Books.Service
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unit;
        public UserService(IUnitOfWork unit)
        {
            _unit = unit
        }
    }
}
