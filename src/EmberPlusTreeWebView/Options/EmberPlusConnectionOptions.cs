namespace EmberPlusTreeWebView.Options;

public sealed class EmberPlusConnectionOptions
{
    public const string SectionName = "EmberPlus";

    public string Host { get; set; } = "localhost";

    public int Port { get; set; } = 9000;

    public int TimeoutMilliseconds { get; set; } = 10000;
}
