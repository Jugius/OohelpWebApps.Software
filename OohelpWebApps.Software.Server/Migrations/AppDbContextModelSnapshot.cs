﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using OohelpWebApps.Software.Server.Database;

#nullable disable

namespace OohelpWebApps.Software.Server.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("OohelpWebApps.Software.Server.Database.DTO.ApplicationInfoDto", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("IsPublic")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Applications");
                });

            modelBuilder.Entity("OohelpWebApps.Software.Server.Database.DTO.ApplicationReleaseDto", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ApplicationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Kind")
                        .HasColumnType("int");

                    b.Property<DateTime>("ReleaseDate")
                        .HasColumnType("date");

                    b.Property<string>("Version")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationId");

                    b.ToTable("Releases");
                });

            modelBuilder.Entity("OohelpWebApps.Software.Server.Database.DTO.ReleaseDetailDto", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<int>("Kind")
                        .HasColumnType("int");

                    b.Property<Guid>("ReleaseId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ReleaseId");

                    b.ToTable("Details");
                });

            modelBuilder.Entity("OohelpWebApps.Software.Server.Database.DTO.ReleaseFileDto", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CheckSum")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("nvarchar(36)");

                    b.Property<string>("Description")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<int>("Kind")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<Guid>("ReleaseId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("RuntimeVersion")
                        .HasColumnType("int");

                    b.Property<int>("Size")
                        .HasColumnType("int");

                    b.Property<DateTime>("Uploaded")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ReleaseId");

                    b.ToTable("Files");
                });

            modelBuilder.Entity("OohelpWebApps.Software.Server.Database.DTO.ApplicationReleaseDto", b =>
                {
                    b.HasOne("OohelpWebApps.Software.Server.Database.DTO.ApplicationInfoDto", "Application")
                        .WithMany("Releases")
                        .HasForeignKey("ApplicationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Application");
                });

            modelBuilder.Entity("OohelpWebApps.Software.Server.Database.DTO.ReleaseDetailDto", b =>
                {
                    b.HasOne("OohelpWebApps.Software.Server.Database.DTO.ApplicationReleaseDto", "Release")
                        .WithMany("Details")
                        .HasForeignKey("ReleaseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Release");
                });

            modelBuilder.Entity("OohelpWebApps.Software.Server.Database.DTO.ReleaseFileDto", b =>
                {
                    b.HasOne("OohelpWebApps.Software.Server.Database.DTO.ApplicationReleaseDto", "Release")
                        .WithMany("Files")
                        .HasForeignKey("ReleaseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Release");
                });

            modelBuilder.Entity("OohelpWebApps.Software.Server.Database.DTO.ApplicationInfoDto", b =>
                {
                    b.Navigation("Releases");
                });

            modelBuilder.Entity("OohelpWebApps.Software.Server.Database.DTO.ApplicationReleaseDto", b =>
                {
                    b.Navigation("Details");

                    b.Navigation("Files");
                });
#pragma warning restore 612, 618
        }
    }
}
