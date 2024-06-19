using ECommerce.API.Models;
using ECommerceWebApi.DataAccessServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingController : ControllerBase
    {
        private readonly IDataAccess _dataaccess;
        public ShoppingController(IDataAccess dataAccess)
        {
            _dataaccess = dataAccess;
            
        }
        [HttpGet("GetProductsCategories")]
        public IActionResult Get()
        {
            return Ok(_dataaccess.GetProductCategories());
        
        }
        [HttpGet("GetProducts")]
        public IActionResult GetProducts(string category,string subcategory,int count)
        {
            return Ok(_dataaccess.GetProducts(category,subcategory,count));

        }
        [HttpGet("GetProductById/{id}")]
        public IActionResult GetProductById(int id)
        {
            return Ok(_dataaccess.GetProductById(id));
        }
        [HttpPost("RegisterUser")]
        public IActionResult RegisterUser([FromBody] User user)
        {
            user.CreatedAt = DateTime.Now.ToString();
            user.ModifiedAt= DateTime.Now.ToString();
            return Ok(_dataaccess.CreateUser(user));
        }
        [HttpPost("LoginUser")]
        public IActionResult LoginUser([FromBody] User user) {
            return Ok(_dataaccess.ValidateLoginAndToken(user.Email, user.Password));
        
        }
        [HttpPost("Review")]
        public IActionResult Review([FromBody] Review review)
        {
            review.CreatedAt = DateTime.Now.ToString();
            _dataaccess.InsertReview(review);
            return Ok("Inserted");
        }
        [HttpGet("GetReviews/{id}")]
        public IActionResult GetReviews(int id)
        {
            return Ok(_dataaccess.Reviews(id));
        }
        [HttpPost("InsertCart/{userId}/{productId}")]
        public IActionResult InsertCart(int userId,int productId)
        {
            return Ok(_dataaccess.InsertCartItems(userId, productId));
        }
        [HttpGet("GetActiveCart/{userId}")]
        public IActionResult GetActiveCart(int userId)
        {
            return Ok(_dataaccess.GetActiveCartofUser(userId));
        }

        [HttpGet("GetAllPreviousCartsofUser/{userId}")]
        public IActionResult GetAllPreviousCartsofUser(int userId)
        {
            return Ok(_dataaccess.GetAllPreviousCartsofUser(userId));
        }

        [HttpGet("GetAllPaymentMethods")]
        public IActionResult GetAllPaymentMethods()
        {
            return Ok(_dataaccess.GetPaymentMethods());
        }
    }
}
