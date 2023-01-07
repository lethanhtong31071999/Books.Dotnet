using Books.BusinessLogic.IBusinessLogic;
using Books.BusinessLogic.IService;
using Books.DataAcess.Repository;
using Books.Model;
using Books.Model.PaginationModel;

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
    }
}
