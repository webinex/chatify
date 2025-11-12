using Microsoft.EntityFrameworkCore;
using Webinex.Chatify.Abstractions;
using Webinex.Chatify.DataAccess;
using Webinex.Chatify.Rows;
using Webinex.Chatify.Services.Caches.Common;

namespace Webinex.Chatify.Services;

internal interface IAccountService
{
    Task<IReadOnlyCollection<Account>> GetAllAsync(AccountContext? onBehalfOf = null);

    Task<IReadOnlyDictionary<string, Account>> ByIdAsync(IEnumerable<string> ids, bool tryCache = false,
        bool required = true);

    Task<Account[]> AddAsync(IEnumerable<AddAccountArgs> commands);
    Task<Account[]> UpdateAsync(IEnumerable<UpdateAccountArgs> commands);
    Task<Account[]> UpdateAsync(IEnumerable<UpdateAccountDataArgs> commands);
}

internal class AccountService : IAccountService
{
    private readonly IEntityCache<AccountRow> _cache;
    private readonly ChatifyDbContext _dbContext;

    public AccountService(IEntityCache<AccountRow> cache, ChatifyDbContext dbContext)
    {
        _cache = cache;
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<Account>> GetAllAsync(AccountContext? onBehalfOf = null)
    {
        var queryable = _dbContext.Accounts.AsQueryable();

        queryable = onBehalfOf != null
            ? queryable.Where(x => x.WorkspaceId == onBehalfOf.WorkspaceId)
            : queryable.Where(x => x.Id != AccountId.SYSTEM);

        var result = await queryable.ToArrayAsync();
        return result.Select(x => x.ToAbstraction()).ToArray();
    }

    public async Task<IReadOnlyDictionary<string, Account>> ByIdAsync(
        IEnumerable<string> ids,
        bool tryCache = false,
        bool required = true)
    {
        if (!tryCache)
        {
            return (await _dbContext.Accounts.FindManyNoTrackingAsync(ids, required))
                .ToDictionary(x => x.Id,
                    x => x.ToAbstraction());
        }

        var result = await GetTryCacheAsync(ids, required);
        return result.ToDictionary(x => x.Key, x => x.Value.ToAbstraction()).AsReadOnly();
    }

    private async Task<IReadOnlyDictionary<string, AccountRow>> GetTryCacheAsync(IEnumerable<string> ids,
        bool required = true)
    {
        return await _cache.GetOrCreateAsync(ids, async notFoundIds =>
        {
            var accounts = await _dbContext.Accounts.FindManyNoTrackingAsync(notFoundIds, required);
            return accounts.ToDictionary(x => x.Id);
        });
    }

    public async Task<Account[]> AddAsync(IEnumerable<AddAccountArgs> commands)
    {
        var accounts = commands.Select(x => new AccountRow(x.Id, x.WorkspaceId, x.Name, x.Avatar, true, null)).ToArray();
        await _dbContext.Accounts.AddRangeAsync(accounts);
        await _dbContext.SaveChangesAsync();
        return accounts.Select(x => x.ToAbstraction()).ToArray();
    }

    public async Task<Account[]> UpdateAsync(IEnumerable<UpdateAccountArgs> commands)
    {
        commands = commands.ToArray();
        var ids = commands.Select(x => x.Id).Distinct().ToArray();
        var accounts = await _dbContext.Accounts.Where(x => ids.Contains(x.Id)).ToArrayAsync();
        var accountById = accounts.ToDictionary(x => x.Id);

        foreach (var command in commands)
        {
            var account = accountById[command.Id];
            account.UpdateName(command.Name);
            account.UpdateAvatar(command.Avatar);
            account.UpdateActive(command.Active);
        }

        await _dbContext.SaveChangesAsync();
        return accounts.Select(x => x.ToAbstraction()).ToArray();
    }

    public async Task<Account[]> UpdateAsync(IEnumerable<UpdateAccountDataArgs> commands)
    {
        commands = [.. commands];
        var ids = commands.Select(x => x.Id).Distinct().ToArray();
        var accounts = await _dbContext.Accounts.Where(x => ids.Contains(x.Id)).ToArrayAsync();
        var accountById = accounts.ToDictionary(x => x.Id);

        foreach (var command in commands)
        {
            var account = accountById[command.Id];
            if (command.Name.HasValue)
                account.UpdateName(command.Name.Value);
            if (command.Avatar.HasValue)
                account.UpdateAvatar(command.Avatar.Value);
            if (command.Active.HasValue)
                account.UpdateActive(command.Active.Value);
            if (command.AutoReply.HasValue)
                account.UpdateAutoReply(command.AutoReply.Value);
        }

        await _dbContext.SaveChangesAsync();
        return [.. accounts.Select(x => x.ToAbstraction())];
    }
}