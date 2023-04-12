using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace OpenAiFineTuning.SpongeBob
{
    public class Episode
    {
        public string Number {get; set;}
        public string Title {get; set;}
        public string TranscriptUrl {get; set;}
        public string[]? Transcript {get; set;}

        public Episode()
        {
            Number = string.Empty;
            Title = string.Empty;
            TranscriptUrl = string.Empty;
            Transcript = null;
        }

        public static async Task<Episode[]> GetAllEpisodesAsync()
        {
            HttpClient hc = new HttpClient();
            HttpResponseMessage response = await hc.GetAsync("https://spongebob.fandom.com/wiki/List_of_transcripts");
            string content = await response.Content.ReadAsStringAsync();

            //Get each data point in each table
            List<Episode> ToReturn = new List<Episode>();
            string[] parts = content.Split(new string[]{"<table "}, StringSplitOptions.RemoveEmptyEntries);
            for (int t = 1; t < parts.Length; t++)
            {
                string part = parts[t];
                if (part.StartsWith("class=\"wikitable"))
                {
                    string[] rows = part.Split(new string[]{"<tr"}, StringSplitOptions.RemoveEmptyEntries);
                    for (int r = 2; r < rows.Length; r++) //start with two so we skip the first row (headers)
                    {

                        string row = rows[r];
                        Episode tl = new Episode();

                        //Get episode number
                        int loc1 = row.IndexOf("<center");
                        loc1 = row.IndexOf(">", loc1 + 1);
                        int loc2 = row.IndexOf("<", loc1 + 1);
                        tl.Number = row.Substring(loc1 + 1, loc2 - loc1 - 1);

                        //Get title
                        loc1 = row.IndexOf("<a href");
                        loc1 = row.IndexOf(">", loc1 +1);
                        loc2 = row.IndexOf("<", loc1 + 1);
                        tl.Title = row.Substring(loc1 + 1, loc2 - loc1 -1);

                        //Get link
                        if (row.Contains("data-uncrawlable-url") == false)
                        {
                            loc1 = row.IndexOf("<center");
                            loc1 = row.IndexOf("<center", loc1 + 1);
                            loc1 = row.IndexOf("<a href", loc1 + 1);
                            loc1 = row.IndexOf("\"", loc1 + 1);
                            loc2 = row.IndexOf("\"", loc1 + 1);
                            tl.TranscriptUrl = row.Substring(loc1 + 1, loc2 - loc1 - 1);
                            tl.TranscriptUrl = "https://spongebob.fandom.com" + tl.TranscriptUrl;

                            ToReturn.Add(tl);
                        }
                    }
                }
            }

            return ToReturn.ToArray();
        }
    
        public async Task GetTranscriptAsync()
        {
            HttpClient hc = new HttpClient();
            HttpResponseMessage response = await hc.GetAsync(TranscriptUrl);
            string content = await response.Content.ReadAsStringAsync();

            //Select which ul has the highest number of list items - because this is likely what we want
            string focus_ul = "";
            int winning_li_count = int.MinValue;
            string[] uls = content.Split(new string[]{"<ul>"}, StringSplitOptions.RemoveEmptyEntries);
            for (int t = 1; t < uls.Length; t++)
            {
                string[] lis2 = uls[t].Split(new string[]{"<li>"}, StringSplitOptions.RemoveEmptyEntries);
                if (lis2.Length > winning_li_count)
                {
                    focus_ul = uls[t];
                    winning_li_count = lis2.Length;
                }
            }


            List<string> ToReturn = new List<string>();
            string[] lis = focus_ul.Split(new string[]{"<li>"}, StringSplitOptions.RemoveEmptyEntries);
            for (int t = 1; t < lis.Length; t++)
            {
                string li = lis[t];
                li = StripHTML(li);
                li = li.Replace("\n", "");
                li = li.Replace("\t", "");

                //Trim out the nonsense
                if (li.Contains("<!-- NewPP"))
                {
                    int loc1 = li.IndexOf("<!-- NewPP");
                    li = li.Substring(0, loc1);
                    ToReturn.Add(li);
                    break;
                }
                else
                {
                    ToReturn.Add(li);
                }
            }

            Transcript = ToReturn.ToArray();
        }

        private string StripHTML(string input)
        {
            return Regex.Replace(input, "<[a-zA-Z/].*?>", String.Empty);
        }
    }
}