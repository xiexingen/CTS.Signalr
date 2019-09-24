using System.ComponentModel.DataAnnotations;

namespace CTS.Signalr.Client.Dtos
{
    public class SendMessageInput
    {
        public string SelectGroups { set;get;}
        public string SelectUsers { set; get; }
        [Required]
        public string Message { set;get;}
    }
}

