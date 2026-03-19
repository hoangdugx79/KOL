-- ============================================================
-- KOL/KOC Hiring Marketplace Database Schema (SQL Server)
-- Optimized for C# .NET MVC / Entity Framework Core
-- ============================================================

-- Azure SQL: Database đã được tạo sẵn trên Portal, không cần CREATE DATABASE
-- CREATE DATABASE KolMarketplace;
-- GO
-- USE KolMarketplace;
-- GO

-- ---------- Identity ----------
CREATE TABLE Users (
  Id              UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
  Email           NVARCHAR(256) NOT NULL UNIQUE,
  Phone           NVARCHAR(30) UNIQUE,
  PasswordHash    NVARCHAR(MAX) NOT NULL,
  FullName        NVARCHAR(255) NOT NULL,
  AvatarUrl       NVARCHAR(MAX),
  Status          NVARCHAR(20) NOT NULL DEFAULT 'active', -- active/banned/pending
  LastLoginAt     DATETIME2,
  CreatedAt       DATETIME2 NOT NULL DEFAULT GETDATE(),
  UpdatedAt       DATETIME2 NOT NULL DEFAULT GETDATE()
);

CREATE TABLE Roles (
  Id    BIGINT IDENTITY(1,1) PRIMARY KEY,
  Code  NVARCHAR(50) NOT NULL UNIQUE, -- admin/customer/kol
  Name  NVARCHAR(100) NOT NULL
);

CREATE TABLE UserRoles (
  UserId UNIQUEIDENTIFIER NOT NULL,
  RoleId BIGINT NOT NULL,
  PRIMARY KEY (UserId, RoleId),
  FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
  FOREIGN KEY (RoleId) REFERENCES Roles(Id) ON DELETE CASCADE
);

CREATE TABLE Sessions (
  Id            UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
  UserId        UNIQUEIDENTIFIER NOT NULL,
  TokenHash     NVARCHAR(MAX) NOT NULL,
  DeviceInfo    NVARCHAR(MAX),
  IpAddress     NVARCHAR(50),
  ExpiresAt     DATETIME2 NOT NULL,
  RevokedAt     DATETIME2,
  CreatedAt     DATETIME2 NOT NULL DEFAULT GETDATE(),
  FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);
CREATE INDEX IDX_Sessions_User_Expires ON Sessions(UserId, ExpiresAt);

-- ---------- KOL Profile ----------
CREATE TABLE KolProfiles (
  UserId          UNIQUEIDENTIFIER PRIMARY KEY,
  InfluencerType  NVARCHAR(10) NOT NULL CHECK (InfluencerType IN ('KOL','KOC')),
  Bio             NVARCHAR(MAX),
  Gender          NVARCHAR(20),
  Dob             DATE,
  LocationCity    NVARCHAR(100),
  LocationCountry NVARCHAR(100),
  MinBudget       DECIMAL(18,2),
  RatingAvg       DECIMAL(3,2) NOT NULL DEFAULT 0,
  RatingCount     INT NOT NULL DEFAULT 0,
  IsVerified      BIT NOT NULL DEFAULT 0,
  CreatedAt       DATETIME2 NOT NULL DEFAULT GETDATE(),
  UpdatedAt       DATETIME2 NOT NULL DEFAULT GETDATE(),
  FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);

CREATE TABLE KolSocialAccounts (
  Id              UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
  KolUserId       UNIQUEIDENTIFIER NOT NULL,
  Platform        NVARCHAR(50) NOT NULL, -- tiktok/instagram/youtube/...
  Username        NVARCHAR(100),
  ProfileUrl      NVARCHAR(MAX),
  Followers       BIGINT,
  AvgViews        BIGINT,
  EngagementRate  DECIMAL(6,3),
  IsVerified      BIT NOT NULL DEFAULT 0,
  CreatedAt       DATETIME2 NOT NULL DEFAULT GETDATE(),
  UpdatedAt       DATETIME2 NOT NULL DEFAULT GETDATE(),
  FOREIGN KEY (KolUserId) REFERENCES KolProfiles(UserId) ON DELETE CASCADE,
  CONSTRAINT UQ_Social_Platform_Username UNIQUE (Platform, Username)
);

CREATE TABLE KolPortfolios (
  Id            UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
  KolUserId     UNIQUEIDENTIFIER NOT NULL,
  Title         NVARCHAR(255) NOT NULL,
  Description   NVARCHAR(MAX),
  MediaUrl      NVARCHAR(MAX),
  CreatedAt     DATETIME2 NOT NULL DEFAULT GETDATE(),
  FOREIGN KEY (KolUserId) REFERENCES KolProfiles(UserId) ON DELETE CASCADE
);

CREATE TABLE KolCategories (
  Id    BIGINT IDENTITY(1,1) PRIMARY KEY,
  Name  NVARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE KolCategoryMap (
  KolUserId   UNIQUEIDENTIFIER NOT NULL,
  CategoryId  BIGINT NOT NULL,
  PRIMARY KEY (KolUserId, CategoryId),
  FOREIGN KEY (KolUserId) REFERENCES KolProfiles(UserId) ON DELETE CASCADE,
  FOREIGN KEY (CategoryId) REFERENCES KolCategories(Id) ON DELETE CASCADE
);

CREATE TABLE Tags (
  Id    BIGINT IDENTITY(1,1) PRIMARY KEY,
  Name  NVARCHAR(80) NOT NULL UNIQUE
);

CREATE TABLE KolTagMap (
  KolUserId UNIQUEIDENTIFIER NOT NULL,
  TagId     BIGINT NOT NULL,
  PRIMARY KEY (KolUserId, TagId),
  FOREIGN KEY (KolUserId) REFERENCES KolProfiles(UserId) ON DELETE CASCADE,
  FOREIGN KEY (TagId) REFERENCES Tags(Id) ON DELETE CASCADE
);

CREATE TABLE RateCards (
  Id            UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
  KolUserId     UNIQUEIDENTIFIER NOT NULL,
  Title         NVARCHAR(255) NOT NULL,
  Currency      NVARCHAR(10) NOT NULL DEFAULT 'VND',
  IsActive      BIT NOT NULL DEFAULT 1,
  EffectiveFrom DATE,
  EffectiveTo   DATE,
  CreatedAt     DATETIME2 NOT NULL DEFAULT GETDATE(),
  UpdatedAt     DATETIME2 NOT NULL DEFAULT GETDATE(),
  FOREIGN KEY (KolUserId) REFERENCES KolProfiles(UserId) ON DELETE CASCADE
);

CREATE TABLE RateCardItems (
  Id               UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
  RateCardId       UNIQUEIDENTIFIER NOT NULL,
  ServiceType      NVARCHAR(50) NOT NULL, 
  Platform         NVARCHAR(50),
  UnitPrice        DECIMAL(18,2) NOT NULL,
  Unit             NVARCHAR(30) NOT NULL, 
  DurationMinutes  INT,
  Description      NVARCHAR(MAX),
  CreatedAt        DATETIME2 NOT NULL DEFAULT GETDATE(),
  UpdatedAt        DATETIME2 NOT NULL DEFAULT GETDATE(),
  FOREIGN KEY (RateCardId) REFERENCES RateCards(Id) ON DELETE CASCADE
);

CREATE TABLE RateCardHistory (
  Id               UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
  RateCardId       UNIQUEIDENTIFIER NOT NULL,
  SnapshotData     NVARCHAR(MAX), -- JSON snapshot
  ChangedAt        DATETIME2 NOT NULL DEFAULT GETDATE(),
  FOREIGN KEY (RateCardId) REFERENCES RateCards(Id) ON DELETE CASCADE
);

-- ---------- Chat + Files ----------
CREATE TABLE Files (
  Id              UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
  UploaderUserId  UNIQUEIDENTIFIER NOT NULL,
  Url             NVARCHAR(MAX) NOT NULL,
  MimeType        NVARCHAR(120),
  SizeBytes       BIGINT,
  Checksum        NVARCHAR(128),
  CreatedAt       DATETIME2 NOT NULL DEFAULT GETDATE(),
  FOREIGN KEY (UploaderUserId) REFERENCES Users(Id)
);

CREATE TABLE BookingRequests (
  Id                  UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
  CustomerUserId      UNIQUEIDENTIFIER NOT NULL,
  KolUserId           UNIQUEIDENTIFIER NOT NULL,
  Title               NVARCHAR(255) NOT NULL,
  Brief               NVARCHAR(MAX),
  BudgetMin           DECIMAL(18,2),
  BudgetMax           DECIMAL(18,2),
  Currency            NVARCHAR(10) NOT NULL DEFAULT 'VND',
  ProposedStartDate   DATE,
  ProposedEndDate     DATE,
  Status              NVARCHAR(30) NOT NULL DEFAULT 'sent', 
  CreatedAt           DATETIME2 NOT NULL DEFAULT GETDATE(),
  UpdatedAt           DATETIME2 NOT NULL DEFAULT GETDATE(),
  FOREIGN KEY (CustomerUserId) REFERENCES Users(Id),
  FOREIGN KEY (KolUserId) REFERENCES KolProfiles(UserId)
);

CREATE TABLE ChatConversations (
  Id                 UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
  Type               NVARCHAR(20) NOT NULL DEFAULT 'direct', 
  BookingRequestId   UNIQUEIDENTIFIER NULL,
  CreatedAt          DATETIME2 NOT NULL DEFAULT GETDATE(),
  UpdatedAt          DATETIME2 NOT NULL DEFAULT GETDATE(),
  FOREIGN KEY (BookingRequestId) REFERENCES BookingRequests(Id)
);

CREATE TABLE ChatMembers (
  ConversationId UNIQUEIDENTIFIER NOT NULL,
  UserId         UNIQUEIDENTIFIER NOT NULL,
  RoleInChat     NVARCHAR(20), 
  LastReadAt     DATETIME2,
  PRIMARY KEY (ConversationId, UserId),
  FOREIGN KEY (ConversationId) REFERENCES ChatConversations(Id) ON DELETE CASCADE,
  FOREIGN KEY (UserId) REFERENCES Users(Id)
);

CREATE TABLE ChatMessages (
  Id              UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
  ConversationId  UNIQUEIDENTIFIER NOT NULL,
  SenderUserId    UNIQUEIDENTIFIER NOT NULL,
  MessageType     NVARCHAR(20) NOT NULL DEFAULT 'text', 
  Content         NVARCHAR(MAX),
  AttachmentId    UNIQUEIDENTIFIER,
  SentAt          DATETIME2 NOT NULL DEFAULT GETDATE(),
  FOREIGN KEY (ConversationId) REFERENCES ChatConversations(Id) ON DELETE CASCADE,
  FOREIGN KEY (SenderUserId) REFERENCES Users(Id),
  FOREIGN KEY (AttachmentId) REFERENCES Files(Id)
);

-- ---------- Booking ----------
CREATE TABLE BookingRequestItems (
  Id                 UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
  BookingRequestId   UNIQUEIDENTIFIER NOT NULL,
  ServiceType        NVARCHAR(50) NOT NULL,
  Platform           NVARCHAR(50),
  Quantity           INT NOT NULL DEFAULT 1,
  ExpectedUnitPrice  DECIMAL(18,2),
  Notes              NVARCHAR(MAX),
  FOREIGN KEY (BookingRequestId) REFERENCES BookingRequests(Id) ON DELETE CASCADE
);

CREATE TABLE Bookings (
  Id                 UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
  BookingRequestId   UNIQUEIDENTIFIER NOT NULL UNIQUE,
  CustomerUserId     UNIQUEIDENTIFIER NOT NULL,
  KolUserId          UNIQUEIDENTIFIER NOT NULL,
  AgreedSubtotal     DECIMAL(18,2) NOT NULL DEFAULT 0,
  PlatformFee        DECIMAL(18,2) NOT NULL DEFAULT 0,
  TaxAmount          DECIMAL(18,2) NOT NULL DEFAULT 0,
  TotalAmount        DECIMAL(18,2) NOT NULL DEFAULT 0,
  Currency           NVARCHAR(10) NOT NULL DEFAULT 'VND',
  Status             NVARCHAR(30) NOT NULL DEFAULT 'pending_payment', 
  CreatedAt          DATETIME2 NOT NULL DEFAULT GETDATE(),
  UpdatedAt          DATETIME2 NOT NULL DEFAULT GETDATE(),
  FOREIGN KEY (BookingRequestId) REFERENCES BookingRequests(Id),
  FOREIGN KEY (CustomerUserId) REFERENCES Users(Id),
  FOREIGN KEY (KolUserId) REFERENCES KolProfiles(UserId)
);

CREATE TABLE BookingItems (
  Id               UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
  BookingId        UNIQUEIDENTIFIER NOT NULL,
  ServiceType      NVARCHAR(50) NOT NULL,
  Platform         NVARCHAR(50),
  Quantity         INT NOT NULL DEFAULT 1,
  UnitPrice        DECIMAL(18,2) NOT NULL DEFAULT 0,
  LineTotal        DECIMAL(18,2) NOT NULL DEFAULT 0,
  DeliverDueDate   DATE,
  FOREIGN KEY (BookingId) REFERENCES Bookings(Id) ON DELETE CASCADE
);

-- ---------- Meetings ----------
CREATE TABLE KolAvailabilitySlots (
  Id          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
  KolUserId   UNIQUEIDENTIFIER NOT NULL,
  StartTime   DATETIME2 NOT NULL,
  EndTime     DATETIME2 NOT NULL,
  Status      NVARCHAR(20) NOT NULL DEFAULT 'available', 
  Note        NVARCHAR(MAX),
  CreatedAt   DATETIME2 NOT NULL DEFAULT GETDATE(),
  UpdatedAt   DATETIME2 NOT NULL DEFAULT GETDATE(),
  FOREIGN KEY (KolUserId) REFERENCES KolProfiles(UserId) ON DELETE CASCADE
);

CREATE TABLE Meetings (
  Id                UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
  BookingId         UNIQUEIDENTIFIER NOT NULL,
  CreatedByUserId   UNIQUEIDENTIFIER NOT NULL,
  MeetingType       NVARCHAR(10) NOT NULL, 
  Title             NVARCHAR(255) NOT NULL,
  Agenda            NVARCHAR(MAX),
  StartTime         DATETIME2 NOT NULL,
  EndTime           DATETIME2 NOT NULL,
  LocationText      NVARCHAR(MAX),
  MeetingUrl        NVARCHAR(MAX),
  Status            NVARCHAR(20) NOT NULL DEFAULT 'scheduled',
  CreatedAt         DATETIME2 NOT NULL DEFAULT GETDATE(),
  UpdatedAt         DATETIME2 NOT NULL DEFAULT GETDATE(),
  FOREIGN KEY (BookingId) REFERENCES Bookings(Id) ON DELETE CASCADE,
  FOREIGN KEY (CreatedByUserId) REFERENCES Users(Id)
);

CREATE TABLE MeetingParticipants (
  MeetingId        UNIQUEIDENTIFIER NOT NULL,
  UserId           UNIQUEIDENTIFIER NOT NULL,
  Role             NVARCHAR(20) NOT NULL, 
  AttendanceStatus NVARCHAR(20) NOT NULL DEFAULT 'invited',
  PRIMARY KEY (MeetingId, UserId),
  FOREIGN KEY (MeetingId) REFERENCES Meetings(Id) ON DELETE CASCADE,
  FOREIGN KEY (UserId) REFERENCES Users(Id)
);

-- ---------- Payments ----------
CREATE TABLE Invoices (
  Id         UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
  BookingId  UNIQUEIDENTIFIER NOT NULL UNIQUE,
  InvoiceNo  NVARCHAR(50) NOT NULL UNIQUE,
  Subtotal   DECIMAL(18,2) NOT NULL DEFAULT 0,
  Fee        DECIMAL(18,2) NOT NULL DEFAULT 0,
  Tax        DECIMAL(18,2) NOT NULL DEFAULT 0,
  Total      DECIMAL(18,2) NOT NULL DEFAULT 0,
  Currency   NVARCHAR(10) NOT NULL DEFAULT 'VND',
  Status     NVARCHAR(20) NOT NULL DEFAULT 'unpaid',
  IssuedAt   DATETIME2 NOT NULL DEFAULT GETDATE(),
  PaidAt     DATETIME2,
  CreatedAt  DATETIME2 NOT NULL DEFAULT GETDATE(),
  UpdatedAt  DATETIME2 NOT NULL DEFAULT GETDATE(),
  FOREIGN KEY (BookingId) REFERENCES Bookings(Id)
);

CREATE TABLE PaymentMethods (
  Id                         UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
  UserId                     UNIQUEIDENTIFIER NOT NULL,
  Provider                   NVARCHAR(30) NOT NULL, 
  ProviderPaymentMethodId    NVARCHAR(255) NOT NULL,
  Type                       NVARCHAR(30) NOT NULL, 
  CardBrand                  NVARCHAR(30),
  CardLast4                  NVARCHAR(4),
  CardExpMonth               INT,
  CardExpYear                INT,
  IsDefault                  BIT NOT NULL DEFAULT 0,
  CreatedAt                  DATETIME2 NOT NULL DEFAULT GETDATE(),
  FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
  CONSTRAINT UQ_PaymentMethod_ProviderId UNIQUE (Provider, ProviderPaymentMethodId)
);

CREATE TABLE PaymentIntents (
  Id                 UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
  InvoiceId          UNIQUEIDENTIFIER NOT NULL,
  Provider           NVARCHAR(30) NOT NULL,
  ProviderIntentId   NVARCHAR(255) NOT NULL UNIQUE,
  Amount             DECIMAL(18,2) NOT NULL,
  Currency           NVARCHAR(10) NOT NULL DEFAULT 'VND',
  MethodType         NVARCHAR(30) NOT NULL, 
  Status             NVARCHAR(30) NOT NULL DEFAULT 'requires_payment',
  ReturnUrl          NVARCHAR(MAX),
  QrPayload          NVARCHAR(MAX),
  CreatedAt          DATETIME2 NOT NULL DEFAULT GETDATE(),
  UpdatedAt          DATETIME2 NOT NULL DEFAULT GETDATE(),
  FOREIGN KEY (InvoiceId) REFERENCES Invoices(Id) ON DELETE CASCADE
);

CREATE TABLE Payments (
  Id                 UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
  PaymentIntentId    UNIQUEIDENTIFIER NOT NULL,
  ProviderChargeId   NVARCHAR(255) UNIQUE,
  PaidAmount         DECIMAL(18,2) NOT NULL DEFAULT 0,
  PaidAt             DATETIME2,
  Status             NVARCHAR(20) NOT NULL, 
  FailureCode        NVARCHAR(100),
  FailureMessage     NVARCHAR(MAX),
  CreatedAt          DATETIME2 NOT NULL DEFAULT GETDATE(),
  FOREIGN KEY (PaymentIntentId) REFERENCES PaymentIntents(Id) ON DELETE CASCADE
);

CREATE TABLE Refunds (
  Id                 UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
  PaymentId          UNIQUEIDENTIFIER NOT NULL,
  ProviderRefundId   NVARCHAR(255) NOT NULL UNIQUE,
  Amount             DECIMAL(18,2) NOT NULL,
  Reason             NVARCHAR(MAX),
  Status             NVARCHAR(20) NOT NULL DEFAULT 'processing',
  CreatedAt          DATETIME2 NOT NULL DEFAULT GETDATE(),
  FOREIGN KEY (PaymentId) REFERENCES Payments(Id) ON DELETE CASCADE
);

CREATE TABLE Coupons (
  Id                 UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
  Code               NVARCHAR(50) NOT NULL UNIQUE,
  DiscountPercent    DECIMAL(5,2),
  DiscountAmount     DECIMAL(18,2),
  MaxUses            INT,
  UsesCount          INT NOT NULL DEFAULT 0,
  ValidFrom          DATETIME2,
  ValidTo            DATETIME2,
  IsActive           BIT NOT NULL DEFAULT 1,
  CreatedAt          DATETIME2 NOT NULL DEFAULT GETDATE()
);

CREATE TABLE CouponUsages (
  Id                 UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
  CouponId           UNIQUEIDENTIFIER NOT NULL,
  UserId             UNIQUEIDENTIFIER NOT NULL,
  BookingId          UNIQUEIDENTIFIER,
  UsedAt             DATETIME2 NOT NULL DEFAULT GETDATE(),
  FOREIGN KEY (CouponId) REFERENCES Coupons(Id),
  FOREIGN KEY (UserId) REFERENCES Users(Id),
  FOREIGN KEY (BookingId) REFERENCES Bookings(Id)
);

CREATE TABLE SubscriptionPlans (
  Id                 UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
  Name               NVARCHAR(100) NOT NULL,
  Price              DECIMAL(18,2) NOT NULL,
  Currency           NVARCHAR(10) NOT NULL DEFAULT 'VND',
  BillingCycle       NVARCHAR(20) NOT NULL DEFAULT 'monthly',
  FeaturesJson       NVARCHAR(MAX),
  IsActive           BIT NOT NULL DEFAULT 1,
  CreatedAt          DATETIME2 NOT NULL DEFAULT GETDATE()
);

CREATE TABLE UserSubscriptions (
  Id                 UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
  UserId             UNIQUEIDENTIFIER NOT NULL,
  PlanId             UNIQUEIDENTIFIER NOT NULL,
  Status             NVARCHAR(20) NOT NULL DEFAULT 'active',
  CurrentPeriodStart DATETIME2 NOT NULL,
  CurrentPeriodEnd   DATETIME2 NOT NULL,
  CancelAtPeriodEnd  BIT NOT NULL DEFAULT 0,
  CreatedAt          DATETIME2 NOT NULL DEFAULT GETDATE(),
  UpdatedAt          DATETIME2 NOT NULL DEFAULT GETDATE(),
  FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
  FOREIGN KEY (PlanId) REFERENCES SubscriptionPlans(Id)
);

-- ---------- Contracts + Deliverables ----------
CREATE TABLE Contracts (
  Id               UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
  BookingId        UNIQUEIDENTIFIER NOT NULL,
  Version          INT NOT NULL DEFAULT 1,
  Title            NVARCHAR(255) NOT NULL DEFAULT 'Service Contract',
  TermsText        NVARCHAR(MAX) NOT NULL,
  Status           NVARCHAR(20) NOT NULL DEFAULT 'draft',
  CreatedByUserId  UNIQUEIDENTIFIER NOT NULL,
  CreatedAt        DATETIME2 NOT NULL DEFAULT GETDATE(),
  UpdatedAt        DATETIME2 NOT NULL DEFAULT GETDATE(),
  FOREIGN KEY (BookingId) REFERENCES Bookings(Id) ON DELETE CASCADE,
  FOREIGN KEY (CreatedByUserId) REFERENCES Users(Id),
  CONSTRAINT UQ_Contract_Booking_Version UNIQUE (BookingId, Version)
);

CREATE TABLE ContractSignatures (
  Id               UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
  ContractId       UNIQUEIDENTIFIER NOT NULL,
  UserId           UNIQUEIDENTIFIER NOT NULL,
  SignedAt         DATETIME2 NOT NULL DEFAULT GETDATE(),
  SignatureData    NVARCHAR(MAX), 
  IpAddress        NVARCHAR(50),
  UserAgent        NVARCHAR(MAX),
  FOREIGN KEY (ContractId) REFERENCES Contracts(Id) ON DELETE CASCADE,
  FOREIGN KEY (UserId) REFERENCES Users(Id),
  CONSTRAINT UQ_Contract_Signer UNIQUE (ContractId, UserId)
);

CREATE TABLE Deliverables (
  Id               UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
  BookingId        UNIQUEIDENTIFIER NOT NULL,
  ItemId           UNIQUEIDENTIFIER,
  DeliverableType  NVARCHAR(50) NOT NULL, 
  Title            NVARCHAR(255) NOT NULL,
  Description      NVARCHAR(MAX),
  DueAt            DATETIME2,
  Status           NVARCHAR(20) NOT NULL DEFAULT 'pending',
  SubmittedAt      DATETIME2,
  ApprovedAt       DATETIME2,
  CreatedAt        DATETIME2 NOT NULL DEFAULT GETDATE(),
  UpdatedAt        DATETIME2 NOT NULL DEFAULT GETDATE(),
  FOREIGN KEY (BookingId) REFERENCES Bookings(Id) ON DELETE CASCADE,
  FOREIGN KEY (ItemId) REFERENCES BookingItems(Id)
);

CREATE TABLE DeliverableAttachments (
  DeliverableId    UNIQUEIDENTIFIER NOT NULL,
  FileId           UNIQUEIDENTIFIER NOT NULL,
  PRIMARY KEY (DeliverableId, FileId),
  FOREIGN KEY (DeliverableId) REFERENCES Deliverables(Id) ON DELETE CASCADE,
  FOREIGN KEY (FileId) REFERENCES Files(Id)
);

-- ---------- Wallet + Payout ----------
CREATE TABLE UserWallets (
  UserId           UNIQUEIDENTIFIER PRIMARY KEY,
  Balance          DECIMAL(18,2) NOT NULL DEFAULT 0,
  LockedBalance    DECIMAL(18,2) NOT NULL DEFAULT 0,
  Currency         NVARCHAR(10) NOT NULL DEFAULT 'VND',
  UpdatedAt        DATETIME2 NOT NULL DEFAULT GETDATE(),
  FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);

CREATE TABLE WalletLedger (
  Id               UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
  WalletUserId     UNIQUEIDENTIFIER NOT NULL,
  Amount           DECIMAL(18,2) NOT NULL,
  TransactionType  NVARCHAR(50) NOT NULL, -- e.g., earning, withdrawal, refund
  ReferenceId      UNIQUEIDENTIFIER, -- Could be BookingId, PayoutId, etc.
  Description      NVARCHAR(MAX),
  CreatedAt        DATETIME2 NOT NULL DEFAULT GETDATE(),
  FOREIGN KEY (WalletUserId) REFERENCES UserWallets(UserId) ON DELETE CASCADE
);

CREATE TABLE PayoutRequests (
  Id               UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
  UserId           UNIQUEIDENTIFIER NOT NULL,
  Amount           DECIMAL(18,2) NOT NULL,
  Currency         NVARCHAR(10) NOT NULL DEFAULT 'VND',
  BankInfoJson     NVARCHAR(MAX),
  Status           NVARCHAR(20) NOT NULL DEFAULT 'pending', -- pending, processed, failed, cancelled
  ProcessedAt      DATETIME2,
  AdminNote        NVARCHAR(MAX),
  CreatedAt        DATETIME2 NOT NULL DEFAULT GETDATE(),
  UpdatedAt        DATETIME2 NOT NULL DEFAULT GETDATE(),
  FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);

-- ---------- Disputes + Moderation ----------
CREATE TABLE Disputes (
  Id               UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
  BookingId        UNIQUEIDENTIFIER NOT NULL,
  RaisedByUserId   UNIQUEIDENTIFIER NOT NULL,
  DisputeType      NVARCHAR(30) NOT NULL DEFAULT 'general',
  Reason           NVARCHAR(MAX) NOT NULL,
  AmountClaimed    DECIMAL(18,2),
  Currency         NVARCHAR(10) NOT NULL DEFAULT 'VND',
  Status           NVARCHAR(20) NOT NULL DEFAULT 'open',
  ResolutionNote   NVARCHAR(MAX),
  ResolvedByAdminId UNIQUEIDENTIFIER,
  CreatedAt        DATETIME2 NOT NULL DEFAULT GETDATE(),
  UpdatedAt        DATETIME2 NOT NULL DEFAULT GETDATE(),
  FOREIGN KEY (BookingId) REFERENCES Bookings(Id) ON DELETE CASCADE,
  FOREIGN KEY (RaisedByUserId) REFERENCES Users(Id),
  FOREIGN KEY (ResolvedByAdminId) REFERENCES Users(Id)
);

CREATE TABLE Reports (
  Id               UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
  ReporterUserId   UNIQUEIDENTIFIER NOT NULL,
  TargetUserId     UNIQUEIDENTIFIER,
  MessageId        UNIQUEIDENTIFIER,
  Reason           NVARCHAR(MAX) NOT NULL,
  Status           NVARCHAR(20) NOT NULL DEFAULT 'open',
  AdminNote        NVARCHAR(MAX),
  CreatedAt        DATETIME2 NOT NULL DEFAULT GETDATE(),
  UpdatedAt        DATETIME2 NOT NULL DEFAULT GETDATE(),
  FOREIGN KEY (ReporterUserId) REFERENCES Users(Id),
  FOREIGN KEY (TargetUserId) REFERENCES Users(Id),
  FOREIGN KEY (MessageId) REFERENCES ChatMessages(Id)
);

-- ---------- System ----------
CREATE TABLE SystemSettings (
  [Key]            NVARCHAR(80) PRIMARY KEY,
  ValueJson        NVARCHAR(MAX) NOT NULL,
  Description      NVARCHAR(MAX),
  UpdatedAt        DATETIME2 NOT NULL DEFAULT GETDATE()
);

-- Seed SystemSettings
INSERT INTO SystemSettings ([Key], ValueJson, Description)
VALUES
  ('platform_fee_percent', '5', 'Marketplace platform fee percent'),
  ('vat_percent', '8', 'VAT percent (example)'),
  ('currency_default', '"VND"', 'Default currency');

CREATE TABLE Notifications (
  Id               UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
  UserId           UNIQUEIDENTIFIER NOT NULL,
  Type             NVARCHAR(30) NOT NULL, 
  Title            NVARCHAR(255) NOT NULL,
  Body             NVARCHAR(MAX),
  Link             NVARCHAR(MAX),
  ReadAt           DATETIME2,
  CreatedAt        DATETIME2 NOT NULL DEFAULT GETDATE(),
  FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);

CREATE TABLE AdminAuditLogs (
  Id               UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
  AdminUserId      UNIQUEIDENTIFIER NOT NULL,
  Action           NVARCHAR(100) NOT NULL,
  TargetTable      NVARCHAR(80),
  TargetId         UNIQUEIDENTIFIER,
  MetadataJson     NVARCHAR(MAX),
  CreatedAt        DATETIME2 NOT NULL DEFAULT GETDATE(),
  FOREIGN KEY (AdminUserId) REFERENCES Users(Id)
);

-- ---------- Seed roles ----------
INSERT INTO Roles (Code, Name) VALUES
  ('admin','Administrator'),
  ('customer','Customer/Brand'),
  ('kol','KOL/KOC');

GO

-- ---------- Reporting Views ----------
CREATE VIEW View_KolSearch
AS
SELECT 
    kp.UserId,
    u.FullName,
    u.AvatarUrl,
    kp.InfluencerType,
    kp.LocationCity,
    kp.LocationCountry,
    kp.MinBudget,
    kp.RatingAvg,
    kp.RatingCount,
    (SELECT STRING_AGG(c.Name, ', ') 
     FROM KolCategoryMap kcm 
     JOIN KolCategories c ON kcm.CategoryId = c.Id 
     WHERE kcm.KolUserId = kp.UserId) AS CategoriesText,
    (SELECT STRING_AGG(t.Name, ', ') 
     FROM KolTagMap ktm 
     JOIN Tags t ON ktm.TagId = t.Id 
     WHERE ktm.KolUserId = kp.UserId) AS TagsText,
    (SELECT STRING_AGG(ks.Platform, ', ')
     FROM KolSocialAccounts ks
     WHERE ks.KolUserId = kp.UserId) AS PlatformsText,
    (SELECT MAX(Followers) 
     FROM KolSocialAccounts ks 
     WHERE ks.KolUserId = kp.UserId) AS MaxFollowers
FROM KolProfiles kp
JOIN Users u ON kp.UserId = u.Id
WHERE u.Status = 'active' AND kp.IsVerified = 1;
GO

CREATE VIEW View_PlatformRevenue
AS
SELECT 
    FORMAT(CreatedAt, 'yyyy-MM') AS RevenueMonth,
    SUM(PlatformFee) AS TotalPlatformFee,
    COUNT(Id) AS TotalBookings
FROM Bookings
WHERE Status IN ('paid', 'in_progress', 'completed')
GROUP BY FORMAT(CreatedAt, 'yyyy-MM');
GO
