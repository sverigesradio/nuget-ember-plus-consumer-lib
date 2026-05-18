using EmberPlusConsumerClassLib.Model;
using EmberPlusTreeWebView.Models;
using EmberPlusTreeWebView.Options;
using Lawo.EmberPlusSharp.Model;
using Lawo.EmberPlusSharp.S101;
using Lawo.Threading.Tasks;
using Microsoft.Extensions.Options;
using System.Net.Sockets;

namespace EmberPlusTreeWebView.Services;

public sealed class EmberTreeSnapshotService
{
    private readonly IOptions<EmberPlusConnectionOptions> options;

    public EmberTreeSnapshotService(IOptions<EmberPlusConnectionOptions> options) => this.options = options;

    public async Task<EmberElementDto> GetTreeAsync(CancellationToken cancellationToken)
    {
        EmberElementDto? root = null;

        await Task.Run(
            () => AsyncPump.Run(async () =>
            {
                var current = this.options.Value;

                using var tcpClient = new TcpClient();
                await tcpClient.ConnectAsync(current.Host, current.Port, cancellationToken);

                using var networkStream = tcpClient.GetStream();
                using var s101Client = new S101Client(tcpClient, networkStream.ReadAsync, networkStream.WriteAsync);
                using var consumer = await Consumer<MyRoot>.CreateAsync(
                    s101Client,
                    current.TimeoutMilliseconds,
                    ChildrenRetrievalPolicy.All);

                await consumer.SendAsync();

                root = MapElement(consumer.Root as IElement
                    ?? throw new InvalidOperationException("Consumer root is not available."));
            }, cancellationToken),
            cancellationToken);

        return root ?? throw new InvalidOperationException("Unable to map Ember+ tree.");
    }

    private static EmberElementDto MapElement(IElement element)
    {
        var baseNode = new EmberElementDto
        {
            Kind = element switch
            {
                INode => "node",
                IParameter => "parameter",
                IFunction => "function",
                _ => "element"
            },
            Number = element.Number,
            Identifier = element.Identifier,
            IdentifierPath = element.IdentifierPath,
            Description = element.Description,
            IsOnline = element.IsOnline
        };

        return element switch
        {
            INode node => baseNode with
            {
                Children = node.Children.Select(MapElement).ToList()
            },
            IParameter parameter => baseNode with
            {
                Value = parameter.Value?.ToString(),
                ParameterType = parameter.Type.ToString(),
                IsWritable = parameter.IsWriteable
            },
            IFunction function => baseNode with
            {
                Arguments = function.Arguments.Select(item => new EmberFunctionFieldDto
                {
                    Name = item.Key,
                    Type = item.Value.ToString()
                }).ToList(),
                Result = function.Result.Select(item => new EmberFunctionFieldDto
                {
                    Name = item.Key,
                    Type = item.Value.ToString()
                }).ToList()
            },
            _ => baseNode
        };
    }
}
