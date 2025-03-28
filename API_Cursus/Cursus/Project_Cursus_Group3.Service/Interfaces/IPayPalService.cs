using Microsoft.AspNetCore.Http;
using Project_Cursus_Group3.Data.Model.VNPAY;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Service.Interfaces
{
    public interface IPayPalService
    {
        Task<string> CreatePaymentAsync(PaymentInformationModel model, HttpContext context);
        Task<(string Status, string OrderId)> CapturePaymentAsync(string token);
        PaypalReturnModel ProcessPaymentCompleteAsync(PaymentInformationPaypal response, string username,string payerId);
        Task<RefundResponseModel> RefundOrderAsync(RefundRequestModel model);
        Task<RefundResponseModel> RequestRefundAsync(RefundRequestModel model);
        Task<RefundResponseModel> ApproveRefundAsync(AdminRefundApprovalModel model, string adminUserName);
    }
}
