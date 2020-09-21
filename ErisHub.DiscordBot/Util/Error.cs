using System;
using System.Collections.Generic;
using System.Text;

namespace ErisHub.DiscordBot.Util
{
    public class Error
    {
        public Error(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}
