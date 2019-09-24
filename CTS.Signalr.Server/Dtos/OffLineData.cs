using MessagePack;

namespace CTS.Signalr.Server.Dtos
{
    /// <summary>
    /// 下线通知
    /// </summary>
    [MessagePackObject(keyAsPropertyName: true)]
    public class OffLineData
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
        /// 是否该用户的最后一个连接
        /// </summary>
        public bool IsLast { set; get; }
    }
}
