using _1125.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Windows;

namespace _1125.DB
{
    internal class EditOrder
    {
        DBConnection connection;

        private EditOrder(DBConnection db)
        {
            connection = db;
        }

        public ObservableCollection<int> GetAllId()
        {
            ObservableCollection<int>  res = new ObservableCollection<int>();

            if (connection == null) return res;
            if (!connection.OpenConnection()) return res;

            try
            {
                var command = connection.CreateCommand("SELECT id FROM orders order by id ASC");

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        res.Add(reader.GetInt32(0));
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return  res;
            }
            finally
            {
                connection.CloseConnection();
            }
        }


        public EditableOrder GetOrder(int id)
        {
            EditableOrder res = null;

            if (connection == null) return null;
            if (!connection.OpenConnection()) return null;

            try
            {
                var command = connection.CreateCommand(@"SELECT id, tel, address, dateorder, status FROM orders WHERE id = @id");
                command.Parameters.AddWithValue("@id", id);

                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    res = new EditableOrder
                    {
                        Id = reader.GetInt32("id"),
                        Phone = reader["tel"]?.ToString(),
                        Address = reader["address"]?.ToString(),
                        DeliveryDate = reader.GetDateTime(reader.GetOrdinal("dateorder")),
                        Status = reader["status"]?.ToString()
                    };
                }

                return res;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
            finally
            {
                connection.CloseConnection();
            }
        }

        public void UpdateOrderDitails(EditableOrder order)
        {
            if (connection == null) return;
            if (!connection.OpenConnection()) return;

            try
            {
                var command = connection.CreateCommand(@"
            UPDATE orders SET dateorder = @date,
            tel = @tel,
            address = @ad,
            status = @status
            WHERE id = @id");

                command.Parameters.AddWithValue("@date", order.DeliveryDate);
                command.Parameters.AddWithValue("@tel", order.Phone);
                command.Parameters.AddWithValue("@ad", order.Address);
                command.Parameters.AddWithValue("@status", order.Status);
                command.Parameters.AddWithValue("@id", order.Id);

                command.ExecuteNonQuery();
                MessageBox.Show("Успешно изменены данные");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                connection.CloseConnection();
            }
        }



        static EditOrder db;
        public static EditOrder GetDb()
        {
            if (db == null)
                db = new EditOrder(DBConnection.GetDbConnection());
            return db;
        }
    }
}
