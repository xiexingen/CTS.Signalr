using System;

namespace Core.Signalr.Template.Web.Dtos
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
