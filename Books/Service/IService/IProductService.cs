using Books.Model;
using Books.Model.ViewModel;

namespace Books.BusinessLogic.IBusinessLogic
{
    public interface IProductService
    {
        public ProductVM HandleUpsertGetMethod(int? id);
        public bool HandleUpsertPostMethod(ProductVM obj, IFormFile? file);
        public bool HandleDeleteProduct(int id);
        public void CreateDataBasedOn1Data();
        public void DeleteData();
        public Pagination<Product> HandleGetAllProductsWithPagination(Pagination<Product> paginationModel);
    }
}
