using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Data.Model.VNPAY
{
    public class PaymentResponseModel
    {
        public string OrderDescription { get; set; }
        public string TransactionId { get; set; }
        public string OrderId { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentId { get; set; }
        public bool Success { get; set; }
        public string Token { get; set; }
        public string VnPayResponseCode { get; set; }
        public string OrderType { get; set; }
        public double Amount { get; set; }
        public DateTime ResponseDate { get; set; }
    }
    public class PaymentInformationModel
    {
        public double Amount { get; set; }
        //public string Name { get; set; }
        public string OrderType { get; set; }
        public int OrderId { get; set; }
    }
    public class PaymentInformationPaypal
    {
        public Decimal Amount { get; set; }
        public string OrderType { get; set; }
        public int OrderId { get; set; }
    }
    public class PaypalReturnModel
    {
        public string description { get; set; }
        public string paymentMethod { get; set; }
        public string username { get; set; }
        public string OrderType { get; set; }
        public double Amount { get; set; }
        public DateTime ResponseDate { get; set; }
        public string PaymentId { get; set; }
    }

    public class RefundRequestModel
    {
        public int OrderId { get; set; }
        public string Reason { get; set; }
        //public decimal Amount { get; set; }
    }

    public class RefundResponseModel
    {
        public bool Success { get; set; }
        public string RefundId { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public DateTime RefundDate { get; set; }
    }
    public class AdminRefundApprovalModel
    {
        public int RefundId { get; set; }
        public bool IsApproved { get; set; }
    }
}