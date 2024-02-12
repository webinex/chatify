using Microsoft.EntityFrameworkCore;
using Webinex.Chatify.Rows;
using File = Webinex.Chatify.Abstractions.File;

namespace Webinex.Chatify.DataAccess;

internal class ChatifyDbContext : DbContext
{
    public DbSet<ChatRow> Chats { get; protected set; } = null!;
    public DbSet<MemberRow> Members { get; protected set; } = null!;
    public DbSet<MessageRow> Messages { get; protected set; } = null!;
    public DbSet<ChatActivityRow> ChatActivities { get; protected set; } = null!;
    public DbSet<AccountRow> Accounts { get; protected set; } = null!;

    public ChatifyDbContext(DbContextOptions<ChatifyDbContext> options) : base(options)
    {
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.DefaultTypeMapping<MessageRow>();
    }

    protected override void OnModelCreating(ModelBuilder model)
    {
        model.HasDefaultSchema("chatify");

        model.Entity<AccountRow>(account =>
        {
            account.ToTable("Accounts");
            account.HasKey(x => x.Id);
            account.Property(x => x.Id).HasMaxLength(250).IsRequired();
            account.Property(x => x.Name).HasMaxLength(500).IsRequired();
        });

        model.Entity<ChatRow>(chat =>
        {
            chat.ToTable("Chats");
            chat.HasKey(x => x.Id);
            chat.Property(x => x.Name).HasMaxLength(500).IsRequired();
            chat.Property(x => x.CreatedById).HasMaxLength(250).IsRequired();
            chat.HasOne<AccountRow>().WithMany().HasForeignKey(x => x.CreatedById);
        });

        model.Entity<ChatActivityRow>(activity =>
        {
            activity.ToTable("ChatActivities");
            activity.HasKey(x => new { x.ChatId, x.AccountId });
            activity.Property(x => x.AccountId).HasMaxLength(250);
            activity.HasOne(x => x.Chat).WithMany().HasForeignKey(x => x.ChatId).OnDelete(DeleteBehavior.NoAction);
            activity.HasOne<AccountRow>().WithMany().HasForeignKey(x => x.AccountId).OnDelete(DeleteBehavior.NoAction);
            activity.HasOne(x => x.LastMessage).WithMany().HasForeignKey(x => x.LastMessageId).OnDelete(DeleteBehavior.NoAction);
            activity.HasOne<AccountRow>().WithMany().HasForeignKey(x => x.LastMessageFromId).OnDelete(DeleteBehavior.NoAction);
            activity.HasOne<MessageRow>().WithMany().HasForeignKey(x => x.LastReadMessageId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        model.Entity<MemberRow>(member =>
        {
            member.ToTable("Members");
            member.HasKey(x => x.Id);
            member.Property(x => x.AccountId).HasMaxLength(250).IsRequired();
            member.Property(x => x.AddedById).HasMaxLength(250).IsRequired();

            member.HasOne<MessageRow>().WithMany().HasForeignKey(x => x.FirstMessageId).OnDelete(DeleteBehavior.NoAction);
            member.HasOne<MessageRow>().WithMany().HasForeignKey(x => x.LastMessageId).OnDelete(DeleteBehavior.NoAction);
            member.HasOne<ChatRow>().WithMany().HasForeignKey(x => x.ChatId).OnDelete(DeleteBehavior.NoAction);
            member.HasOne<AccountRow>().WithMany().HasForeignKey(x => x.AccountId).OnDelete(DeleteBehavior.NoAction);
            member.HasOne<AccountRow>().WithMany().HasForeignKey(x => x.AddedById).OnDelete(DeleteBehavior.NoAction);
        });

        model.Entity<MessageRow>(message =>
        {
            message.ToTable("Messages");
            message.HasKey(x => x.Id);
            message.Property(x => x.Text).HasMaxLength(int.MaxValue).IsRequired();
            message.Property(x => x.Files).HasConversion(new JsonValueConverter<IReadOnlyCollection<File>>());
            message.HasOne(x => x.Author).WithMany().HasForeignKey(x => x.AuthorId);
        });
    }
}