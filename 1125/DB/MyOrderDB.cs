using _1125.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Windows;

namespace _1125.DB
{
    internal class MyOrderDB
    {
        DBConnection connection;

        private MyOrderDB(DBConnection db)
        {
            connection = db;
        }

        public ObservableCollection<CustomerOrder> GetOrdersByUserId(int userId)
        {
            var orders = new ObservableCollection<CustomerOrder>();

            if (!connection.OpenConnection()) return orders;

            try
            {
                var command = connection.CreateCommand(@"
            SELECT ord.id, ord.status, ord.tel, ord.address, ord.dateorder,
                   p.name, ordComp.quantity, img.data
            FROM orders ord
            JOIN ordercomposition ordComp ON ord.id = ordComp.orderid
            JOIN product p ON ordComp.productrid = p.id
            JOIN productimage img ON p.id = img.productID
            WHERE ord.userid = @id
            ORDER BY ord.id");

                command.Parameters.AddWithValue("@id", userId);

                using var reader = command.ExecuteReader();

                Dictionary<int, CustomerOrder> orderMap = new();

                while (reader.Read())
                {
                    int orderId = reader.GetInt32(0);

                    if (!orderMap.TryGetValue(orderId, out var order))
                    {
                        order = new CustomerOrder
                        {
                            Id = orderId,
                            Status = reader.IsDBNull(1) ? "" : reader.GetString(1),
                            Phone = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            Address = reader.IsDBNull(3) ? "" : reader.GetString(3),
                            DeliveryDate = reader.IsDBNull(4) ? null : reader.GetDateTime(4)
                        };
                        orderMap[orderId] = order;
                        orders.Add(order);
                    }

                    var item = new OrderItem
                    {
                        ProductName = reader.GetString(5),
                        Quantity = reader.GetInt32(6),
                        ImageData = (byte[])reader["data"]
                    };

                    order.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке заказов: " + ex.Message);
            }
            finally
            {
                connection.CloseConnection();
            }

            return orders;
        }

        static MyOrderDB db;
        public static MyOrderDB GetDb()
        {
            if (db == null)
                db = new MyOrderDB(DBConnection.GetDbConnection());
            return db;
        }
    }
}
