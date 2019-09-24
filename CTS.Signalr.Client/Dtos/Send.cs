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
            GroupId= string.Empty;
        }
        /// <summary>
        /// 接受消息的组 值为项目Id
        /// </summary>
        public string GroupId { set; get; }

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
}
