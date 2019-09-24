namespace CTS.Signalr.Server.Models
{
    /// <summary>
    /// 对应appsettings中的App节点的配置信息
    /// </summary>
    public class AppSetting
    {
        public JwtSetting JwtSetting { set;get;}
        public RedisCache RedisCache { set;get;}
        public RedisCache SignalrRedisCache { set; get; }
        public string CORS { set;get;}
        /// <summary>
        /// 是否主站点(用于运行清理任务等)
        /// </summary>
        public bool MainSite { set;get;}
    }

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

    public class RedisCache
    {
        public string ConnectionString { set;get;}
        public int DatabaseId { set; get; }
    }
}
