﻿// <auto-generated />
using System;
using DataServer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DataServer.Migrations
{
    [DbContext(typeof(ClientContext))]
    [Migration("20231020003028_AddSeedData")]
    partial class AddSeedData
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.12");

            modelBuilder.Entity("DataServer.Models.Client", b =>
                {
                    b.Property<string>("IPAddress")
                        .HasColumnType("TEXT");

                    b.Property<int?>("Port")
                        .HasColumnType("INTEGER");

                    b.Property<int>("CompletedJobs")
                        .HasColumnType("INTEGER");

                    b.HasKey("IPAddress", "Port");

                    b.ToTable("Clients");
                });
#pragma warning restore 612, 618
        }
    }
}
