using AutoMapper;
using Project_Cursus_Group3.Data.Entities;
using Project_Cursus_Group3.Data.Model;
using Project_Cursus_Group3.Data.Model.BookmarkDetail;
using Project_Cursus_Group3.Data.Model.CategoryModel;
using Project_Cursus_Group3.Data.Model.ChapterModel;
using Project_Cursus_Group3.Data.Model.CourseModel;
using Project_Cursus_Group3.Data.Model.FeedBackModel;
using Project_Cursus_Group3.Data.Model.LessonModel;
using Project_Cursus_Group3.Data.Model.ReasonModel;
using Project_Cursus_Group3.Data.Model.Report;
using Project_Cursus_Group3.Data.Model.ReportModel;
using Project_Cursus_Group3.Data.Model.UserModel;
using Project_Cursus_Group3.Data.ViewModels;
using Project_Cursus_Group3.Data.ViewModels.BookMarkDTO;
using Project_Cursus_Group3.Data.ViewModels.ChapterDTO;
using Project_Cursus_Group3.Data.ViewModels.CourseDTO;
using Project_Cursus_Group3.Data.ViewModels.FeedbackDTO;
using Project_Cursus_Group3.Data.ViewModels.LessonDTO;
using Project_Cursus_Group3.Data.ViewModels.OrderDTO;
using Project_Cursus_Group3.Data.ViewModels.PurchaseDTO;
using Project_Cursus_Group3.Data.ViewModels.ReasonDTO;
using Project_Cursus_Group3.Data.ViewModels.Report;
using Project_Cursus_Group3.Data.ViewModels.ReportDTO;
using Project_Cursus_Group3.Data.ViewModels.RoleDTO;
using Project_Cursus_Group3.Data.ViewModels.StoreInforDTO;
using Project_Cursus_Group3.Data.ViewModels.TransactionsDTO;
using Project_Cursus_Group3.Data.ViewModels.UserDTO;
using Project_Cursus_Group3.Data.ViewModels.WalletDTO;


namespace Project_Cursus_Group3.Data.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Category, CategoryViewModel>().ReverseMap();
            CreateMap<Category, CreateCategoryModel>().ReverseMap();
            CreateMap<Category, UpdateCategoryModel>().ReverseMap();
            //User

            CreateMap<User, UserViewGet>()
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.Role))
                .ReverseMap();


            CreateMap<User, UserProfileUpdateModel>()
 .ForMember(dest => dest.Avatar, opt => opt.Ignore())
 .ForMember(dest => dest.Certification, opt => opt.Ignore())
 .ReverseMap();


            //Role
            CreateMap<Role, RoleViewModel>().ReverseMap();
            CreateMap<User, UserViewModel>().ReverseMap();

            //Course
            CreateMap<Course, CourseViewGET>().ForMember(dest => dest.AverageStarRating, opt => opt.MapFrom(src =>
                   src.PurchasedCourses.SelectMany(pc => pc.Feedback)
            .Where(fb => fb.Star.HasValue)
            .Select(fb => fb.Star.Value)
            .DefaultIfEmpty(0)
            .Average()
                    )).ReverseMap();


            CreateMap<Course, CourseViewRevenue>().ForMember(dest => dest.Revenue, opt => opt.MapFrom(src => src.OrderDetails.Sum(ord => ord.Price))).ReverseMap();


            CreateMap<Course, CourseViewModel>().ReverseMap();
            CreateMap<Course, UpdateCourseModel>().ReverseMap();
            CreateMap<UpdateCourseModel, CourseViewModel>().ReverseMap();
            CreateMap<Course, CreateCourseModel>().ReverseMap();


            //Bookmark
            CreateMap<Bookmark, BookmarkViewModel>().ReverseMap();


            //BookmarkDetail
            CreateMap<BookmarkDetail, BookmarkDetailViewModel>().ForMember(dest => dest.Course, opt => opt.MapFrom(src => src.Course)).ReverseMap();
            CreateMap<BookmarkDetail, CreateBookmarkDetailModel>().ReverseMap();





            //Report
            CreateMap<Report, CreateReportModel>().ReverseMap();
            CreateMap<Report, ReportViewModel>().ReverseMap();
            CreateMap<Report, UpdateReportModel>().ReverseMap();
            CreateMap<Report, ViewReportModel>()
    .ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.PurchasedCourse.Course.CourseTitle));

            //purchase
            CreateMap<PurchasedCourse, PurchaseViewModel>().ReverseMap();

            //Feedback
            CreateMap<Feedback, FeedbackViewModel>()
                .ForMember(dest => dest.course, opt => opt.MapFrom(src => src.PurchasedCourse.Course));
            CreateMap<Feedback, UpdateFeedbackModel>();
            CreateMap<Feedback, AddFeedbackModel>();
            CreateMap<Course, ViewCoursesModel>()
    .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));

            //Chapter
            CreateMap<Chapter, ChapterViewModel>();
            CreateMap<Chapter, UpdateChapterModel>();
            CreateMap<Chapter, AddChapterModel>();

            //reason
            CreateMap<Reason, ReasonViewModel>();

            CreateMap<Reason, UpdateReasonModel>();


            //Order
            CreateMap<Order, OrderViewModel>().ForMember(t => t.TotalPrice, o => o.MapFrom(p => p.OrderPrice));
            CreateMap<UpdateReasonModel, Reason>();
            //CreateMap<Reason, UpdateReasonModel>();

            //Lesson
            CreateMap<Lesson, LessonViewModel>().ReverseMap();
            CreateMap<Lesson, AddLessonModel>().ReverseMap();
            CreateMap<Lesson, UpdateLessonModel>().ReverseMap();

            CreateMap<Wallet, WalletViewModel>()
    .ForMember(dest => dest.Transactions, opt => opt.MapFrom(src => src.Transactions));

            CreateMap<Transactions, TransactionsViewModel>().ReverseMap();
            CreateMap<StoreViewModel, StoreInfos>().ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<StoreInfos, StoreViewModel>().ReverseMap();
        }
    }

}
