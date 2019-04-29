﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using SkietbaanBE.Models;
using System;

namespace SkietbaanBE.Migrations
{
    [DbContext(typeof(ModelsContext))]
    [Migration("20190425084656_myMigration")]
    partial class myMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.3-rtm-10026")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("SkietbaanBE.Models.Award", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CompetitionId");

                    b.Property<string>("Description");

                    b.Property<int>("Month");

                    b.Property<int?>("UserId");

                    b.Property<int>("Year");

                    b.HasKey("Id");

                    b.HasIndex("CompetitionId");

                    b.HasIndex("UserId");

                    b.ToTable("Awards");
                });

            modelBuilder.Entity("SkietbaanBE.Models.Competition", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BestScoresNumber");

                    b.Property<int>("Hours");

                    b.Property<int>("MaximumScore");

                    b.Property<string>("Name");

                    b.Property<bool>("Status");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasFilter("[Name] IS NOT NULL");

                    b.ToTable("Competitions");
                });

            modelBuilder.Entity("SkietbaanBE.Models.Group", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsActive");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasFilter("[Name] IS NOT NULL");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("SkietbaanBE.Models.Notifications", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsRead");

                    b.Property<string>("NotificationMessage");

                    b.Property<string>("TimeOfArrival");

                    b.Property<string>("TypeOfNotification");

                    b.Property<int?>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("SkietbaanBE.Models.OTP", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<DateTime>("OTPExpiry");

                    b.Property<int>("OneTimePassword");

                    b.HasKey("UserId");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("OTPs");
                });

            modelBuilder.Entity("SkietbaanBE.Models.Requirement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("Accuracy");

                    b.Property<int?>("CompetitionId");

                    b.Property<string>("Standard");

                    b.Property<double>("Total");

                    b.HasKey("Id");

                    b.HasIndex("CompetitionId");

                    b.ToTable("Requirements");
                });

            modelBuilder.Entity("SkietbaanBE.Models.Score", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CompetitionId");

                    b.Property<float?>("Latitude");

                    b.Property<float?>("Longitude");

                    b.Property<string>("PictureURL");

                    b.Property<DateTime?>("UploadDate");

                    b.Property<int?>("UserId");

                    b.Property<double>("UserScore");

                    b.HasKey("Id");

                    b.HasIndex("CompetitionId");

                    b.HasIndex("UserId");

                    b.ToTable("Scores");
                });

            modelBuilder.Entity("SkietbaanBE.Models.TimeSpent", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<int>("HoursSpent");

                    b.HasKey("UserId");

                    b.ToTable("TimeSpents");
                });

            modelBuilder.Entity("SkietbaanBE.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Admin");

                    b.Property<DateTime?>("AdvanceExpiryDate");

                    b.Property<string>("Email");

                    b.Property<DateTime>("EntryDate");

                    b.Property<DateTime?>("MemberExpiryDate");

                    b.Property<string>("MemberID");

                    b.Property<DateTime?>("MemberStartDate");

                    b.Property<string>("Name");

                    b.Property<string>("Password");

                    b.Property<string>("PhoneNumber");

                    b.Property<string>("Surname");

                    b.Property<string>("Token");

                    b.Property<string>("Username");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("SkietbaanBE.Models.UserCompetitionTotalScore", b =>
                {
                    b.Property<int>("CompetitionId");

                    b.Property<int>("UserId");

                    b.Property<double>("Average");

                    b.Property<double>("Best");

                    b.Property<double>("PreviousTotal");

                    b.Property<double>("Total");

                    b.Property<int>("Year");

                    b.HasKey("CompetitionId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("UserCompetitionTotalScores");
                });

            modelBuilder.Entity("SkietbaanBE.Models.UserGroup", b =>
                {
                    b.Property<int>("GroupId");

                    b.Property<int>("UserId");

                    b.HasKey("GroupId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("UserGroups");
                });

            modelBuilder.Entity("SkietbaanBE.Models.Award", b =>
                {
                    b.HasOne("SkietbaanBE.Models.Competition", "Competition")
                        .WithMany()
                        .HasForeignKey("CompetitionId");

                    b.HasOne("SkietbaanBE.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("SkietbaanBE.Models.Notifications", b =>
                {
                    b.HasOne("SkietbaanBE.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("SkietbaanBE.Models.OTP", b =>
                {
                    b.HasOne("SkietbaanBE.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SkietbaanBE.Models.Requirement", b =>
                {
                    b.HasOne("SkietbaanBE.Models.Competition", "Competition")
                        .WithMany()
                        .HasForeignKey("CompetitionId");
                });

            modelBuilder.Entity("SkietbaanBE.Models.Score", b =>
                {
                    b.HasOne("SkietbaanBE.Models.Competition", "Competition")
                        .WithMany()
                        .HasForeignKey("CompetitionId");

                    b.HasOne("SkietbaanBE.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("SkietbaanBE.Models.TimeSpent", b =>
                {
                    b.HasOne("SkietbaanBE.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SkietbaanBE.Models.UserCompetitionTotalScore", b =>
                {
                    b.HasOne("SkietbaanBE.Models.Competition", "Competition")
                        .WithMany()
                        .HasForeignKey("CompetitionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SkietbaanBE.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SkietbaanBE.Models.UserGroup", b =>
                {
                    b.HasOne("SkietbaanBE.Models.Group", "Group")
                        .WithMany()
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SkietbaanBE.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
