﻿using System.Data;
using LinqToDB;
using LinqToDB.Data;
using Webinex.Chatify.Abstractions;
using Webinex.Chatify.DataAccess;
using Webinex.Chatify.Rows;
using Webinex.Chatify.Services.Common.Caches;

namespace Webinex.Chatify.Services;

internal interface IAccountService
{
    Task<IReadOnlyCollection<Account>> GetAllAsync(AccountContext? onBehalfOf = null);

    Task<IReadOnlyDictionary<string, Account>> ByIdAsync(
        IEnumerable<string> ids,
        bool tryCache = false,
        bool required = true);

    Task<Account[]> AddAsync(IEnumerable<AddAccountArgs> commands);
    Task<Account[]> UpdateAsync(IEnumerable<UpdateAccountArgs> commands);
}

internal class AccountService : IAccountService
{
    private readonly IEntityCache<AccountRow> _cache;
    private readonly IChatifyDataConnectionFactory _dataConnectionFactory;

    public AccountService(IEntityCache<AccountRow> cache, IChatifyDataConnectionFactory dataConnectionFactory)
    {
        _cache = cache;
        _dataConnectionFactory = dataConnectionFactory;
    }

    public async Task<IReadOnlyCollection<Account>> GetAllAsync(AccountContext? onBehalfOf = null)
    {
        await using var connection = _dataConnectionFactory.Create();
        var queryable = connection.AccountRows.AsQueryable();

        queryable = onBehalfOf != null
            ? queryable.Where(x => x.WorkspaceId == onBehalfOf.WorkspaceId)
            : queryable.Where(x => x.Id != AccountContext.System.Id);

        var result = await queryable.ToArrayAsync();
        return result.Select(x => x.ToAbstraction()).ToArray();
    }

    public async Task<IReadOnlyDictionary<string, Account>> ByIdAsync(
        IEnumerable<string> ids,
        bool tryCache = false,
        bool required = true)
    {
        ids = ids.Distinct().ToArray();
        await using var connection = _dataConnectionFactory.Create();

        if (!tryCache)
        {
            var dbResult = await connection.AccountRows.Where(x => ids.Contains(x.Id)).ToArrayAsync();
            if (dbResult.Length != ids.Count() && required) throw new InvalidOperationException();
            return dbResult.ToDictionary(x => x.Id, x => x.ToAbstraction());
        }

        var result = await GetTryCacheAsync(ids, required);
        return result.ToDictionary(x => x.Key, x => x.Value.ToAbstraction()).AsReadOnly();
    }

    private async Task<IReadOnlyDictionary<string, AccountRow>> GetTryCacheAsync(
        IEnumerable<string> ids,
        bool required = true)
    {
        return await _cache.GetOrCreateAsync(ids, async notFoundIds =>
        {
            await using var connection = _dataConnectionFactory.Create();
            var dbResult = await connection.AccountRows.Where(x => notFoundIds.Contains(x.Id)).ToArrayAsync();
            if (dbResult.Length != notFoundIds.Count() && required) throw new InvalidOperationException();
            return dbResult.ToDictionary(x => x.Id);
        });
    }

    public async Task<Account[]> AddAsync(IEnumerable<AddAccountArgs> commands)
    {
        await using var connection = _dataConnectionFactory.Create();
        var accounts = commands.Select(x => new AccountRow(x.Id, x.WorkspaceId, x.Name, x.Avatar, true)).ToArray();
        await connection.AccountRows.BulkCopyAsync(accounts);
        return accounts.Select(x => x.ToAbstraction()).ToArray();
    }

    public async Task<Account[]> UpdateAsync(IEnumerable<UpdateAccountArgs> commands)
    {
        await using var connection = _dataConnectionFactory.Create();
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted);

        commands = commands.ToArray();
        var ids = commands.Select(x => x.Id).Distinct().ToArray();
        var accounts = await connection.AccountRows.Where(x => ids.Contains(x.Id)).ToArrayAsync();
        var accountById = accounts.ToDictionary(x => x.Id);

        foreach (var command in commands)
        {
            var account = accountById[command.Id];
            account.UpdateName(command.Name);
            account.UpdateAvatar(command.Avatar);
            account.UpdateActive(command.Active);
        }

        var dataTable = connection.CreateTempTable(accounts);
        await dataTable.UpdateAsync(connection.AccountRows, e => e);
        await transaction.CommitAsync();
        return accounts.Select(x => x.ToAbstraction()).ToArray();
    }
}
