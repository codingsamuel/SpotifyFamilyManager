﻿// <auto-generated />

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SFM.Models;

namespace SFM.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    internal class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("SFM.Models.Config", b =>
            {
                b.Property<long>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("bigint")
                    .HasAnnotation("Npgsql:ValueGenerationStrategy",
                        NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                b.Property<string>("Key")
                    .IsRequired()
                    .HasColumnType("character varying(100)")
                    .HasMaxLength(100);

                b.Property<string>("Value")
                    .IsRequired()
                    .HasColumnType("character varying(255)")
                    .HasMaxLength(255);

                b.HasKey("Id");

                b.ToTable("Configs");
            });

            modelBuilder.Entity("SFM.Models.SpotifyUser", b =>
            {
                b.Property<long>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("bigint")
                    .HasAnnotation("Npgsql:ValueGenerationStrategy",
                        NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                b.Property<string>("ApiUrl")
                    .HasColumnType("text");

                b.Property<DateTime>("Created")
                    .HasColumnType("timestamp without time zone");

                b.Property<string>("DisplayName")
                    .IsRequired()
                    .HasColumnType("text");

                b.Property<string>("Email")
                    .IsRequired()
                    .HasColumnType("text");

                b.Property<string>("ImageUrl")
                    .HasColumnType("text");

                b.Property<string>("Product")
                    .IsRequired()
                    .HasColumnType("text");

                b.Property<string>("SpotifyId")
                    .IsRequired()
                    .HasColumnType("text");

                b.Property<string>("Type")
                    .HasColumnType("text");

                b.Property<DateTime>("Updated")
                    .HasColumnType("timestamp without time zone");

                b.Property<string>("Uri")
                    .HasColumnType("text");

                b.HasKey("Id");

                b.ToTable("SpotifyUsers");
            });

            modelBuilder.Entity("SFM.Models.Subscription", b =>
            {
                b.Property<long>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("bigint")
                    .HasAnnotation("Npgsql:ValueGenerationStrategy",
                        NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                b.Property<bool>("Active")
                    .HasColumnType("boolean");

                b.Property<DateTime>("LastPayment")
                    .HasColumnType("timestamp without time zone");

                b.Property<int>("PaymentInterval")
                    .HasColumnType("integer");

                b.Property<double>("Price")
                    .HasColumnType("double precision");

                b.Property<long>("SpotifyUserId")
                    .HasColumnType("bigint");

                b.Property<string>("Token")
                    .HasColumnType("text");

                b.HasKey("Id");

                b.HasIndex("SpotifyUserId");

                b.ToTable("Subscriptions");
            });

            modelBuilder.Entity("SFM.Models.UserAddress", b =>
            {
                b.Property<long>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("bigint")
                    .HasAnnotation("Npgsql:ValueGenerationStrategy",
                        NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                b.Property<string>("City")
                    .HasColumnType("text");

                b.Property<string>("Number")
                    .HasColumnType("text");

                b.Property<string>("PostCode")
                    .HasColumnType("text");

                b.Property<long>("SpotifyUserId")
                    .HasColumnType("bigint");

                b.Property<string>("State")
                    .HasColumnType("text");

                b.Property<string>("Street")
                    .HasColumnType("text");

                b.HasKey("Id");

                b.HasIndex("SpotifyUserId");

                b.ToTable("UserAddresses");
            });

            modelBuilder.Entity("SFM.Models.Subscription", b =>
            {
                b.HasOne("SFM.Models.SpotifyUser", "SpotifyUser")
                    .WithMany()
                    .HasForeignKey("SpotifyUserId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity("SFM.Models.UserAddress", b =>
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