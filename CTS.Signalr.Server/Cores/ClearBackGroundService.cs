using CTS.Signalr.Server.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CTS.Signalr.Server.Cores
{
    /// <summary>
    /// 后台任务，用户退出清理redis中空的key&以及Group(考录到可能用户频繁刷新的情况，不在signalr中处理)
    /// 也可以通过iis的回收来自动处理(但是如果是self host形式就必行了，所以还是通过后台任务吧)
    /// </summary>
    internal class ClearBackGroundService :IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly SignalrRedisHelper _signalrRedisHelper;
        private readonly IHubContext<NotifyHub> _notifyHub;
        private Timer _timer;

        public ClearBackGroundService(ILogger<ClearBackGroundService> logger, SignalrRedisHelper signalrRedisHelper, IHubContext<NotifyHub> notifyHub)
        {
            _logger = logger;
            _signalrRedisHelper = signalrRedisHelper;
            _notifyHub = notifyHub;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromHours(4));
            return Task.CompletedTask;
        }


        /// <summary>
        /// 处理redis中signalr用户系列为空的键、为空的组等
        /// </summary>
        /// <param name="state"></param>
        private void DoWork(object state)
        {
            if (DateTime.Now.Hour >= 1 && DateTime.Now.Hour <= 5)
            {
                _logger.LogInformation($"-----------------Signalr 站点开始保洁工作({DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")})------------------------");
                // 暴力将redis中signalr相关缓存的清空，同时将signalr中的组移除
                // 得到所有Group，用户列表的字典
                //foreach (var group in _signalrRedisHelper.GetServer().Keys(pattern: $"{SignalrRedisHelper.PREFIXGROUP}*"))
                //{
                //    // var groupUsers=_signalrRedisHelper.GetUsersByGroup(group).Result;
                //}
                _logger.LogInformation("-----------------Signalr 站点开始保洁工作结束------------------------");
            }
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
