using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
#pragma warning disable CS8601

namespace sample
{
    internal class SqlController
    {
        public static string connection = Strings.Tokens.MySqlConnection;
        public static MySqlConnection database = new MySqlConnection(connection);
        public static MySqlCommand command = null;

        public SqlController()
        {
            if (database.State == System.Data.ConnectionState.Closed)
                database.OpenAsync().Wait();
        }

        public static async Task insert(string insert)
        {

            using (var conn = new MySqlConnection(connection))
            {
                using (MySqlCommand comm = new MySqlCommand(insert, conn))
                {
                    await conn.OpenAsync();

                    await comm.ExecuteNonQueryAsync();

                    await conn.CloseAsync();

                }
            }
        }

        public static async Task<T> select<T>(string select)
        {
            T output;
            using (var conn = new MySqlConnection(connection))
            {
                using (MySqlCommand comm = new MySqlCommand(select, conn))
                {
                    await conn.OpenAsync();
                    using (MySqlDataReader reader = (MySqlDataReader)await comm.ExecuteReaderAsync())
                    {


                        await reader.ReadAsync();

                        output = (T)reader[0];

                        await reader.CloseAsync();


                    }
                    await conn.CloseAsync();
                }
            }
            return output;
        }
        
        public static async Task<User> GetUser(long chatId)
        {
            User user = new User();
            using (var conn = new MySqlConnection(connection))
            {
                using (MySqlCommand comm = new MySqlCommand($"SELECT * FROM users WHERE chatId={chatId} LIMIT 1", conn))
                {
                    await conn.OpenAsync();
                    using (MySqlDataReader reader = (MySqlDataReader)await comm.ExecuteReaderAsync())
                    {


                        await reader.ReadAsync();

                        user.ChatId = (long)reader[0];
                        user.Code = (int)reader[1];
                        user.CommandLine = (string)reader[2];
                        user.Fio = (string)reader[3];
                        user.tg = (string)reader[4];
                        user.Group = (string)reader[5];
                        user.Contact = (string)reader[6];
                        user.Comand = (await GetComand((int)reader[7]))?.Title;
                        user.IsRegEnd = (bool)reader[8];
                        user.ExelId = (string)reader[9];

                        await reader.CloseAsync();


                    }
                    await conn.CloseAsync();
                }
            }

            return user;
        }
        public static async Task<User> GetUserCode(int code)
        {
            User user = new User();
            using (var conn = new MySqlConnection(connection))
            {
                using (MySqlCommand comm = new MySqlCommand($"SELECT * FROM users WHERE code={code} LIMIT 1", conn))
                {
                    await conn.OpenAsync();
                    using (MySqlDataReader reader = (MySqlDataReader)await comm.ExecuteReaderAsync())
                    {


                        await reader.ReadAsync();

                        user.ChatId = (long)reader[0];
                        user.Code = (int)reader[1];
                        user.CommandLine = (string)reader[2];
                        user.Fio = (string)reader[3];
                        user.tg = (string)reader[4];
                        user.Group = (string)reader[5];
                        user.Contact = (string)reader[6];
                        user.Comand = (await GetComand((int)reader[7]))?.Title;
                        user.IsRegEnd = (bool)reader[8];
                        user.ExelId = (string)reader[9];


                        await reader.CloseAsync();


                    }
                    await conn.CloseAsync();
                }
            }

            return user;
        }
       

        public static async Task<List<User>> GetUsers()
        {
            List<User> users = new List<User>();
            using (var conn = new MySqlConnection(connection))
            {
                using (MySqlCommand comm = new MySqlCommand($"SELECT * FROM users", conn))
                {
                    await conn.OpenAsync();

                    using(MySqlDataReader reader = (MySqlDataReader)await comm.ExecuteReaderAsync())
                    {
                        while(await reader.ReadAsync())
                        {
                            var user = new User();
                            user.ChatId = (long)reader[0];
                            user.Code = (int)reader[1];
                            user.CommandLine = (string)reader[2];
                            user.Fio = (string)reader[3];
                            user.tg = (string)reader[4];
                            user.Group = (string)reader[5];
                            user.Contact = (string)reader[6];

                            user.Comand = (await GetComand((int)reader[7]))?.Title;

                            users.Add(user);
                        }
                        await reader.CloseAsync();
                    }              

                    await conn.CloseAsync();
                }
            }

            return users;
        }

        public static async Task<Admin> GetAdmin(long chatId)
        {
            Admin admin = new Admin();
            using (var conn = new MySqlConnection(connection))
            {
                using (MySqlCommand comm = new MySqlCommand($"SELECT * FROM admins WHERE chatId={chatId} LIMIT 1", conn))
                {
                    await conn.OpenAsync();
                    using (MySqlDataReader reader = (MySqlDataReader)await comm.ExecuteReaderAsync())
                    {


                        await reader.ReadAsync();

                        admin.chatId = (long)reader[0];
                        admin.commandLine=(string)reader[1];    
                        admin.isMainAdmin = (bool)reader[2];    

     

                        await reader.CloseAsync();


                    }
                    await conn.CloseAsync();
                }
            }

            return admin;
        }
        public static async Task<Comand> GetComand(int id)
        {
            Comand comand = new Comand();
            using (var conn = new MySqlConnection(connection))
            {
                try
                {
                    using (MySqlCommand comm = new MySqlCommand($"SELECT * FROM comands WHERE id={id} LIMIT 1", conn))
                    {
                        await conn.OpenAsync();
                        using (MySqlDataReader reader = (MySqlDataReader)await comm.ExecuteReaderAsync())
                        {


                            await reader.ReadAsync();

                            comand.Id = id;
                            comand.Captaine = (long)reader[1];
                            comand.Title = (string)reader[2];



                            await reader.CloseAsync();


                        }
                        await conn.CloseAsync();
                    }

                }
                catch (Exception)
                {

                    return null;                
                }
            }

            return comand;
        }
        public static async Task<List<Comand>> GetComandsPaginated(int lastId)
        {
            List<Comand> comands = new List<Comand>();
            using (var conn = new MySqlConnection(connection))
            {
                try
                {
                    using (MySqlCommand comm = new MySqlCommand($"SELECT * FROM comands WHERE id>= {lastId} LIMIT 10", conn))
                    {
                        await conn.OpenAsync();
                        using (MySqlDataReader reader = (MySqlDataReader)await comm.ExecuteReaderAsync())
                        {

                            
                            while (await reader.ReadAsync())
                            {
                                var comand = new Comand();
                                comand.Id = (int)reader[0];
                                comand.Captaine = (long)reader[1];
                                comand.Title = (string)reader[2];
                                comand.Count = (int)reader[3];

                                comands.Add(comand);
                            }
                            await reader.CloseAsync();


                        }
                        await conn.CloseAsync();
                    }

                }
                catch (Exception)
                {

                    return null;
                }
            }

            return comands;
        }
        public static async Task<List<Comand>> GetComands()
        {
            List<Comand> comands = new List<Comand>();
            using (var conn = new MySqlConnection(connection))
            {
                try
                {
                    using (MySqlCommand comm = new MySqlCommand($"SELECT * FROM comands", conn))
                    {
                        await conn.OpenAsync();
                        using (MySqlDataReader reader = (MySqlDataReader)await comm.ExecuteReaderAsync())
                        {


                            while (await reader.ReadAsync())
                            {
                                var comand = new Comand();
                                comand.Id = (int)reader[0];
                                comand.Captaine = (long)reader[1];
                                comand.Title = (string)reader[2];
                                comand.Count = (int)reader[3];

                                comands.Add(comand);
                            }
                            await reader.CloseAsync();


                        }
                        await conn.CloseAsync();
                    }

                }
                catch (Exception)
                {

                    return null;
                }
            }

            return comands;
        }

        public static async Task Update(string update)
        {
            using (var conn = new MySqlConnection(connection))
            {

                using (MySqlCommand comm = new MySqlCommand(update, conn))
                {
                    await conn.OpenAsync();

                    await comm.ExecuteNonQueryAsync();

                    await conn.CloseAsync();

                }

            }
        }

    
       
    }
}
