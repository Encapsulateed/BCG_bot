using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using static sample.Strings;

namespace sample
{
    internal class Buttons
    {
        public static InlineKeyboardMarkup StartKeyBoard = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Да! Хочу зарегистрироваться!",Strings.Queries.StartQuery)
            }

        });

        public static InlineKeyboardMarkup IsBmstuKeyBoard = new InlineKeyboardMarkup(new[]
       {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Да",$"{Strings.Queries.IsBmstuQuery} 1")
            },
              new[]
            {
                InlineKeyboardButton.WithCallbackData("Нет",$"{Strings.Queries.IsBmstuQuery} 0")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Не студент",$"{Strings.Queries.IsBmstuQuery} 2")
            }
            ,
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Назад","BackToInputFio")
            }

        });

        public static InlineKeyboardMarkup BackToInputGroup = new InlineKeyboardMarkup(new[]
        {
             new[]
            {
                InlineKeyboardButton.WithCallbackData("Назад","BackToGroup")
            }
        });
        public static InlineKeyboardMarkup BackToInputFio = new InlineKeyboardMarkup(new[]
       {
             new[]
            {
                InlineKeyboardButton.WithCallbackData("Назад","BackToInputFio")
            }
        });
        public static InlineKeyboardMarkup BackToInputContact = new InlineKeyboardMarkup(new[]
        {
             new[]
            {
                InlineKeyboardButton.WithCallbackData("Назад","BackToContact")
            }
        });
        public static InlineKeyboardMarkup BackToInputBirth = new InlineKeyboardMarkup(new[]
        {
             new[]
            {
                InlineKeyboardButton.WithCallbackData("Назад","BackToBirth")
            }
        });
        public static InlineKeyboardMarkup BackToInputUniversity = new InlineKeyboardMarkup(new[]
    {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Назад","BackToInputUniversity")
            }

        });
        public static InlineKeyboardMarkup BackToUniversitySelection = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Назад","BackToUniversitySelection")
            }

        });
        public static InlineKeyboardMarkup NeedScoresKeyBoard = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Да","NeedSocres 1")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Нет","NeedSocres 0")
            }
            ,
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Назад","BackToContact")
            }
        });

        public static InlineKeyboardMarkup IsCaptineKeyBoard = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Создать новую","createComand")
            },
            new[]
            {
               InlineKeyboardButton.WithCallbackData("Вступить в существующую","selectComand")

            }
        });

        public static InlineKeyboardMarkup RegEndBmstu = new InlineKeyboardMarkup(new[]
        {
               new[]
               {
                   InlineKeyboardButton.WithCallbackData("✅ Всё супер",$"SendToXls")
                },
               new[]
               {
                InlineKeyboardButton.WithCallbackData("Ошибся в ФИО",$"FioError"),
                InlineKeyboardButton.WithCallbackData("Ошибся в группе",$"GroupError")
               },  
               new[]
               {
                InlineKeyboardButton.WithCallbackData("Ошибся в номере телефона",$"ConcactError")
               }
        });
        public static InlineKeyboardMarkup RegEndNotBmstu = new InlineKeyboardMarkup(new[]
       {
               new[]
               {
                   InlineKeyboardButton.WithCallbackData("✅ Всё супер",$"SendToXls")
                },
               new[]
               {
                InlineKeyboardButton.WithCallbackData("Ошибся в ФИО",$"FioError"),
                
               },
               new[]
               {
                InlineKeyboardButton.WithCallbackData("Ошибся в номере телефона",$"ConcactErorr")
               }
        });

        public static InlineKeyboardMarkup MainAdminKeyBoard = new InlineKeyboardMarkup(new[]
        {
             new[]
               {
                   InlineKeyboardButton.WithCallbackData("✅ Ввести код человека",$"inputCode")
              },
              //new[]
              // {
              //    InlineKeyboardButton.WithCallbackData("Зарегистрировать человека","regNewUser")
              //},
               new[] 
               {
                  InlineKeyboardButton.WithCallbackData("Расслыка","sendMessage")
               }
        });
        public static InlineKeyboardMarkup AdminKeyBoard = new InlineKeyboardMarkup(new[]
        {
             new[]
               {
                   InlineKeyboardButton.WithCallbackData("✅ Ввести код человека",$"inputCode")
              }
        });
            
    }
}
