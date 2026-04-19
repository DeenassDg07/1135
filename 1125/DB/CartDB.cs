using _1125.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace _1125.DB
{
    internal class CartDB
    {
        DBConnection connection;

        private CartDB(DBConnection db)
        {
            connection = db;
        }

        public void InsertIntoCart(Product product, User user)
        {
            if (connection == null) return;
            if (!connection.OpenConnection()) return;

            try
            {
                var command = connection.CreateCommand(@"SELECT id FROM cart WHERE userId = @userId");
                command.Parameters.AddWithValue("@userId", user.Id);
                var cartId = command.ExecuteScalar();

                if (cartId == null)
                {
                    command = connection.CreateCommand(@"INSERT INTO cart (userId) VALUES (@userId); SELECT LAST_INSERT_ID();");
                    command.Parameters.AddWithValue("@userId", user.Id);
                    cartId = Convert.ToInt32(command.ExecuteScalar());

                    command = connection.CreateCommand(@"
                        INSERT INTO cart_items (cart_id, product_id, quantity)
                        VALUES (@cartId, @productId, 1)");
                    command.Parameters.AddWithValue("@cartId", cartId);
                    command.Parameters.AddWithValue("@productId", product.Id);
                    command.ExecuteNonQuery();
                }
                else
                {
                    command = connection.CreateCommand(@"
                        INSERT INTO cart_items (cart_id, product_id, quantity)
                        VALUES (@cartId, @productId, 1)
                        ON DUPLICATE KEY UPDATE quantity = quantity + 1;");
                    command.Parameters.AddWithValue("@cartId", cartId);
                    command.Parameters.AddWithValue("@productId", product.Id);
                    command.ExecuteNonQuery();
                }
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

        public ObservableCollection<CartItem> GetCartItems(int userId)
        {
            var list = new ObservableCollection<CartItem>();
            if (!connection.OpenConnection()) return list;

            try
            {
                var command = connection.CreateCommand(@"
            SELECT pi.`data`, p.*, ci.quantity
            FROM cart_items ci
            JOIN product p ON p.id = ci.product_id
            JOIN cart c ON c.id = ci.cart_id
            LEFT JOIN productimage pi ON pi.productID = p.id
            WHERE c.userId = @userId
        ");
                command.Parameters.AddWithValue("@userId", userId);

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var product = new Product
                    {
                        Id = reader.GetInt32("id"),
                        Name = reader.GetString("name"),
                        Description = reader.GetString("description"),
                        Price = reader.GetInt32("price"),
                        Availability = reader.GetInt32("availability"), // Убедитесь, что это поле извлекается
                        ImageData = reader.IsDBNull(reader.GetOrdinal("data")) ? null : (byte[])reader["data"],
                    };

                    var item = new CartItem
                    {
                        Product = product,
                        Quantity = reader.GetInt32("quantity"),
                    };
                    list.Add(item);
                }
            }
            finally
            {
                connection.CloseConnection();
            }

            return list;
        }

        public void RemoveFromCart(int userId, int productId)
        {
            if (!connection.OpenConnection()) return;
            try
            {
                var command = connection.CreateCommand(@"SELECT id FROM cart WHERE userId = @userId");
                command.Parameters.AddWithValue("@userId", userId);
                var cartId = command.ExecuteScalar();
                if (cartId == null) return;

                command = connection.CreateCommand(@"
                    DELETE FROM cart_items 
                    WHERE cart_id = @cartId AND product_id = @productId
                ");
                command.Parameters.AddWithValue("@cartId", cartId);
                command.Parameters.AddWithValue("@productId", productId);
                command.ExecuteNonQuery();
            }
            finally
            {
                connection.CloseConnection();
            }
        }

        public void UpdateCartQuantity(int userId, int productId, int quantity)
        {
            if (!connection.OpenConnection()) return;
            try
            {
                var command = connection.CreateCommand(@"SELECT id FROM cart WHERE userId = @userId");
                command.Parameters.AddWithValue("@userId", userId);
                var cartId = command.ExecuteScalar();
                if (cartId == null) return;

                if (quantity <= 0)
                {
                    command = connection.CreateCommand(@"
                        DELETE FROM cart_items WHERE cart_id = @cartId AND product_id = @productId
                    ");
                    command.Parameters.AddWithValue("@cartId", cartId);
                    command.Parameters.AddWithValue("@productId", productId);
                    command.ExecuteNonQuery();
                }
                else
                {
                    command = connection.CreateCommand(@"
                        UPDATE cart_items SET quantity = @quantity 
                        WHERE cart_id = @cartId AND product_id = @productId
                    ");
                    command.Parameters.AddWithValue("@cartId", cartId);
                    command.Parameters.AddWithValue("@productId", productId);
                    command.Parameters.AddWithValue("@quantity", quantity);
                    command.ExecuteNonQuery();
                }
            }
            finally
            {
                connection.CloseConnection();
            }
        }

        public int CreateOrder(int userId, DateTime orderDate, string tel, string address)
        {
            if (!connection.OpenConnection()) return -1;

            try
            {
                connection.BeginTransaction();

                var command = connection.CreateCommand(@"
            SELECT id FROM cart WHERE userId = @userId
        ");
                command.Parameters.AddWithValue("@userId", userId);
                var cartIdObj = command.ExecuteScalar();
                if (cartIdObj == null)
                {
                    connection.RollbackTransaction();
                    return -1;
                }
                int cartId = Convert.ToInt32(cartIdObj);

                command = connection.CreateCommand(@"
            SELECT ci.product_id, ci.quantity, p.price, p.availability
            FROM cart_items ci
            JOIN product p ON ci.product_id = p.id
            WHERE ci.cart_id = @cartId
        ");
                command.Parameters.AddWithValue("@cartId", cartId);

                var cartItems = new List<(int productId, int quantity, int price, int availability)>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int productId = reader.GetInt32("product_id");
                        int quantity = reader.GetInt32("quantity");
                        int price = reader.GetInt32("price");
                        int availability = reader.GetInt32("availability");

                        if (availability < quantity)
                        {
                            connection.RollbackTransaction();
                            MessageBox.Show($"Недостаточно товара на складе для товара ID {productId}. Осталось: {availability}, требуется: {quantity}");
                            return -1;
                        }

                        cartItems.Add((productId, quantity, price, availability));
                    }
                }

                if (cartItems.Count == 0)
                {
                    connection.RollbackTransaction();
                    return -1;
                }

                command = connection.CreateCommand(@"
            INSERT INTO orders (userid, dateorder, tel, address, status)
            VALUES (@userid, @orderDate, @tel, @address, 'Ожидает подтверждения');
            SELECT LAST_INSERT_ID();
        ");
                command.Parameters.AddWithValue("@userid", userId);
                command.Parameters.AddWithValue("@orderDate", orderDate);
                command.Parameters.AddWithValue("@tel", tel);
                command.Parameters.AddWithValue("@address", address);

                int orderId = Convert.ToInt32(command.ExecuteScalar());

                foreach (var item in cartItems)
                {
                    // Добавление в состав заказа
                     command = connection.CreateCommand(@"
        INSERT INTO ordercomposition (orderid, productrid, quantity, price)
        VALUES (@orderId, @productId, @quantity, @price)
                         ");
                    command.Parameters.AddWithValue("@orderId", orderId);
                    command.Parameters.AddWithValue("@productId", item.productId);
                    command.Parameters.AddWithValue("@quantity", item.quantity);
                    command.Parameters.AddWithValue("@price", item.price);
                    command.ExecuteNonQuery();

                    // Обновление доступности товара
                    command = connection.CreateCommand(@"
        UPDATE product SET availability = availability - @quantity
        WHERE id = @productId
    ");
                    command.Parameters.AddWithValue("@quantity", item.quantity);
                    command.Parameters.AddWithValue("@productId", item.productId);
                    command.ExecuteNonQuery();
                }


                command = connection.CreateCommand(@"
            DELETE FROM cart_items WHERE cart_id = @cartId
        ");
                command.Parameters.AddWithValue("@cartId", cartId);
                command.ExecuteNonQuery();

                connection.CommitTransaction();

                return orderId;
            }
            catch (Exception ex)
            {
                connection.RollbackTransaction();
                MessageBox.Show($"Ошибка при создании заказа: {ex.Message}");
                return -1;
            }
            finally
            {
                connection.CloseConnection();
            }
        }

        public void ClearCart(int userId)
        {
            var db = DBConnection.GetDbConnection();
            if (!db.OpenConnection())
            {
                // Не удалось открыть соединение
                return;
            }

            try
            {
                string sql = "DELETE FROM cart WHERE userId = @userId";

                using (var cmd = db.CreateCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при очистке корзины: " + ex.Message);
            }
            finally
            {
                db.CloseConnection();
            }
        }


        public void AddOrderItems(int orderId, ObservableCollection<CartItem> cartItems)
        {
            if (!connection.OpenConnection()) return;

            try
            {
                 foreach (var item in cartItems)
                {
                    var command = connection.CreateCommand(@"
                    INSERT INTO ordercomposition (orderid, productrid, quantity, price)
                    VALUES (@orderId, @productId, @quantity, @price)
                    ON DUPLICATE KEY UPDATE
                    quantity = quantity + VALUES(quantity),
                    price = VALUES(price)
                    ");

                    command.Parameters.AddWithValue("@orderId", orderId);
                    command.Parameters.AddWithValue("@productId", item.Product.Id);
                    command.Parameters.AddWithValue("@quantity", item.Quantity);
                    command.Parameters.AddWithValue("@price", item.Product.Price);
                    command.ExecuteNonQuery();
                }
            }
            finally
            {
                connection.CloseConnection();
            }
        }


        static CartDB db;
        public static CartDB GetDb()
        {
            if (db == null)
                db = new CartDB(DBConnection.GetDbConnection());
            return db;
        }
    }
}
