using System;

namespace OpenAiFineTuning
{
    public class Message
    {
        public int speaker {get; set;}
        public string body {get; set;}

        public Message()
        {
            body = string.Empty;
        }
    }
}