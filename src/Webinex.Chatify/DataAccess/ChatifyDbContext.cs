using Microsoft.EntityFrameworkCore;
using Webinex.Chatify.Types;
using File = Webinex.Chatify.Types.File;

namespace Webinex.Chatify.DataAccess;

public class ChatifyDbContext : DbContext
{
    public DbSet<Chat> Chats { get; protected set; } = null!;
    public DbSet<Member> Members { get; protected set; } = null!;
    public DbSet<Message> Messages { get; protected set; } = null!;
    public DbSet<Delivery> Deliveries { get; protected set; } = null!;
    public DbSet<ChatActivity> ChatActivities { get; protected set; } = null!;
    public DbSet<Account> Accounts { get; protected set; } = null!;

    public ChatifyDbContext(DbContextOptions<ChatifyDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder model)
    {
        model.HasDefaultSchema("chatify");

        model.Entity<Account>(account =>
        {
            account.ToTable("Accounts");
            account.HasKey(x => x.Id);
            account.Property(x => x.Id).HasMaxLength(250).IsRequired();
            account.Property(x => x.Name).HasMaxLength(500).IsRequired();
        });

        model.Entity<Chat>(chat =>
        {
            chat.ToTable("Chats");
            chat.HasKey(x => x.Id);
            chat.Property(x => x.Name).HasMaxLength(500).IsRequired();
            chat.Property(x => x.CreatedById).HasMaxLength(250).IsRequired();
            chat.HasOne<Account>().WithMany().HasForeignKey(x => x.CreatedById);
        });

        model.Entity<Delivery>(delivery =>
        {
            delivery.ToTable("Deliveries");
            delivery.HasKey(x => new { x.ChatId, x.MessageId, x.ToId });
            delivery.Property(x => x.ToId).HasMaxLength(250).IsRequired();
            delivery.Property(x => x.FromId).HasMaxLength(250).IsRequired();
            delivery.HasOne<Chat>().WithMany().HasForeignKey(x => x.ChatId);
            delivery.HasOne<Message>().WithMany().HasForeignKey(x => x.MessageId);
            delivery.HasOne<Account>().WithMany().HasForeignKey(x => x.ToId).OnDelete(DeleteBehavior.NoAction);
            delivery.HasOne<Account>().WithMany().HasForeignKey(x => x.FromId).OnDelete(DeleteBehavior.NoAction);
        });

        model.Entity<ChatActivity>(lastDelivery =>
        {
            lastDelivery.ToTable("ChatActivities");
            lastDelivery.HasKey(x => new { x.ChatId, x.AccountId });
            lastDelivery.Property(x => x.AccountId).HasMaxLength(250);
            lastDelivery.HasOne<Chat>().WithMany().HasForeignKey(x => x.ChatId);
            lastDelivery.HasOne<Account>().WithMany().HasForeignKey(x => x.AccountId);
            lastDelivery.HasOne<Message>().WithMany().HasForeignKey(x => x.LastMessageId);
            lastDelivery.HasOne<Account>().WithMany().HasForeignKey(x => x.LastMessageFromId);
        });

        model.Entity<Member>(member =>
        {
            member.ToTable("Members");
            member.HasKey(x => new { x.ChatId, x.AccountId });
            member.Property(x => x.AccountId).HasMaxLength(250).IsRequired();
            member.Property(x => x.AddedById).HasMaxLength(250).IsRequired();

            member.HasOne<Chat>().WithMany().HasForeignKey(x => x.ChatId);
            member.HasOne<Account>().WithMany().HasForeignKey(x => x.AccountId).OnDelete(DeleteBehavior.NoAction);
            member.HasOne<Account>().WithMany().HasForeignKey(x => x.AddedById).OnDelete(DeleteBehavior.NoAction);
        });

        model.Entity<Message>(message =>
        {
            message.ToTable("Messages");
            message.HasKey(x => x.Id);
            message.Property(x => x.Text).HasMaxLength(int.MaxValue).IsRequired();
            message.Property(x => x.Files).HasConversion(new JsonValueConverter<IReadOnlyCollection<File>>());
        });
    }
}