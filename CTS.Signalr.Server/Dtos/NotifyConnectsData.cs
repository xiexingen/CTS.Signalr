using MessagePack;

namespace CTS.Signalr.Server.Dtos
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class NotifyConnectsData
    {
        /// <summary>
        /// 连接Id 多个以,隔开
        /// </summary>
        public string Connects { set; get; }
        public virtual object NotifyObj { set; get; }
    }
}
