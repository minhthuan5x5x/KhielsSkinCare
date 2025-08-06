using KhielsSkincare.Models;
using KhielsSkincare.Repository;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


public class CartSumViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        // Lấy giỏ hàng từ session
        var cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();

        int productCount = cart.Count;

        // Truyền tổng số lượng sản phẩm vào view
        return View(productCount);
    }
}