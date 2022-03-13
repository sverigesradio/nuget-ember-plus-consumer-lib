# EmBER+ consumer lib
A library that gives you the possibility to create an EmBER+ consumer.


# How to use?
1. Add the NuGet package to your solution
2. Initiate the EmBER+ consumer
    ```csharp

    // Note that the most-derived subtype MyRoot needs to be passed to the generic base class.
    // Represents the root containing dynamic and optional static elements in the object tree accessible through Consumer<TRoot>.Root
    private class MyRoot : DynamicRoot<MyRoot> { }

    AsyncPump.Run(async () =>
    {
        // Create TCP connection
        var tcpClient = new TcpClient();
        await tcpClient.ConnectAsync("localhost", 9001);

        // Establish S101 protocol
        // S101 provides message packaging, CRC integrity checks and a keep-alive mechanism.
        var stream = tcpClient.GetStream();
        var s101Client = new S101Client(tcpClient, stream.ReadAsync, stream.WriteAsync);

        // Create consumer
        var consumer = await Consumer<MyRoot>.CreateAsync(s101Client);
        await consumer.SendAsync();

        // Navigate down tree until IParameter is reached or desired INode
        var mixer = (consumer.Root as INode).Children.First(c => c.Identifier == "MixerEmberIdentifier");
        var mute = (IParameter)mixer.Children.First(c => c.Identifier == "Mute");

        mute.Value = true;
    });
    ```
3. Rock on with your creations

# Soluton contains:
- Lawo - Common shared classes (Required by main library) - .NET Standard 2.1, .NET Framework 4.5, .NET Framework 4.8
- Lawo.EmberPlusSharp - The EmBER+ Consumer main library - .NET Standard 2.1, .NET Framework 4.5, .NET Framework 4.8

- Lawo.GlowAnalyzerProxy.Main - An EmBER+ analyzer tool for Glow debugging - WPF, .NET Framework 4.5
- Lawo.GlowLogConverter.Main - Converts the log format received from the Glow Analyzer Proxy - WPF, .NET Framework 4.5


License
=======
The consumer is a library 'ember-plus-sharp' from Lawo GmbH.
```
LawoEmberPlusSharp.net -- .NET implementation of the Ember+ Protocol
Copyright (c) 2012-2019 Lawo GmbH (http://www.lawo.com).
Distributed under the Boost Software License, Version 1.0.
```
There has been some modifications to the source code for .NET Standard adaption.


## Responsible maintainer
- [Team Unicorn](mailto:teamunicorn@sr.se)


## Build
![Workflow](https://github.com/sverigesradio/nuget-ember-plus-consumer-lib/workflows/ContinuousIntegration%20Release%20NuGet/badge.svg)