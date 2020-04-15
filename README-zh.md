## Overview

Shadowsocks-Net是使用C#（.NET Core）开发的跨平台版本的Shadowsocks。

<br/>

## 版本
Shadowsocks-Net计划会有多个发布版本，功能特性比较见下表。

|完成度|版本                           |ss-local        |ss-remote       |local http<sup>[1](#fn_local_http)</sup>   |混淆|规则<br/>过滤|服务器<br/>选择策略     |图形<br/>用户界面     |
|-|-|-|-|-|-|-|-|-|
| 90%    |Minimal-<br/>cross-platform    |√              | √             |√                                         |    |           |                        |                      |
| 10%    |Windows                        |√              |                |√                                         |√  |√         |√                      |√                    |
| 1%     |Linux                          |√              | √             |√                                         |√  |√         |                        |                      |



Minimal版现已可测试，支持的加密算法：

```console
aes-256-gcm, aes-192-gcm, aes-128-gcm, chacha20-ietf-poly1305, xchacha20-ietf-poly1305
```

<br/>




## 开发说明

#### 架构示意图
![arch][shadowsocks_net_arch]

Shadowsocks-Net对网络编程部分做了简单封装，使得上层可以专注socks5协议。
由于Shadowsocks主要实现了socks5协议，因此现在上层的代码很薄。socks5总的来说只做了两件事：1. 协商、2. 转发。Shadowsocks-Net试图让用C#开发Shadowsocks变得更有趣也更简单。

<br/>

_Master分支是经典的Shadowsocks实现。
[pluggable-tunnel](https://github.com/shadowsocks/Shadowsocks-Net/tree/pluggable-tunnel) 分支有一个稍微不同的架构，
提供了集成多路复用隧道的灵活性。_

<br/>

#### 添加加密算法的步骤

1. 实现统一的加密接口`IShadowsocksAeadCipher`或者`IShadowsocksStreamCipher`
```c#
class MyCipher : IShadowsocksAeadCipher
{
    //implementation
}
```

2. 使用`Cipher`特性标记
```c#
[Cipher("my-cipher-name")]
class MyCipher : IShadowsocksAeadCipher
{
    //implementation
}
```
此时`MyCipher`已被识别。

<br/>

#### 对混淆的支持
混淆同加密一样，在Shadowsocks-Net中都是通过管道过滤器来工作的。相对于加密，混淆的逻辑可能更复杂。
但由于其他部分已被封装，现在只需关注网络流的读写，实现自己的过滤器`ClientFilter`即可。
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
Shadowsocks-Net中加密、混淆、对UDP转发的封包都是通过过滤器实现的。过滤器是可插拔模块。所以也可以使用过滤器来解析自定义协议。

下面这个过滤器每次在发送之前向数据开头插入四个字节`0x12, 0x34, 0xAB, 0xCD`，相应地读取时跳过了开头四个字节：
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

##### 创建过滤器的步骤

1. 选择合适的`Category`和`Priority`，它们决定了过滤器在过滤器链中的顺序。框架预置了几个`Category`：
```c#
    public enum ClientFilterCategory
    {
        Obfuscation = 1,
        Cipher = 2,
        Encapsulation = 3,
        Custom = 4
    }
```

2. 继承`ClientFilter`
```c#
    public abstract class ClientFilter
    {
        public abstract ClientFilterResult OnReading(ClientFilterContext filterContext);
        public abstract ClientFilterResult OnWriting(ClientFilterContext filterContext);
    }
```

3. 将过滤器添加至管道
```c#
    DuplexPipe.AddFilter(IClient client, IClientFilter filter);
```

一个典型的例子：[UdpEncapsulationFilter.cs](Shadowsocks-Net/Shadowsocks/Local/UdpEncapsulationFilter.cs)。
<br/>

#### 对TCP或UDP之外协议的支持
由于设计上抽象出了接口，其他通信协议现在也可以集成进来。实现自己的`IClient`和`IServer`即可，无需修改其他部分。
`IClient`和`IServer`也很简单：

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
例如把KCP协议集成进来实现`KcpClient`和`KcpServer`，这时Shadowsocks-Net就使用KCP作为传输层协议。

<br/>

## 编译
#### 编译环境
Visual Studio 2019 Community， .NET Framework 4.6（暂时用来设计winform），.NET Standard 2.1 & .NET Core 3.1。
#### 如何编译
在Visual Studio中生成整个解决方案即可。整个工程目前是100% C#，核心是.NET Standard 2.1的类库。
<br/>
或者使用.NET Core CLI的`dotnet build`命令。发布用`dotnet publish`，发布单独可执行文件使用`dotnet publish -r <RID> -p:PublishSingleFile=true`
<br/>

## Roadmap
#### 任务列表
- ☑ 核心重写
- ☐ Windows端
- ☐ 统一规则过滤器
- ☐ 目标IP、域名过滤
- ☐ Linux端

<br/>



## Usage
与使用其他版本的Shadowsocks类似。Minimal版已在Windows和Debian10 x64上测试，运行参数通过配置文件修改。

以Windows为例：
服务端修改`config.json`后执行`shadowsocks-net-remote.exe`：
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

客户端修改`servers.json`和`app-config.json`后执行`shadowsocks-net-local.exe`：
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
Linux上雷同。 安装[.NET Core传送门](https://dotnet.microsoft.com/download)。
<br/>

## Contribute
还有很多代码等待被添加。
<br/>



<br/>
<br/>

---
<a name="fn_local_http">local http</a>：即本地socks5转http。

[libev版]:https://github.com/shadowsocks/shadowsocks-libev
[shadowsocks-windows]: https://github.com/shadowsocks/shadowsocks-windows
[shadowsocks_net_arch]: https://github.com/shadowsocks/Shadowsocks-Net/blob/master/ssarch.png?