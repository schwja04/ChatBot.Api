﻿// <auto-generated />
using System;
using ChatBot.Api.Infrastructure;
using ChatBot.Api.Infrastructure.Repositories.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ChatBot.Api.EntityFrameworkCore.SqlServer.Migrations
{
    [DbContext(typeof(ChatBotContext))]
    [Migration("20241030040232_Init")]
    partial class Init
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ChatBot.Api.Domain.ChatContextEntity.ChatContext", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<Guid>("ContextId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ContextId")
                        .IsUnique();

                    b.ToTable("ChatContexts");
                });

            modelBuilder.Entity("ChatBot.Api.Domain.ChatContextEntity.ChatContext", b =>
                {
                    b.OwnsMany("ChatBot.Api.Domain.ChatContextEntity.ChatMessage", "_messages", b1 =>
                        {
                            b1.Property<long>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("bigint");

                            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b1.Property<long>("Id"));

                            b1.Property<Guid>("ChatContextContextId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("Content")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<DateTimeOffset>("CreatedAt")
                                .HasColumnType("datetimeoffset");

                            b1.Property<Guid>("MessageId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("PromptKey")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<int>("Role")
                                .HasColumnType("int");

                            b1.HasKey("Id");

                            b1.HasIndex("ChatContextContextId");

                            b1.HasIndex("MessageId")
                                .IsUnique();

                            b1.ToTable("ChatMessage");

                            b1.WithOwner()
                                .HasForeignKey("ChatContextContextId")
                                .HasPrincipalKey("ContextId");
                        });

                    b.Navigation("_messages");
                });
#pragma warning restore 612, 618
        }
    }
}
