using Books.Data;
using Books.DataAcess.Repository.IRepository;
using Books.Model;
using Books.Model.PaginationModel;
using Model.Utility;
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

        public void Update(OrderHeader objFromDba)
        {
                _db.Update(objFromDba);
        }

        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            var orderFromDb = base.GetFirstOrDefault(u => u.Id == id);
            if (orderFromDb != null)
            {
                orderFromDb.OrderStatus = orderStatus;
                if (!String.IsNullOrEmpty(paymentStatus))
                {
                    orderFromDb.PaymentStatus = paymentStatus;
                }
            }
        }


        public void UpdateStripePayment(int id, string sessionId, string paymentIntentId)
        {
            var orderHeader = base.GetFirstOrDefault(x => id == x.Id);
            if (orderHeader != null)
            {
                orderHeader.PaymentDate = DateTime.Now;
                orderHeader.PaymentIntentId = paymentIntentId;
                orderHeader.SessionId = sessionId;
            }
        }

        public PaginatedOrderHeader GetAllWithPagination(PaginatedOrderHeader pagingModel, string includedProps = null)
        {
            // Set up based on status and role
            IQueryable<OrderHeader> query = _db.OrderHeaders;
            if(!String.IsNullOrEmpty(pagingModel.Status))
            {
                switch (pagingModel.Status)
                {
                    case "pending":
                        query = query.Where(x => x.OrderStatus == SD.StatusPending);
                        break;
                    case "paymentpending":
                        query = query.Where(x => x.PaymentStatus == SD.PaymentStatusPending);
                        break;
                    case "completed":
                        query = query.Where(x => x.OrderStatus == SD.StatusCompleted);
                        break;
                    case "approved":
                        query = query.Where(x => x.OrderStatus == SD.StatusApproved);
                        break;
                    case "inprocess":
                        query = query.Where(x => x.OrderStatus == SD.StatusInProcess);
                        break;
                    case "shipped":
                        query = query.Where(x => x.OrderStatus == SD.StatusShipped);
                        break;
                    case "cancelled":
                        query = query.Where(x => x.OrderStatus == SD.StatusCancelled);
                        break;
                    case "refunded":
                        query = query.Where(x => x.OrderStatus == SD.StatusRefunded);
                        break;
                    default:
                        break;
                }
            }
            if(pagingModel.IsCustomer)
            {
                query = query.Where(x=>x.UserId == pagingModel.CustomerId);
            }
            pagingModel.RecordsTotal = query.Count();


            var textSearch = pagingModel.Filter.TextSearch.ToLower();
            if (textSearch != null && textSearch.Trim().Length > 0)
            {
                // Filter with text search
                query = query
                    .Where(x => x.Name.ToLower().Contains(textSearch)
                            || x.PhoneNumber.ToLower().Contains(textSearch)
                            || x.Id.ToString().Contains(textSearch))
                    .Skip(pagingModel.Filter.Start).Take(pagingModel.Filter.Length);
                pagingModel.Filter.Start = 0;
                pagingModel.RecordsFiltered = query.Count();
            }
            else
            {
                // Filter without search
                query = query.Skip(pagingModel.Filter.Start).Take(pagingModel.Filter.Length);
            }
            query = base.IncludeProperty(query, includedProps);
            pagingModel.Data = query.ToList<OrderHeader>();
            return pagingModel;
        }
    }
}
