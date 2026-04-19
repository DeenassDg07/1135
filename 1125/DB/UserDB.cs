using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;
using System.Windows;
using _1125.Model;
using System.Data;
using System.Data.Common;
using System.Security.Cryptography;

namespace _1125.DB
{
    public class UserDB
    {
        DBConnection connection;

        private UserDB(DBConnection db)
        {
            this.connection = db;
        }


        public bool Insert(User User)
        {
            bool result = false;
            if (connection == null)
                return result;

            if (connection.OpenConnection())
            {
                MySqlCommand cmd = connection.CreateCommand("insert into `user` Values (0,2, @login, @password );select LAST_INSERT_ID();");



                cmd.Parameters.Add(new MySqlParameter("login", User.Login));
                cmd.Parameters.Add(new MySqlParameter("password", SHA512(User.Password)));


                try
                {

                    int id = (int)(ulong)cmd.ExecuteScalar();
                    if (id > 0)
                    {
                       
                        User.Id = id;
                        result = true;
                    }
                    else
                    {
                        MessageBox.Show("Пользователь не добавлен");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            connection.CloseConnection();
            return result;
        }
        public static string SHA512(string input)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(input);
            using (var hash = System.Security.Cryptography.SHA512.Create())
            {
                var hashedInputBytes = hash.ComputeHash(bytes);

                // Convert to text
                // StringBuilder Capacity is 128, because 512 bits / 8 bits in byte * 2 symbols for byte 
                var hashedInputStringBuilder = new System.Text.StringBuilder(128);
                foreach (var b in hashedInputBytes)
                    hashedInputStringBuilder.Append(b.ToString("X2"));
                return hashedInputStringBuilder.ToString();
            }
        }
        internal User Auth(string l, string p)
        {
            User user = null;
            if (connection == null)
                return null;

            if (connection.OpenConnection())
            {
                var command = connection.CreateCommand("select user.`id`, `login`, `Password`, r.`id` , r.`name`  from `user` join role r on r.id = user.roleid where login = @login and Password = @password");
                command.Parameters.Add(new MySqlParameter("login", l));

                p = SHA512(p); //хэшируем пароль
                command.Parameters.Add(new MySqlParameter("password", p));
                try
                {
                    MySqlDataReader dr = command.ExecuteReader();
                    if (dr.Read())
                    {
                        user = new User
                        {
                            Id = dr.GetInt32(0),
                            Login = dr.IsDBNull(1) ? "" : dr.GetString("login"),
                            Password = dr.IsDBNull(2) ? null : dr.GetString("password"),
                            Role = dr.GetString("name")
                        };

                        // Устанавливаем текущего пользователя
                        User.Current = user;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            connection.CloseConnection();
            return user;
        }

        internal bool Update(User edit)
        {
            bool result = false;
            if (connection == null)
                return result;

            if (connection.OpenConnection())
            {
                var mc = connection.CreateCommand($"update `user` set `login`=@Login, `password`=@Password where `ID` = {edit.Id}");
                mc.Parameters.Add(new MySqlParameter("login", edit.Login));           
                mc.Parameters.Add(new MySqlParameter("password", edit.Password));
                try
                {
                    mc.ExecuteNonQuery();
                    result = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            connection.CloseConnection();
            return result;
        }

        internal bool Remove(User remove)
        {
            bool result = false;
            if (connection == null)
                return result;

            if (connection.OpenConnection())
            {
                var mc = connection.CreateCommand($"delete from `user` where `id` = {remove.Id}");
                try
                {
                    mc.ExecuteNonQuery();
                    result = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            connection.CloseConnection();
            return result;
        }

        static UserDB db;
        public static UserDB GetDb()
        {
            if (db == null)
                db = new UserDB(DBConnection.GetDbConnection());
            return db;
        }

 
    }
}