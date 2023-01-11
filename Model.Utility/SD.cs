using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Utility
{
    public class SD
    {
        public const string Role_User_Individual = "Individual";
        public const string Role_User_Company = "Company";
        public const string Role_Admin = "Admin";
        public const string Role_Employee = "Employee";

        public const int MaximumDisplayPage = 5;
        public const int MaximumDisplayProduct = 8;

        public const string StatusPending = "Pending";
        public const string StatusApproved = "Approved";
        public const string StatusInProcess = "Processing";
        public const string StatusShipped = "Shipped";
        public const string StatusCompleted = "Completed";
        public const string StatusCancelled = "Cancelled";
        public const string StatusRefunded = "Refunded";

        public const string PaymentStatusPending = "Pending";
        public const string PaymentStatusApproved = "Approved";
        public const string PaymentStatusDelayedPayment = "ApprovedForDelayedPayment";
        public const string PaymentStatusRejected = "Rejected";

        public const string SessionCart = "SessionBookStoreCart";

        public const string EmailSendFrom = "BookStore@lethanhtong.com";
        public const string ConfirmationBuyingSubject = "Confirmation Buying Product";
        public const string ConfirmationPaymentSubject = "Confirmation Payment Product";
        public const string ConfirmationCancelSubject = "Confirmation Cancel Product";
    }
}
