using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Xml;
using System.Xml.Linq;
using OpenAiFineTuning.SpongeBob;

namespace OpenAiFineTuning
{
    public class Program
    {
        public static void Main(string[] args)
        {

            TranscriptLink[]? links = Newtonsoft.Json.JsonConvert.DeserializeObject<TranscriptLink[]>(System.IO.File.ReadAllText(@"C:\Users\timh\Downloads\tah\OpenAI-GPT-Fine-Tuning\data\spongebob\TranscriptLinks.json"));
            if (links != null)
            {
                links[0].GetTranscriptAsync().Wait();
                Console.WriteLine(JsonConvert.SerializeObject(links[0] , Newtonsoft.Json.Formatting.Indented));
            }
            
        }

        public static void AssembleTrainingFromConversations()
        {

            //settings
            string conversations_dir = @"C:\Users\timh\Downloads\tah\conversations";
            string output_file_path = @"C:\Users\timh\Downloads\tah\openai-fine-tuning\output.jsonl";
            int character_limit = 10000; //How many characters a prompt is allowed to go up to


            //The end-product
            JArray TRAINING = new JArray();

            //Assemble
            string[] files = System.IO.Directory.GetFiles(conversations_dir);
            foreach (string file in files)
            {
                Console.WriteLine("Working on '" + System.IO.Path.GetFileName(file) + "... ");
                string content = System.IO.File.ReadAllText(file);
                Message[]? msgs = JsonConvert.DeserializeObject<Message[]>(content);
                if (msgs != null)
                {
                    List<Message> buffer = new List<Message>();
                    foreach (Message msg in msgs)
                    {

                        if (msg.speaker == 1) //If this is a message where Tim responded, use the prompts
                        {
                            if (buffer.Count > 0)
                            {

                                //Remove until we are within the limit
                                while (MessagesToConversationString(buffer.ToArray()).Length > character_limit)
                                {
                                    buffer.RemoveAt(0);
                                }

                                //Add
                                JObject jo = new JObject();
                                jo.Add("prompt", MessagesToConversationString(buffer.ToArray()));
                                jo.Add("completion", "TIM: " + "\n" + msg.body);
                                TRAINING.Add(jo);
                            }
                        }

                        buffer.Add(msg); //Add the message to the buffer
                    }
                }
            }
        
            //Output to file
            Console.WriteLine("Writing... ");
            FileStream fs = System.IO.File.OpenWrite(output_file_path);
            StreamWriter sw = new StreamWriter(fs);
            foreach (JObject jo in TRAINING)
            {
                sw.WriteLine(jo.ToString(Newtonsoft.Json.Formatting.None));
            }
            sw.Close();
            fs.Close();

            Console.WriteLine("Done! Written to " + output_file_path);
        
        }

        private static string MessagesToConversationString(Message[] messages)
        {
            string ToReturn = "";
            foreach (Message msg in messages)
            {
                if (msg.speaker == 0)
                {
                    ToReturn = ToReturn + "PARTNER: " + "\n" + msg.body + "\n\n";
                }
                else
                {
                    ToReturn = ToReturn + "TIM: " + "\n" + msg.body + "\n\n";
                }
            }
            return ToReturn;
        }
    
        public static void AssembleConversationsFromSmsTexts()
        {
            //SETTINGS
            string sms_xml = @"C:\Users\timh\Downloads\tah\SMS backup\20190508\Samsung Galaxy S5 Texts Backup 5-8-2019.xml";
            string conversations_dir = @"C:\Users\timh\Downloads\tah\openai-fine-tuning\conversations";

            //The end product
            Dictionary<string, List<Message>> CONVERSATIONS = new Dictionary<string, List<Message>>();

            //Read if there are any
            string[] files = System.IO.Directory.GetFiles(conversations_dir);
            foreach (string file in files)
            {
                string file_name = System.IO.Path.GetFileNameWithoutExtension(file);
                Console.Write("Retrieving from '" + file_name + "'... ");
                string content = System.IO.File.ReadAllText(file);
                List<Message>? msgs = JsonConvert.DeserializeObject<List<Message>>(content);
                if (msgs != null)
                {
                    CONVERSATIONS.Add(file_name, msgs);
                    Console.WriteLine(msgs.Count.ToString("#,##0"));
                }
            }

            //Read from the target
            int on_number = 1;
            Stream s = System.IO.File.OpenRead(sms_xml);
            StreamReader sr = new StreamReader(s);
            while (true)
            {
                try
                {
                    string? line = sr.ReadLine();
                    if (line != null)
                    {
                        if (line.Trim().StartsWith("<sms "))
                        {
                            Console.WriteLine("Parsing SMS # " + on_number.ToString("#,##0") + "... ");
                            on_number = on_number + 1;

                            XElement x = XElement.Parse(line);

                            //Values we will extract
                            string body = "";
                            string with_number = "";
                            int speaker = -1; //0 = somebody else, 1 = tim
                            DateTimeOffset date = new DateTime();

                            //Get the body
                            XAttribute? xbody = x.Attribute("body");
                            if (xbody != null)
                            {
                                body = xbody.Value;
                            }

                            //Get the with number (address)
                            XAttribute? xwith = x.Attribute("address");
                            if (xwith != null)
                            {
                                with_number = xwith.Value.Replace(" ", "").Replace("-", "").Replace("+", "").Replace("(", "").Replace(")", "");
                            }

                            //Speaker
                            XAttribute? xtype = x.Attribute("type");
                            if (xtype != null)
                            {
                                if (xtype.Value == "1") //This was a text message I received.
                                {
                                    speaker = 0;
                                }
                                else if (xtype.Value == "2") //This ws a text message I sent.
                                {
                                    speaker = 1;
                                }
                            }
                        

                            //Date
                            XAttribute? xdate = x.Attribute("date");
                            if (xdate != null)
                            {
                                date = DateTimeOffset.FromUnixTimeMilliseconds(Convert.ToInt64(xdate.Value));
                            }

                            //Construct
                            Message m = new Message();
                            m.speaker = speaker;
                            m.body = body;
                            m.date = date.DateTime;


                            //Add
                            bool added = false;
                            foreach (KeyValuePair<string, List<Message>> kvp in CONVERSATIONS)
                            {
                                if (added == false)
                                {
                                    if (kvp.Key == with_number)
                                    {
                                        kvp.Value.Add(m);
                                        added = true;
                                    }
                                }
                            }
                            if (added == false)
                            {
                                CONVERSATIONS.Add(with_number, new List<Message>(){m});
                            }
                        
                        
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("FAILURE! " + ex.Message);
                }
            }

            //Sort all by date
            Dictionary<string, List<Message>> CONVERSATIONS_SORTED = new Dictionary<string, List<Message>>();
            foreach (KeyValuePair<string, List<Message>> kvp in CONVERSATIONS)
            {
                Console.WriteLine("Sorting '" + kvp.Key + "'");

                //Resort messages
                List<Message> sorted = new List<Message>();
                while (kvp.Value.Count > 0)
                {
                    Message oldest = kvp.Value[0];
                    foreach (Message m in kvp.Value)
                    {
                        if (m.date < oldest.date)
                        {
                            oldest = m;
                        }
                    }
                    sorted.Add(oldest);
                    kvp.Value.Remove(oldest);
                }

                //Re-add
                CONVERSATIONS_SORTED.Add(kvp.Key, sorted);
            }
            CONVERSATIONS = CONVERSATIONS_SORTED;

            //Write
            foreach (KeyValuePair<string, List<Message>> kvp in CONVERSATIONS)
            {
                string path = Path.Combine(conversations_dir, kvp.Key + ".json");
                if (System.IO.File.Exists(path))
                {
                    Console.WriteLine("Deleting file '" + path + "'");
                    System.IO.File.Delete(path);
                }
                Console.WriteLine("Writing '" + kvp.Key + "'");
                FileStream fs = System.IO.File.Create(path);
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(JsonConvert.SerializeObject(kvp.Value, Newtonsoft.Json.Formatting.Indented));
                sw.Close();
                fs.Close();
            }
        }

        public static void Download()
        {
            string url = "http://www.gilligansisle.com/scripts/script98.html";

            //Get data
            HttpClient hc = new HttpClient();
            HttpRequestMessage req = new HttpRequestMessage();
            req.RequestUri = new Uri(url);
            req.Method = HttpMethod.Get;
            HttpResponseMessage resp = hc.Send(req);
            string web = resp.Content.ReadAsStringAsync().Result;

            
            //Get the content
            int loc1 = web.IndexOf("<pre><B>");
            loc1 = web.IndexOf("<B>", loc1 + 1);
            loc1 = web.IndexOf(">", loc1 + 1);
            string content = web.Substring(loc1 + 1);

            //Get the title
            loc1 = web.IndexOf("<FONT SIZE=+1>");
            loc1 = web.IndexOf(">", loc1 + 1);
            int loc2 = web.IndexOf("<", loc1 + 1);
            string title = web.Substring(loc1 + 1, loc2 - loc1 - 1);
            title = title.Replace("\"", "");
            title = title.Replace("Episode #", "");
            title = title.Replace(", ", " - ");
            title = title.Replace(Environment.NewLine, "");



            //string content = System.IO.File.ReadAllText(@"C:\Users\timh\Downloads\tah\openai-fine-tuning\input.txt");
            string[] lines = content.Split(Environment.NewLine);
            List<Speech> script = new List<Speech>();
            Speech buffer = new Speech();
            foreach (string line in lines)
            {
                if (line.StartsWith("\t\t\t\t"))
                {
                    string character = line.Replace("\t", "").Trim().ToLower();
                    if (character.Contains("(") == false)
                    {
                        if (buffer.Character != string.Empty)
                        {
                            script.Add(buffer);
                        }

                        buffer = new Speech();
                        buffer.Character = character;
                    }
                }
                else if (line.StartsWith("\t\t\t"))
                {
                    string dline = line.Replace("\t", "");
                    dline = dline.Replace("…", "");

                    //Add with a space if needed
                    if (buffer.Dialog.EndsWith(" "))
                    {
                        buffer.Dialog = buffer.Dialog + dline;
                    }
                    else
                    {
                        buffer.Dialog = buffer.Dialog + " " + dline;
                    }
                    
                }
            }


            //Write to file
            string path = System.IO.Path.Combine(@"C:\Users\timh\Downloads\tah\openai-fine-tuning\gilligans_island_scripts\season3", title + ".json");
            FileStream twt = System.IO.File.Create(path);
            StreamWriter sw = new StreamWriter(twt);
            sw.WriteAsync(JsonConvert.SerializeObject(script)).Wait();
            sw.Close();
            twt.Close();
            Console.WriteLine("Wrote to");
            Console.WriteLine(path);
        }
    
        private static Speech[][] GetAllScripts(string top_dir = @"C:\Users\timh\Downloads\tah\openai-fine-tuning\gilligans_island_scripts")
        {
            List<Speech[]> ToReturn = new List<Speech[]>();

            //Get files
            string[] files = System.IO.Directory.GetFiles(top_dir);
            foreach (string file in files)
            {
                string content = System.IO.File.ReadAllText(file);
                Speech[]? scr = Newtonsoft.Json.JsonConvert.DeserializeObject<Speech[]>(content);
                if (scr != null)
                {
                    ToReturn.Add(scr);
                }
            }

            //Get directories
            string[] dirs = System.IO.Directory.GetDirectories(top_dir);
            foreach (string dir in dirs)
            {
                Speech[][] from_dir = GetAllScripts(dir);
                ToReturn.AddRange(from_dir);
            }

            return ToReturn.ToArray();
        }
    
    
    }
}