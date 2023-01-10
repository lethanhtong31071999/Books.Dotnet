using Books.BusinessLogic.IBusinessLogic;
using Books.BusinessLogic.IService;
using Books.DataAcess.Repository;
using Books.Model;
using Books.Model.PaginationModel;
using Books.Service.IService;
using Model.Utility;

namespace Books.Service
{
    public class OrderManagementService: IOrderManagementService
    {
        private readonly IUnitOfWork _unit;
        private readonly IStripeService _stripeService;
        public OrderManagementService(IUnitOfWork unit, IStripeService stripeService)
        {
            _unit = unit;
            _stripeService = stripeService; 
        }

        public PaginatedOrderHeader HandleGetAllProductsWithPagination(PaginatedOrderHeader paginatedOrderHeaderModel)
        {
            return _unit.OrderHeaderRepo.GetAllWithPagination(paginatedOrderHeaderModel, includedProps: "User");
        }

        public void UpdateDetailInformation(OrderHeader orderHeader)
        {
            var orderHeaderFromDba = _unit.OrderHeaderRepo.GetFirstOrDefault(x => x.Id == orderHeader.Id, isTracked: false);
            if (orderHeaderFromDba != null)
            {
                orderHeaderFromDba.Name = orderHeader.Name;
                orderHeaderFromDba.PhoneNumber = orderHeader.PhoneNumber;
                orderHeaderFromDba.StreetAddress = orderHeader.StreetAddress;
                orderHeaderFromDba.City = orderHeader.City;
                orderHeaderFromDba.State = orderHeader.State;
                orderHeaderFromDba.PostalCode = orderHeader.PostalCode;
                if (!String.IsNullOrEmpty(orderHeader.Carrier))
                {
                    orderHeaderFromDba.Carrier = orderHeader.Carrier;
                }
                if (!String.IsNullOrEmpty(orderHeader.TrackingNumber))
                {
                    orderHeaderFromDba.TrackingNumber = orderHeader.TrackingNumber;
                }
                _unit.OrderHeaderRepo.Update(orderHeaderFromDba);
                _unit.Save();
            }
        }

        public void UpdateOrderStatus(int orderId, string updatedStatus, string updatedById)
        {
            var orderHeaderFromDba = _unit.OrderHeaderRepo.GetFirstOrDefault(x => x.Id == orderId);
            if(orderHeaderFromDba != null)
            {
                var newStatus = "";
                var detailProcess = new DetailProcess()
                {
                    OrderHeaderId = orderId,
                    UpdatedById = updatedById,
                    UpdatedAt = DateTime.Now,
                    ProcessName = updatedStatus,
                };
                switch (updatedStatus)
                {
                    case SD.StatusInProcess:
                        newStatus = SD.StatusInProcess;
                        break;
                    case SD.StatusApproved:
                        newStatus = SD.StatusApproved;
                        break;
                    case SD.StatusShipped:
                        newStatus = SD.StatusShipped;
                        orderHeaderFromDba.ShippingDate = DateTime.Now;
                        if(orderHeaderFromDba.PaymentStatus == SD.PaymentStatusDelayedPayment)
                            orderHeaderFromDba.PaymentDueDate = DateTime.Now.AddDays(30);
                        break;
                    case SD.StatusCompleted:
                        newStatus = SD.StatusCompleted;
                        orderHeaderFromDba.CompletingDate = DateTime.Now;
                        break;
                    case SD.StatusCancelled:
                        newStatus = SD.StatusCancelled;
                        orderHeaderFromDba.CancellingDate = DateTime.Now;
                        break;
                    case SD.StatusRefunded:
                        newStatus = SD.StatusRefunded;
                        _stripeService.Refund(orderHeaderFromDba.OrderTotal, orderHeaderFromDba.PaymentIntentId);
                        orderHeaderFromDba.RefundingDate = DateTime.Now;
                        break;
                    default:
                        break;
                }
                _unit.DetailProcessRepo.Add(detailProcess);
                _unit.OrderHeaderRepo.Update(orderHeaderFromDba);
                _unit.OrderHeaderRepo.UpdateStatus(orderId, newStatus);
                _unit.Save();
            }
        }
    }
}
