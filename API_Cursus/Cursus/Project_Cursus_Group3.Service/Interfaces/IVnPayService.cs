using Microsoft.AspNetCore.Http;
using Project_Cursus_Group3.Data.Model.VNPAY;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Service.Interfaces
{
    public interface IVnPayService
    {
        Task<string> CreatePaymentUrlAsync(PaymentInformationModel model, HttpContext context);
        PaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}