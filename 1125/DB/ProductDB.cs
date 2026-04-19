using _1125.Model;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Windows;

namespace _1125.DB
{
    internal class ProductDB
    {
        DBConnection connection;

        private ProductDB(DBConnection db)
        {
            connection = db;
        }

        internal List<Product> SelectAll()
        {
            List<Product> products = new List<Product>();
            if (connection == null)
                return products;

            if (connection.OpenConnection())
            {
                var command = connection.CreateCommand(
                    "SELECT p.id, p.name, p.description, p.availability, p.price, productimage.data, c.name as category, p.categoryid categoryID  FROM product p  LEFT JOIN productimage ON p.id = productimage.productid join category c on c.id  = p.categoryid");

                try
                {
                    using (var dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var product = new Product
                            {
                                Id = dr.GetInt32("id"),
                                Name = dr.IsDBNull(dr.GetOrdinal("name")) ? "" : dr.GetString("name"),
                                Description = dr.IsDBNull(dr.GetOrdinal("description")) ? "" : dr.GetString("description"),
                                Availability = dr.IsDBNull(dr.GetOrdinal("availability")) ? 0 : dr.GetInt32("availability"),
                                Price = dr.IsDBNull(dr.GetOrdinal("price")) ? 0 : dr.GetInt32("price"),
                                ImageData = dr.IsDBNull(dr.GetOrdinal("data")) ? null : (byte[])dr["data"],
                                CategoryName = dr.IsDBNull(dr.GetOrdinal("category")) ? "" : dr.GetString("category"),
                                CategoryId = dr.GetInt32("categoryID")
                            };
                            products.Add(product);
                        }
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
            return products;
        }

        internal List<Category> SelectCategorys()
        {
            List<Category> Category = new List<Category>();
            if (connection == null)
                return Category;

            if (connection.OpenConnection())
            {
                var command = connection.CreateCommand(
                    "SELECT id, name from category");

                try
                {
                    using (var dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var product = new Category
                            {
                                Id = dr.GetInt32("id"),
                                Name = dr.IsDBNull(dr.GetOrdinal("name")) ? "" : dr.GetString("name") 
                         
                            };
                            Category.Add(product);
                        }
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
            return Category;
        }


        internal void Insert(Product product)
        {
            if (connection == null) return;
            if (!connection.OpenConnection()) return;

            try
            {
                // Включаем categoryid в INSERT
                var command = connection.CreateCommand(@"
            INSERT INTO product
                (name, description, availability, price, categoryid)
            VALUES
                (@name, @description, @availability, @price, @categoryid);
            SELECT LAST_INSERT_ID();");

                command.Parameters.AddWithValue("@name", product.Name);
                command.Parameters.AddWithValue("@description", product.Description ?? "");
                command.Parameters.AddWithValue("@availability", product.Availability);
                command.Parameters.AddWithValue("@price", product.Price);
                command.Parameters.AddWithValue("@categoryid", product.CategoryId);

                var id = Convert.ToInt32(command.ExecuteScalar());
                product.Id = id;

                if (product.ImageData != null)
                { 
                    var imageCommand = connection.CreateCommand(@"
                    INSERT INTO productimage(productID, data)
                    VALUES (@productID, @data)");
                    imageCommand.Parameters.AddWithValue("@productID", product.Id);
                    imageCommand.Parameters.AddWithValue("@data", product.ImageData);
                    imageCommand.ExecuteNonQuery();
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

        internal void Update(Product product)
        {
            if (connection == null) return;
            if (!connection.OpenConnection()) return;

            try
            {
                // Включаем categoryid в UPDATE
                var command = connection.CreateCommand(@"
            UPDATE product SET
                name        = @name,
                description = @description,
                availability= @availability,
                price       = @price,
                categoryid  = @categoryid
            WHERE id = @id");

                command.Parameters.AddWithValue("@name", product.Name);
                command.Parameters.AddWithValue("@description", product.Description ?? "");
                command.Parameters.AddWithValue("@availability", product.Availability);
                command.Parameters.AddWithValue("@price", product.Price);
                command.Parameters.AddWithValue("@categoryid", product.CategoryId);
                command.Parameters.AddWithValue("@id", product.Id);

                command.ExecuteNonQuery();

                if (product.ImageData != null)
                {
                    var imageCommand = connection.CreateCommand(@"
                UPDATE productimage SET data = @data
                WHERE productid = @productid");
                    imageCommand.Parameters.AddWithValue("@data", product.ImageData);
                    imageCommand.Parameters.AddWithValue("@productid", product.Id);

                    int rows = imageCommand.ExecuteNonQuery();
                    if (rows == 0)
                    {
                        var insertImageCmd = connection.CreateCommand(@"
                    INSERT INTO productimage (productid, data)
                    VALUES (@productid, @data)");
                        insertImageCmd.Parameters.AddWithValue("@productid", product.Id);
                        insertImageCmd.Parameters.AddWithValue("@data", product.ImageData);
                        insertImageCmd.ExecuteNonQuery();
                    }
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


        internal void Delete(Product product)
        {
            if (connection == null) return;
            if (!connection.OpenConnection()) return;

            try
            {
                // Включаем categoryid в UPDATE
                var command = connection.CreateCommand(@"Delete from product WHERE id = @id");

                command.Parameters.AddWithValue("@id", product.Id);

                command.ExecuteNonQuery();
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



        static ProductDB db;
        public static ProductDB GetDb()
        {
            if (db == null)
                db = new ProductDB(DBConnection.GetDbConnection());
            return db;
        }
    }
}
