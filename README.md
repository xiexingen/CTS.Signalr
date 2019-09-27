# Core-Signalr-Template
基于.netcore 的可以单独的实时推送

## 介绍
ASP.NET Core SignalR 是一个开源代码库，它简化了向应用添加实时 Web 功能的过程。 实时 Web 功能使服务器端代码能够即时将内容推送到客户端。  
SignalR 的适用对象：  
- 需要来自服务器的高频率更新的应用。 例如：游戏、社交网络、投票、拍卖、地图和 GPS 应用。
- 仪表板和监视应用。 示例包括公司仪表板、销售状态即时更新或行程警示。
- 协作应用。 协作应用的示例包括白板应用和团队会议软件。
- 需要通知的应用。 社交网络、电子邮件、聊天、游戏、行程警示以及许多其他应用都使用通知。
SignalR 提供了一个用于创建服务器到客户端远程过程调用（RPC）的 API。 RPC 通过服务器端 .NET Core 代码调用客户端上的 JavaScript 函数。
以下是 ASP.NET Core SignalR 的一些功能：
- 自动管理连接。
- 同时向所有连接的客户端发送消息。 例如，聊天室。
- 将消息发送到特定的客户端或客户端组。
- 扩展以处理增加的流量。

## 业务需求
- 一个人可以开多个tab有多个连接
- 给指定的一个、一批人推送(以User为中心对该用户的所有连接进行推送(浏览器多个tab))
- 给指定的组中某些人推送(群聊)
- 给指定的人某些Connect推送(登录排斥，不允许多台电同时脑登录)

## 改进部分
- 优先使用socket进行通信
- 支持一个用户多个连接
- 使用MessagePack进行传输
- 使用Redis作为底板来支持横向扩展

更多文章可以[查看文章](https://blog.xxgtalk.cn/2019/09/20/dotnetcore/signalr/00-introduct/)

本系列共分为10篇，包括基础知识介绍，项目实战等，目录如下

[.net core 3.0 Signalr - 01 基础篇](/2019/09/21/dotnetcore/signalr/01-base/)   
[.net core 3.0 Signalr - 02 使用强类型的Hub](/2019/09/22/dotnetcore/signalr/02-type-hub/)   
[.net core 3.0 Signalr - 03 使用MessagePack压缩传输内容](/2019/09/29/dotnetcore/signalr/03-message-pack/)   
[.net core 3.0 Signalr - 04 使用Redis做底板来支持横向扩展](/2019/10/01/dotnetcore/signalr/04-redis/)   
[.net core 3.0 Signalr - 05 使用jwt将用户跟signalr关联](/2019/10/02/dotnetcore/signalr/05-jwt/)   
[.net core 3.0 Signalr - 06 业务实现-业务分析](/2019/10/03/dotnetcore/signalr/06-analysis/)   
[.net core 3.0 Signalr - 07 业务实现-服务端 自定义管理组、用户、连接](/2019/10/04/dotnetcore/signalr/07-self-manager/)   
[.net core 3.0 Signalr - 08 业务实现-客户端demo](/2019/10/05/dotnetcore/signalr/08-clientdemo/)   
[.net core 3.0 Signalr - 09 待改进&交流](/2019/10/06/dotnetcore/signalr/09-todo/) 

## Demo源码地址
> https://github.com/xiexingen/CTS.Signalr

## 强烈推荐的参考文档  
> 微软官方文档:https://docs.microsoft.com/zh-CN/aspnet/core/signalr/introduction?view=aspnetcore-3.0  
> 发现写的不错的博客: https://www.cnblogs.com/cgzl/p/9509207.html  
> 发现写的不错的博客:https://www.cnblogs.com/cgzl/p/9515516.html
