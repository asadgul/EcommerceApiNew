using ECommerce.API.Models;
using Microsoft.IdentityModel.Tokens;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ECommerceWebApi.DataAccessServices
{
    public class DataAccess : IDataAccess
    {
        private string _connectionstring;
        
        public DataAccess(IConfiguration configuration)
        {
            _connectionstring = configuration["ConnectionStrings:DbServer"];
        }
        public List<ProductCategory> GetProductCategories()
        {
            List<ProductCategory> products = new List<ProductCategory>();

            try
            {            
            using (SqlConnection sqlConnection=new SqlConnection(_connectionstring))
            {
                SqlCommand cmd = sqlConnection.CreateCommand();
                sqlConnection.Open();
                string query = "select * from ProductCategories";
                cmd.CommandText = query;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) { 
                var category = new ProductCategory();
                    category.Id = (int)reader["CategoryId"];
                    category.Category = (string)reader["Category"];
                    category.SubCategory = (string)reader["SubCategory"];
                    products.Add(category);
                }
            }
            }
            catch(Exception e)
            {

            }
            return products; 
        }

       public Offer Getoffer(int id)
        {
            Offer offer = null;
            using (SqlConnection sqlConnection = new(_connectionstring))
            {
                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlConnection.Open();
                string query = "select * from Offers where OfferId='" + id + "'";
                sqlCommand.CommandText = query;
                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    offer = new Offer();
                    offer.Id = (int)reader["OfferId"];
                    offer.Title = (string)reader["Title"];
                    offer.Discount = (int)reader["Discount"];
                }
            }
            return offer;
        }

       

       public ProductCategory GetProductCategory(int id)
        {
            var productcategory = new ProductCategory();
            using (SqlConnection sqlconnection=new SqlConnection(_connectionstring))
            {
                SqlCommand sqlCommand=sqlconnection.CreateCommand();    
                sqlconnection.Open();
                string sqlquery= "select * from ProductCategories where CategoryId='"+id+"'";
                sqlCommand.CommandText = sqlquery;
                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    productcategory.Id=(int)reader["CategoryId"];
                    productcategory.Category = (string)reader["Category"];
                    productcategory.SubCategory = (string)reader["SubCategory"];
                }
            }
            return productcategory;
        }

      public  List<Product> GetProducts(string category, string subcategory, int count)
        {
            List<Product> products = new List<Product>();
            using (SqlConnection sqlConnection = new(_connectionstring))
            {
                SqlCommand cmd = sqlConnection.CreateCommand();
                sqlConnection.Open();
                string query = "select top  " + count + " * from Products where CategoryId=(select CategoryId from ProductCategories where Category=@c and SubCategory=@s)order by NEWID()";
                cmd.CommandText = query;
                cmd.Parameters.Add("@c",System.Data.SqlDbType.NVarChar).Value = category;
                cmd.Parameters.Add("@s", System.Data.SqlDbType.NVarChar).Value = subcategory;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var product = new Product()
                    {
                        Id = (int)reader["ProductId"],
                        Description = (string)reader["Description"],
                        Title = (string)reader["Title"],
                        Price = (Double)reader["Price"],
                        ImageName = (string)reader["ImageName"],
                        Quantity = (int)reader["Quantity"]
                    };
                    var categoryId=(int)reader["CategoryId"];
                    product.ProductCategory = GetProductCategory(categoryId);
                    var offerid = (int)reader["OfferId"];
                    product.Offer = Getoffer(offerid);
                    products.Add(product);

                }
                return products; 

            }

        }

        public Product GetProductById(int id)
        {
            Product product = null; 
            using (SqlConnection sqlConnection = new(_connectionstring))
            {
                SqlCommand sqlCommand=sqlConnection.CreateCommand();
                sqlConnection.Open();
                string query = "select * from Products where ProductId=@pdi";
                sqlCommand.CommandText = query;
                sqlCommand.Parameters.Add("@pdi",System.Data.SqlDbType.Int).Value=id;
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                while (sqlDataReader.Read()) {
                    product = new Product();
                    product.Id = (int)sqlDataReader["ProductId"];
                    product.ImageName=(string)sqlDataReader["ImageName"];
                    product.Description=(string)sqlDataReader["Description"];
                    product.Title=(string)sqlDataReader["Title"];
                    product.Quantity = (int)sqlDataReader["Quantity"];
                    product.Price = (double)sqlDataReader["Price"];
                    var CategoryId = (int)sqlDataReader["CategoryId"];
                    product.ProductCategory=GetProductCategory(CategoryId);
                    var offerid = (int)sqlDataReader["OfferId"];
                    product.Offer=Getoffer(offerid);
                }
            }
            return product;
        }

        public bool CreateUser(User user)
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(_connectionstring))
                {
                    sqlConnection.Open();
                    using (SqlCommand cmd = sqlConnection.CreateCommand())
                    {
                        string query = "select top 1 * from Users where Email=@email and Password=@pass";
                        cmd.CommandText = query;
                        cmd.Parameters.Add("@email", System.Data.SqlDbType.VarChar).Value = user.Email;
                        cmd.Parameters.Add("@pass", System.Data.SqlDbType.VarChar).Value = user.Password;
                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            sqlConnection.Close();
                            return false;
                        }
                    }
                    using (SqlCommand cmd = sqlConnection.CreateCommand())
                    {
                        string createus = "INSERT INTO [dbo].[Users]([FirstName],[LastName],[Email],[Address],[Mobile],[Password],[CreatedAt],[ModifiedAt])VALUES(@Fname,@LName,@Email,@address,@Mobile,@password,@createdat,@modifiedat)";
                        cmd.CommandText = createus;
                        cmd.Parameters.Add("@Fname", System.Data.SqlDbType.NVarChar).Value = user.FirstName;
                        cmd.Parameters.Add("@LName", System.Data.SqlDbType.NVarChar).Value = user.Password;
                        cmd.Parameters.Add("@Email", System.Data.SqlDbType.NVarChar).Value = user.Email;
                        cmd.Parameters.Add("@address", System.Data.SqlDbType.NVarChar).Value = user.Address;
                        cmd.Parameters.Add("@Mobile", System.Data.SqlDbType.NVarChar).Value = user.Mobile;
                        cmd.Parameters.Add("@password", System.Data.SqlDbType.NVarChar).Value = user.Password;
                        cmd.Parameters.Add("@createdat", System.Data.SqlDbType.NVarChar).Value = user.CreatedAt;
                        cmd.Parameters.Add("@modifiedat", System.Data.SqlDbType.VarChar).Value = user.ModifiedAt;
                        return cmd.ExecuteNonQuery() > 0;

                    }
                }

            }
            catch (Exception ex) {
                
            
            }
            return false;

        }

        public string ValidateLoginAndToken(string email,string password)
        {
            try
            {
                User getuser = new User();
                using (SqlConnection sqlConnection = new(_connectionstring))
                {
                    SqlCommand sqlCommand = sqlConnection.CreateCommand();
                    sqlConnection.Open();
                    string query = "select * from users where Email=@email and Password=@password";
                    sqlCommand.CommandText = query;
                    sqlCommand.Parameters.Add("@email", System.Data.SqlDbType.NVarChar).Value = email;
                    sqlCommand.Parameters.Add("@password", System.Data.SqlDbType.NVarChar).Value = password;
                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    if (!sqlDataReader.HasRows)
                        return "";

                    while (sqlDataReader.Read())
                    {
                        getuser.Id = (int)sqlDataReader["UserId"];
                        getuser.Email = (string)sqlDataReader["Email"];
                        getuser.FirstName = (string)sqlDataReader["FirstName"];
                        getuser.LastName = (string)sqlDataReader["LastName"];
                    }
                    // Jwt Authentication
                    string Jwtkey = "eyJhbGciOiJIUzI1NiJ9.ew0KICAic3ViIjogIjEyMzQ1Njc4OTAiLA0KICAibmFtZSI6ICJBbmlzaCBOYXRoIiwNCiAgImlhdCI6IDE1MTYyMzkwMjINCn0.gLB3UrMPmgoSi-XuQt9i7XWjwq-2WqhG9uoash5an8g";
                    string duration = "60";
                    var symmetrickey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Jwtkey));
                    var credentials = new SigningCredentials(symmetrickey, SecurityAlgorithms.HmacSha256);
                    var claims = new Claim[]
                    {
                    new Claim("id",getuser.Id.ToString()),
                    new Claim("Email",getuser.Email.ToString()),
                    new Claim("FirstName",getuser.FirstName.ToString()),
                    new Claim("LastName",getuser.LastName.ToString()),

                    };
                    var jwttoken = new JwtSecurityToken(
                        issuer: "localhost",
                        audience: "localhost",
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(Int32.Parse(duration)),
                        signingCredentials: credentials);
                    return new JwtSecurityTokenHandler().WriteToken(jwttoken);
                }
            }
            catch(Exception e)
            {

            }
            return "";
        }

        public void InsertReview(Review review)
        {
            using (SqlConnection sqlConnection = new(_connectionstring))
            {
                SqlCommand sqlCommand= sqlConnection.CreateCommand();
                sqlConnection.Open();
                string query = "Insert into Reviews(UserId,ProductId,Review,CreatedAt) values(@uid,@pid,@review,@creat)";
                sqlCommand.CommandText = query;
                sqlCommand.Parameters.Add("@uid",System.Data.SqlDbType.Int).Value=review.User.Id;
                sqlCommand.Parameters.Add("@pid", System.Data.SqlDbType.Int).Value = review.Product.Id;
                sqlCommand.Parameters.Add("@review", System.Data.SqlDbType.NVarChar).Value = review.Value;
                sqlCommand.Parameters.Add("@creat", System.Data.SqlDbType.NVarChar).Value = review.CreatedAt;
                sqlCommand.ExecuteNonQuery();
            }
        }

        public List<Review> Reviews(int productId)
        {
            List<Review> lis= new List<Review>();
            try
            {
                using (SqlConnection sqlConnection=new SqlConnection(_connectionstring))
                {
                    sqlConnection.Open();
                    SqlCommand sqlCommand= sqlConnection.CreateCommand();
                    string query = "select * from Reviews where ProductId=@pid";
                    sqlCommand.CommandText = query;
                    sqlCommand.Parameters.Add("@pid",System.Data.SqlDbType.Int).Value=productId;
                    SqlDataReader sqlDataReader=sqlCommand.ExecuteReader();
                    while (sqlDataReader.Read())
                    {
                        lis.Add(new Review()
                        {
                            Id = (int)sqlDataReader["ReviewId"],
                            User = GetUser((int)sqlDataReader["UserId"]),
                            Product = GetProductById(productId),
                            CreatedAt = (string)sqlDataReader["CreatedAt"],
                            Value = (string)sqlDataReader["Review"]
                        });

                    }

                }

            }
            catch(Exception e)
            {

            }
            return lis;

        }

        private User GetUser(int id)
        {
            User user = new User();
            using (SqlConnection sqlConnection = new(_connectionstring))
            {
                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlConnection.Open();
                string query = "select * from Users where UserId=@uid";
                sqlCommand.CommandText = query;
                sqlCommand.Parameters.Add("@uid",System.Data.SqlDbType.Int).Value = id;
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    user.Id = (int)sqlDataReader["UserId"];
                    user.LastName = (string)sqlDataReader["LastName"];
                    user.FirstName = (string)sqlDataReader["FirstName"];
                    user.Mobile = (string)sqlDataReader["Mobile"];
                    user.CreatedAt = (string)sqlDataReader["CreatedAt"];
                    user.ModifiedAt = (string)sqlDataReader["ModifiedAt"];                    
                }
            }
            return user;
        }

        public bool InsertCartItems(int userId, int productId)
        {
            using (SqlConnection sqlConnection = new(_connectionstring)){
                using (SqlCommand sqlCommand=sqlConnection.CreateCommand())
                {
                    sqlConnection.Open();
                    string query = "select count(*) from Carts where UserId=@Uid AND  Ordered='false'";
                    sqlCommand.CommandText = query;
                    sqlCommand.Parameters.Add("@Uid",System.Data.SqlDbType.Int).Value=userId;
                    int count=(int)sqlCommand.ExecuteScalar();
                    if (count == 0)
                    {
                        query = "INSERT INTO [dbo].[Carts] ([UserId],[Ordered],[OrderedOn])VALUES(@Usid,@Ordered,@Orderedon)";
                        sqlCommand.CommandText = query;
                        sqlCommand.Parameters.Add("@Usid", System.Data.SqlDbType.Int).Value = userId;
                        sqlCommand.Parameters.Add("@Ordered", System.Data.SqlDbType.NVarChar).Value = "false";
                        sqlCommand.Parameters.Add("@Orderedon", System.Data.SqlDbType.NVarChar).Value = "";
                        sqlCommand.ExecuteNonQuery();
                    }
                        query = "select CartId  from Carts where UserId=@useid and Ordered='false'";
                        sqlCommand.CommandText = query;
                        sqlCommand.Parameters.Add("@useid", System.Data.SqlDbType.Int).Value = userId;
                        int cartId=(int)sqlCommand.ExecuteScalar();
                        query = "Insert into CartItems(CartId,ProductId) Values (@cid,@pid)";
                        sqlCommand.CommandText = query;
                        sqlCommand.Parameters.Add("@cid", System.Data.SqlDbType.Int).Value = cartId;
                        sqlCommand.Parameters.Add("@pid", System.Data.SqlDbType.Int).Value = productId;
                        sqlCommand.ExecuteNonQuery();
                    
                }
                return true;
                //using (SqlCommand sqlCommandselect = sqlConnection.CreateCommand())
                //{
                //    sqlConnection.Open();
                //   string query = "select CartId  from Carts where UserId=@uid and Ordered='false'";
                //    sqlCommandselect.CommandText = query;

                //}

            }
        }

        public Cart GetActiveCartofUser(int uid)
        {
            Cart cart = new Cart();
            using (SqlConnection sqlConnection=new SqlConnection(_connectionstring))
            {
                SqlCommand sqlCommand=sqlConnection.CreateCommand();
                sqlConnection.Open();
                string query = "select count(*) from Carts where UserId=@Uid and Ordered='false'";
                sqlCommand.CommandText = query;
                sqlCommand.Parameters.Add("@Uid", System.Data.SqlDbType.Int).Value = uid;
                int  count=(int) sqlCommand.ExecuteScalar();
                if (count == 0)
                {
                    return cart;
                }
                query = "select CartId from Carts where UserId=@Userid and Ordered='false'";
                sqlCommand.CommandText = query;
                sqlCommand.Parameters.Add("@Userid", System.Data.SqlDbType.Int).Value = uid;
                int cartid = (int)sqlCommand.ExecuteScalar();
                query = "select * from CartItems where CartId=@cartId";
                sqlCommand.CommandText = query;
                sqlCommand.Parameters.Add("@cartId", System.Data.SqlDbType.Int).Value = cartid;
                SqlDataReader sqlDataReader=sqlCommand.ExecuteReader();
                while (sqlDataReader.Read()) 
                {
                    CartItem cartItem = new CartItem()
                    {
                        Id = (int)sqlDataReader["CartItemId"],
                        Product = GetProductById((int)sqlDataReader["ProductId"])
                    };
                    cart.CartItems.Add(cartItem);                
                }
                cart.Id= cartid;
                cart.User = GetUser(uid);
                cart.Ordered = false;
                cart.OrderedOn = "";
            }
            return cart;
        }

        public Cart GetCart(int cartId)
        {
            Cart cart = new Cart();
            using (SqlConnection sqlConnection=new SqlConnection(_connectionstring))
            {
                SqlCommand sqlCommand=sqlConnection.CreateCommand();
                sqlConnection.Open();
                string query = "Select * from CartItems where CartId=@cardId";
                sqlCommand.CommandText = query;
                sqlCommand.Parameters.Add("@cardId", System.Data.SqlDbType.Int).Value=cartId;
                SqlDataReader sqlDataReader= sqlCommand.ExecuteReader();
                while (sqlDataReader.Read()) {
                    CartItem item = new CartItem()
                    {
                        Id = (int)sqlDataReader["CartItemId"],
                        Product = GetProductById((int)sqlDataReader["ProductId"])
                    };
                    cart.CartItems.Add(item);                
                }
                sqlDataReader.Close();
                query = "select * from Carts where CartId=@crtid";
                sqlCommand.CommandText = query;
                sqlCommand.Parameters.Add("@crtid",System.Data.SqlDbType.Int).Value= cartId;
                sqlDataReader= sqlCommand.ExecuteReader();
                while (sqlDataReader.Read()) {
                    cart.Id = cartId;
                    cart.User = GetUser((int)sqlDataReader["UserId"]);
                    cart.Ordered = bool.Parse((string)sqlDataReader["Ordered"]);
                    cart.OrderedOn = (string)sqlDataReader["OrderedOn"];
                }
                sqlDataReader.Close();
            }
            return cart;
        }

        public List<Cart> GetAllPreviousCartsofUser(int userid)
        {
            List<Cart> carts = new List<Cart>();
            using (SqlConnection sqlConnection=new SqlConnection(_connectionstring))
            {
                sqlConnection.Open();
                SqlCommand sqlCommand= sqlConnection.CreateCommand();
                string query = "select CartId from Carts where UserId=@userId and Ordered='true'";
                sqlCommand.CommandText = query;
                sqlCommand.Parameters.Add("@userId", System.Data.SqlDbType.Int).Value=userid;
                SqlDataReader reader=sqlCommand.ExecuteReader();
                while (reader.Read()) {
                    var cartId = (int)reader["CartId"];
                    carts.Add(GetCart(cartId)); 
                }
            }
            return carts;
        }

        public List<PaymentMethod> GetPaymentMethods()
        {
            List<PaymentMethod> paymentMethods = new List<PaymentMethod>();
            using (SqlConnection sqlConnection=new SqlConnection(_connectionstring))
            {
                sqlConnection.Open();
                SqlCommand cmd= sqlConnection.CreateCommand();
                string query = "select * from PaymentMethods";
                cmd.CommandText = query;
                SqlDataReader sqlDataReader=cmd.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    paymentMethods.Add(new PaymentMethod()
                    {
                        Id = (int)sqlDataReader["PaymentMethodId"],
                        Type=(string)sqlDataReader["Type"],
                        Provider = (string)sqlDataReader["Provider"],
                        Available = bool.Parse((string)sqlDataReader["Available"]),
                        Reason = (string)sqlDataReader["Reason"]
                    });
                }
            }
            return paymentMethods;
        }
    }
}
