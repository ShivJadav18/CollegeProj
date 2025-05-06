using System.IdentityModel.Tokens.Jwt;
using ElectroSphereProj.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Razorpay.Api;

namespace ElectroSphereProj.Controllers;

public class PaymentController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly ElectronicDataBaseContext _context;

    public PaymentController(IConfiguration configuration , ElectronicDataBaseContext context)
    {
        _context = context;
        _configuration = configuration;
    }
    
    public IActionResult InitiatePayment(string amount)
    {
        var key = _configuration.GetSection("Razor").GetChildren().FirstOrDefault(config => config.Key == "RazorKey").Value;
        var secret = _configuration.GetSection("Razor").GetChildren().FirstOrDefault(config => config.Key == "RazorSecret").Value;

        RazorpayClient razorpayClient = new RazorpayClient(key, secret);
        Dictionary<string, object> options = new Dictionary<string, object> { };

        options.Add("amount", Convert.ToDecimal(amount) * 100);
        options.Add("currency", "INR");

        Razorpay.Api.Order order = razorpayClient.Order.Create(options);
        ViewBag.orderId = order["id"].ToString();
        ViewBag.key = key;

        return View("CheckoutView");
    }

    public IActionResult CheckoutView(){
        return View();
    }

    [HttpPost]
    public IActionResult CheckoutView(string razorpay_payment_id,string razorpay_order_id,string razorpay_signature){
        Dictionary<string,string> options = new Dictionary<string, string>{};

        options.Add("razorpay_payment_id",razorpay_payment_id);
        options.Add("razorpay_order_id",razorpay_order_id);
        options.Add("razorpay_signature",razorpay_signature);

        try{
            Utils.verifyPaymentSignature(options);
            TempData["success"] = "successfully completed payment.";
            string token = Request.Cookies["jwtCookie"];
            int CustomerId = Convert.ToInt32(GetClaimValueHelper(token, "Userid"));
            List<Cartdetailtable> cartdetailtables = _context.Cartdetailtables.Include(c=>c.Item).Where(c => c.Customerid == CustomerId).ToList();

            Data.Order order = new Data.Order {};
            List<Ordertoitem> ordertoitems = new List<Ordertoitem>{};
            order.CustomerId = CustomerId;
            order.Updatedby = 1;
            order.Createdby = 1;
            order.Createdat = DateTime.Now;
            order.Updatedat = DateTime.Now;
            order.Orderstatus = "in progress";
            order.Ordertype = "online";
            order.Orderdate = DateTime.Now;
            order.Totalamount = 0;
            _context.Add(order);
            _context.SaveChanges();

            foreach(Cartdetailtable cartdetailtable in cartdetailtables){
                Ordertoitem ordertoitem = new Ordertoitem{};
                ordertoitem.Amount = cartdetailtable.Item.Rate;
                ordertoitem.Quantity = cartdetailtable.Itemqun;
                ordertoitem.ItemId = cartdetailtable.Itemid;
                ordertoitem.OrderId = order.OrderId;
                ordertoitem.Status = "in progress";
                ordertoitem.Instruction = "";
                order.Totalamount = order.Totalamount + (cartdetailtable.Itemqun * cartdetailtable.Item.Rate);
                _context.Add(ordertoitem);
                _context.SaveChanges();
            }
            Data.Payment payment = new Data.Payment{};
            payment.OrderId = order.OrderId;
            payment.Amount = order.Totalamount;
            payment.Createdat = order.Createdat;
            payment.Updatedat = order.Updatedat;
            payment.Paymentmethod = "Online";
            payment.Paymentstatus = "success";

            _context.Add(payment);
            _context.SaveChanges();

            _context.RemoveRange(cartdetailtables);
            _context.SaveChanges();

            return RedirectToAction("Dashboardpage","Dashboard");
        }catch(Exception e){
            TempData["error"] = "payment failure.";
             return RedirectToAction("Dashboardpage","Dashboard");
        }

        return View();
    }

    private string GetClaimValueHelper(string token, string claimType)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var jwtToken = tokenHandler.ReadJwtToken(token);

        var claim = jwtToken.Claims.FirstOrDefault(c => c.Type == claimType);

        return claim.Value;
    }

    

}