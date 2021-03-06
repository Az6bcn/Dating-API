﻿// <auto-generated />
using DatingAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace DatingAPI.Migrations
{
    [DbContext(typeof(DataDbContext))]
    [Migration("20190420143154_LikeEntityJoinTableOnDeleteBehaviourFKRequiredAddedIdentityPK")]
    partial class LikeEntityJoinTableOnDeleteBehaviourFKRequiredAddedIdentityPK
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.2-rtm-10011")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DatingAPI.Model.Like", b =>
                {
                    b.Property<int>("LikerUserID");

                    b.Property<int>("LikeeUserID");

                    b.Property<DateTime>("Date");

                    b.Property<int>("LikeID")
                        .ValueGeneratedOnAdd();

                    b.HasKey("LikerUserID", "LikeeUserID");

                    b.HasIndex("LikeeUserID");

                    b.ToTable("Likes");
                });

            modelBuilder.Entity("DatingAPI.Model.Photo", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CloudinaryID");

                    b.Property<DateTime>("DateAdded");

                    b.Property<DateTime?>("Deleted");

                    b.Property<string>("Description");

                    b.Property<bool>("IsMain");

                    b.Property<string>("Url");

                    b.Property<int>("UserId");

                    b.HasKey("ID");

                    b.HasIndex("UserId");

                    b.ToTable("Photos");
                });

            modelBuilder.Entity("DatingAPI.Model.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("City");

                    b.Property<string>("Country");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<DateTime>("DateOfBirth");

                    b.Property<string>("Gender");

                    b.Property<string>("Interests");

                    b.Property<string>("Introduction");

                    b.Property<string>("KnownAs");

                    b.Property<DateTime>("LastActive");

                    b.Property<string>("LookingFor");

                    b.Property<byte[]>("PasswordHash");

                    b.Property<byte[]>("PasswordSalt");

                    b.Property<string>("Username");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("DatingAPI.Model.Value", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Values");
                });

            modelBuilder.Entity("DatingAPI.Model.Like", b =>
                {
                    b.HasOne("DatingAPI.Model.User", "LikeeUser")
                        .WithMany("Likees")
                        .HasForeignKey("LikeeUserID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("DatingAPI.Model.User", "LikerUser")
                        .WithMany("Likers")
                        .HasForeignKey("LikerUserID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("DatingAPI.Model.Photo", b =>
                {
                    b.HasOne("DatingAPI.Model.User", "User")
                        .WithMany("Photos")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
