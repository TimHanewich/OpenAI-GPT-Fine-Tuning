using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;

namespace OpenAiFineTuning
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Speech[][] scripts = GetAllScripts();
            foreach (Speech[] script in scripts)
            {
                foreach (Speech s in script)
                {
                    if (s.Character == "gilligan")
                    {
                        Console.WriteLine(s.Dialog);
                        Console.WriteLine();
                    }
                }
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