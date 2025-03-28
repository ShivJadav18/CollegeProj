using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Razorpay.Api;

namespace ElectroSphereProj.Controllers;

public class PaymentController : Controller
{
    private readonly IConfiguration _configuration;

    public PaymentController(IConfiguration configuration)
    {
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
            return RedirectToAction("Dashboardpage","Dashboard");
        }catch(Exception e){
            TempData["error"] = "payment failure.";
             return RedirectToAction("Dashboardpage","Dashboard");
        }

        return View();
    }

    

}