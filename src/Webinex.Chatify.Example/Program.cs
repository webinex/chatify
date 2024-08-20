using LinqToDB;
using Microsoft.Data.SqlClient;
using Webinex.Chatify;
using Webinex.Chatify.AspNetCore;
using Webinex.Chatify.Example;
using Webinex.Flippo;
using Webinex.Flippo.AspNetCore;

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
    .AddScoped<IChatifyAspNetCoreContextProvider, ChatifyAspNetCoreContextProvider>()
    .AddChatify(x => x
        .UseDataConnection(sql => sql.UseSqlServer(builder.Configuration.GetConnectionString("Db")!)));

builder.Services
    .AddControllers()
    .AddChatifyAspNetCore(x => x
        .AddController()
        .AddSignalR<ExampleHub>())
    .AddFlippoController();

var filesDirectory = Path.Join(builder.Environment.ContentRootPath, ".files");
if (!Directory.Exists(filesDirectory))
{
    Directory.CreateDirectory(filesDirectory);
}

builder.Services.AddFlippo(x =>
    x.AddFileSystemBlob(filesDirectory).UseSasToken("a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3",
        TimeSpan.FromHours(1)));

var app = builder.Build();

if (builder.Configuration.GetValue<bool>("WebAppHost"))
{
    app.UseDefaultFiles();
    app.UseStaticFiles();
    app.MapWhen(
        x => !x.Request.Path.StartsWithSegments("/api"),
        x => x.UseSpa(_ => { }));
}

await using (var databaseSqlScriptStream = typeof(Program).Assembly
                 .GetManifestResourceStream("Webinex.Chatify.Example.database.sql"))

await using (var databaseSeedScriptStream = typeof(Program).Assembly
                 .GetManifestResourceStream("Webinex.Chatify.Example.databaseSeed.sql"))
await using (var sql = new SqlConnection(builder.Configuration.GetConnectionString("Db")))
{
    var databaseSqlScript = await new StreamReader(databaseSqlScriptStream!).ReadToEndAsync();
    var databaseSqlCommand = new SqlCommand(databaseSqlScript, sql);

    var databaseSeedSqlScript = await new StreamReader(databaseSeedScriptStream!).ReadToEndAsync();
    var databaseSeedSqlCommand = new SqlCommand(databaseSeedSqlScript, sql);

    await sql.OpenAsync();
    await databaseSqlCommand.ExecuteNonQueryAsync();
    await databaseSeedSqlCommand.ExecuteNonQueryAsync();
}

app.UseCors();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ExampleHub>("/api/chatify/hub");

app.Run();
