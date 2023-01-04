using Books.BusinessLogic.IBusinessLogic;
using Books.DataAcess.Repository;
using Books.Model;
using Books.Model.PaginationModel;
using Books.Model.ViewModel;

using Microsoft.AspNetCore.Mvc.Rendering;

namespace Books.BusinessLogic
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unit;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductService(IUnitOfWork unit, IWebHostEnvironment webHostEnv)
        {
            _unit = unit;
            _webHostEnvironment = webHostEnv;
        }

        public ProductVM HandleUpsertGetMethod(int? id)
        {
            var viewModel = new ProductVM();
            var categories = _unit.CategoryRepo
                    .GetAll()
                    .Select(x => new SelectListItem()
                    {
                        Text = x.Name,
                        Value = x.Id.ToString()
                    });
            var coverTypes = _unit.CoverTypeRepo
                .GetAll()
                .Select(x =>

                     new SelectListItem()
                     {
                         Text = x.Name,
                         Value = x.Id.ToString(),
                     }
                );
            viewModel.CoverTypeSelectList = coverTypes;
            viewModel.CategorySelectList = categories;

            if (id == null || id == 0)
            {
                // Create Function               
                viewModel.Product = new Product();
            }
            else
            {
                // Update Function
                var obj = _unit.ProductRepo.GetFirstOrDefault(x => x.Id == id, isTrack: false);
                if (obj != null)
                {
                    viewModel.Product = obj;
                }
            }
            return viewModel;
        }
        public bool HandleUpsertPostMethod(ProductVM obj, IFormFile? file)
        {
            try
            {
                if (file != null)
                {
                    // Handle file image
                    var fileName = CreateImageUrlAndGetFileName(file, obj.Product.ImageUrl);
                    obj.Product.ImageUrl = @$"\images\products\{fileName}";
                }
                if (obj.Product.Id == 0)
                {
                    // Create
                    _unit.ProductRepo.Add(obj.Product);
                    _unit.Save();
                }
                else
                {
                    // Update
                    _unit.ProductRepo.Update(obj.Product);
                    _unit.Save();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool HandleDeleteProduct(int id)
        {
            try
            {
                var objFromDba = _unit.ProductRepo.GetFirstOrDefault(x => x.Id == id, isTrack: true);
                var oldPath = Path.Combine(_webHostEnvironment.WebRootPath, objFromDba.ImageUrl);
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
                if (objFromDba != null)
                {
                    _unit.ProductRepo.Remove(objFromDba);
                    _unit.Save();
                }
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }
        public Pagination<Product> HandleGetAllProductsWithPagination(Pagination<Product> paginationModel)
        {
            return _unit.ProductRepo
                .GetAllWithPagination(paginationModel, includedProps: "Category,CoverType");
        }

        private string CreateImageUrlAndGetFileName(IFormFile file, string oldImgUrl)
        {
            string rootPath = _webHostEnvironment.WebRootPath;
            // Remove old image path
            if (oldImgUrl != null)
            {
                var oldPath = Path.Combine(rootPath, oldImgUrl.Substring(1));
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
            }
            // Create a path upload
            string pathUpload = Path.Combine(rootPath, @"images\products");
            // Create a unique file name
            string uniqueString = Guid.NewGuid().ToString();
            string fileName = $"{uniqueString}{file.FileName}";
            using (var fileStream = new FileStream(Path.Combine(pathUpload, fileName), FileMode.Create))
            {
                file.CopyTo(fileStream);
            }
            return fileName;
        }

        // Clone and Delete Products
        public void CreateDataBasedOn1Data()
        {
            var obj = _unit.ProductRepo.GetFirstOrDefault(x => true);
            var products = new List<Product>();
            for (int i = 0; i < 100; i++)
            {
                products.Add(new Product()
                {
                    Title = obj.Title + " " + i,
                    Description = obj.Description,
                    ISBN = obj.ISBN,
                    Author = obj.Author + " " + i,
                    ListPrice = obj.ListPrice,
                    Price = obj.Price,
                    Price50 = obj.Price50,
                    Price100 = obj.Price100,
                    CategoryId = obj.CategoryId,
                    CoverTypeId = obj.CoverTypeId,
                    ImageUrl = obj.ImageUrl,
                });
            }
            _unit.ProductRepo.AddRange(products.AsEnumerable<Product>());
            _unit.Save();
        }
        public void DeleteData()
        {
            _unit.ProductRepo.RemoveRange(_unit.ProductRepo.GetAll());
            _unit.Save();
        }

    }
}

