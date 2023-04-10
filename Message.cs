using System;

namespace OpenAiFineTuning
{
    public class Message
    {
        public int speaker {get; set;} //0 = conversational partner, 1 = tim
        public string body {get; set;}
        public DateTime date {get; set;}

        public Message()
        {
            body = string.Empty;
        }
    }
}