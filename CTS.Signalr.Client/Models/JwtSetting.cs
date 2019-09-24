namespace CTS.Signalr.Client.Models
{
    /// <summary>
    /// JWT设置
    /// </summary>
    public class JwtSetting
    {
        /// <summary>
        /// 发行者 表示token是谁颁发的
        /// </summary>
        public string Issuer { set; get; }
        /// <summary>
        /// 表示哪些客户端可以使用这个token
        /// </summary>
        public string Audience { set; get; }
        /// <summary>
        /// 加密的Key 必须大于16位
        /// </summary>
        public string SecretKey { set; get; }
    }
}
