using Books.BusinessLogic.IService;
using Books.DataAcess.Repository;
using Books.Model;
using Books.Model.PaginationModel;
using Books.Model.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.Utility;
using System.Security.Claims;

namespace Books.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IBusinessLogic _businessLogic;
        private readonly IUnitOfWork _unit;
        public OrderController(IBusinessLogic businessLogic, IUnitOfWork unit)
        {
            _businessLogic = businessLogic;
            _unit = unit;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Detail(int orderId)
        {
            var model = new OrderVM()
            {
                OrderHeader = _unit.OrderHeaderRepo
                    .GetFirstOrDefault(x => x.Id == orderId, includedProps: "User"),
                OrderDetails = _unit.OrderDetailRepo
                    .GetAllWithCondition(x => x.OrderHeaderId == orderId, includedProps: "Product"),
                IsCustomer = !(User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee)),
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult UpdateOrder(OrderVM obj, string? nextStatus)
        {
            if(obj.OrderHeader != null)
            {
                _businessLogic.OrderManagementService.UpdateDetailInformation(obj.OrderHeader);                
                TempData["success"] = "Update order detail successfully!";
            }
            
            return RedirectToAction("Detail", "Order", new { orderId = obj.OrderHeader.Id });
        }

        [HttpGet]
        public IActionResult UpdateStatusOrder(int orderId, string? updatedStatus)
        {
            if (!String.IsNullOrEmpty(updatedStatus) && orderId != 0)
            {
                _businessLogic.OrderManagementService.UpdateOrderStatus(orderId, updatedStatus);
                TempData["success"] = "Update the order status successfully!";
            }
            return RedirectToAction("Detail", "Order", new { orderId = orderId });
        }

        #region API Call
        [HttpPost]
        public IActionResult GetAllOrderHeaders(string status)
        {
            var paginationModel = new PaginatedOrderHeader()
            {
                Filter = new Filter()
                {
                    Length = Convert.ToInt32(Request.Form["length"].FirstOrDefault()),
                    Start = Convert.ToInt32(Request.Form["start"].FirstOrDefault()),
                    TextSearch = Request.Form["search[value]"].FirstOrDefault()
                },
                Draw = Convert.ToInt32(Request.Form["draw"].FirstOrDefault()),
                RecordsFiltered = 0,
                RecordsTotal = 0,
                Data = null,
                Status = status,
            };
            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                paginationModel.IsCustomer = false;
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                paginationModel.CustomerId = claim.Value;
                paginationModel.IsCustomer = true;
            }
            _businessLogic.OrderManagementService.HandleGetAllProductsWithPagination(paginationModel);
            return Json(new
            {
                data = paginationModel.Data,
                recordsFiltered = paginationModel.RecordsTotal,
                recordsTotal = paginationModel.RecordsTotal,
                draw = paginationModel.Draw,
            });
        }
        #endregion  
    }
}
