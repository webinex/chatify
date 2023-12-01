using Microsoft.EntityFrameworkCore;
using Webinex.Chatify;
using Webinex.Chatify.Abstractions;
using Webinex.Chatify.AspNetCore;
using Webinex.Chatify.Common.Events;
using Webinex.Chatify.DataAccess;
using Webinex.Chatify.Example;
using Webinex.Chatify.Types;
using File = Webinex.Chatify.Types.File;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddFakeAuth()
    .AddCors(x => x
        .AddDefaultPolicy(policy => policy
            .WithOrigins(builder.Configuration.GetSection("CORS:Origins").Get<string[]>()!)
            .AllowCredentials()
            .AllowAnyHeader()
            .AllowAnyMethod()
            .WithExposedHeaders("x-coded-failure")));

builder.Services
    .AddHttpContextAccessor()
    .AddSignalR()
    .Services
    .AddSingleton<IChatifyAspNetCoreContext, ChatifyAspNetCoreContext>()
    .AddChatify(x => x
        .UseDbContext(sql => sql
            .UseSqlServer(builder.Configuration.GetConnectionString("Db"))));

builder.Services
    .AddControllers()
    .AddChatifyAspNetCore(x => x
        .AddController()
        .AddSignalR<ExampleHub>());

var app = builder.Build();

if (builder.Configuration.GetValue<bool>("WebAppHost"))
{
    app.UseDefaultFiles();
    app.UseStaticFiles();
    app.MapWhen(
        x => !x.Request.Path.StartsWithSegments("/api"),
        x => x.UseSpa(_ => { }));
}

using (var scope = app.Services.CreateScope())
{
    var provider = scope.ServiceProvider;
    var dbContext = provider.GetRequiredService<ChatifyDbContext>();
    var eventService = provider.GetRequiredService<IEventService>();
    dbContext.Database.EnsureCreated();

    var anyAccount = dbContext.Accounts.Any();

    if (!anyAccount)
    {
        dbContext.Accounts.AddRange(
            new Account(AccountId.SYSTEM, "System", null, false),
            new Account("1", "James Doe", "/api/avatar/1", true),
            new Account("2", "Kevin Hearth", "/api/avatar/2", true),
            new Account("3", "Colin Wolshire", "/api/avatar/3", true),
            new Account("4", "Annabeth Smith", "/api/avatar/4", true),
            new Account("5", "Anna Crossfort", "/api/avatar/5", true),
            new Account("6", "Jessica Parker", "/api/avatar/6", true));

        dbContext.SaveChanges();

        dbContext.Chats.AddRange(
            Chat.New(eventService, new AccountContext("1"), "Development",
                members: new[] { "1", "2", "3" },
                message: new MessageContent("New development chat!", new List<File>())),
            Chat.New(eventService, new AccountContext("1"), "Production",
                members: new[] { "1", "2", "3" },
                message: new MessageContent("New production chat!", new List<File>())));

        eventService.FlushAsync().GetAwaiter().GetResult();

        dbContext.SaveChanges();
    }
}

app.UseCors();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ExampleHub>("/api/chatify/hub");

app.Run();