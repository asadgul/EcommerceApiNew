using ECommerce.API.Models;

namespace ECommerceWebApi.DataAccessServices
{
    public interface IDataAccess
    {
        public List<ProductCategory> GetProductCategories();
        public ProductCategory GetProductCategory(int id);
        public Offer Getoffer(int id);
        public Product GetProductById(int id);
        public List<Product> GetProducts(string category, string subcategory, int count);
        public bool CreateUser(User user);
        public string ValidateLoginAndToken(string email,string password);
        public void InsertReview(Review review);
        public List<Review> Reviews(int productId);
        public bool InsertCartItems(int userId,int productId);
        public Cart GetActiveCartofUser(int uid);
        public Cart GetCart(int cartId);
        public List<Cart> GetAllPreviousCartsofUser(int userid);

        public List<PaymentMethod> GetPaymentMethods();


    }
}
