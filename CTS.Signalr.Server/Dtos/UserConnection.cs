using System;

namespace CTS.Signalr.Server.Dtos
{
    [Serializable]
    public class UserConnection
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public string UserId { set;get;}
        /// <summary>
        /// 连接Id
        /// </summary>
        public string ConnectionId { set;get;}
    }
}
