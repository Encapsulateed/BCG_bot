using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace sample
{
    internal class User
    {
        public long ChatId { get; set; }
        public int Code { get; set; }
        
        public string? CommandLine { get; set; }
        public string? ExelId { get; set; }
        public string University { get; set; }
        public string Fio { get; set; }
        public string Group { get; set; }
        public string Birth { get; set; }
        public string Contact { get; set; }
        public string tg { get; set; }
        public string Comand { get; set; }
        public string Exp { get; set; }
        public string exp_hack { get; set; }

        public bool IsAdmin { get; set; }
        public bool IsRegEnd { get; set; }
        public bool NeedScores { get; set; }

        public static async Task Registartion(long chatId,string tgLink)
        {
            string insert = $"INSERT IGNORE INTO users (chatId,CommandLine,tgLink) VALUES({(long)chatId},'{"Input_Fio"}','{tgLink}')";
            await SqlController.insert(insert);
        }

        public async Task Update<T>(string param, T value, string where, long key)
        {
            string update = null;

            if (value is string)
                update = $"UPDATE users SET {param}='{value}' WHERE {where}={key}";
            else
                update = $"UPDATE users SET {param}={value} WHERE {where}={key}";
            if (update != null)
                await SqlController.Update(update);
        }

        public static async Task<User> GetUserByChatId(long chatId)
        {
            return await SqlController.GetUser(chatId);
        }
        public static async Task<User> GetUserByCode(int code)
        {
            return await SqlController.GetUserCode(code);
        }
        public static async Task<List<User>> GetUsers(int param)
        {
            List<User> baseList = new List<User>();
            List<User> output  = new List<User>();
            // param 0 -> all users
            // param 1 -> not registered
            // param 2 -> fv
            // param 3 -> registered

            baseList = await SqlController.GetUsers();

            //for (int i = 0; i < 500; i++)
            //{
            //    try
            //    {
            //        User get = await GetUserByCode(i);
            //        if (get != null)
            //            baseList.Add(get);

            //    }
            //    catch (Exception)
            //    {

            //    }
            //}

            try
            {
                if (param == 0)
                {
                    output = (List<User>)((from user in baseList where user != null select user).ToList());
                }
                else if (param == 1)
                {
                    output = (List<User>)((from user in baseList where user.IsRegEnd == false select user).ToList());
                }
                else if (param == 2)
                {
                    output = (List<User>)((from user in baseList where user.NeedScores == true select user).ToList());
                }
                else if (param == 3)
                {
                    output = (List<User>)((from user in baseList where user.IsRegEnd == true select user).ToList());
                }

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
            }
            
            return output;
        }

        public string makeCodeString()
        {
            if (this.Code < 10)
            {
                return "00" + this.Code.ToString();
            }
            else if (this.Code >= 10 && this.Code < 100)
            {
               return "0" + this.Code.ToString();
            }
            return this.Code.ToString();
        }
    }
}
