using Books.Model;
using Books.Model.PaginationModel;
using Books.Model.ViewModel;

namespace Books.BusinessLogic.IBusinessLogic
{
    public interface IOrderManagementService
    {
        public PaginatedOrderHeader HandleGetAllProductsWithPagination(PaginatedOrderHeader paginationModel);
        public void UpdateDetailInformation(OrderHeader orderHeader);
        public void UpdateOrderStatus(int orderId, string nextStatus);
    }
}
