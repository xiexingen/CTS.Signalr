using CTS.Signalr.Client.Dtos;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CTS.Signalr.Client.Helpers
{
    public class SignalrHelper
    {
        private readonly IConfiguration _configuration;

        public SignalrHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task PushNotifyAsync(Send send)
        {
            var signalrAddress = _configuration.GetSection("SignalrAddress").Value;
            var sendUrl =$"{signalrAddress}api/Notify/Post";
            var stringContent = new StringContent(JsonConvert.SerializeObject(send), Encoding.UTF8, "application/json");
            await new HttpClient().PostAsync(sendUrl, stringContent);
        }

        public async Task PushNotifyToConnectsAsync(SendToConnects send)
        {
            var signalrAddress = _configuration.GetSection("SignalrAddress").Value;
            var sendUrl = $"{signalrAddress}api/Notify/PostConnects";
            var stringContent = new StringContent(JsonConvert.SerializeObject(send), Encoding.UTF8, "application/json");
            await new HttpClient().PostAsync(sendUrl, stringContent);
        }
    }
}
