using ChatBot.Domain.ChatContextEntity;
using ChatBot.Domain.PromptEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatBot.Infrastructure.Repositories.Persistence.EntityFrameworkCore;

public class ChatBotDbContext(DbContextOptions<ChatBotDbContext> options) : DbContext(options)
{
    public DbSet<ChatContext> ChatContexts { get; set; }
    public DbSet<Prompt> Prompts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ChatContextConfiguration());
        modelBuilder.ApplyConfiguration(new PromptConfiguration());
    }
}

public class PromptConfiguration : IEntityTypeConfiguration<Prompt>
{
    public void Configure(EntityTypeBuilder<Prompt> builder)
    {
        builder.Property<long>("Id")
            .IsRequired()
            .ValueGeneratedOnAdd();
        builder.HasKey("Id");
        
        builder.Property(p => p.PromptId).IsRequired();
        builder.Property(p => p.Key).IsRequired();
        builder.Property(p => p.Value).IsRequired();
        builder.Property(p => p.OwnerId).IsRequired();
        
        builder.HasIndex(p => p.PromptId).IsUnique();
        builder.HasIndex(p => new { Owner = p.OwnerId, p.Key }).IsUnique();
    }
}

public class ChatContextConfiguration : IEntityTypeConfiguration<ChatContext>
{
    public void Configure(EntityTypeBuilder<ChatContext> builder)
    {
        // Configure primary key
        builder.Property<long>("Id")
            .IsRequired()
            .ValueGeneratedOnAdd();
        builder.HasKey("Id");
        builder.Property(cc => cc.ContextId).IsRequired();

        // Configure properties
        builder.Property(cc => cc.Title).IsRequired();
        builder.Property(cc => cc.UserId).IsRequired();
        builder.Property(cc => cc.CreatedAt)
            .HasConversion(x => x.UtcDateTime, x => new DateTimeOffset(x, TimeSpan.Zero))
            .IsRequired();
        builder.Property(cc => cc.UpdatedAt)
            .HasConversion(x => x.UtcDateTime, x => new DateTimeOffset(x, TimeSpan.Zero))
            .IsRequired();

        // Configure one-to-many relationship between ChatContext and ChatMessage
        builder.Ignore(cc => cc.Messages);
        
        // Configure the relationship
        builder.OwnsMany<ChatMessage>("_messages", cmNavBuilder =>
        {
            cmNavBuilder.WithOwner().HasPrincipalKey("ContextId");
            // Configure primary key
            cmNavBuilder.Property<long>("Id")
                .IsRequired()
                .ValueGeneratedOnAdd();  // Auto-incrementing integer
            cmNavBuilder.HasKey("Id"); 
        
            cmNavBuilder.Property(cm => cm.MessageId).IsRequired();

            // Configure properties
            cmNavBuilder.Property(cm => cm.Content).IsRequired();
            cmNavBuilder.Property(cm => cm.PromptKey).IsRequired();
            cmNavBuilder.Property(cm => cm.CreatedAt)
                .HasConversion(x => x.UtcDateTime, x => new DateTimeOffset(x, TimeSpan.Zero))
                .IsRequired();
            cmNavBuilder.Property(cm => cm.Role).IsRequired();
        
            cmNavBuilder.HasIndex(cm => cm.MessageId).IsUnique();
        });
        
        builder.HasIndex(cc => cc.ContextId).IsUnique();
    }
}