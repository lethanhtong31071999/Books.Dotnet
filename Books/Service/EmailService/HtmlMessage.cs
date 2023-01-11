using Books.DataAcess.Repository;
using Books.Model;
using Model.Utility;

namespace Books.Service.EmailService
{
    public class HtmlMessage
    {
        private readonly IUnitOfWork _unit;
        public string Message { get; set; }
        public HtmlMessage(IUnitOfWork unit)
        {
            _unit = unit;
        }

        public string Buy(OrderHeader order)
        {
            var result = "";
            var orderDetails = _unit.OrderDetailRepo.GetAllWithCondition(x => x.OrderHeaderId == order.Id, includedProps: "Product").ToList();
            if(order != null && orderDetails.Count > 0)
            {
                result += $"<p>Dear <strong>Mr/Ms. {order.User.Name}</strong>,</p>";

                result += $"<p>We received your order ({orderDetails.Count()} items) including: \n<ul>";
                for (int i = 0; i < orderDetails.Count(); i++)
                {
                    var item = orderDetails[i];
                    result += $"<li>{item.Product.Title}  x  {item.Count}  =  {(item.Count * item.FinalPrice).ToString("c")}</li>";
                }
                result += $"Total: {order.OrderTotal.ToString("c")}";
                result += $"</ul>You can track your order at <a href='https://localhost:44330/Admin/Order/Detail?orderId={order.Id}'>here</a>.</p>";

                result += $"<strong>Thanks for your choosing us! If you have any concerns, please tell us via email {SD.EmailSendFrom}.</strong>";
                result += "<strong>Best regards</strong>";
            }
            return result;
        }

        public string Payment(OrderHeader order)
        {
            var result = "";
            if (order != null )
            {
                result += $"<p>Dear <strong>Mr/Ms. {order.User.Name}</strong>,</p>";

                result += $"<p>You paid for your order successfully. There is a brief information of your order: \n<ul>";
                result += $"<li>Id: {order.Id}</li>";
                result += $"<li>Total: {order.OrderTotal}</li>";
                result += $"<li>Order date: {order.OrderDate}</li>";
                result += $"<li>Payment date: {order.PaymentDate}</li>";
                result += $"<li>Payment status: {order.PaymentStatus}</li>";
                result += $"</ul>You can track your order at <a href='https://localhost:44330/Admin/Order/Detail?orderId={order.Id}'>here</a>.</p>";

                result += $"<strong>Thanks for your choosing us! If you have any concerns, please tell us via email {SD.EmailSendFrom}.</strong>";
                result += "<strong>Best regards</strong>";
            }
            return result;
        }

        public string Cancel(OrderHeader order)
        {
            var result = "";
            if (order != null)
            {
                result += $"<p>Dear <strong>Mr/Ms. {order.User.Name}</strong>,</p>";

                result += $"<p>You canceled your order successfully. There is a brief information of your order: \n<ul>";
                result += $"<li>Id: {order.Id}</li>";
                result += $"<li>Total: {order.OrderTotal}</li>";
                result += $"<li>Cancel date: {order.CancellingDate}</li>";
                result += $"</ul>You can track your order at <a href='https://localhost:44330/Admin/Order/Detail?orderId={order.Id}'>here</a>.</p>";

                result += $"<strong>Thanks for your choosing us! If you have any concerns, please tell us via email {SD.EmailSendFrom}.</strong>";
                result += "<strong>Best regards</strong>";
            }
            return result;
        }
    }
}
