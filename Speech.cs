using System;

namespace OpenAiFineTuning
{
    public class Speech
    {
        public string Character {get; set;}
        public string Dialog {get; set;}

        public Speech()
        {
            Character = string.Empty;
            Dialog = string.Empty;
        }
    }
}