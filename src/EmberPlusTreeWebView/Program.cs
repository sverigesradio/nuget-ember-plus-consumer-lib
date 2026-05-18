using EmberPlusTreeWebView.Options;
using EmberPlusTreeWebView.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<EmberPlusConnectionOptions>(
    builder.Configuration.GetSection(EmberPlusConnectionOptions.SectionName));
builder.Services.AddSingleton<EmberTreeSnapshotService>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/tree", async (
    EmberTreeSnapshotService service,
    string? host,
    int? port,
    CancellationToken cancellationToken) =>
{
    try
    {
        if (string.IsNullOrWhiteSpace(host))
        {
            return Results.BadRequest(new { detail = "Query parameter 'host' is required." });
        }

        if (port is null or < 1 or > 65535)
        {
            return Results.BadRequest(new { detail = "Query parameter 'port' must be between 1 and 65535." });
        }

        return Results.Ok(await service.GetTreeAsync(host, port.Value, cancellationToken));
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
