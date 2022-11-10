using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace sample
{
    internal class Comand
    {
        public int Id { get; set; }
        public long Captaine { get; set; }
        public string? Title { get; set; }
        public int Count { get; set; }

        public static async Task Create(string title, long owner)
        {
            string insert = $"INSERT INTO comands (captain,title,count) VALUES({(long)owner},'{title}',1)";
            await SqlController.insert(insert);
        }
        public static async Task<int> Get_id(string title)
        {
            string insert = $"SELECT id FROM comands WHERE title='{title}'";
            return await SqlController.select<int>(insert);
        }
        public static async Task<List<Comand>> GetAllCommands(int param)
        {

            //0 -> @all
            //1 -> not full
            //2 -> full
            
            List<Comand> baseList = await SqlController.GetComands();
            List<Comand> output = new List<Comand>();

            switch (param)
            {
                case 0:
                    output = baseList.Where(comand => comand != null).ToList();
                    break;
                case 1:
                    output = baseList.Where(comand => comand.Count < 3).ToList();

                    break;
                case 2:
                    output = baseList.Where(comand => comand.Count == 3).ToList();

                    break;
            }

            return output;
        }
        public static async Task<int> GetLastCommnadId()
        {
            string select = "SELECT id FROM comands ORDER BY id DESC LIMIT 1";
            return await SqlController.select<int>(select);
        }

    }
}
