begin transaction create_chatify_schema

begin try

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
                Id                  nvarchar(250)   not null
                                        constraint PK_Accounts
                                        primary key,
                WorkspaceId         nvarchar(250)   not null,
                Avatar              nvarchar(500),
                Name                nvarchar(500)   not null,
                Type                int             not null,
                AutoReplyText       nvarchar(max)   null,
                AutoReplyStart      datetimeoffset  null,
                AutoReplyEnd        datetimeoffset  null,
                Active              bit             not null
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
            -- ======== CHAT META ==========
            -- =============================

            create table [chatify].[ChatMeta]
            (
                ChatId    uniqueidentifier not null
                    constraint FK_ChatMeta_Chats_ChatId
                        references chatify.Chats
                    primary key,
                LastIndex int              not null
            )

            -- =============================
            -- ======== MESSAGES ===========
            -- =============================

            create table chatify.Messages
            (
                Id       char(65)         not null
                    constraint PK_Messages
                        primary key,
                ChatId   uniqueidentifier not null,
                Text     nvarchar(max)    not null,
                AuthorId nvarchar(250)    not null
                    constraint FK_Messages_Accounts_AuthorId
                        references chatify.Accounts,
                SentAt   datetimeoffset   not null,
                [Index]  int              not null,
                Files    nvarchar(max)    not null
            )

            -- =============================
            -- ========== MEMBERS ============
            -- =============================

            create table chatify.Members
            (
                Id                uniqueidentifier not null
                    constraint PK_Members
                        primary key,
                ChatId            uniqueidentifier not null
                    constraint FK_Members_Chats_ChatId
                        references chatify.Chats,
                AccountId         nvarchar(250)    not null
                    constraint FK_Members_Accounts_AccountId
                        references chatify.Accounts,
                AddedById         nvarchar(250)    not null
                    constraint FK_Members_Accounts_AddedById
                        references chatify.Accounts,
                AddedAt           datetimeoffset   not null,
                FirstMessageId    char(65)         not null
                    constraint FK_Members_Messages_FirstMessageId
                        references chatify.Messages,
                FirstMessageIndex int              not null,
                LastMessageId     char(65)         null
                    constraint FK_Members_Messages_LastMessageId
                        references chatify.Messages,
                LastMessageIndex  int              null,
            )

            create index IX_Members_AccountId
                on chatify.Members (AccountId)

            create index IX_Members_AddedById
                on chatify.Members (AddedById)

            create index IX_Members_FirstMessageId
                on chatify.Members (FirstMessageId)

            create index IX_Members_LastMessageId
                on chatify.Members (LastMessageId)

            -- =============================
            -- ======== ACTIVITIES =========
            -- =============================

            create table chatify.ChatActivities
            (
                ChatId               uniqueidentifier not null
                    constraint FK_ChatActivities_Chats_ChatId
                        references chatify.Chats,
                AccountId            nvarchar(250)    not null
                    constraint FK_ChatActivities_Accounts_AccountId
                        references chatify.Accounts,
                LastMessageFromId    nvarchar(250)    not null
                    constraint FK_ChatActivities_Accounts_LastMessageFromId
                        references chatify.Accounts,
                LastMessageId        char(65)         not null
                    constraint FK_ChatActivities_Messages_LastMessageId
                        references chatify.Messages,
                LastMessageIndex     int              not null,
                LastReadMessageId    char(65)         null
                    constraint FK_ChatActivities_Messages_LastReadMessageId
                        references chatify.Messages,
                LastReadMessageIndex int              null,
                Active               bit              not null,
                constraint PK_ChatActivities
                    primary key (ChatId, AccountId)
            )

            create index IX_ChatActivities_AccountId
                on chatify.ChatActivities (AccountId)

            create index IX_ChatActivities_LastMessageFromId
                on chatify.ChatActivities (LastMessageFromId)

            create index IX_ChatActivities_LastMessageId
                on chatify.ChatActivities (LastMessageId)

            create index IX_ChatActivities_LastReadMessageId
                on chatify.ChatActivities (LastReadMessageId)
        end

    commit transaction create_chatify_schema
end try
begin catch
    rollback transaction create_chatify_schema;
    throw
end catch
