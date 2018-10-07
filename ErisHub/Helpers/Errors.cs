using System.Collections.Generic;

namespace ErisHub.Helpers
{
    public class Errors
    {
        private readonly List<string> _messages;

        public Errors()
        {
            _messages = new List<string>();
        }

        public void Add(string message)
        {
            _messages.Add(message);
        }

        public string[] Messages => _messages.ToArray();
    }

}
