using Books.Data;
using Books.DataAcess.Repository.IRepository;
using Books.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.DataAcess.Repository
{
    public class OrderDetailRepo : Repository<OrderDetail>, IOrderDetailRepo
    {
        private readonly ApplicationDbContext _db;
        public OrderDetailRepo(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(OrderDetail obj)
        {

        }
    }
}
