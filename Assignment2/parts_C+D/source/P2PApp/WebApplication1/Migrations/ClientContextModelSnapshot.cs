﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebApplication1.Data;

#nullable disable

namespace WebApplication1.Migrations
{
    [DbContext(typeof(ClientContext))]
    partial class ClientContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.12");

            modelBuilder.Entity("WebApplication1.Models.Client", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("IPAddress")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Port")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Clients");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            IPAddress = "192.168.1.1",
                            Port = 8080
                        },
                        new
                        {
                            Id = 2,
                            IPAddress = "192.168.1.2",
                            Port = 8081
                        },
                        new
                        {
                            Id = 3,
                            IPAddress = "192.168.1.3",
                            Port = 8082
                        },
                        new
                        {
                            Id = 4,
                            IPAddress = "192.168.1.4",
                            Port = 8083
                        },
                        new
                        {
                            Id = 5,
                            IPAddress = "192.168.1.5",
                            Port = 8084
                        });
                });
#pragma warning restore 612, 618
        }
    }
}