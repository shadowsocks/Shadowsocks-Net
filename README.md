## Pluggable-tunnel branch

Coming soon...


This branch considers Shadowsocks into three parts: the local part of the proxy server (ss-local), the remote part of the proxy server (ss-remote), and the secure tunnel between them.

ss-local provides SOCKS5 proxy service to clients, ss-remote dose forwarding / relaying, the secure tunnel is responsible for the secure communication between ss-local and ss-remote:
<center>

![arch][shadowsocks_net_arch_pt]
</center>

More detailed architecture:
<center>

![arch][shadowsocks_net_arch]
</center>
<br/>

#### How to create a tunnel

1. Implement `ITunnelLocal` and `ITunnelRemote`
```c#
public interface ITunnelLocal
{
    Task<IClient> ConnectTcp();
    Task<IClient> ConnectUdp();
}
```
```c#
public interface ITunnelRemote
{
    Task<IClient> AcceptTcp();
    Task<IClient> AcceptUdp();
}
```

`ITunnelLocal` is consumed by ss-local, and `ITunnelRemote` is consumed by ss-remote.

2. Mark with `Tunnel` attribute
```c#
[Tunnel('my-tunnel-name')]
public MyTunnelLocal : ITunnelLocal
{
    //implementation, configuration, etc.
}
```
```c#
[Tunnel('my-tunnel-name')]
public MyTunnelRemote : ITunnelRemote
{
    //implementation, configuration, etc.
}
```
Now ss-local and ss-remote are able to communicate through `my-tunnel-name`.
<br/>


[shadowsocks_net_arch]: https://github.com/shadowsocks/Shadowsocks-Net/blob/pluggable-tunnel/ssarch.png
[shadowsocks_net_arch_pt]: https://github.com/shadowsocks/Shadowsocks-Net/blob/pluggable-tunnel/ssarchpt.png