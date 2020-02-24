## Overview

Shadowsocks-Net是使用C#（.NET Core）开发的Shadowsocks。包含服务器端和客户端，核心功能完整，跨平台。

#### Shadowsocks-Net完成： 
1. [shadowsocks-windows](https://github.com/shadowsocks/shadowsocks-windows) 的工程重构（基本已重写）。
2. [shadowsocks-windows](https://github.com/shadowsocks/shadowsocks-windows) 中核心功能到Linux平台的移植（使用.NET Standard / .NET Core）。

#### Shadowsocks-Net目标：
运用C#语言和.NET平台，开发好玩、易读、易扩展的Shadowsocks。

<br/>

## 版本
Shadowsocks-Net计划有多个发布版本，功能特性比较见下表。

|版本                     |ss-local        |ss-remote       |local http[^1] |混淆|规则过滤|服务器选择策略|图形用户界面|
|-|-|-|-|-|-|-|-|
|Minimal（cross-platform）|√              | √             |               |√  |        |              |            |
|Windows                  |√              |                |√             |√  |√      |√            |√          |
|Linux                    |√              | √             |√             |√  |√      |              |            |

[^1]:local http即本地的http代理服务。

#### 加密算法
Shadowsocks-Net默认提供了：
```console
aes-256-gcm, aes-192-gcm, aes-128-gcm.
```
项目正在重构中，暂未加入更多加密算法，但向Shadowsocks-Net中添加加密算法很方便。



<br/>




## 开发说明

#### 架构示意图
![arch][shadowsocks_net_arch]

Shadowsocks-Net对网络编程部分做了小巧的封装，使得上层可以专注socks5协议。
由于Shadowsocks主要实现了socks5协议，因此现在上层的代码很薄。socks5总的来说只做了两件事：1. 协商、2. 转发。

<br/>

#### 如何添加一个新的加密算法？

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

#### 如何自定义混淆？
混淆同加密一样，在Shadowsocks-Net中都是通过管道过滤器来工作的。相对于加密，混淆的逻辑可能更复杂。
但由于其他部分已被封装，现在只需关注网络流的读写，实现自己的过滤器`IPipeFilter`即可。
```c#
public interface IPipeFilter 
{
    PipeFilterResult BeforeWriting(PipeFilterContext filterContext);
    PipeFilterResult AfterReading(PipeFilterContext filterContext);
}
```
Shadowsocks-Net中加密、混淆、对UDP转发的封包都是通过过滤器实现的。所以也可以使用过滤器来解析自定义协议，实现`IPipeFilter`接口即可添加处理逻辑，不需要阅读全部代码。

<br/>

#### 如何使用TCP或UDP之外的协议？
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

## Roadmap
#### 任务列表
- ☑ 核心重写
- ☐ Windows端功能完善
- ☐ 统一规则过滤器
- ☐ 目标IP、域名过滤
- ☐ Linux端

<br/>


## FAQ
1. 与现有[shadowsocks-windows]有什么不同？
    1. Shadowsocks-Net主要为了完成对[shadowsocks-windows]的重构，程序的重新设计。使之更容易阅读代码，快速上手，方便修改和扩展功能。
    2. Shadowsocks-Net的Windows版界面计划更简洁一些。
2. 客户端与现有的服务端是否兼容？
    1. 兼容。Shadowsocks-Net目前只是在现有实现上增加了混淆支持。
<br/>


## Usage
如果你之前有使用Shadowsocks的经验，Shadowsocks-Net的使用将会十分容易。
这里下载[安装.NET Core](https://dotnet.microsoft.com/download)。

<br/>

## Contribute
还有很多代码可以被添加。
<br/>


*[//]: ## License

<br/>
<br/>

[libev版]:https://github.com/shadowsocks/shadowsocks-libev
[shadowsocks-windows]: https://github.com/shadowsocks/shadowsocks-windows
[shadowsocks_net_arch]:https://github.com/shadowsocks/Shadowsocks-Net/ssarch.png