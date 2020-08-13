﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SFM.Models;

namespace SFM.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20200813203954_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("SFM.Models.SpotifyUser", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("SpotifyUsers");
                });

            modelBuilder.Entity("SFM.Models.Subscription", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastPayment")
                        .HasColumnType("datetime");

                    b.Property<int>("PaymentInterval")
                        .HasColumnType("int");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<long>("SpotifyUserId")
                        .HasColumnType("bigint");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("SpotifyUserId");

                    b.ToTable("Subscriptions");
                });

            modelBuilder.Entity("SFM.Models.Subscription", b =>
                {
                    b.HasOne("SFM.Models.SpotifyUser", "SpotifyUser")
                        .WithMany()
                        .HasForeignKey("SpotifyUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}