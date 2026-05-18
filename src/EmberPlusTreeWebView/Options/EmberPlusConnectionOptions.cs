namespace EmberPlusTreeWebView.Options;

public sealed class EmberPlusConnectionOptions
{
    public const string SectionName = "EmberPlus";

    public int TimeoutMilliseconds { get; set; } = 10000;
}
