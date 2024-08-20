-- drop table chatify.ThreadMeta
-- drop table chatify.ThreadWatches
-- drop table chatify.ThreadMessages
-- drop table chatify.Threads
-- drop table chatify.ChatActivities
-- drop table chatify.ChatMeta
-- drop table chatify.Members
-- drop table chatify.ChatMessages
-- drop table chatify.Chats
-- drop table chatify.Accounts
-- drop schema chatify

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
                Id          varchar(250)  not null
                    constraint PK_Accounts
                        primary key,
                WorkspaceId varchar(250)  not null,
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
                CreatedById varchar(250)     not null
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
            -- ====== CHAT MESSAGES ========
            -- =============================

            create table chatify.ChatMessages
            (
                Id       varchar(65)      not null
                    constraint PK_ChatMessages
                        primary key,
                ChatId   uniqueidentifier not null,
                Text     nvarchar(max)    not null,
                AuthorId varchar(250)     not null
                    constraint FK_ChatMessages_Accounts_AuthorId
                        references chatify.Accounts,
                SentAt   datetimeoffset   not null,
                Files    nvarchar(max)    not null
            )

            -- =============================
            -- ========== MEMBERS ============
            -- =============================

            create table chatify.ChatMembers
            (
                Id             uniqueidentifier not null
                    constraint PK_Members
                        primary key,
                ChatId         uniqueidentifier not null
                    constraint FK_Members_Chats_ChatId
                        references chatify.Chats,
                AccountId      varchar(250)     not null
                    constraint FK_Members_Accounts_AccountId
                        references chatify.Accounts,
                AddedById      varchar(250)     not null
                    constraint FK_Members_Accounts_AddedById
                        references chatify.Accounts,
                AddedAt        datetimeoffset   not null,
                FirstMessageId varchar(65)      not null
                    constraint FK_Members_ChatMessages_FirstMessageId
                        references chatify.ChatMessages,
                LastMessageId  varchar(65)      null
                    constraint FK_Members_Messages_LastMessageId
                        references chatify.ChatMessages,
            )

            create index IX_Members_AccountId
                on chatify.ChatMembers (AccountId)

            create index IX_Members_AddedById
                on chatify.ChatMembers (AddedById)

            create index IX_Members_FirstMessageId
                on chatify.ChatMembers (FirstMessageId)

            create index IX_Members_LastMessageId
                on chatify.ChatMembers (LastMessageId)

            -- =============================
            -- ======== ACTIVITIES =========
            -- =============================

            create table chatify.ChatActivities
            (
                ChatId            uniqueidentifier not null
                    constraint FK_ChatActivities_Chats_ChatId
                        references chatify.Chats,
                AccountId         varchar(250)     not null
                    constraint FK_ChatActivities_Accounts_AccountId
                        references chatify.Accounts,
                LastMessageFromId varchar(250)     not null
                    constraint FK_ChatActivities_Accounts_LastMessageFromId
                        references chatify.Accounts,
                LastMessageId     varchar(65)      not null
                    constraint FK_ChatActivities_ChatMessages_LastMessageId
                        references chatify.ChatMessages,
                LastReadMessageId varchar(65)      null
                    constraint FK_ChatActivities_ChatMessages_LastReadMessageId
                        references chatify.ChatMessages,
                Active            bit              not null,
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


            -- =============================
            -- ========== THREADS ==========
            -- =============================

            create table [chatify].[Threads]
            (
                Id            varchar(250)   not null
                    constraint PK_Threads
                        primary key,
                Name          nvarchar(500)  not null,
                CreatedAt     datetimeoffset not null,
                CreatedById   varchar(250)   not null
                    constraint FK_Threads_Accounts_CreatedById
                        references chatify.Accounts
                        on delete cascade,
                Archived      bit            not null,
                LastMessageId varchar(65)    null
            )

            create index IX_Threads_CreatedById
                on chatify.Threads (CreatedById)

            create index IX_Threads_LastMessageId
                on chatify.Threads (LastMessageId)

            -- =============================
            -- ====== THREAD MESSAGE =======
            -- =============================

            create table [chatify].[ThreadMessages]
            (
                Id       varchar(65)    not null
                    constraint PK_ThreadMessages
                        primary key,
                ThreadId varchar(250)   not null
                    constraint FK_ThreadMessages_Threads_ThreadId
                        references chatify.Threads,
                SentById varchar(250)   not null
                    constraint FK_ThreadMessages_Accounts_SentById
                        references chatify.Accounts,
                SentAt   datetimeoffset not null,
                Text     nvarchar(max)  not null,
                Files    nvarchar(max)  not null
            )

            create index IX_ThreadMessages_SentById
                on chatify.ThreadMessages (SentById)

            create index IX_ThreadMessages_ThreadId
                on chatify.ThreadMessages (ThreadId)

            -- ======================================================
            -- ======= THREAD -> THREAD MESSAGE FOREIGN KEY =========
            -- ======================================================

            alter table chatify.Threads
                add constraint FK_Threads_ThreadMessages_LastMessageId
                    foreign key (LastMessageId)
                        references chatify.ThreadMessages (Id)


            -- =============================
            -- ======= THREAD META =========
            -- =============================

            create table [chatify].[ThreadMeta]
            (
                ThreadId  varchar(250) not null
                    constraint FK_ThreadMeta_Threads_ThreadId
                        references chatify.Threads
                    constraint PK_ThreadMeta primary key,
                LastIndex int          null
            )

            -- =============================
            -- ======= THREAD WATCH ========
            -- =============================

            create table [chatify].[ThreadWatches]
            (
                ThreadId          varchar(250) not null
                    constraint FK_ThreadWatches_Threads_ThreadId
                        references chatify.Threads,
                AccountId         varchar(250) not null
                    constraint FK_ThreadWatches_Accounts_AccountId
                        references chatify.Accounts,
                LastReadMessageId varchar(65)  null
                    constraint FK_ThreadWatches_ThreadMessages_LastReadMessageId
                        references chatify.ThreadMessages,
                constraint PK_ThreadWatches
                    primary key (ThreadId, AccountId)
            )

            create index IX_ThreadWatches_AccountId
                on chatify.ThreadWatches (AccountId)

            create index IX_ThreadWatches_ThreadId
                on chatify.ThreadWatches (ThreadId)
        end

    commit transaction create_chatify_schema
end try
begin catch
    rollback transaction create_chatify_schema;
    throw
end catch
