using EmberPlusTreeWebView.Options;
using EmberPlusTreeWebView.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<EmberPlusConnectionOptions>(
    builder.Configuration.GetSection(EmberPlusConnectionOptions.SectionName));
builder.Services.AddSingleton<EmberTreeSnapshotService>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/tree", async (EmberTreeSnapshotService service, CancellationToken cancellationToken) =>
{
    try
    {
        return Results.Ok(await service.GetTreeAsync(cancellationToken));
    }
    catch (Exception exception)
    {
        return Results.Problem(
            statusCode: StatusCodes.Status503ServiceUnavailable,
            title: "Failed to fetch Ember+ tree",
            detail: exception.Message);
    }
});

app.Run();
