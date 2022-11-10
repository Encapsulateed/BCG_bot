using Org.BouncyCastle.Math.EC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using static System.Formats.Asn1.AsnWriter;

namespace sample
{
    internal class Bot
    {

        private static string token = Strings.Tokens.BotToken;

        private static TelegramBotClient bot = new TelegramBotClient(token);
        private static GoogleHelper googleHelper = new GoogleHelper();

        private static bool isFioValid(string Fio)
        {
            if (Fio.Contains("'\'") || Fio.Contains("/") || Fio.Contains("'"))
                return false;
            if (Fio.Split(' ').Length == 2 || Fio.Split(' ').Length == 3)
            {
                foreach (char el in Fio)
                {
                    if (el != ' ')
                    {
                        if (((int)el >= 97 && (int)el <= 122))
                        {
                            return false;
                        }
                    }

                }
            }
            else
                return false;

            return true;
        }

        private static bool isGroupValid(string group)
        {
            if (group.Contains("'\'") || group.Contains("/") || group.Contains("'"))
                return false;
            try
            {
                string[] parts = group.Split('-');
                if (parts.Length == 2)
                {
                    if (parts[0].Length == 0 && parts[0].Length > 3)
                    {
                        return false;
                    }
                    if (parts[1].Length >= 2)
                    {
                        if (!char.IsDigit(parts[1][0]) && !char.IsDigit(parts[1][1]))
                            return false;
                    }
                    else
                        return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private static bool isContactValid(string contact)
        {
            if (contact.Contains("'\'") || contact.Contains("/") || contact.Contains("'"))
                return false;
            if (contact.Length == 11)
            {
                if (contact.StartsWith("8"))
                {
                    for (int i = 1; i < contact.Length; i++)
                    {
                        if (!char.IsDigit(contact[i]))
                            return false;
                    }
                }
                else
                {
                    return false;
                }

            }
            else
                return false;

            return true;
        }

        private static bool IsDateValid(string date)
        {
            try
            {
                DateTime output = Convert.ToDateTime(date);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        private static bool IsAdult(DateTime birht)
        {
            return ((DateTime.Now - birht).TotalDays / 365) >= 18;
        }

        public static int ConvertCodeToInt(string code)
        {

            string code_no_litres = null;
            for (int i = 0; i < code.Length; i++)
            {
                if (char.IsDigit(code[i]))
                    code_no_litres += code[i];
            }

            return Convert.ToInt32(code_no_litres);
        }

        private static void Logger(Update update)
        {
            Console.WriteLine(DateTime.Now.ToShortTimeString());
            Console.WriteLine($"{update.Type}\n");
            if (update.Type == UpdateType.Message)
            {
                if (string.IsNullOrEmpty(update.Message.Text) == false)
                    Console.WriteLine($"Message: {update.Message.Text}\nFrom: {update.Message.From.Id}");

            }
            else
            {
                Console.WriteLine($"Querry: {update.CallbackQuery.Data}\nText: {update.CallbackQuery.Message.Text}\nFrom: {update.CallbackQuery.From.Id}");
            }
            Console.WriteLine();
        }

        private static async Task EndReg(long chatId)
        {

            User activeUser = await User.GetUserByChatId(chatId);
            InlineKeyboardMarkup keyboard;
            string data;


            keyboard = Buttons.RegEndBmstu;


            data = $"🔹 ФИО: {activeUser.Fio}\n🔹 Моб.тел: {activeUser.Contact}\n🔹 Учебная группа: {activeUser.Group}\n" +
                $"🔹 Команда: {activeUser.Comand}";
   


            await bot.SendTextMessageAsync(chatId, $"Анкета подошла к концу. Проверка персональных данных.\n\n{data}\n\nВсё правильно? Нажимай на зелёную кнопку и приходи на Bauman Code Games" +
                                                    "", ParseMode.Markdown, replyMarkup: keyboard);

            await activeUser.Update("CommandLine", "", "chatId", chatId);

        }

        private static async Task UserCommandLineHandler(User activeUser, string message_text)
        {
            string commandLine = activeUser.CommandLine;


            if (commandLine.Contains("Input"))
            {
                if (commandLine == "Input_Fio")
                {
                    if (isFioValid(message_text))
                    {
                        await activeUser.Update("Fio", message_text, "chatId", activeUser.ChatId);
                        await activeUser.Update("CommandLine", "Input_Group", "chatId", activeUser.ChatId);

                        await bot.SendTextMessageAsync(activeUser.ChatId, Strings.Messages.AskGroup, replyMarkup: Buttons.BackToInputFio);
                        //await bot.SendTextMessageAsync(activeUser.ChatId, Strings.Messages.IsBmst, replyMarkup: Buttons.IsBmstuKeyBoard);
                    }
                    else
                    {
                        await bot.SendTextMessageAsync(activeUser.ChatId, Strings.Messages.FioError);
                    }
                }
                else if (commandLine == "Input_Group")
                {
                    if (isGroupValid(message_text))
                    {
                        await bot.SendTextMessageAsync(activeUser.ChatId, Strings.Messages.AskContact, replyMarkup: Buttons.BackToInputGroup);

                        await activeUser.Update(param: "univer_group", message_text, "chatId", activeUser.ChatId);
                        await activeUser.Update("CommandLine", "Input_Contact", "chatId", activeUser.ChatId);
                    }
                    else
                    {
                        await bot.SendTextMessageAsync(activeUser.ChatId, Strings.Messages.GroupError);

                    }
                }
                else if (commandLine == "Input_Contact")
                {
                    if (isContactValid(message_text))
                    {
                        await activeUser.Update("Contact", message_text, "chatId", activeUser.ChatId);

                        await activeUser.Update("CommandLine", "Input_exp", "chatId", activeUser.ChatId);
                        await bot.SendTextMessageAsync(activeUser.ChatId, Strings.Messages.Exp_ask);
                        // 

                    }
                    else
                    {
                        await bot.SendTextMessageAsync(activeUser.ChatId, Strings.Messages.ContactError);
                    }
                }
                else if (commandLine == "Input_Title")
                {
                    try
                    {
                        await Comand.Create(message_text, activeUser.ChatId);

                        int comandId = await Comand.Get_id(message_text);
                        await activeUser.Update("comand", comandId, "ChatId", activeUser.ChatId);

             
                        await bot.SendTextMessageAsync(activeUser.ChatId, Strings.Messages.CommandInfo);
                        await EndReg(activeUser.ChatId);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        await bot.SendTextMessageAsync(activeUser.ChatId, Strings.Messages.CommandAlreadyExists);
                    }
                }
                else if (commandLine == "Input_exp")
                {
                    await activeUser.Update("exp", message_text, "chatId", activeUser.ChatId);

                    await activeUser.Update("CommandLine", "Input_Exp_hack", "chatId", activeUser.ChatId);
                    await bot.SendTextMessageAsync(activeUser.ChatId, Strings.Messages.Exp_hack_ask);
                }
                else if (commandLine == "Input_Exp_hack") 
                {
                    await activeUser.Update("exp_hack", message_text, "chatId", activeUser.ChatId);

                    await bot.SendTextMessageAsync(activeUser.ChatId, Strings.Messages.isCapitane, replyMarkup: Buttons.IsCaptineKeyBoard);
                }
            }
            else if (commandLine.Contains("Change"))
            {
                if (commandLine == "Change_Fio")
                {
                    if (isFioValid(message_text))
                    {
                        await activeUser.Update("Fio", message_text, "chatId", activeUser.ChatId);
                        await activeUser.Update("CommandLine", "_", "chatId", activeUser.ChatId);
                        await EndReg(activeUser.ChatId);
                    }
                    else
                    {
                        await bot.SendTextMessageAsync(activeUser.ChatId, Strings.Messages.FioError);


                    }
                }
                else if (commandLine == "Change_Group")
                {
                    if (isGroupValid(message_text))
                    {
                        await activeUser.Update("univer_group", message_text, "chatId", activeUser.ChatId);
                        await activeUser.Update("CommandLine", "_", "chatId", activeUser.ChatId);
                        await EndReg(activeUser.ChatId);
                    }
                    else
                    {
                        await bot.SendTextMessageAsync(activeUser.ChatId, Strings.Messages.GroupError);

                    }
                }
                else if (commandLine == "Change_Contact")
                {
                    if (isContactValid(message_text))
                    {
                        await activeUser.Update("Contact", message_text, "chatId", activeUser.ChatId);
                        await activeUser.Update("CommandLine", "_", "chatId", activeUser.ChatId);
                        await EndReg(activeUser.ChatId);
                    }
                    else
                    {
                        await bot.SendTextMessageAsync(activeUser.ChatId, Strings.Messages.ContactError);

                    }
                }
            }
        }
        private static async Task AdminCommandLineHandler(Admin activeAdmin,string message_text)
        {
            string commandLine = activeAdmin.commandLine;

            if(commandLine == "Input_Code")
            {
                int code = ConvertCodeToInt(message_text);

                User get_user = await User.GetUserByCode(code);
                if(get_user != null)
                {
                    InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Да",$"SendUserToXls {get_user.ExelId} {get_user.NeedScores} {get_user.Code}")
                        },
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Нет","repete_input_code")

                        }
                    });
                    await bot.SendTextMessageAsync(activeAdmin.chatId, $"Убедись пожалуйста, что это он {get_user.Fio}",replyMarkup: keyboard);
                }
                else
                {
                    await bot.SendTextMessageAsync(activeAdmin.chatId, "Ой, кажется такого пользователя не существует, попробуй ещё раз");
                }
            }
            else if (commandLine.Contains("SendMessage"))
            {
                int mess_type = ConvertCodeToInt(commandLine.Split(' ')[1]);
                int param = Convert.ToInt32(commandLine.Split(' ')[2]);
                 


                await bot.SendTextMessageAsync(activeAdmin.chatId, "Рассылка начата");
                await bot.SendTextMessageAsync(activeAdmin.chatId, "Админ-панель", replyMarkup: Buttons.MainAdminKeyBoard);

                //mess_type == 0 -> users
                //mess_type == 1 -> comands 

                if (mess_type == 0)
                {

                    List<User> users = await User.GetUsers(param);
                    foreach (var user in users)
                    {
                        try
                        {
                            await Task.Run(async () =>
                            {
                                await bot.SendTextMessageAsync(user.ChatId, message_text);
                                await Task.Delay(50);
                            });

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }

                    }
                }
                else if(mess_type == 1)
                {
                    List<Comand> comands = await Comand.GetAllCommands(param);

                    foreach(var comand in comands)
                    {
                        await Task.Run(async () =>
                        {
                          //  Console.WriteLine($"send to {comand.Captaine}");
                            await Task.Delay(50);

                            await bot.SendTextMessageAsync(comand.Captaine, message_text);
                        });
                    }
                }

                await bot.SendTextMessageAsync(activeAdmin.chatId, "Рассылка Закончена");
            }

        }
        public static async Task Start()
        {

            Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);

            while (true)
            {
                try
                {
                    var cancellationToken = CancellationToken.None;
                    var receiverOptions = new ReceiverOptions
                    {
                        AllowedUpdates = { }, // receive all update types
                    };

                    var updateReceiver = new QueuedUpdateReceiver(bot, receiverOptions);



                    try
                    {
                        await foreach (Update update in updateReceiver.WithCancellation(cancellationToken))
                        {

                            _ = Task.Run(() =>
                            {
                                try
                                {
                                    _ = HandleUpdateAsync(update);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex);
                                }

                                return Task.CompletedTask;
                            });

                        }
                    }
                    catch (OperationCanceledException exception)
                    {
                        Console.WriteLine(exception);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    await Start();
                }
            }
        }

        private static async Task HandleUpdateAsync( Update update)
        {
            if (update != null)
            {
                Logger(update);


                if (update.Type == UpdateType.Message)
                {

                    Message message = update.Message;

                    if (message.Type == MessageType.Text)
                    {
                        await TextHandler(message);
                    }

                }
                else if (update.Type == UpdateType.CallbackQuery)
                {

                    await CallBackHandler(update);

                }
            }
        }
        private static Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

        private static async Task TextHandler(Message message)
        {

            User activeUser = null;
            Admin activeAdmin = null;

            long chatId = message.From.Id;
            string message_text = message.Text;

            await bot.SendChatActionAsync(chatId, ChatAction.Typing);


            if (activeUser == null)
            {
                try
                {
                    activeUser = await User.GetUserByChatId(chatId);
                }
                catch (Exception ex)
                {
                }

            }
            if (activeAdmin == null)
            {
                try
                {
                    activeAdmin = await Admin.GetAdminByChatId(chatId);

                }
                catch (Exception)
                {


                }
            }



            if (message_text.Contains("start"))
            {

                if (message_text == "/start")
                {
                    await bot.SendTextMessageAsync(chatId, Strings.Messages.StartMessage, replyMarkup: Buttons.StartKeyBoard);

                }

                string param = message_text.Split(' ')[1];
                if (param == "Main")
                {
                    await Admin.Registation(chatId, true);
                    await bot.SendTextMessageAsync(chatId, "Админ-панель", replyMarkup: Buttons.MainAdminKeyBoard);
                }
                else if (param == "Admin")
                {
                    await Admin.Registation(chatId, false);
                    await bot.SendTextMessageAsync(chatId, "Админ-панель", replyMarkup: Buttons.AdminKeyBoard);

                }
            }


            if (activeUser != null)
            {
                if (activeUser.IsRegEnd == false)
                {
                    await UserCommandLineHandler(activeUser, message_text);
                }
                else
                {
                    if (message_text != "/start")
                        await bot.SendTextMessageAsync(activeUser.ChatId, Strings.Messages.RegEnd);
                }
            }
            else if (activeAdmin != null)
            {
                await AdminCommandLineHandler(activeAdmin, message_text);
            }



        }
        private static async Task CallBackHandler(Update update)
        {

            User activeUser = null;
            Admin activeAdmin = null;

            long chatId = update.CallbackQuery.From.Id;
            string query = update.CallbackQuery.Data;

            await bot.SendChatActionAsync(chatId, ChatAction.Typing);

            if (activeUser == null)
            {
                try
                {
                    activeUser = await User.GetUserByChatId(chatId);
                }
                catch (Exception ex)
                {
                    
                }
               
            }
            if (activeAdmin == null)
            {
                try
                {
                    activeAdmin = await Admin.GetAdminByChatId(chatId);

                }
                catch (Exception)
                {

                    
                }
            }

        
            if (query == Strings.Queries.StartQuery)
            {
                if(activeUser == null)
                {
                    await User.Registartion(chatId, update?.CallbackQuery?.From?.Username);
                    
                    await bot.SendTextMessageAsync(chatId, Strings.Messages.AskFio);
                }
              
            }
            if (activeUser != null)
            {
                if (query.Contains("Approve"))
                {
                    var user = await User.GetUserByChatId(Convert.ToInt64(query.Split(' ')[1]));
                    int comand = Convert.ToInt32(query.Split(' ')[2]);

                    int now_in_comand = await SqlController.select<int>($"SELECT count FROM comands WHERE id = {comand}");
                    await SqlController.Update($"UPDATE comands SET count = {now_in_comand + 1} WHERE id ={comand}");

                    await user.Update("comand", comand, "chatId", user.ChatId);


                    await bot.SendTextMessageAsync(user.ChatId, "Поздравляем, ты принят в команду!");
                    await EndReg(user.ChatId);
                }
                else if (query.Contains("NotInComand"))
                {

                    var user = await User.GetUserByChatId(Convert.ToInt64(query.Split(' ')[1]));
                    int comand = Convert.ToInt32(query.Split(' ')[2]);

                    await bot.SendTextMessageAsync(user.ChatId, "К сожалению, тебя не приняли в команду (");

                    await bot.SendTextMessageAsync(user.ChatId, Strings.Messages.isCapitane, replyMarkup: Buttons.IsCaptineKeyBoard);


                  


                }


                if (activeUser.IsRegEnd == false)
                {
                    if (query.Contains("NeedSocres"))
                    {
                        int value = Convert.ToInt32(query.Split(' ')[1]);

                        await activeUser.Update("NeedScores", value, "chatId", chatId);

                        await EndReg(chatId);

                    }
                    else if (query.Contains("Back"))
                    {
                        if (query == "BackToInputFio")
                        {
                            await bot.SendTextMessageAsync(chatId, Strings.Messages.AskFio);
                            await activeUser.Update(param: "CommandLine", value: "Input_Fio", where: "chatId", key: chatId);
                        }

                        else if (query == "BackToContact")
                        {
                            //bmstu: group -> birth -> contact
                            //not bmstu: university -> birth -> contact
                            //not a student: select not a student -> birth -> contact
                            await bot.SendTextMessageAsync(activeUser.ChatId, Strings.Messages.AskContact, replyMarkup: Buttons.BackToInputGroup);

                            await activeUser.Update("CommandLine", "Input_Contact", "chatId", activeUser.ChatId);

                        }
                        else if (query == "BackToGroup")
                        {
                            await bot.SendTextMessageAsync(chatId, Strings.Messages.AskGroup, replyMarkup: Buttons.BackToInputFio);
                            await activeUser.Update(param: "CommandLine", value: "Input_Group", where: "chatId", key: chatId);


                        }

                    }
                    else if (query.Contains("Error"))
                    {
                        if (query == "FioError")
                        {
                            await bot.SendTextMessageAsync(chatId, Strings.Messages.AskFioAgain);
                            await activeUser.Update("CommandLine", "Change_Fio", "chatId", chatId);
                        }
                        else if (query == "GroupError")
                        {
                            await bot.SendTextMessageAsync(chatId, Strings.Messages.AskGroupAgain);
                            await activeUser.Update("CommandLine", "Change_Group", "chatId", chatId);
                        }
                        else if (query == "ConcactError")
                        {
                            await bot.SendTextMessageAsync(chatId, Strings.Messages.AskContactAgain);
                            await activeUser.Update("CommandLine", "Change_Contact", "chatId", chatId);
                        }
                        else if (query == "ScoresError")
                        {
                            await activeUser.Update("NeedScores", Convert.ToInt16(!activeUser.NeedScores), "chatId", chatId);

                            await EndReg(chatId);
                        }

                    }
                    else if (query == "SendToXls")
                    {
                        await activeUser.Update("isRegEnd", 1, "chatId", chatId);

                        var mess = await bot.SendTextMessageAsync(chatId, $"Это твой уникальный код `{activeUser.makeCodeString()}`.\nНе забудь показать его организаторам на мероприятии ", ParseMode.Markdown); ;
                        await bot.PinChatMessageAsync(chatId, mess.MessageId);


                        string exel = await googleHelper.InputUser(activeUser, GoogleHelper.Sheets[0]);
                        await activeUser.Update("Exel_Id", exel, "chatId", chatId);
                    }
                    else if (query == "createComand")
                    {
                        await bot.SendTextMessageAsync(chatId, Strings.Messages.AskComandTitle);
                        await activeUser.Update("CommandLine", "Input_Title", "chatId", chatId);
                    }
                    else if (query.Contains("selectComand"))
                    {
                        int id = 0;
                        int back_id = 0;
                        
                        try
                        {
                            id = Convert.ToInt32(query.Split(' ')[1]);
                            back_id = Convert.ToInt32(query.Split(' ')[2]);
                        }
                        catch (Exception ex)
                        {

                        }

                        if (id == -1)
                            await bot.SendTextMessageAsync(chatId, Strings.Messages.CommandSelection);

                        List<Comand> commands = new List<Comand>();

                        commands = await SqlController.GetComandsPaginated(id);

                        List<List<InlineKeyboardButton>> commands_buttons = new List<List<InlineKeyboardButton>>();


                        if (commands.Count != 0)
                        {

                            int last_id = commands[commands.Count - 1].Id;
                            int first_id = commands[0].Id;

                            foreach (var com in commands)
                            {
                                var line = new List<InlineKeyboardButton>();
                                InlineKeyboardButton button = null;

                                if (com.Count > 3)
                                    button = InlineKeyboardButton.WithCallbackData(com.Title, $"ComandFull");
                                else
                                    button = InlineKeyboardButton.WithCallbackData(com.Title, $"SendPullTo {com.Captaine}");
                                line.Add(button);
                                commands_buttons.Add(line);
                            }

                            long last_comand_id = await Comand.GetLastCommnadId();

                            var panel = new List<InlineKeyboardButton>();
                            InlineKeyboardButton back = null;

                            if (id == 0)
                                back = InlineKeyboardButton.WithCallbackData("Назад", $"BackToContact");
                            else
                                back = InlineKeyboardButton.WithCallbackData($"Назад ", $"selectComand {back_id}");
                            panel.Add(back);

                            if (last_id < last_comand_id)
                            {
                                var next = InlineKeyboardButton.WithCallbackData("Далее", $"selectComand {last_id+1} {first_id -1 }");
                                panel.Add(next);
                            }
                            commands_buttons.Add(panel);

                            var panel2 = new List<InlineKeyboardButton>();

                            var upd = InlineKeyboardButton.WithCallbackData("Обновить список", $"selectComand 0");

                            panel2.Add(upd);
                            commands_buttons.Add(panel2);

                        }
                        else
                        {
                            var back = InlineKeyboardButton.WithCallbackData("Назад", $"BackToContact");
                            var upd = InlineKeyboardButton.WithCallbackData("Обновить список", $"selectComand 0");
                            var panel2 = new List<InlineKeyboardButton>();
                            panel2.Add(upd);
                            panel2.Add(back);
                            commands_buttons.Add(panel2);
                        }

                        InlineKeyboardMarkup commands_keyBoard = new InlineKeyboardMarkup(commands_buttons);

                        await bot.SendTextMessageAsync(chatId, "Выбор команды", replyMarkup: commands_keyBoard);
                    }
                    else if (query == "ComandFull")
                    {
                        await bot.SendTextMessageAsync(chatId, "К сожалению, в этой команде уже есть 3 человека");
                        await bot.SendTextMessageAsync(activeUser.ChatId, Strings.Messages.isCapitane, replyMarkup: Buttons.IsCaptineKeyBoard);
                    }
                    else if (query.Contains("SendPullTo"))
                    {
                        User owner = await User.GetUserByChatId(Convert.ToInt64(query.Split(' ')[1]));
                        int comand = await Comand.Get_id(owner.Comand);

                        await bot.SendTextMessageAsync(chatId, "Ваш запрос отправлен капитану команды ждём пока он ответит )");

                        string exp_1 = await SqlController.select<string>($"SELECT exp FROM users WHERE chatId={activeUser.ChatId}");
                        string exp_2 = await SqlController.select<string>($"SELECT exp_hack FROM users WHERE chatId={activeUser.ChatId}");

                        string req_mess = $"Привет! В твою команду хочет вступить {activeUser.Fio}, хочешь видеть его вместе с тобой ?\n\nВот его личные данные:\n{exp_1}\n\n{exp_2}"; ;

                        InlineKeyboardMarkup asnwers = new InlineKeyboardMarkup(new[]
                        {
                            new[]
                            {
                                InlineKeyboardButton.WithCallbackData("Да",$"Approve {activeUser.ChatId} {comand}")
                            },
                             new[]
                            {
                                InlineKeyboardButton.WithCallbackData("Нет",$"NotInComand {activeUser.ChatId} {comand}")
                            }
                        });

                        await bot.SendTextMessageAsync(owner.ChatId, req_mess, replyMarkup: asnwers);
                    }

                }
                else
                {
                    if (query != "SendToXls")
                    {
                        if(!query.Contains("Approve") && !query.Contains("NotInComand"))
                            await bot.SendTextMessageAsync(activeUser.ChatId, Strings.Messages.RegEnd);


                    }
                }
            }
            


            if (activeAdmin != null)
            {
                if (query == "inputCode")
                {
                    await bot.SendTextMessageAsync(chatId, "Введи код, который тебе показал участник");
                    await activeAdmin.Update("CommandLine", "Input_Code", "chatId", chatId);
                }
                else if (query == "sendMessage")
                {
                    InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Всем участникам","SendMessage 0 0")
                        },
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Не закончившим регистрацию","SendMessage 0 1")
                        },
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Только зарегистрированным","SendMessage 0 2")
                        },
                        new[]
                        {
                           InlineKeyboardButton.WithCallbackData("Капитанам всех команд","SendMessage 1 0")

                        }
                        ,
                        new[]
                        {
                           InlineKeyboardButton.WithCallbackData("Капитанам не полным команд","SendMessage 1 1")

                        }
                         ,
                        new[]
                        {
                           InlineKeyboardButton.WithCallbackData("Капитанам полных команд","SendMessage 1 2")

                        }

                    });
                    await bot.SendTextMessageAsync(chatId, "Выбери тип рассылки", replyMarkup: keyboard);
                }
                else if (query.Contains("SendUserToXls"))
                {
                    await activeAdmin.Update("CommandLine", "-", "chatId", chatId);

                    string exel = query.Split(' ')[1];
                    bool scores = Convert.ToBoolean(query.Split(' ')[2]);

                    await googleHelper.Update(GoogleHelper.Sheets[0], exel, "ДА");

                    if (scores)
                    {
                        int code = Convert.ToInt32(query.Split(' ')[3]);
                        User get = await User.GetUserByCode(code);

                        await googleHelper.InputUser(get, GoogleHelper.Sheets[1]);

                    }

                    if (activeAdmin.isMainAdmin)
                    {
                        await bot.SendTextMessageAsync(chatId, "Админ-панель", replyMarkup: Buttons.MainAdminKeyBoard);
                    }
                    else
                    {
                        await bot.SendTextMessageAsync(chatId, "Админ-панель", replyMarkup: Buttons.AdminKeyBoard);
                    }
                }
                else if (query == "repete_input_code")
                {
                    await bot.SendTextMessageAsync(chatId, "Введи код, который тебе показал участник");
                    await activeAdmin.Update("CommandLine", "Input_Code", "chatId", chatId);
                }
                else if (query.Contains("SendMessage"))
                {
                    int mess_type = Convert.ToInt32(query.Split(' ')[1]);
                    int param = Convert.ToInt32(query.Split(' ')[2]);

                    await bot.SendTextMessageAsync(chatId, "Введи сообщение для рассылки");
                    await activeAdmin.Update("CommandLine", $"SendMessage {mess_type} {param}", "chatId", chatId);
                }
            }
            try
            {
               await bot.DeleteMessageAsync(chatId, messageId: update.CallbackQuery.Message.MessageId);

            }
            catch (Exception)
            {

            }


        }
    }
}
