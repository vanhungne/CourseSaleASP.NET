using Microsoft.EntityFrameworkCore;
using Project_Cursus_Group3.Data.Entities;

namespace Project_Cursus_Group3.Data.Data
{
    public class CursusDbContext : DbContext
    {
        public CursusDbContext()
        {
            
        }
        public CursusDbContext(DbContextOptions<CursusDbContext> dbContextOptions) : base(dbContextOptions)
        {

        }

        public DbSet<Role> Role { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Bookmark> Bookmark { get; set; }
        public DbSet<BookmarkDetail> BookmarkDetail { get; set; }
        public DbSet<Category> Category { get; set; }

        public DbSet<Chapter> Chapter { get; set; }
        public DbSet<Course> Course { get; set; }
        public DbSet<Feedback> Feedback { get; set; }
        public DbSet<Lesson> Lesson { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderDetail> OrderDetail { get; set; }
        public DbSet<Payment> Payment { get; set; }
        public DbSet<Transactions> Transactions { get; set; }
        public DbSet<ReTransactions> ReTransactions { get; set; }
        public DbSet<PurchasedCourse> PurchasedCourse { get; set; }
        public DbSet<Quiz> Quiz { get; set; }
        public DbSet<QuizAnswer> QuizAnswer { get; set; }
        public DbSet<QuizAttemp> QuizAttemp { get; set; }
        public DbSet<QuizQuestion> QuizQuestion { get; set; }
        public DbSet<Reason> Reason { get; set; }
        public DbSet<Report> Report { get; set; }
        public DbSet<Wallet> Wallet { get; set; }
        public DbSet<StoreInfos> StoreInfos { get; set; }
        public DbSet<AboutUs> AboutUs { get; set; }
        public DbSet<Refunds> Refunds { get; set; }
        public DbSet<Certifications> Certifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
                .HasOne(c => c.ParentCategory)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.NoAction); // Prevent cascade delete

            modelBuilder.Entity<BookmarkDetail>()
           .HasKey(bd => new { bd.BookmarkId, bd.CourseId });

            modelBuilder.Entity<BookmarkDetail>()
                .HasOne(bd => bd.Bookmark)
                .WithMany(b => b.BookmarkDetails)
                .HasForeignKey(bd => bd.BookmarkId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BookmarkDetail>()
                .HasOne(bd => bd.Course)
                .WithMany(c => c.BookmarkDetails)
                .HasForeignKey(bd => bd.CourseId)
                .OnDelete(DeleteBehavior.NoAction);

            //PurchasedCourse
            //modelBuilder.Entity<PurchasedCourse>()
            //.HasKey(pc => new { pc.CourseId, pc.UserName });

            //// Foreign key relationships
            //modelBuilder.Entity<PurchasedCourse>()
            //    .HasOne(pc => pc.Course)
            //    .WithMany() // Adjust as per your Course relationship
            //    .HasForeignKey(pc => pc.CourseId)
            //    .HasConstraintName("FK_PurchasedCourse_Course_CourseId")
            //    .OnDelete(DeleteBehavior.NoAction);

            //modelBuilder.Entity<PurchasedCourse>()
            //    .HasOne(pc => pc.User)
            //    .WithMany() // Assuming User can have multiple PurchasedCourses
            //    .HasForeignKey(pc => pc.UserName) // Make sure this matches the User entity's key
            //    .HasConstraintName("FK_PurchasedCourse_User_UserName")
            //    .OnDelete(DeleteBehavior.NoAction); // Or set as needed

            // Define composite primary key
            modelBuilder.Entity<PurchasedCourse>()
                .HasKey(pc => new { pc.CourseId, pc.UserName });

            // Foreign key relationship for Course
            modelBuilder.Entity<PurchasedCourse>()
                .HasOne(pc => pc.Course)
                .WithMany(c => c.PurchasedCourses)  // Assuming Course can have many PurchasedCourses
                .HasForeignKey(pc => pc.CourseId);

            // Foreign key relationship for User
            modelBuilder.Entity<PurchasedCourse>()
                .HasOne(pc => pc.User)
                .WithMany(u => u.PurchasedCourses)  // Assuming User can have many PurchasedCourses
                .HasForeignKey(pc => pc.UserName)
                .OnDelete(DeleteBehavior.NoAction);  // Disable cascading delete if needed

            // One-to-one relationships
            modelBuilder.Entity<PurchasedCourse>()
                .HasMany(pc => pc.Feedback)
                .WithOne(f => f.PurchasedCourse)
                .HasForeignKey(f => new { f.CourseId, f.UserName }); // Foreign key in Feedback

            modelBuilder.Entity<PurchasedCourse>()
                .HasOne(pc => pc.Report)
                .WithOne(r => r.PurchasedCourse)
                .HasForeignKey<Report>(r => new { r.CourseId, r.UserName }); // Foreign key in Report

            //OrrderDetail
            modelBuilder.Entity<OrderDetail>()
           .HasKey(od => new { od.OrderId, od.CourseId });

            modelBuilder.Entity<OrderDetail>()
          .HasOne(od => od.Course)
          .WithMany(c => c.OrderDetails)  // Adjust as per your relationship
          .HasForeignKey(od => od.CourseId);

            modelBuilder.Entity<OrderDetail>()
           .HasOne(od => od.Order)
           .WithMany(o => o.OrderDetails)
           .HasForeignKey(od => od.OrderId)
           .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<QuizAttemp>()
           .HasOne(qa => qa.User)
           .WithMany(u => u.QuizAttemps) // Assuming User can have multiple QuizAttempts
           .HasForeignKey(qa => qa.UserName)
           .OnDelete(DeleteBehavior.NoAction); // Prevent cascading deletes to avoid cycles

            // Configure QuizAttempt to Quiz relationship
            modelBuilder.Entity<QuizAttemp>()
                .HasOne(qa => qa.Quiz)
                .WithMany(q => q.QuizAttemps) // Assuming Quiz can have multiple QuizAttempts
                .HasForeignKey(qa => qa.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-one relationships
            modelBuilder.Entity<Course>()
                .HasOne(pc => pc.Reason)
                .WithOne(f => f.Course)
                .HasForeignKey<Reason>(f => f.CourseId); // Foreign key in Feedback

            modelBuilder.Entity<User>()
                .HasOne(pc => pc.Wallet)
                .WithOne(f => f.User)
                .HasForeignKey<Wallet>(f => f.UserName); // Foreign key in Feedback

            modelBuilder.Entity<User>()
               .HasOne(pc => pc.Bookmark)
               .WithOne(f => f.User)
               .HasForeignKey<Bookmark>(f => f.UserName); // Foreign key in Feedback

            modelBuilder.Entity<Refunds>()
                .Property(r => r.Amount)
                .HasColumnType("decimal(18, 2)");
        }
    }
}
