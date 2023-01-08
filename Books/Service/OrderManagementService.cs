using Books.BusinessLogic.IBusinessLogic;
using Books.BusinessLogic.IService;
using Books.DataAcess.Repository;
using Books.Model;
using Books.Model.PaginationModel;
using Model.Utility;

namespace Books.Service
{
    public class OrderManagementService: IOrderManagementService
    {
        private readonly IUnitOfWork _unit;
        public OrderManagementService(IUnitOfWork unit)
        {
            _unit = unit;
        }

        public PaginatedOrderHeader HandleGetAllProductsWithPagination(PaginatedOrderHeader paginationModel)
        {
            return _unit.OrderHeaderRepo.GetAllWithPagination(paginationModel, includedProps: "User");
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

        public void UpdateOrderStatus(int orderId, string updatedStatus)
        {
            var orderHeaderFromDba = _unit.OrderHeaderRepo.GetFirstOrDefault(x => x.Id == orderId, isTracked: false);
            if(orderHeaderFromDba != null)
            {
                var newStatus = "";
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
                        _unit.OrderHeaderRepo.Update(orderHeaderFromDba);
                        _unit.Save();
                        break;
                    case SD.StatusCompleted:
                        newStatus = SD.StatusCompleted;
                        orderHeaderFromDba.CompletingDate = DateTime.Now;
                        _unit.OrderHeaderRepo.Update(orderHeaderFromDba);
                        break;
                    case SD.StatusCancelled:
                        newStatus = SD.StatusCancelled;
                        orderHeaderFromDba.CancellingDate = DateTime.Now;
                        _unit.OrderHeaderRepo.Update(orderHeaderFromDba);
                        break;
                    case SD.StatusRefunded:
                        // Bo sung them
                        newStatus = SD.StatusRefunded;
                        orderHeaderFromDba.RefundingDate = DateTime.Now;
                        _unit.OrderHeaderRepo.Update(orderHeaderFromDba);
                        break;
                    default:
                        break;
                }
                _unit.OrderHeaderRepo.UpdateStatus(orderId, newStatus);
                _unit.Save();
            }
        }
    }
}
