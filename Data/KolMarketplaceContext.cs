using System;
using System.Collections.Generic;
using KOL_KOC_TAAA.Models;
using Microsoft.EntityFrameworkCore;

namespace KOL_KOC_TAAA.Data;

public partial class KolMarketplaceContext : DbContext
{
    public KolMarketplaceContext(DbContextOptions<KolMarketplaceContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AdminAuditLog> AdminAuditLogs { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<BookingItem> BookingItems { get; set; }

    public virtual DbSet<BookingRequest> BookingRequests { get; set; }

    public virtual DbSet<BookingRequestItem> BookingRequestItems { get; set; }

    public virtual DbSet<ChatConversation> ChatConversations { get; set; }

    public virtual DbSet<ChatMember> ChatMembers { get; set; }

    public virtual DbSet<ChatMessage> ChatMessages { get; set; }

    public virtual DbSet<Contract> Contracts { get; set; }

    public virtual DbSet<ContractSignature> ContractSignatures { get; set; }

    public virtual DbSet<Coupon> Coupons { get; set; }

    public virtual DbSet<CouponUsage> CouponUsages { get; set; }

    public virtual DbSet<Deliverable> Deliverables { get; set; }

    public virtual DbSet<Dispute> Disputes { get; set; }

    public virtual DbSet<KOL_KOC_TAAA.Models.File> Files { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<KolAvailabilitySlot> KolAvailabilitySlots { get; set; }

    public virtual DbSet<KolCategory> KolCategories { get; set; }

    public virtual DbSet<KolPortfolio> KolPortfolios { get; set; }

    public virtual DbSet<KolProfile> KolProfiles { get; set; }

    public virtual DbSet<KolSocialAccount> KolSocialAccounts { get; set; }

    public virtual DbSet<Meeting> Meetings { get; set; }

    public virtual DbSet<MeetingParticipant> MeetingParticipants { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PaymentIntent> PaymentIntents { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<PayoutRequest> PayoutRequests { get; set; }

    public virtual DbSet<RateCard> RateCards { get; set; }

    public virtual DbSet<RateCardHistory> RateCardHistories { get; set; }

    public virtual DbSet<RateCardItem> RateCardItems { get; set; }

    public virtual DbSet<Refund> Refunds { get; set; }

    public virtual DbSet<Report> Reports { get; set; }
    
    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Session> Sessions { get; set; }

    public virtual DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }

    public virtual DbSet<SystemSetting> SystemSettings { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserSubscription> UserSubscriptions { get; set; }

    public virtual DbSet<UserWallet> UserWallets { get; set; }

    public virtual DbSet<ViewKolSearch> ViewKolSearches { get; set; }

    public virtual DbSet<ViewPlatformRevenue> ViewPlatformRevenues { get; set; }

    public virtual DbSet<WalletLedger> WalletLedgers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AdminAuditLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AdminAud__3214EC07400C8D3C");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.AdminUser).WithMany(p => p.AdminAuditLogs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AdminAudi__Admin__5E54FF49");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Bookings__3214EC079E01364F");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Currency).HasDefaultValue("VND");
            entity.Property(e => e.Status).HasDefaultValue("pending_payment");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.BookingRequest).WithOne(p => p.Booking)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Bookings__Bookin__2BFE89A6");

            entity.HasOne(d => d.CustomerUser).WithMany(p => p.Bookings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Bookings__Custom__2CF2ADDF");

            entity.HasOne(d => d.KolUser).WithMany(p => p.Bookings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Bookings__KolUse__2DE6D218");
        });

        modelBuilder.Entity<BookingItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BookingI__3214EC0763E9C488");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Quantity).HasDefaultValue(1);

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingItems).HasConstraintName("FK__BookingIt__Booki__3493CFA7");
        });

        modelBuilder.Entity<BookingRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BookingR__3214EC07344099AA");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Currency).HasDefaultValue("VND");
            entity.Property(e => e.Status).HasDefaultValue("sent");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.CustomerUser).WithMany(p => p.BookingRequests)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BookingRe__Custo__07C12930");

            entity.HasOne(d => d.KolUser).WithMany(p => p.BookingRequests)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BookingRe__KolUs__08B54D69");
        });

        modelBuilder.Entity<BookingRequestItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BookingR__3214EC0740878422");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Quantity).HasDefaultValue(1);

            entity.HasOne(d => d.BookingRequest).WithMany(p => p.BookingRequestItems).HasConstraintName("FK__BookingRe__Booki__1F98B2C1");
        });

        modelBuilder.Entity<ChatConversation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ChatConv__3214EC0709A2F1E1");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Type).HasDefaultValue("direct");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.BookingRequest).WithMany(p => p.ChatConversations).HasConstraintName("FK__ChatConve__Booki__0F624AF8");
        });

        modelBuilder.Entity<ChatMember>(entity =>
        {
            entity.HasKey(e => new { e.ConversationId, e.UserId }).HasName("PK__ChatMemb__112854B3EAABED57");

            entity.HasOne(d => d.Conversation).WithMany(p => p.ChatMembers).HasConstraintName("FK__ChatMembe__Conve__123EB7A3");

            entity.HasOne(d => d.User).WithMany(p => p.ChatMembers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChatMembe__UserI__1332DBDC");
        });

        modelBuilder.Entity<ChatMessage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ChatMess__3214EC079EC4AAF5");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.MessageType).HasDefaultValue("text");
            entity.Property(e => e.SentAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Attachment).WithMany(p => p.ChatMessages).HasConstraintName("FK__ChatMessa__Attac__1AD3FDA4");

            entity.HasOne(d => d.Conversation).WithMany(p => p.ChatMessages).HasConstraintName("FK__ChatMessa__Conve__18EBB532");

            entity.HasOne(d => d.SenderUser).WithMany(p => p.ChatMessages)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChatMessa__Sende__19DFD96B");
        });

        modelBuilder.Entity<Contract>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Contract__3214EC070BB1293F");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue("draft");
            entity.Property(e => e.Title).HasDefaultValue("Service Contract");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Version).HasDefaultValue(1);

            entity.HasOne(d => d.Booking).WithMany(p => p.Contracts).HasConstraintName("FK__Contracts__Booki__18B6AB08");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.Contracts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Contracts__Creat__19AACF41");
        });

        modelBuilder.Entity<ContractSignature>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Contract__3214EC071FB372B5");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.SignedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Contract).WithMany(p => p.ContractSignatures).HasConstraintName("FK__ContractS__Contr__1F63A897");

            entity.HasOne(d => d.User).WithMany(p => p.ContractSignatures)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ContractS__UserI__2057CCD0");
        });

        modelBuilder.Entity<Coupon>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Coupons__3214EC079A31847C");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<CouponUsage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CouponUs__3214EC079276FCD9");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.UsedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Booking).WithMany(p => p.CouponUsages).HasConstraintName("FK__CouponUsa__Booki__7FEAFD3E");

            entity.HasOne(d => d.Coupon).WithMany(p => p.CouponUsages)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CouponUsa__Coupo__7E02B4CC");

            entity.HasOne(d => d.User).WithMany(p => p.CouponUsages)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CouponUsa__UserI__7EF6D905");
        });

        modelBuilder.Entity<Deliverable>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Delivera__3214EC07F6917BAF");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue("pending");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Booking).WithMany(p => p.Deliverables).HasConstraintName("FK__Deliverab__Booki__2704CA5F");

            entity.HasOne(d => d.Item).WithMany(p => p.Deliverables).HasConstraintName("FK__Deliverab__ItemI__27F8EE98");

            entity.HasMany(d => d.Files).WithMany(p => p.Deliverables)
                .UsingEntity<Dictionary<string, object>>(
                    "DeliverableAttachment",
                    r => r.HasOne<KOL_KOC_TAAA.Models.File>().WithMany()
                        .HasForeignKey("FileId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Deliverab__FileI__2BC97F7C"),
                    l => l.HasOne<Deliverable>().WithMany()
                        .HasForeignKey("DeliverableId")
                        .HasConstraintName("FK__Deliverab__Deliv__2AD55B43"),
                    j =>
                    {
                        j.HasKey("DeliverableId", "FileId").HasName("PK__Delivera__9759EE855C32F97F");
                        j.ToTable("DeliverableAttachments");
                    });
        });

        modelBuilder.Entity<Dispute>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Disputes__3214EC071D580E51");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Currency).HasDefaultValue("VND");
            entity.Property(e => e.DisputeType).HasDefaultValue("general");
            entity.Property(e => e.Status).HasDefaultValue("open");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Booking).WithMany(p => p.Disputes).HasConstraintName("FK__Disputes__Bookin__477199F1");

            entity.HasOne(d => d.RaisedByUser).WithMany(p => p.DisputeRaisedByUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Disputes__Raised__4865BE2A");

            entity.HasOne(d => d.ResolvedByAdmin).WithMany(p => p.DisputeResolvedByAdmins).HasConstraintName("FK__Disputes__Resolv__4959E263");
        });

        modelBuilder.Entity<KOL_KOC_TAAA.Models.File>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Files__3214EC07BD03CE19");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.UploaderUser).WithMany(p => p.Files)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Files__UploaderU__00200768");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Invoices__3214EC0739018FE2");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Currency).HasDefaultValue("VND");
            entity.Property(e => e.IssuedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue("unpaid");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Booking).WithOne(p => p.Invoice)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Invoices__Bookin__55F4C372");
        });

        modelBuilder.Entity<KolAvailabilitySlot>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__KolAvail__3214EC07F423B075");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue("available");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.KolUser).WithMany(p => p.KolAvailabilitySlots).HasConstraintName("FK__KolAvaila__KolUs__3B40CD36");
        });

        modelBuilder.Entity<KolCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__KolCateg__3214EC0741A32A79");
        });

        modelBuilder.Entity<KolPortfolio>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__KolPortf__3214EC0720A545EC");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.KolUser).WithMany(p => p.KolPortfolios).HasConstraintName("FK__KolPortfo__KolUs__5BE2A6F2");
        });

        modelBuilder.Entity<KolProfile>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__KolProfi__1788CC4C855D8B4B");

            entity.Property(e => e.UserId).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.User).WithOne(p => p.KolProfile).HasConstraintName("FK__KolProfil__UserI__4F7CD00D");

            entity.HasMany(d => d.Categories).WithMany(p => p.KolUsers)
                .UsingEntity<Dictionary<string, object>>(
                    "KolCategoryMap",
                    r => r.HasOne<KolCategory>().WithMany()
                        .HasForeignKey("CategoryId")
                        .HasConstraintName("FK__KolCatego__Categ__628FA481"),
                    l => l.HasOne<KolProfile>().WithMany()
                        .HasForeignKey("KolUserId")
                        .HasConstraintName("FK__KolCatego__KolUs__619B8048"),
                    j =>
                    {
                        j.HasKey("KolUserId", "CategoryId").HasName("PK__KolCateg__CF7BBAB6C6BC134F");
                        j.ToTable("KolCategoryMap");
                    });

            entity.HasMany(d => d.Tags).WithMany(p => p.KolUsers)
                .UsingEntity<Dictionary<string, object>>(
                    "KolTagMap",
                    r => r.HasOne<Tag>().WithMany()
                        .HasForeignKey("TagId")
                        .HasConstraintName("FK__KolTagMap__TagId__693CA210"),
                    l => l.HasOne<KolProfile>().WithMany()
                        .HasForeignKey("KolUserId")
                        .HasConstraintName("FK__KolTagMap__KolUs__68487DD7"),
                    j =>
                    {
                        j.HasKey("KolUserId", "TagId").HasName("PK__KolTagMa__B8BCE68CDE9E47AB");
                        j.ToTable("KolTagMap");
                    });
        });

        modelBuilder.Entity<KolSocialAccount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__KolSocia__3214EC07442CA00F");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.KolUser).WithMany(p => p.KolSocialAccounts).HasConstraintName("FK__KolSocial__KolUs__571DF1D5");
        });

        modelBuilder.Entity<Meeting>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Meetings__3214EC07ACA2788E");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue("scheduled");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Booking).WithMany(p => p.Meetings).HasConstraintName("FK__Meetings__Bookin__41EDCAC5");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.Meetings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Meetings__Create__42E1EEFE");
        });

        modelBuilder.Entity<MeetingParticipant>(entity =>
        {
            entity.HasKey(e => new { e.MeetingId, e.UserId }).HasName("PK__MeetingP__3881658850908D47");

            entity.Property(e => e.AttendanceStatus).HasDefaultValue("invited");

            entity.HasOne(d => d.Meeting).WithMany(p => p.MeetingParticipants).HasConstraintName("FK__MeetingPa__Meeti__46B27FE2");

            entity.HasOne(d => d.User).WithMany(p => p.MeetingParticipants)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MeetingPa__UserI__47A6A41B");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Notifica__3214EC07CC3AC2C6");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications).HasConstraintName("FK__Notificat__UserI__59904A2C");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Payments__3214EC070035EFFF");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.PaymentIntent).WithMany(p => p.Payments).HasConstraintName("FK__Payments__Paymen__6BE40491");
        });

        modelBuilder.Entity<PaymentIntent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PaymentI__3214EC075882E9D9");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Currency).HasDefaultValue("VND");
            entity.Property(e => e.Status).HasDefaultValue("requires_payment");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Invoice).WithMany(p => p.PaymentIntents).HasConstraintName("FK__PaymentIn__Invoi__65370702");
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PaymentM__3214EC07E9C314DC");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.User).WithMany(p => p.PaymentMethods).HasConstraintName("FK__PaymentMe__UserI__5CA1C101");
        });

        modelBuilder.Entity<PayoutRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PayoutRe__3214EC077F622403");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Currency).HasDefaultValue("VND");
            entity.Property(e => e.Status).HasDefaultValue("pending");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.User).WithMany(p => p.PayoutRequests).HasConstraintName("FK__PayoutReq__UserI__3EDC53F0");
        });

        modelBuilder.Entity<RateCard>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RateCard__3214EC0739E2FA13");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Currency).HasDefaultValue("VND");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.KolUser).WithMany(p => p.RateCards).HasConstraintName("FK__RateCards__KolUs__70DDC3D8");
        });

        modelBuilder.Entity<RateCardHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RateCard__3214EC0720D66330");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.ChangedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.RateCard).WithMany(p => p.RateCardHistories).HasConstraintName("FK__RateCardH__RateC__7B5B524B");
        });

        modelBuilder.Entity<RateCardItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RateCard__3214EC072A31E57D");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.RateCard).WithMany(p => p.RateCardItems).HasConstraintName("FK__RateCardI__RateC__76969D2E");
        });

        modelBuilder.Entity<Refund>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Refunds__3214EC07731CB205");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue("processing");

            entity.HasOne(d => d.Payment).WithMany(p => p.Refunds).HasConstraintName("FK__Refunds__Payment__72910220");
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Reports__3214EC07653B89CD");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue("open");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Message).WithMany(p => p.Reports).HasConstraintName("FK__Reports__Message__51EF2864");

            entity.HasOne(d => d.ReporterUser).WithMany(p => p.ReportReporterUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reports__Reporte__5006DFF2");

            entity.HasOne(d => d.TargetUser).WithMany(p => p.ReportTargetUsers).HasConstraintName("FK__Reports__TargetU__50FB042B");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC075A65DD97");
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Sessions__3214EC07C8117745");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.User).WithMany(p => p.Sessions).HasConstraintName("FK__Sessions__UserId__46E78A0C");
        });

        modelBuilder.Entity<SubscriptionPlan>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Subscrip__3214EC07FB5E398D");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.BillingCycle).HasDefaultValue("monthly");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Currency).HasDefaultValue("VND");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<SystemSetting>(entity =>
        {
            entity.HasKey(e => e.Key).HasName("PK__SystemSe__C41E0288F51C252B");

            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tags__3214EC0776105E0D");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07CF3607B8");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue("active");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    r => r.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("FK__UserRoles__RoleI__4222D4EF"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK__UserRoles__UserI__412EB0B6"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId").HasName("PK__UserRole__AF2760AD2D12419B");
                        j.ToTable("UserRoles");
                    });
            entity.HasIndex(e => e.Phone)
                .IsUnique()
                .HasFilter("[Phone] IS NOT NULL")
                .HasDatabaseName("UQ_Users_Phone_Filtered");
        });

        modelBuilder.Entity<UserSubscription>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserSubs__3214EC07757170EA");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue("active");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Plan).WithMany(p => p.UserSubscriptions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserSubsc__PlanI__0F2D40CE");

            entity.HasOne(d => d.User).WithMany(p => p.UserSubscriptions).HasConstraintName("FK__UserSubsc__UserI__0E391C95");
        });

        modelBuilder.Entity<UserWallet>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__UserWall__1788CC4CA2D95105");

            entity.Property(e => e.UserId).ValueGeneratedNever();
            entity.Property(e => e.Currency).HasDefaultValue("VND");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.User).WithOne(p => p.UserWallet).HasConstraintName("FK__UserWalle__UserI__32767D0B");
        });

        modelBuilder.Entity<ViewKolSearch>(entity =>
        {
            entity.ToView("View_KolSearch");
        });

        modelBuilder.Entity<ViewPlatformRevenue>(entity =>
        {
            entity.ToView("View_PlatformRevenue");
        });

        modelBuilder.Entity<WalletLedger>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__WalletLe__3214EC0702E8B7C4");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.WalletUser).WithMany(p => p.WalletLedgers).HasConstraintName("FK__WalletLed__Walle__373B3228");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
