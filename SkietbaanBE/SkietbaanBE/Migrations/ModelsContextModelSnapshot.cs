﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using SkietbaanBE.Models;
using System;

namespace SkietbaanBE.Migrations
{
    [DbContext(typeof(ModelsContext))]
    partial class ModelsContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.3-rtm-10026")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("SkietbaanBE.Models.Competition", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<bool>("Status");

                    b.HasKey("Id");

                    b.ToTable("Competitions");
                });

            modelBuilder.Entity("SkietbaanBE.Models.Group", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("SkietbaanBE.Models.Notification", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("NotificationMessage");

                    b.Property<string>("NotitficationContent");

                    b.Property<bool>("isRead");

                    b.HasKey("Id");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("SkietbaanBE.Models.Score", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CompetitionId");

                    b.Property<string>("PictureURL");

                    b.Property<DateTime?>("UploadDate");

                    b.Property<int?>("UserId");

                    b.Property<int>("UserScore");

                    b.HasKey("Id");

                    b.HasIndex("CompetitionId");

                    b.HasIndex("UserId");

                    b.ToTable("Scores");
                });

            modelBuilder.Entity("SkietbaanBE.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Admin");

                    b.Property<string>("Email");

                    b.Property<DateTime>("EntryDate");

                    b.Property<DateTime>("MemberExpiry");

                    b.Property<string>("MemberID");

                    b.Property<string>("Password");

                    b.Property<string>("Token");

                    b.Property<string>("Username");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("SkietbaanBE.Models.UserCompStats", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BestScore");

                    b.Property<int>("CompScore");

                    b.Property<int?>("CompetitionId");

                    b.Property<int?>("UserId");

                    b.Property<int>("Year");

                    b.HasKey("Id");

                    b.HasIndex("CompetitionId");

                    b.HasIndex("UserId");

                    b.ToTable("UserCompStats");
                });

            modelBuilder.Entity("SkietbaanBE.Models.UserGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("GroupId");

                    b.Property<int?>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.HasIndex("UserId");

                    b.ToTable("UserGroups");
                });

            modelBuilder.Entity("SkietbaanBE.Models.UserNotification", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("NotificationId");

                    b.Property<int?>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("NotificationId");

                    b.HasIndex("UserId");

                    b.ToTable("UserNotifications");
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

            modelBuilder.Entity("SkietbaanBE.Models.UserCompStats", b =>
                {
                    b.HasOne("SkietbaanBE.Models.Competition", "Competition")
                        .WithMany()
                        .HasForeignKey("CompetitionId");

                    b.HasOne("SkietbaanBE.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("SkietbaanBE.Models.UserGroup", b =>
                {
                    b.HasOne("SkietbaanBE.Models.Group", "Group")
                        .WithMany()
                        .HasForeignKey("GroupId");

                    b.HasOne("SkietbaanBE.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("SkietbaanBE.Models.UserNotification", b =>
                {
                    b.HasOne("SkietbaanBE.Models.Notification", "Notification")
                        .WithMany()
                        .HasForeignKey("NotificationId");

                    b.HasOne("SkietbaanBE.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });
#pragma warning restore 612, 618
        }
    }
}
