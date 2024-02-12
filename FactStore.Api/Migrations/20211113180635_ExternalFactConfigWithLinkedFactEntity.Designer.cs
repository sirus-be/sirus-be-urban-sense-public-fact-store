﻿// <auto-generated />
using System;
using FactStore.Api.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace FactStore.Api.Migrations
{
    [DbContext(typeof(FactStoreDbContext))]
    [Migration("20211113180635_ExternalFactConfigWithLinkedFactEntity")]
    partial class ExternalFactConfigWithLinkedFactEntity
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityByDefaultColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("FactStore.Api.Models.ExternalFactConfigEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<string>("AuthenitcationUrl")
                        .HasColumnType("text");

                    b.Property<bool>("Authentication")
                        .HasColumnType("boolean");

                    b.Property<string>("ClientId")
                        .HasColumnType("text");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<int>("FactId")
                        .HasColumnType("integer");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("text");

                    b.Property<DateTime?>("LastModifiedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("RequestRenewalMinutes")
                        .HasColumnType("integer");

                    b.Property<string>("Secret")
                        .HasColumnType("text");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasMaxLength(2000)
                        .HasColumnType("character varying(2000)");

                    b.HasKey("Id");

                    b.HasIndex("FactId");

                    b.ToTable("ExternalFactConfigs");
                });

            modelBuilder.Entity("FactStore.Api.Models.FactEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<int>("FactTypeId")
                        .HasColumnType("integer");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("text");

                    b.Property<DateTime?>("LastModifiedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(2000)
                        .HasColumnType("character varying(2000)");

                    b.HasKey("Id");

                    b.HasIndex("FactTypeId");

                    b.HasIndex("Key")
                        .HasDatabaseName("IDX_Fact_Key");

                    b.ToTable("Facts");
                });

            modelBuilder.Entity("FactStore.Api.Models.FactTypeEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("text");

                    b.Property<DateTime?>("LastModifiedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .HasDatabaseName("IDX_FactType_Name");

                    b.ToTable("FactTypes");
                });

            modelBuilder.Entity("FactStore.Api.Models.RoleEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("text");

                    b.Property<DateTime?>("LastModifiedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .HasDatabaseName("IDX_Role_Name");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("FactTypeEntityRoleEntity", b =>
                {
                    b.Property<int>("FactTypesId")
                        .HasColumnType("integer");

                    b.Property<int>("RolesId")
                        .HasColumnType("integer");

                    b.HasKey("FactTypesId", "RolesId");

                    b.HasIndex("RolesId");

                    b.ToTable("FactTypeRoles");
                });

            modelBuilder.Entity("FactStore.Api.Models.ExternalFactConfigEntity", b =>
                {
                    b.HasOne("FactStore.Api.Models.FactEntity", "Fact")
                        .WithMany("ExternalFactConfigs")
                        .HasForeignKey("FactId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Fact");
                });

            modelBuilder.Entity("FactStore.Api.Models.FactEntity", b =>
                {
                    b.HasOne("FactStore.Api.Models.FactTypeEntity", "FactType")
                        .WithMany("Facts")
                        .HasForeignKey("FactTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FactType");
                });

            modelBuilder.Entity("FactTypeEntityRoleEntity", b =>
                {
                    b.HasOne("FactStore.Api.Models.FactTypeEntity", null)
                        .WithMany()
                        .HasForeignKey("FactTypesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FactStore.Api.Models.RoleEntity", null)
                        .WithMany()
                        .HasForeignKey("RolesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("FactStore.Api.Models.FactEntity", b =>
                {
                    b.Navigation("ExternalFactConfigs");
                });

            modelBuilder.Entity("FactStore.Api.Models.FactTypeEntity", b =>
                {
                    b.Navigation("Facts");
                });
#pragma warning restore 612, 618
        }
    }
}
