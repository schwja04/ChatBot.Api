﻿// <auto-generated />
using System;
using ChatBot.Infrastructure.Repositories.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ChatBot.Infrastructure.EntityFrameworkCore.Postgresql.Migrations
{
    [DbContext(typeof(ChatBotDbContext))]
    partial class ChatBotContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ChatBot.Domain.ChatContextEntity.ChatContext", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<Guid>("ContextId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

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

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("PromptId")
                        .HasColumnType("uuid");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("PromptId")
                        .IsUnique();

                    b.HasIndex("OwnerId", "Key")
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

                            NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b1.Property<long>("Id"));

                            b1.Property<Guid>("ChatContextContextId")
                                .HasColumnType("uuid");

                            b1.Property<string>("Content")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<DateTime>("CreatedAt")
                                .HasColumnType("timestamp with time zone");

                            b1.Property<Guid>("MessageId")
                                .HasColumnType("uuid");

                            b1.Property<string>("PromptKey")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<int>("Role")
                                .HasColumnType("integer");

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
