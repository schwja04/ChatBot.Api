using ChatBot.Api.Domain.ChatContextEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatBot.Api.Infrastructure;

public class ChatBotContext(DbContextOptions<ChatBotContext> options) : DbContext(options)
{
    public DbSet<ChatContext> ChatContexts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ChatContextConfiguration());
        // modelBuilder.ApplyConfiguration(new ChatMessageConfiguration());
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
        builder.Property(cc => cc.Username).IsRequired();
        builder.Property(cc => cc.CreatedAt).IsRequired();
        builder.Property(cc => cc.UpdatedAt).IsRequired();

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
            cmNavBuilder.Property(cm => cm.CreatedAt).IsRequired();
            cmNavBuilder.Property(cm => cm.Role).IsRequired();
        
            cmNavBuilder.HasIndex(cm => cm.MessageId).IsUnique();
        });
        
        // Configure EF to use the private _messages field for Messages
        // builder.Metadata.FindNavigation(nameof(ChatContext.Messages))!
        //     .SetPropertyAccessMode(PropertyAccessMode.PreferProperty);
        
        builder.HasIndex(cc => cc.ContextId).IsUnique();
    }
}