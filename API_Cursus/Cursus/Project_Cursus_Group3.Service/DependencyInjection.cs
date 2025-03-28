using Microsoft.Extensions.DependencyInjection;
using PayPalCheckoutSdk.Core;
using Project_Cursus_Group3.Service.Interfaces;
using Project_Cursus_Group3.Service.Repository;
using Project_Cursus_Group3.Service.Services;

namespace Project_Cursus_Group3.Service
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection service)
        {
            service.AddTransient<ICategoryServices, CategoryServices>();
            service.AddTransient<IAuthenServices, AuthenServices>();
            service.AddTransient<IUserServices, UserServices>();
            service.AddTransient<ICourseServices, CourseServices>();

            service.AddTransient<IBookmarkServices, BookmarkServices>();
            service.AddTransient<IBookmarkDetailServices, BookmarkDetailServices>();

            service.AddTransient<ILessonServices, LessonServices>();

            service.AddTransient<IAdminServices, AdminServices>();
            service.AddScoped<IReportService, ReportService>();
            service.AddScoped<ICartServices, CartServices>();

            service.AddScoped<IFeedbackServices, FeedbackServices>();

            service.AddTransient<IPurchasedCourseServices, PurchasedCourseServices>();
            service.AddScoped<IVnPayService, VnPayService>();
            service.AddScoped<IReasonServices, ReasonServices>();
            service.AddScoped<IRatePriceService, RatePriceService>();
            service.AddScoped<ITransactionService, TransactionService>();
            service.AddScoped<IChapterServices, ChapterServices>();
            service.AddScoped<IForgotPasswordServices, ForgotPasswordServices>();
            service.AddScoped<IWalletService, WalletService>();
            service.AddScoped<IStoreInforSerrvice,StoreInforSerrvice>();
            service.AddScoped<IPayPalService, PayPalService>();
            service.AddScoped<ICertificationService, CertificationService>();
            return service;
        }
    }
}
