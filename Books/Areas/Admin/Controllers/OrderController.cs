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
        public IActionResult UpdateOrder(OrderVM obj)
        {
            var orderHeaderFromDba = _unit.OrderHeaderRepo.GetFirstOrDefault(x => x.Id == obj.OrderHeader.Id, isTrack: false);
            if (orderHeaderFromDba != null)
            {
                orderHeaderFromDba.Name = obj.OrderHeader.Name;
                orderHeaderFromDba.PhoneNumber = obj.OrderHeader.PhoneNumber;
                orderHeaderFromDba.StreetAddress = obj.OrderHeader.StreetAddress;
                orderHeaderFromDba.City = obj.OrderHeader.City;
                orderHeaderFromDba.State = obj.OrderHeader.State;
                orderHeaderFromDba.PostalCode = obj.OrderHeader.PostalCode;
                if (!String.IsNullOrEmpty(obj.OrderHeader.Carrier))
                {
                    orderHeaderFromDba.Carrier = obj.OrderHeader.Carrier;
                }
                if (!String.IsNullOrEmpty(obj.OrderHeader.TrackingNumber))
                {
                    orderHeaderFromDba.TrackingNumber = obj.OrderHeader.TrackingNumber;
                }
                _unit.OrderHeaderRepo.Update(orderHeaderFromDba);
                _unit.Save();
            }
            return RedirectToAction("Detail", "Order", new { orderId = obj.OrderHeader.Id });
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
