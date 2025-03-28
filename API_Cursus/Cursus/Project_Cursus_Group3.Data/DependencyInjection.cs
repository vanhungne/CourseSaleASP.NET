using Microsoft.Extensions.DependencyInjection;
using Project_Cursus_Group3.Data.Interfaces;
using Project_Cursus_Group3.Data.Repository;
using Project_Cursus_Group3.Service.Interfaces;

namespace Project_Cursus_Group3.Data
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddRepository(this IServiceCollection service)
        {
            service.AddTransient(typeof(IUnitOfWork), typeof(UnitOfWork));
            service.AddTransient(typeof(IRepository<>), typeof(Repository<>));
            service.AddTransient<ICategoryRepository, CategoryRepository>();
            service.AddTransient<ICourseRepository, CourseRepository>();
            service.AddTransient<IAuthenRepository, AuthenRepository>();
            service.AddTransient<IUserRepository, UserRepository>();
            service.AddTransient<ICourseRepository, CourseRepository>();

            service.AddTransient<ILessonRepository, LessonRepository>();

            service.AddTransient<IBookmarkRepository, BookmarkRepository>();
            service.AddTransient<IBookmarkDetailRepository, BookmarkDetailRepository>();

            service.AddTransient<IEmailSender, EmailSenderRepository>();
            service.AddTransient<IAdminRepository, AdminRepository>();
            service.AddScoped<IReportRepository, ReportRepository>();
            service.AddScoped<ICartRepository, CartRepository>();
            service.AddTransient<IFeedbackRepository, FeedbackRepository>();
            service.AddTransient<IPurchasedCourseRepository, PurchasedCourseRepository>();
            service.AddScoped<IReasonRepository, ReasonRepository>();

            service.AddScoped<IOrderRepository, OrderRepository>();
            service.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
            service.AddScoped<IPaymentRepository, PaymentRepository>();
            service.AddScoped<ITransactionRepository, TransactionRepository>();

            service.AddScoped<IChapterRepository, ChapterRepository>();

            service.AddScoped<IWalletRepository, WalletRepository>();
            service.AddScoped<IAboutUsRepository, AboutUsRepository>();
            service.AddScoped<IStoreInforRepository, StoreInforRepository>();
            service.AddScoped<ICertificationRepository, CertificationRepository>();
            return service;
        }
    }
}
