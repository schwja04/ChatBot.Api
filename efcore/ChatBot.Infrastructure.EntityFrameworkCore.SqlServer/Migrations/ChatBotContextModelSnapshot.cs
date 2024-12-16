﻿// <auto-generated />
using System;
using ChatBot.Infrastructure;
using ChatBot.Infrastructure.Repositories.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ChatBot.Infrastructure.EntityFrameworkCore.SqlServer.Migrations
{
    [DbContext(typeof(ChatBotDbContext))]
    partial class ChatBotContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ChatBot.Domain.ChatContextEntity.ChatContext", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<Guid>("ContextId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ContextId")
                        .IsUnique();

                    b.ToTable("ChatContexts");
                });

            modelBuilder.Entity("ChatBot.Domain.PromptEntity.Prompt", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Owner")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<Guid>("PromptId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("PromptId")
                        .IsUnique();

                    b.HasIndex("Owner", "Key")
                        .IsUnique();

                    b.ToTable("Prompts");
                });

            modelBuilder.Entity("ChatBot.Domain.ChatContextEntity.ChatContext", b =>
                {
                    b.OwnsMany("ChatBot.Domain.ChatContextEntity.ChatMessage", "_messages", b1 =>
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

                            b1.Property<DateTime>("CreatedAt")
                                .HasColumnType("datetime2");

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
