## Overview

Shadowsocks-Net is a cross-platform version of Shadowsocks developed in C# (.NET Core).
<br/>

[中文](https://github.com/shadowsocks/Shadowsocks-Net/blob/master/README-zh.md)

## Version
Shadowsocks-Net plans to release multiple versions, the feature comparison is shown in the table below.

|Completion<br/>of<br/>development|Version                        |ss-local        |ss-remote       |Local<br/>HTTP<sup>[1](#fn_local_http)</sup>|Obfuscation|URL/IP<br/>filtering|Server<br/>scheduling<br/>strategy|GUI|
|-|-|-|-|-|-|-|-|-|
| 90%                             |Minimal-<br/>cross-platform    |√              | √             |√                                          |           |                    |                                  |   |
| 10%                             |Windows                        |√              |                |√                                          |√         |√                  |√                                |√ |
| 1%                              |Linux                          |√              | √             |√                                          |√         |√                  |                                  |   |



The Minimal vesion is available for testing now, supported encryption algorithms:

```console
aes-256-gcm, aes-192-gcm, aes-128-gcm, chacha20-ietf-poly1305, xchacha20-ietf-poly1305
```

<br/>




## Development Instructions

#### Schematic diagram of architecture
![arch][shadowsocks_net_arch]

Shadowsocks-Net encapsulates the network programming part simply so that the upper layer can focus on the socks5 protocol.
Since Shadowsocks mainly implements socks5 protocol, the upper layer code is now very thin. 
 Socks5 generally only do two things: 1. negotiation, 2. forwarding. Shadowsocks-Net tries to make developing Shadowsocks in C# more enjoyable and easier.
<br/>

_Master branch is a classical implementation of Shadowsocks. 
The [pluggable-tunnel](https://github.com/shadowsocks/Shadowsocks-Net/tree/pluggable-tunnel) branch has a slightly different architecture,
which gives flexibility to integrate multiplexed tunnels._

<br/>

#### Steps to add encryption algorithm

1. Implement the unified encryption interface `IShadowsocksAeadCipher` or `IShadowsocksStreamCipher`
```c#
class MyCipher : IShadowsocksAeadCipher
{
    //implementation
}
```

2. Mark with `Cipher` attribute
```c#
[Cipher("my-cipher-name")]
class MyCipher : IShadowsocksAeadCipher
{
    //implementation
}
```
`MyCipher` is recognized now.

<br/>

#### Obfuscation support
Obfuscation is similar to encryption, in Shadowsocks-Net, it works as a filter. The logic of obfuscation can be more complicated than encryption.
But as the other parts have been encapsulated, now only need to focus on reading and writing the network stream, and implement a `ClientFilter`.
```c#
public interface IClientReaderFilter
{
    ClientFilterResult OnReading(ClientFilterContext filterContext);
}
```
```c#
public interface IClientWriterFilter
{
    ClientFilterResult OnWriting(ClientFilterContext filterContext);        
}
```
Encryption, obfuscation and UDP encapsulation are all implemented by filters in Shadowsocks-Net. Filters are pluggable. Therefore, filters can also be used to interpret custom protocols.

The following filter inserts four bytes `0x12`, `0x34`, `0xAB`, `0xCD` into the beginning of the data each time before sending, 
and correspondingly skips the first four bytes when receiving:
```c#
class TestClientFilter : ClientFilter
{
    public override ClientFilterResult OnWriting(ClientFilterContext ctx)
    {
        byte[] data = ctx.Memory.ToArray();
        byte[] newData = new byte[data.Length + 4];
        newData[0] = 0x12;
        newData[1] = 0x34;
        newData[2] = 0xAB;
        newData[3] = 0xCD;
        Array.Copy(data, 0, newData, 4, data.Length);
        return new ClientFilterResult(ctx.Client, newData, ...);
    }

    public override ClientFilterResult OnReading(ClientFilterContext ctx)
    {
        byte[] data = ctx.Memory.ToArray();
        byte[] newData = data.Skip(4).ToArray();
        return new ClientFilterResult(ctx.Client, newData, ...);
    }
}
```
<br/>

##### Steps to create filter

1. Choose the proper `Category` and `Priority`, which determine the order of the filter in the filter chain. 
The framework has preseted several categories:
```c#
    public enum ClientFilterCategory
    {
        Obfuscation = 1,
        Cipher = 2,
        Encapsulation = 3,
        Custom = 4
    }
```

2. Inherit `ClientFilter`
```c#
    public abstract class ClientFilter
    {
        public abstract ClientFilterResult OnReading(ClientFilterContext filterContext);
        public abstract ClientFilterResult OnWriting(ClientFilterContext filterContext);
    }
```

3. Add filter to pipe
```c#
    DuplexPipe.AddFilter(IClient client, IClientFilter filter);
```

A typical case: [UdpEncapsulationFilter.cs](Shadowsocks-Net/Shadowsocks/Local/UdpEncapsulationFilter.cs).
<br/>
<br/>

#### Support for protocols other than TCP or UDP
As interfaces have already been abstracted by design, other communication protocols can now also be integrated. 
Just implement `IClient` and `IServer`, no need to change other parts.

`IClient` and `IServer` are also simple:

```c#
public partial interface IClient : IPeer
{        
    ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default);
    ValueTask<int> WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default);
    
    IPEndPoint LocalEndPoint { get; }
    void Close();
    event EventHandler<ClientEventArgs> Closing;
}
```
```c#
public interface IServer : IPeer
{
    void Listen();
    void StopListen();
}
public interface IServer<TClient> : IServer
    where TClient : IClient
{
    Task<TClient> Accept();       
}
```
For instance, in order to integrate the [KCP] protocol, we implemented `KcpClient` and `KcpServer`, 
then Shadowsocks-Net uses KCP as the transport layer protocol.
<br/>

## Compile
#### Environment and dependence
Visual Studio 2019 Community, .NET Framework 4.6 (temporarily used to design winform), .NET Standard 2.1 & .NET Core 3.1.
#### How to compile
Simply build the entire solution in Visual Studio. 
<br/>This project is currently written 100% in C#, the core is .NET Standard 2.1 class library.
<br/>

<br/>

## Roadmap
#### Task list
- ☑ Core rewrite
- ☐ Windows end
- ☐ Unified filter rule
- ☐ Target IP, domain check
- ☐ Linux version

<br/>



## Usage
Similar to using other versions of Shadowsocks.<br/>
The Minimal version has been tested on Windows and Debian10 x64, the parameters are configured through configuration file.

Take an example on Windows:<br/>
On server side, edit `config.json`, run `shadowsocks-net-remote.exe`:
```json
{
  //"server_host": null,
  "server_port": 6666,
  "use_ipv6": false,
  "timeout": 5,
  "password": "password1",
  "method": "aes-128-gcm"
}
```

<br/>

On local side, edit `servers.json` and `app-config.json`, run `shadowsocks-net-local.exe`:
```json
[
  {
    "remarks": "Test Server",
    "server": "10.10.10.102",
    "server_port": 6666,
    "password": "password1",
    "method": "aes-128-gcm",
    "obfs": null,
    "category": null
  }
]
```
```json
{
 "Socks5Proxy": {
    "Port": 2080,
    "UseIPv6Address": false,
    "UseLoopbackAddress": true
  },
  "HttpProxy": {
    "Port": 8080,
    "UseIPv6Address": false,
    "UseLoopbackAddress": true
  }
}

```
Resemble on Linux. Installing [.NET Core Portal.](https://dotnet.microsoft.com/download)
<br/>

## Contribute
There is still a lot of code waiting to be added.
<br/>



<br/>
<br/>

---
<a name="fn_local_http">Local HTTP</a>: Transform socks5 into HTTP.

[KCP]:https://github.com/skywind3000/kcp
[libev版]:https://github.com/shadowsocks/shadowsocks-libev
[shadowsocks-windows]: https://github.com/shadowsocks/shadowsocks-windows
[shadowsocks_net_arch]: https://github.com/shadowsocks/Shadowsocks-Net/blob/master/ssarch.png