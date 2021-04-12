using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace ShopApp.DBA
{
    class ProductDBA
    {
        //private static SqlConnection con = new SqlConnection("Data Source = (LocalDB)/MSSQLLocalDB; AttachDbFilename=C:/Users/uwshi/source/repos/WpfApp3/WpfApp3/ShopAppDB.mdf;Integrated Security = True");
        private static SqlConnection con = new SqlConnection(Properties.Settings.Default.connection_String);
        #region product
        #region addEditDelete
        public static bool addProduct(Product p)
        {
            string insert = "INSERT INTO Product " +
                            "(Id, DepartmentId, Name, Description, Cost, Stock, LocationId)" +
                            "VALUES (@Id, @DId, @Name, @Desc, @Cost, @Stock, @LId) ";

            SqlCommand insertCommand = new SqlCommand(insert, con);

            int dId;
            int lId;

            if(p.ProductDept.Equals(null))
            {
                dId = 0;
            }
            else
            {
                dId = p.ProductDept.DepartmentId;
            }

            if(p.ProductLocation.Equals(null))
            {
                lId = 1;
            }
            else
            {
                lId = p.ProductLocation.LocationId;
            }

            insertCommand.Parameters.AddWithValue("@Id", p.ProductId);
            insertCommand.Parameters.AddWithValue("@DId", dId);
            insertCommand.Parameters.AddWithValue("@Name", p.ProductName);
            insertCommand.Parameters.AddWithValue("@Desc", p.ProductDesc);
            insertCommand.Parameters.AddWithValue("@Cost", p.ProductCost);
            insertCommand.Parameters.AddWithValue("@Stock", p.ProductStock);
            insertCommand.Parameters.AddWithValue("@LId", lId);

            int count = 0;
            try
            {
                con.Open();
                count = insertCommand.ExecuteNonQuery();
            }
            catch (SqlException ex) { throw ex; }
            finally { con.Close(); }

            return count > 0;
        }

        public static bool editProduct(Product p, int eId)
        {
            string update = "UPDATE Product SET Id = @Id, DepartmentId = @DId, Name = @Name, " +
                "Description = @Desc, Cost = @Cost, Stock = @Stock, LocationId = @LId WHERE Id = @EId";

            SqlCommand updateCommand = new SqlCommand(update, con);
            updateCommand.Parameters.AddWithValue("@Id", p.ProductId);
            updateCommand.Parameters.AddWithValue("@DId", p.ProductDept.DepartmentId);
            updateCommand.Parameters.AddWithValue("@Name", p.ProductName);
            updateCommand.Parameters.AddWithValue("@Desc", p.ProductDesc);
            updateCommand.Parameters.AddWithValue("@Cost", p.ProductCost);
            updateCommand.Parameters.AddWithValue("@Stock", p.ProductStock);
            updateCommand.Parameters.AddWithValue("@LId", p.ProductLocation.LocationId);
            updateCommand.Parameters.AddWithValue("@EId", eId);

            int count = 0;
            try
            {
                con.Open();
                count = updateCommand.ExecuteNonQuery();
            }
            catch (SqlException ex) { throw ex; }
            finally { con.Close(); }

            return count > 0;
        }

        public static void deleteProduct(int pId)
        {
            string delete = "DELETE Product WHERE Id = @Id";

            SqlCommand deleteCommand = new SqlCommand(delete, con);
            deleteCommand.Parameters.AddWithValue("@Id", pId);

            try
            {
                con.Open();
                deleteCommand.ExecuteNonQuery();
            }
            catch (Exception ex) { throw ex; }
            finally { con.Close(); }
        }
        #endregion
        #region getters
        public static List<Product> getProducts()
        {
            string select = "SELECT * FROM Product";

            SqlCommand selectCommand = new SqlCommand(select, con);
            List<Product> products = new List<Product>();
            con.Open();
            SqlDataReader reader = selectCommand.ExecuteReader();
            while (reader.Read())
            {
                Department pDepartment = DepartmentDBA.getDepartmentById(Int32.Parse(reader["DepartmentId"].ToString()));
                Location pLocation = LocationDBA.getLocationById(Int32.Parse(reader["LocationId"].ToString()));
                Product p = new Product(Int32.Parse(reader["Id"].ToString()), pDepartment, reader["Name"].ToString(),
                    reader["Description"].ToString(), Double.Parse(reader["Cost"].ToString()), Int32.Parse(reader["Stock"].ToString()), pLocation);
                products.Add(p);
            }
            con.Close();

            return products;
        }
        #endregion
        #endregion

        #region customer
        #region addEditDelete
        public static bool addCustomer(Customer c)
        {
            string insert = "INSERT INTO Customer " +
                            "(Id, Name, Phone, Email, Address)" +
                            "VALUES (@Id, @Name, @Phone, @Address, @Email) ";

            SqlCommand insertCommand = new SqlCommand(insert, con);
            insertCommand.Parameters.AddWithValue("@Id", c.CustomerId);
            insertCommand.Parameters.AddWithValue("@Name", c.CustomerName);
            insertCommand.Parameters.AddWithValue("@Phone", c.CustomerPhone);
            insertCommand.Parameters.AddWithValue("@Address", c.CustomerEmail);
            insertCommand.Parameters.AddWithValue("@Email", c.CustomerAddress);

            int count = 0;
            try
            {
                con.Open();
                count = insertCommand.ExecuteNonQuery();
            }
            catch (SqlException ex) { throw ex; }
            finally { con.Close(); }

            return count > 0;
        }

        public static bool editCustomer(Customer c, int eId)
        {
            string update = "UPDATE Customer SET Id = @Id, Name = @Name, Phone = @Phone, " +
                "Email = @Email, Address = @Address) WHERE Id = @EId";

            SqlCommand updateCommand = new SqlCommand(update, con);
            updateCommand.Parameters.AddWithValue("@Id", c.CustomerId);
            updateCommand.Parameters.AddWithValue("@Name", c.CustomerName);
            updateCommand.Parameters.AddWithValue("@Phone", c.CustomerPhone);
            updateCommand.Parameters.AddWithValue("@Address", c.CustomerEmail);
            updateCommand.Parameters.AddWithValue("@Email", c.CustomerAddress);
            updateCommand.Parameters.AddWithValue("@EId", eId);

            int count = 0;
            try
            {
                con.Open();
                count = updateCommand.ExecuteNonQuery();
            }
            catch (SqlException ex) { throw ex; }
            finally { con.Close(); }

            return count > 0;
        }

        public static void deleteCustomer(int cId)
        {
            string delete = "DELETE Customer WHERE Id = @Id";

            SqlCommand deleteCommand = new SqlCommand(delete, con);
            deleteCommand.Parameters.AddWithValue("@Id", cId);

            try
            {
                con.Open();
                deleteCommand.ExecuteNonQuery();
            }
            catch (Exception ex) { throw ex; }
            finally { con.Close(); }
        }
        #endregion
        #region getters
        public static List<Customer> getCustomers()
        {
            string select = "SELECT * FROM Customer";

            SqlCommand selectCommand = new SqlCommand(select, con);
            List<Customer> customers = new List<Customer>();
            con.Open();
            SqlDataReader reader = selectCommand.ExecuteReader();
            while (reader.Read())
            {
                Customer c = new Customer(Int32.Parse(reader["Id"].ToString()), reader["Name"].ToString(),
                    reader["Phone"].ToString(), reader["Email"].ToString(), reader["Address"].ToString());
                customers.Add(c);
            }
            con.Close();

            return customers;
        }

        public static Customer getCustomerById(int cId)
        {
            string select = "SELECT * FROM Customer WHERE Id = @Id";
            SqlCommand selectCommand = new SqlCommand(select, con);
            selectCommand.Parameters.AddWithValue("@id", cId);
            Customer customer = new Customer();
            con.Open();
            SqlDataReader reader = selectCommand.ExecuteReader();
            while (reader.Read())
            {
                customer = new Customer(Int32.Parse(reader["Id"].ToString()), reader["Name"].ToString(),
                    reader["Phone"].ToString(), reader["Email"].ToString(), reader["Address"].ToString());
            }
            con.Close();

            return customer;
        }
        #endregion
        #endregion

        

        

        #region order
        #region addEditDelete
        public static bool addOrder(Order o)
        {
            string insert = "INSERT INTO Order " +
                            "(Id, CustomerId, Total, Time)" +
                            "VALUES (@Id, @CId, @Total, @Time) ";

            SqlCommand insertCommand = new SqlCommand(insert, con);
            insertCommand.Parameters.AddWithValue("@Id", o.OrderId);
            insertCommand.Parameters.AddWithValue("@CId", o.OrderCustomer.CustomerId);
            insertCommand.Parameters.AddWithValue("@Total", o.OrderTotal);
            insertCommand.Parameters.AddWithValue("@Time", o.OrderTime);

            int count = 0;
            try
            {
                con.Open();
                count = insertCommand.ExecuteNonQuery();
            }
            catch (SqlException ex) { throw ex; }
            finally { con.Close(); }

            return count > 0;
        }

        public static bool editOrder(Order o, int eId)
        {
            string update = "UPDATE Order SET Id = @Id, CustomerId = @CId, Total = @Total, Time = @Time) WHERE Id = @EId";

            SqlCommand updateCommand = new SqlCommand(update, con);
            updateCommand.Parameters.AddWithValue("@Id", o.OrderId);
            updateCommand.Parameters.AddWithValue("@CId", o.OrderCustomer.CustomerId);
            updateCommand.Parameters.AddWithValue("@Total", o.OrderTotal);
            updateCommand.Parameters.AddWithValue("@Time", o.OrderTime);
            updateCommand.Parameters.AddWithValue("@EId", eId);

            int count = 0;
            try
            {
                con.Open();
                count = updateCommand.ExecuteNonQuery();
            }
            catch (SqlException ex) { throw ex; }
            finally { con.Close(); }

            return count > 0;
        }

        public static void deleteOrder(int oId)
        {
            string delete = "DELETE Location WHERE Id = @Id";

            SqlCommand deleteCommand = new SqlCommand(delete, con);
            deleteCommand.Parameters.AddWithValue("@Id", oId);

            try
            {
                con.Open();
                deleteCommand.ExecuteNonQuery();
            }
            catch (Exception ex) { throw ex; }
            finally { con.Close(); }
        }
        #endregion

        #region getters
        public static List<Order> getOrders()
        {
            string select = "SELECT * FROM Order";

            SqlCommand selectCommand = new SqlCommand(select, con);
            List<Order> orders = new List<Order>();
            con.Open();
            SqlDataReader reader = selectCommand.ExecuteReader();
            while (reader.Read())
            {
                Customer oCustomer = getCustomerById(Int32.Parse(reader["CustomerId"].ToString()));
                Order o = new Order(Int32.Parse(reader["Id"].ToString()), oCustomer,
                    Double.Parse(reader["Total"].ToString()), DateTime.Parse(reader["Time"].ToString()));
                orders.Add(o);
            }
            con.Close();

            return orders;
        }
        #endregion
        #endregion
    }
}
