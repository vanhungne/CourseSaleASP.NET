﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Project_Cursus_Group3.Data.Data;

#nullable disable

namespace Project_Cursus_Group3.Data.Migrations
{
    [DbContext(typeof(CursusDbContext))]
    [Migration("20241002130638_AddNullableAvatar")]
    partial class AddNullableAvatar
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Project_Cursus_Group3.Data.Entities.Bookmark", b =>
                {
                    b.Property<int>("BookmarkId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("BookmarkId"));

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("BookmarkId");

                    b.HasIndex("UserName")
                        .IsUnique()
                        .HasFilter("[UserName] IS NOT NULL");

                    b.ToTable("Bookmark");
                });

            modelBuilder.Entity("Project_Cursus_Group3.Data.Entities.BookmarkDetail", b =>
                {
                    b.Property<int?>("BookmarkId")
                        .HasColumnType("int")
                        .HasColumnOrder(0);

                    b.Property<int?>("CourseId")
                        .HasColumnType("int")
                        .HasColumnOrder(1);

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("BookmarkId", "CourseId");

                    b.HasIndex("CourseId");

                    b.ToTable("BookmarkDetail");
                });

            modelBuilder.Entity("Project_Cursus_Group3.Data.Entities.Category", b =>
                {
                    b.Property<int>("CategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CategoryId"));

                    b.Property<string>("CategoryName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ParentCategoryId")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CategoryId");

                    b.HasIndex("ParentCategoryId");

                    b.ToTable("Category");
                });

            modelBuilder.Entity("Project_Cursus_Group3.Data.Entities.Chapter", b =>
                {
                    b.Property<int>("ChapterId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ChapterId"));

                    b.Property<string>("ChapterTitle")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("CourseId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Duration")
                        .HasColumnType("int");

                    b.Property<double?>("Process")
                        .HasColumnType("float");

                    b.Property<string>("SubDescription")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ChapterId");

                    b.HasIndex("CourseId");

                    b.ToTable("Chapter");
                });

            modelBuilder.Entity("Project_Cursus_Group3.Data.Entities.Course", b =>
                {
                    b.Property<int>("CourseId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CourseId"));

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<string>("CourseCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CourseTitle")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Discount")
                        .HasColumnType("float");

                    b.Property<string>("Image")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsComment")
                        .HasColumnType("bit");

                    b.Property<int>("Level")
                        .HasColumnType("int");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.Property<string>("ShortDescription")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TotalEnrollment")
                        .HasColumnType("int");

                    b.Property<string>("Username")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("CourseId");

                    b.HasIndex("CategoryId");

                    b.HasIndex("Username");

                    b.ToTable("Course");
                });

            modelBuilder.Entity("Project_Cursus_Group3.Data.Entities.Feedback", b =>
                {
                    b.Property<int>("FeedbackId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("FeedbackId"));

                    b.Property<string>("Attachment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Content")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("CourseId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("Star")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("FeedbackId");

                    b.HasIndex("CourseId", "UserName")
                        .IsUnique()
                        .HasFilter("[CourseId] IS NOT NULL AND [UserName] IS NOT NULL");

                    b.ToTable("Feedback");
                });

            modelBuilder.Entity("Project_Cursus_Group3.Data.Entities.Lesson", b =>
                {
                    b.Property<int>("LessonId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("LessonId"));

                    b.Property<int?>("ChapterId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Duration")
                        .HasColumnType("int");

                    b.Property<string>("LessonTitle")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double?>("Process")
                        .HasColumnType("float");

                    b.Property<string>("VideoURL")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("LessonId");

                    b.HasIndex("ChapterId");

                    b.ToTable("Lesson");
                });

            modelBuilder.Entity("Project_Cursus_Group3.Data.Entities.Order", b =>
                {
                    b.Property<int>("OrderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("OrderId"));

                    b.Property<string>("OrderCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("datetime2");

                    b.Property<double?>("OrderPrice")
                        .HasColumnType("float");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("OrderId");

                    b.HasIndex("UserName");

                    b.ToTable("Order");
                });

            modelBuilder.Entity("Project_Cursus_Group3.Data.Entities.OrderDetail", b =>
                {
                    b.Property<int>("OrderId")
                        .HasColumnType("int");

                    b.Property<int>("CourseId")
                        .HasColumnType("int");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.HasKey("OrderId", "CourseId");

                    b.HasIndex("CourseId");

                    b.ToTable("OrderDetail");
                });

            modelBuilder.Entity("Project_Cursus_Group3.Data.Entities.Payment", b =>
                {
                    b.Property<int>("PaymentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PaymentId"));

                    b.Property<double?>("Amount")
                        .HasColumnType("float");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("OrderId")
                        .HasColumnType("int");

                    b.Property<string>("PaymentCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PaymentId");

                    b.HasIndex("OrderId");

                    b.ToTable("Payment");
                });

            modelBuilder.Entity("Project_Cursus_Group3.Data.Entities.PurchasedCourse", b =>
                {
                    b.Property<int>("CourseId")
                        .HasColumnType("int");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CourseId", "UserName");

                    b.HasIndex("UserName");

                    b.ToTable("PurchasedCourse");
                });

            modelBuilder.Entity("Project_Cursus_Group3.Data.Entities.Quiz", b =>
                {
                    b.Property<int>("QuizId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("QuizId"));

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("LessonId")
                        .HasColumnType("int");

                    b.Property<int?>("PassScore")
                        .HasColumnType("int");

                    b.Property<int?>("QuizTime")
                        .HasColumnType("int");

                    b.Property<string>("QuizTitle")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("QuizId");

                    b.HasIndex("LessonId");

                    b.ToTable("Quiz");
                });

            modelBuilder.Entity("Project_Cursus_Group3.Data.Entities.QuizAnswer", b =>
                {
                    b.Property<int>("QuizAnswerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("QuizAnswerId"));

                    b.Property<string>("AnswerText")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("IsCorrect")
                        .HasColumnType("bit");

                    b.Property<int?>("QuizQuestionId")
                        .HasColumnType("int");

                    b.HasKey("QuizAnswerId");

                    b.HasIndex("QuizQuestionId");

                    b.ToTable("QuizAnswer");
                });

            modelBuilder.Entity("Project_Cursus_Group3.Data.Entities.QuizAttemp", b =>
                {
                    b.Property<int>("AttempId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AttempId"));

                    b.Property<int?>("CompletedTime")
                        .HasColumnType("int");

                    b.Property<int?>("QuizId")
                        .HasColumnType("int");

                    b.Property<int?>("Score")
                        .HasColumnType("int");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("AttempId");

                    b.HasIndex("QuizId");

                    b.HasIndex("UserName");

                    b.ToTable("QuizAttemp");
                });

            modelBuilder.Entity("Project_Cursus_Group3.Data.Entities.QuizQuestion", b =>
                {
                    b.Property<int>("QuizQuestionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("QuizQuestionId"));

                    b.Property<string>("QuestionText")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("QuestionType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("QuizId")
                        .HasColumnType("int");

                    b.HasKey("QuizQuestionId");

                    b.HasIndex("QuizId");

                    b.ToTable("QuizQuestion");
                });

            modelBuilder.Entity("Project_Cursus_Group3.Data.Entities.Reason", b =>
                {
                    b.Property<int>("ReasonId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ReasonId"));

                    b.Property<string>("Content")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("CourseId")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ReasonId");

                    b.HasIndex("CourseId")
                        .IsUnique()
                        .HasFilter("[CourseId] IS NOT NULL");

                    b.ToTable("Reason");
                });

            modelBuilder.Entity("Project_Cursus_Group3.Data.Entities.Report", b =>
                {
                    b.Property<int>("ReportId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ReportId"));

                    b.Property<string>("Attachment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Content")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("CourseId")
                        .HasColumnType("int");

                    b.Property<string>("Issue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("ReportId");

                    b.HasIndex("CourseId", "UserName")
                        .IsUnique()
                        .HasFilter("[CourseId] IS NOT NULL AND [UserName] IS NOT NULL");

                    b.ToTable("Report");
                });

            modelBuilder.Entity("Project_Cursus_Group3.Data.Entities.Role", b =>
                {
                    b.Property<int>("RoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RoleId"));

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("RoleId");

                    b.ToTable("Role");
                });

            modelBuilder.Entity("Project_Cursus_Group3.Data.Entities.User", b =>
                {
                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Avatar")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Certification")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Comment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DOB")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserName");

                    b.HasIndex("RoleId");

                    b.ToTable("User");
                });

            modelBuilder.Entity("Project_Cursus_Group3.Data.Entities.Wallet", b =>
                {
                    b.Property<int>("WalletId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("WalletId"));

                    b.Property<double>("Balance")
                        .HasColumnType("float");

                    b.Property<DateTime>("TransactionTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("WalletId");

                    b.HasIndex("UserName")
                        .IsUnique();

                    b.ToTable("Wallet");
                });

            modelBuilder.Entity("Project_Cursus_Group3.Data.Entities.Bookmark", b =>
                {
                    b.HasOne("Project_Cursus_Group3.Data.Entities.User", "User")
                        .WithOne("Bookmark")
                        .HasForeignKey("Project_Cursus_Group3.Data.Entities.Bookmark", "UserName");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Project_Cursus_Group3.Data.Entities.BookmarkDetail", b =>
                {
                    b.HasOne("Project_Cursus_Group3.Data.Entities.Bookmark", "Bookmark")
                        .WithMany("BookmarkDetails")
                        .HasForeignKey("BookmarkId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Project_Cursus_Group3.Data.Entities.Course", "Course")
                        .WithMany("BookmarkDetails")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Bookmark");

                    b.Navigation("Course");
                });

            modelBuilder.Entity("Project_Cursus_Group3.Data.Entities.Category", b =>
                {
                    b.HasOne("Project_Cursus_Group3.Data.Entities.Category", "ParentCategory")
                        .WithMany("SubCategories")
                        .HasForeignKey("ParentCategoryId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("ParentCategory");
                });

            modelBuilder.Entity("Project_Cursus_Group3.Data.Entities.Chapter", b =>
                {
                    b.HasOne("Project_Cursus_Group3.Data.Entities.Course", "Course")
                        .WithMany("Chapters")
                        .HasForeignKey("CourseId");

                    b.Navigation("Course");
                });

            modelBuilder.Entity("Project_Cursus_Group3.Data.Entities.Course", b =>
                {
                    b.HasOne("Project_Cursus_Group3.Data.Entities.Category", "Category")
                        .WithMany("Courses")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Project_Cursus_Group3.Data.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("Username");

                    b.Navigation("Category");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Project_Cursus_Group3.Data.Entities.Feedback", b =>
                {
                    b.HasOne("Project_Cursus_Group3.Data.Entities.PurchasedCourse", "PurchasedCourse")
                        .WithOne("Feedback")
                        .HasForeignKey("Project_Cursus_Group3.Data.Entities.Feedback", "CourseId", "UserName");

                    b.Navigation("PurchasedCourse");
                });

            modelBuilder.Entity("Project_Cursus_Group3.Data.Entities.Lesson", b =>
                {
                    b.HasOne("Project_Cursus_Group3.Data.Entities.Chapter", "Chapter")
                        .WithMany("Lessons")
                        .HasForeignKey("ChapterId");

                    b.Navigation("Chapter");
                });

            modelBuilder.Entity("Project_Cursus_Group3.Data.Entities.Order", b =>
                {
                    b.HasOne("Project_Cursus_Group3.Data.Entities.User", "User")
                        .WithMany("Orders")
                        .HasForeignKey("UserName");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Project_Cursus_Group3.Data.Entities.OrderDetail", b =>
                {
                    b.HasOne("Project_Cursus_Group3.Data.Entities.Course", "Course")
                        .WithMany("OrderDetails")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Project_Cursus_Group3.Data.Entities.Order", "Order")
                        .WithMany("OrderDetails")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Course");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("Project_Cursus_Group3.Data.Entities.Payment", b =>
                {
                    b.HasOne("Project_Cursus_Group3.Data.Entities.Order", "Order")
                        .WithMany("Payments")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");
                });

            modelBuilder.Entity("Project_Cursus_Group3.Data.Entities.PurchasedCourse", b =>
                {
                    b.HasOne("Project_Cursus_Group3.Data.Entities.Course", "Course")
                        .WithMany("PurchasedCourses")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Project_Cursus_Group3.Data.Entities.User", "User")
                        .WithMany("PurchasedCourses")
                        .HasForeignKey("UserName")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Course");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Project_Cursus_Group3.Data.Entities.Quiz", b =>
                {
                    b.HasOne("Project_Cursus_Group3.Data.Entities.Lesson", "Lesson")
                        .WithMany("Quizzes")
                        .HasForeignKey("LessonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Lesson");
                });

            modelBuilder.Entity("Project_Cursus_Group3.Data.Entities.QuizAnswer", b =>
                {
                    b.HasOne("Project_Cursus_Group3.Data.Entities.QuizQuestion", "QuizQuestion")
                        .WithMany("QuizAnswers")
                        .HasForeignKey("QuizQuestionId");

                    b.Navigation("QuizQuestion");
                });

            modelBuilder.Entity("Project_Cursus_Group3.Data.Entities.QuizAttemp", b =>
                {
                    b.HasOne("Project_Cursus_Group3.Data.Entities.Quiz", "Quiz")
                        .WithMany("QuizAttemps")
                        .HasForeignKey("QuizId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Project_Cursus_Group3.Data.Entities.User", "User")
                        .WithMany("QuizAttemps")
                        .HasForeignKey("UserName")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("Quiz");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Project_Cursus_Group3.Data.Entities.QuizQuestion", b =>
                {
                    b.HasOne("Project_Cursus_Group3.Data.Entities.Quiz", "Quiz")
                        .WithMany("QuizQuestions")
                        .HasForeignKey("QuizId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Quiz");
                });

            modelBuilder.Entity("Project_Cursus_Group3.Data.Entities.Reason", b =>
                {
                    b.HasOne("Project_Cursus_Group3.Data.Entities.Course", "Course")
                        .WithOne("Reason")
                        .HasForeignKey("Project_Cursus_Group3.Data.Entities.Reason", "CourseId");

                    b.Navigation("Course");
                });

            modelBuilder.Entity("Project_Cursus_Group3.Data.Entities.Report", b =>
                {
                    b.HasOne("Project_Cursus_Group3.Data.Entities.PurchasedCourse", "PurchasedCourse")
                        .WithOne("Report")
                        .HasForeignKey("Project_Cursus_Group3.Data.Entities.Report", "CourseId", "UserName");

                    b.Navigation("PurchasedCourse");
                });

            modelBuilder.Entity("Project_Cursus_Group3.Data.Entities.User", b =>
                {
                    b.HasOne("Project_Cursus_Group3.Data.Entities.Role", "Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");
                });

            modelBuilder.Entity("Project_Cursus_Group3.Data.Entities.Wallet", b =>
                {
                    b.HasOne("Project_Cursus_Group3.Data.Entities.User", "User")
                        .WithOne("Wallet")
                        .HasForeignKey("Project_Cursus_Group3.Data.Entities.Wallet", "UserName")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Project_Cursus_Group3.Data.Entities.Bookmark", b =>
                {
                    b.Navigation("BookmarkDetails");
                });

            modelBuilder.Entity("Project_Cursus_Group3.Data.Entities.Category", b =>
                {
                    b.Navigation("Courses");

                    b.Navigation("SubCategories");
                });

            modelBuilder.Entity("Project_Cursus_Group3.Data.Entities.Chapter", b =>
                {
                    b.Navigation("Lessons");
                });

            modelBuilder.Entity("Project_Cursus_Group3.Data.Entities.Course", b =>
                {
                    b.Navigation("BookmarkDetails");

                    b.Navigation("Chapters");

                    b.Navigation("OrderDetails");

                    b.Navigation("PurchasedCourses");

                    b.Navigation("Reason")
                        .IsRequired();
                });

            modelBuilder.Entity("Project_Cursus_Group3.Data.Entities.Lesson", b =>
                {
                    b.Navigation("Quizzes");
                });

            modelBuilder.Entity("Project_Cursus_Group3.Data.Entities.Order", b =>
                {
                    b.Navigation("OrderDetails");

                    b.Navigation("Payments");
                });

            modelBuilder.Entity("Project_Cursus_Group3.Data.Entities.PurchasedCourse", b =>
                {
                    b.Navigation("Feedback")
                        .IsRequired();

                    b.Navigation("Report")
                        .IsRequired();
                });

            modelBuilder.Entity("Project_Cursus_Group3.Data.Entities.Quiz", b =>
                {
                    b.Navigation("QuizAttemps");

                    b.Navigation("QuizQuestions");
                });

            modelBuilder.Entity("Project_Cursus_Group3.Data.Entities.QuizQuestion", b =>
                {
                    b.Navigation("QuizAnswers");
                });

            modelBuilder.Entity("Project_Cursus_Group3.Data.Entities.Role", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("Project_Cursus_Group3.Data.Entities.User", b =>
                {
                    b.Navigation("Bookmark");

                    b.Navigation("Orders");

                    b.Navigation("PurchasedCourses");

                    b.Navigation("QuizAttemps");

                    b.Navigation("Wallet");
                });
#pragma warning restore 612, 618
        }
    }
}
