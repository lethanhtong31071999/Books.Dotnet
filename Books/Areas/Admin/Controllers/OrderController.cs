using Books.BusinessLogic.IService;
using Books.DataAcess.Repository;
using Books.Model;
using Books.Model.PaginationModel;
using Books.Model.ViewModel;
using Books.Service.IService;
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
        private readonly IStripeService _stripe;
        public OrderController(IBusinessLogic businessLogic, IUnitOfWork unit, IStripeService stripe)
        {
            _businessLogic = businessLogic;
            _unit = unit;
            _stripe = stripe;
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
            if (obj.OrderHeader != null)
            {
                _businessLogic.OrderManagementService.UpdateDetailInformation(obj.OrderHeader);
                TempData["success"] = "Update order detail successfully!";
            }

            return RedirectToAction("Detail", "Order", new { orderId = obj.OrderHeader.Id });
        }

        [HttpGet]
        public IActionResult UpdateStatusOrder(int orderId, string? updatedStatus)
        {
            var orderHeader = _unit.OrderHeaderRepo.GetFirstOrDefault(x => x.Id == orderId);
            if (orderHeader != null)
            {
                if (!String.IsNullOrEmpty(updatedStatus))
                {
                    var claimsIdentity = (ClaimsIdentity)User.Identity;
                    var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                    // For Cancel Order
                    if (updatedStatus == SD.StatusCancelled || updatedStatus == SD.StatusRefunded)
                    {
                        _businessLogic.OrderManagementService.UpdateOrderStatus(orderId, updatedStatus, claims.Value);
                    }
                    else
                    {
                        // For all status order
                        if (!(String.IsNullOrEmpty(orderHeader.TrackingNumber) || String.IsNullOrEmpty(orderHeader.Carrier)))
                        {
                            _businessLogic.OrderManagementService.UpdateOrderStatus(orderId, updatedStatus, claims.Value);
                            TempData["success"] = "Update the order status successfully!";
                        }
                        else TempData["error"] = "Please fullfill all detail information before updating the status by SAVING!";
                    }
                }
                else TempData["error"] = "Something went wrong!";
            }

            return RedirectToAction("Detail", "Order", new { orderId = orderId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PayNow()
        {
            int orderHeaderId = Convert.ToInt32(Request.Form["OrderHeader.Id"].FirstOrDefault());
            var orderDetails = _unit.OrderDetailRepo.GetAllWithCondition(x => x.OrderHeaderId == orderHeaderId, includedProps: "Product")
                .AsEnumerable<OrderDetail>();
            if (orderDetails.Count() > 0)
            {
                var paymentStripe = orderDetails.Select(x => new PaymentStripe()
                {
                    Product = x.Product,
                    Count = x.Count,
                    FinalPrice = x.FinalPrice,
                }).AsEnumerable<PaymentStripe>();
                var session = _stripe.Payment(paymentStripe, orderHeaderId);
                if (session != null)
                {
                    var orderHeader = _unit.OrderHeaderRepo.GetFirstOrDefault(x => x.Id == orderHeaderId);
                    orderHeader.SessionId = session.Id;
                    _unit.OrderHeaderRepo.Update(orderHeader);
                    _unit.Save();
                    Response.Headers.Add("Location", session.Url);
                }
                else
                {
                    TempData["error"] = "Something went wrong!";
                    return RedirectToAction("Index", "Home", new { area = "Customer" });
                }
            }
            return new StatusCodeResult(303);
        }

        #region API Call
        [HttpPost]
        public IActionResult GetAllOrderHeaders(string status)
        {
            var paginatedOrderHeaderModel = new PaginatedOrderHeader()
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
                paginatedOrderHeaderModel.IsCustomer = false;
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                paginatedOrderHeaderModel.CustomerId = claim.Value;
                paginatedOrderHeaderModel.IsCustomer = true;
            }
            _businessLogic.OrderManagementService.HandleGetAllProductsWithPagination(paginatedOrderHeaderModel);
            return Json(new
            {
                data = paginatedOrderHeaderModel.Data,
                recordsFiltered = paginatedOrderHeaderModel.RecordsTotal,
                recordsTotal = paginatedOrderHeaderModel.RecordsTotal,
                draw = paginatedOrderHeaderModel.Draw,
            });
        }
        #endregion  
    }
}
