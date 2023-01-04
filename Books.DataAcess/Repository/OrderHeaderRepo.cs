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
    public class OrderHeaderRepo : Repository<OrderHeader>, IOrderHeaderRepo
    {
        private readonly ApplicationDbContext _db;
        public OrderHeaderRepo(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(OrderHeader obj)
        {
        }

        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            var orderFromDb = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);
            if (orderFromDb != null)
            {
                orderFromDb.OrderStatus = orderStatus;
                if (paymentStatus != null)
                {
                    orderFromDb.PaymentStatus = paymentStatus;
                }
            }
        }

        public void UpdateStripePayment(int id, string sessionId, string paymentIntentId)
        {
            var orderHeader = base.GetFirstOrDefault(x => id == x.Id, isTrack: true);
            if(orderHeader != null)
            {
                orderHeader.PaymentDate = DateTime.Now;
                orderHeader.PaymentIntentId = paymentIntentId;
                orderHeader.SessionId = sessionId;
            }
        }
    }
}
