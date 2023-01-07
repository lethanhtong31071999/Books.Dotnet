using Books.Model;
using Books.Model.PaginationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.DataAcess.Repository.IRepository
{
    public interface IOrderHeaderRepo : IRepository<OrderHeader>
    {
        public void Update(OrderHeader obj);
        public void UpdateStripePayment(int id, string sessionId, string paymentIntentId);
        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null);
        public PaginatedOrderHeader GetAllWithPagination(PaginatedOrderHeader pagingModel, string includedProps = null);
    }
}
