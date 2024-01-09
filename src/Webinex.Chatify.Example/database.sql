if not exists (select *
               from sys.schemas
               where name = 'chatify')
    begin
        EXEC ('create schema [chatify] authorization [dbo]');
    end

if not exists (select *
               from sys.tables
               where name = 'Accounts')
    begin

        -- =============================
        -- ========= ACCOUNTS ==========
        -- =============================

        create table [chatify].[Accounts]
        (
            Id          nvarchar(250) not null
                constraint PK_Accounts
                    primary key,
            WorkspaceId nvarchar(250) not null,
            Avatar      nvarchar(500),
            Name        nvarchar(500) not null,
            Type        int           not null,
            Active      bit           not null
        )

        -- =============================
        -- ========== CHATS ============
        -- =============================

        create table [chatify].[Chats]
        (
            Id          uniqueidentifier not null
                constraint PK_Chats
                    primary key,
            Name        nvarchar(500)    not null,
            CreatedAt   datetimeoffset   not null,
            CreatedById nvarchar(250)    not null
                constraint FK_Chats_Accounts_CreatedById
                    references chatify.Accounts
                    on delete cascade
        )

        create index IX_Chats_CreatedById
            on chatify.Chats (CreatedById)

        -- =============================
        -- ========== MEMBERS ============
        -- =============================

        create table chatify.Members
        (
            ChatId    uniqueidentifier not null
                constraint FK_Members_Chats_ChatId
                    references chatify.Chats,
            AccountId nvarchar(250)    not null
                constraint FK_Members_Accounts_AccountId
                    references chatify.Accounts,
            AddedById nvarchar(250)    not null
                constraint FK_Members_Accounts_AddedById
                    references chatify.Accounts,
            AddedAt   datetimeoffset   not null,
            constraint PK_Members
                primary key (ChatId, AccountId)
        )

        create index IX_Members_AccountId
            on chatify.Members (AccountId)

        create index IX_Members_AddedById
            on chatify.Members (AddedById)


        -- =============================
        -- ======== MESSAGES ===========
        -- =============================

        create table chatify.Messages
        (
            Id       nvarchar(450)    not null
                constraint PK_Messages
                    primary key,
            ChatId   uniqueidentifier not null,
            Text     nvarchar(max)    not null,
            AuthorId nvarchar(250)    not null
                constraint FK_Messages_Accounts_AuthorId
                    references chatify.Accounts,
            SentAt   datetimeoffset   not null,
            Files    nvarchar(max)    not null
        )

        -- =============================
        -- ======== DELIVERIES =========
        -- =============================

        create table chatify.Deliveries
        (
            ChatId    uniqueidentifier not null
                constraint FK_Deliveries_Chats_ChatId
                    references chatify.Chats
                    on delete cascade,
            MessageId nvarchar(450)    not null
                constraint FK_Deliveries_Messages_MessageId
                    references chatify.Messages
                    on delete cascade,
            ToId      nvarchar(250)    not null
                constraint FK_Deliveries_Accounts_ToId
                    references chatify.Accounts,
            FromId    nvarchar(250)    not null
                constraint FK_Deliveries_Accounts_FromId
                    references chatify.Accounts,
            [Read]    bit              not null,
            constraint PK_Deliveries
                primary key (ChatId, MessageId, ToId)
        )

        create index IX_Deliveries_FromId
            on chatify.Deliveries (FromId)

        create index IX_Deliveries_MessageId
            on chatify.Deliveries (MessageId)

        create index IX_Deliveries_ToId
            on chatify.Deliveries (ToId)

        -- =============================
        -- ======== ACTIVITIES =========
        -- =============================

        create table chatify.ChatActivities
        (
            ChatId            uniqueidentifier not null
                constraint FK_ChatActivities_Chats_ChatId
                    references chatify.Chats,
            AccountId         nvarchar(250)    not null
                constraint FK_ChatActivities_Accounts_AccountId
                    references chatify.Accounts,
            LastMessageFromId nvarchar(250)    not null
                constraint FK_ChatActivities_Accounts_LastMessageFromId
                    references chatify.Accounts,
            LastMessageId     nvarchar(450)    not null
                constraint FK_ChatActivities_Messages_LastMessageId
                    references chatify.Messages,
            constraint PK_ChatActivities
                primary key (ChatId, AccountId)
        )

        create index IX_ChatActivities_AccountId
            on chatify.ChatActivities (AccountId)

        create index IX_ChatActivities_LastMessageFromId
            on chatify.ChatActivities (LastMessageFromId)

        create index IX_ChatActivities_LastMessageId
            on chatify.ChatActivities (LastMessageId)
    end