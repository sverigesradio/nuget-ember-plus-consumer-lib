namespace EmberPlusTreeWebView.Models;

public sealed record EmberElementDto
{
    public string Kind { get; init; } = string.Empty;

    public int Number { get; init; }

    public string Identifier { get; init; } = string.Empty;

    public string IdentifierPath { get; init; } = string.Empty;

    public string? Description { get; init; }

    public bool IsOnline { get; init; }

    public string? Value { get; init; }

    public string? ParameterType { get; init; }

    public bool? IsWritable { get; init; }

    public List<EmberFunctionFieldDto>? Arguments { get; init; }

    public List<EmberFunctionFieldDto>? Result { get; init; }

    public List<EmberElementDto>? Children { get; init; }
}

public sealed record EmberFunctionFieldDto
{
    public string Name { get; init; } = string.Empty;

    public string Type { get; init; } = string.Empty;
}
