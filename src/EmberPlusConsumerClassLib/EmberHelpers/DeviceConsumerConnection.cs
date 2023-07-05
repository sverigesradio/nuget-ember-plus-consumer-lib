#region copyright
/*
 * NuGet EmBER+ Consumer Lib
 *
 * Copyright (c) 2023 Roger Sandholm, Stockholm, Sweden
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 * 1. Redistributions of source code must retain the above copyright
 *    notice, this list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright
 *    notice, this list of conditions and the following disclaimer in the
 *    documentation and/or other materials provided with the distribution.
 * 3. The name of the author may not be used to endorse or promote products
 *    derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR
 * IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
 * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
 * IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
 * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 * NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 * DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
 * THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
 * THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
#endregion copyright

using Lawo.EmberPlusSharp.Model;
using System.Net.Sockets;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;
using Lawo.EmberPlusSharp.S101;
using System.Threading;
using Lawo.Threading.Tasks;

namespace EmberPlusConsumerClassLib.EmberHelpers
{
    public class DeviceConsumerConnection<RT> where RT : Root<RT>
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Get information about connection status to the EmBER+ provider.
        /// It's a good idea to verify "Consumer?.Root.IsOnline" is online.
        /// To make sure the root exists.
        /// </summary>
        public event Action<string, bool>? OnConnectionChanged;

        private string _providerHost = "localhost";
        private int _providerPort = 9001;

        public Consumer<RT> Consumer { get; private set; }
        private S101Client _connectionClient;

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public bool IsConnectedToProvider { get; private set; } = false;

        public DeviceConsumerConnection(ILogger logger) {
            _logger = logger;
        }

        /// <summary>
        /// Connect to an EmBER+ provider.
        /// </summary>
        /// <param name="providerHost">host DNS</param>
        /// <param name="providerPort">defaults to 9001</param>
        /// <returns></returns>
        public async Task Connect(string providerHost, int providerPort = 9001)
        {
            _providerHost = providerHost;
            _providerPort = providerPort;

            await Task.Run(() =>
            {
                SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

                AsyncPump.Run(async () =>
                {
                    while (IsConnectedToProvider != true || _cancellationTokenSource.Token.IsCancellationRequested == false)
                    {
                        try
                        {
                            // Initiate connection
                            S101Client client = await S101Extension.CreateClient(_providerHost, _providerPort, _logger);
                            _connectionClient = client;

                            Consumer<RT> consumer = await Consumer<RT>.CreateAsync(client, 10000, ChildrenRetrievalPolicy.DirectOnly);
                            Consumer = consumer;
                            Consumer.ConnectionLost += OnConsumer_ConnectionLost;
                            Consumer.Root.ChildrenRetrievalPolicy = ChildrenRetrievalPolicy.DirectOnly;
                            await Consumer.SendAsync();

                            //_ = (Consumer?.Root.IsOnline);

                            _logger.LogInformation($"Connected to EmBER+ provider on '{_providerHost}:{_providerPort}'");
                            IsConnectedToProvider = true;
                            OnConnectionChanged?.Invoke($"{_providerHost}:{_providerPort}", IsConnectedToProvider);
                            break;

                            // Check identity node for hardware model
                            //var identity = await ParseProductIdentity(consumer.Root, consumer);
                            //_logger.LogDebug($"Setting up connection for product: {identity.Product}");
                            //IMixerProvider provider = await Factory(identity, host, port, consumer);

                        }
                        catch (SocketException ex)
                        {
                            _logger.LogError($"Socket Exception: {ex.Message}");
                        }
                        catch (TaskCanceledException ex)
                        {
                            _logger.LogError($"Unable to connect to EmBER+ provider: {ex.Message}");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Exception when connecting to EmBER+ provider");
                        }

                        _logger.LogDebug("Not connected yet, will try again in 5s");
                        await Task.Delay(5000);
                    }
                }, _cancellationTokenSource.Token);
            }, _cancellationTokenSource.Token);
        }

        public void Disconnect()
        {
            IsConnectedToProvider = false;
            if (Consumer != null)
            {
                Consumer.ConnectionLost -= OnConsumer_ConnectionLost;
                Consumer.Dispose();
            }
            if (_connectionClient != null)
            {
                _connectionClient.Dispose();
            }
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
            }

            OnConnectionChanged?.Invoke($"{_providerHost}:{_providerPort}", IsConnectedToProvider);
            _logger.LogInformation($"Disconnected EmBER+ provider on '{_providerHost}:{_providerPort}'");
        }

        private void OnConsumer_ConnectionLost(object sender, Lawo.IO.ConnectionLostEventArgs e)
        {
            _logger.LogWarning(e.Exception, $"Lost connection with EmBER+ provider on '{_providerHost}:{_providerPort}'");
            IsConnectedToProvider = false;
            OnConnectionChanged?.Invoke($"{_providerHost}:{_providerPort}", IsConnectedToProvider);
        }
    }

}
