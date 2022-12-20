using Books.DataAcess.Repository;
using Books.Model;
using Microsoft.AspNetCore.Mvc;

namespace Books.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _unit;
        public CoverTypeController(IUnitOfWork unit)
        {
            _unit = unit;
        }

        public IActionResult Index()
        {
            var coverTypes = _unit.CoverRepo.GetAll().AsEnumerable<CoverType>();
            return View(coverTypes);
        }

        // Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CoverType obj)
        {
            if(!ModelState.IsValid) return View(obj);
            _unit.CoverRepo.Add(obj);
            _unit.Save();
            TempData["success"] = "Cover type was created successfully!";
            return RedirectToAction("Index", "CoverType");
        }

        // Delete
        public IActionResult Delete(int? id)
        {
            if (id == 0 || id == null) return NotFound();
            var obj = _unit.CoverRepo.GetFirstOrDefault(obj => obj.Id == id);
            if (obj != null) return View(obj);
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int? id, bool trashParam = true)
        {
            if (id == 0 || id == null) return View();
            var entity = _unit.CoverRepo.GetFirstOrDefault(x => x.Id == id);
            if (entity != null)
            {
                _unit.CoverRepo.Remove(entity);
                _unit.Save();
                TempData["success"] = "Cover type was deleted successfully!";
                return RedirectToAction("Index", "CoverType");
            }
            return View();
        }

        // Update
        [HttpGet]
        public IActionResult Update(int? id)
        {
            if (id == null || id == 0) return View();
            var entity = _unit.CoverRepo.GetFirstOrDefault(x => x.Id == id);
            return entity != null ? View(entity) : View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(CoverType obj)
        {
            if (!ModelState.IsValid) return View(obj);
            var entity = _unit.CoverRepo.GetFirstOrDefault(x => x.Id == obj.Id);
            if (entity != null)
            {
                entity.Name = obj.Name;
                _unit.Save();
                TempData["success"] = "Cover type was updated successfully!";
                return RedirectToAction("Index", "CoverType");
            }
            return View();
        }
    }
}
