﻿// <auto-generated />
using System;
using MarketMonitorApp.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace MarketMonitorApp.Migrations
{
    [DbContext(typeof(MarketMonitorDbContext))]
    partial class MarketMonitorDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.29")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("MarketMonitorApp.Entities.Actualization", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<int>("DistributorId")
                        .HasColumnType("int");

                    b.Property<bool>("IsEntered")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastActualization")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("DistributorId");

                    b.ToTable("Actualizations");
                });

            modelBuilder.Entity("MarketMonitorApp.Entities.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("DistributorId")
                        .HasColumnType("int");

                    b.Property<string>("LinkToCategory")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("DistributorId");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("MarketMonitorApp.Entities.Distributor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Distributors");
                });

            modelBuilder.Entity("MarketMonitorApp.Entities.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("ActualizationId")
                        .HasColumnType("int");

                    b.Property<string>("IdProduct")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<bool>("IsNew")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.HasIndex("ActualizationId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("MarketMonitorApp.Entities.Actualization", b =>
                {
                    b.HasOne("MarketMonitorApp.Entities.Distributor", "Distributor")
                        .WithMany()
                        .HasForeignKey("DistributorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Distributor");
                });

            modelBuilder.Entity("MarketMonitorApp.Entities.Category", b =>
                {
                    b.HasOne("MarketMonitorApp.Entities.Distributor", "Distributor")
                        .WithMany("Categories")
                        .HasForeignKey("DistributorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Distributor");
                });

            modelBuilder.Entity("MarketMonitorApp.Entities.Product", b =>
                {
                    b.HasOne("MarketMonitorApp.Entities.Actualization", "Actualization")
                        .WithMany("Products")
                        .HasForeignKey("ActualizationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Actualization");
                });

            modelBuilder.Entity("MarketMonitorApp.Entities.Actualization", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("MarketMonitorApp.Entities.Distributor", b =>
                {
                    b.Navigation("Categories");
                });
#pragma warning restore 612, 618
        }
    }
}
