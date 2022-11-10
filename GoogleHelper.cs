using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace sample
{
    internal class GoogleHelper
    {
        private static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        private static readonly string ApplicationName = "baumanbot";

        private static readonly string SpreadSheetId = Strings.Tokens.GogleToken;

        public static readonly List<string> Sheets = new List<string>() { "Участники", "Хотят Баллы" };


        public SheetsService service;
        public GoogleCredential credential;

        public GoogleHelper()
        {
            using (var stream = new FileStream("secretes.json", FileMode.Open, FileAccess.Read))
            {
                this.credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);

            }

            this.service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName
            });
        }

        public async Task<string> InputUser(User user , string where)
        {

            string exp_1 = await SqlController.select<string>($"SELECT exp FROM users WHERE chatId={user.ChatId}");
            string exp_2 = await SqlController.select<string>($"SELECT exp_hack FROM users WHERE chatId={user.ChatId}");

            var range = $"{where}!A:I";
            var valueRange = new ValueRange();
            var objectList = new List<object>() { user.Code, user.Fio, user.Group,user.Comand, user.Contact, user.tg, DateTime.Now.ToShortDateString(),exp_1,exp_2 };
            valueRange.Values = new List<IList<object>> { objectList };

            var appendRequest = service.Spreadsheets.Values.Append(valueRange, SpreadSheetId, range);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;

            var appendResonse = appendRequest.Execute();


            int id = Convert.ToInt32(appendResonse.TableRange.Split('!')[1].Split(':')[1].Remove(0, 1)) + 1;

            return "J" + id.ToString();
        }

        public async Task Update(string where, string exel_id, string value)
        {

            await Task.Run(async () =>
            {
                var range = $"{where}!{exel_id}";

                var valueRange = new ValueRange();


                var objList = new List<object>() { value };
                valueRange.Values = new List<IList<object>> { objList };

                var updateRequest = service.Spreadsheets.Values.Update(valueRange, SpreadSheetId, range);
                updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;

                var updateResponse = await updateRequest.ExecuteAsync();
            });
        }
    }
}
