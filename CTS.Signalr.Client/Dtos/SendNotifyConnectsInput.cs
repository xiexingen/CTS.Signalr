namespace CTS.Signalr.Client.Dtos
{
    public class SendNotifyConnectsInput
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

        /// <summary>
        ///  内容
        /// </summary>
        public string Content { set;get;}
    }
}
