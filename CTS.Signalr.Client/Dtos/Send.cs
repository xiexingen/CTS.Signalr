using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTS.Signalr.Client.Dtos
{
    /// <summary>
    /// 要推送的实体对象
    /// </summary>
    public class Send
    {
        public Send()
        {
            UserIds = string.Empty;
            GroupIds= string.Empty;
        }
        /// <summary>
        /// 接受消息的组 值为项目Id
        /// </summary>
        public string GroupIds { set; get; }

        /// <summary>
        /// 用户Id列表
        /// </summary>
        public string UserIds { set; get; }

        /// <summary>
        /// 是否排除指定用户
        /// </summary>
        public bool ExcludeUsers { set; get; }

        public virtual object NotifyObj { set; get; }
    }

    /// <summary>
    /// 推送给指定连接
    /// </summary>
    public class SendToConnects
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public string UserId { set; get; }
        /// <summary>
        /// 连接Id
        /// </summary>
        public string ConnectionId { set; get; }
        /// <summary>
        /// 是否排除指定连接,当且仅当UserId有值的情况才有效
        /// </summary>
        public bool ExcludeConnectId { set; get; }
        public virtual object NotifyObj { set; get; }
    }
}
