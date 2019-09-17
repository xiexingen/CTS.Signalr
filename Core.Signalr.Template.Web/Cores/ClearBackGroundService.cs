using Core.Signalr.Template.Web.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Signalr.Template.Web.Cores
{
    /// <summary>
    /// 后台任务，用户退出清理redis中空的key&以及Group(考录到可能用户频繁刷新的情况，不在signalr中处理)
    /// 也可以通过iis的回收来自动处理(但是如果是self host形式就必行了，所以还是通过后台任务吧)
    /// </summary>
    public class ClearBackGroundService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly SignalrRedisHelper _signalrRedisHelper;
        private readonly IHubContext<NotifyHub> _notifyHub;
        private Timer _timer;

        public ClearBackGroundService(ILogger logger, SignalrRedisHelper signalrRedisHelper, IHubContext<NotifyHub> notifyHub, Timer timer)
        {
            _logger = logger;
            _signalrRedisHelper = signalrRedisHelper;
            _notifyHub = notifyHub;
            _timer = timer;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
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
            if (DateTime.Now.Hour>=1 && DateTime.Now.Hour<=5) {
                _logger.LogInformation("-----------------Signalr 站点开始保洁工作------------------------");
                _signalrRedisHelper.GetDatabase().HashGetAll("_signalr_");



                _logger.LogInformation("-----------------Signalr 站点开始保洁工作结束------------------------");
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            _timer?.Dispose();
        }
    }
}
