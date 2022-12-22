using Books.DataAcess.Repository;
using Books.Model;
using Books.Model.ViewModel;
using Books.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Books.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unit;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unit, IWebHostEnvironment webHostEnv)
        {
            _unit = unit;
            _webHostEnvironment = webHostEnv;
        }

        public IActionResult Index()
        {           
            return View();
        }

        // Upsert
        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            var viewModel = new ProductVM();
            IEnumerable<SelectListItem> categories = _unit.CategoryRepo
                    .GetAll()
                    .Select(x => new SelectListItem()
                    {
                        Text = x.Name,
                        Value = x.Id.ToString()
                    });
            IEnumerable<SelectListItem> coverTypes = _unit.CoverTypeRepo
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
                else
                    TempData["error"] = "Something went wrong!";
            }

            return View(viewModel);
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM obj, IFormFile? file)
        {
            if (ModelState.IsValid)
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
                    TempData["success"] = "Product was created successfully!";
                }
                else
                {
                    // Update
                    _unit.ProductRepo.Update(obj.Product);
                    _unit.Save();
                    TempData["success"] = "Product was updated successfully!";
                }
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "The values in fields are wrong!";
                return View(obj);
            }
        }

        // Delete
        [HttpDelete]
        public IActionResult Delete(int? id, bool trashParam = true)
        {
            if (id != null && id != 0)
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
                    return Json(new
                    {
                        success = true,
                        message = "Deleted successfully!"
                    });
                }
            }
            return Json(new
            {
                success = false,
                message = "Something went wrong!"
            });
        }

        // Image file
        private string CreateImageUrlAndGetFileName(IFormFile file, string oldImgUrl)
        {
            // Remove old image path
            if (oldImgUrl != null)
            {
                var oldPath = Path.Combine(_webHostEnvironment.WebRootPath, oldImgUrl);
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
            }
            // Create a path upload
            string rootPath = _webHostEnvironment.WebRootPath;
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

        #region API endpoint

        [HttpGet]
        public IActionResult GetAllProducts(string txtSearch, int? page)
        {
            var products = _unit.ProductRepo.GetAll(includedProps: "Category,CoverType");
            return Json(new
            {
                data = products,
                draw = 4,
                recordsTotal = 57,
                recordsFiltered = 57,
            });
        }

        #endregion

        // Clone Data
        private void CloneData()
        {
            var obj = _unit.ProductRepo.GetFirstOrDefault(x => true);
            var products = new List<Product>();
            for (int i = 0; i < 100; i++)
            {
                products.Add(new Product()
                {
                    Title = obj.Title,
                    Description = obj.Description,
                    ISBN = obj.ISBN,
                    Author = obj.Author,
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
    }
}
