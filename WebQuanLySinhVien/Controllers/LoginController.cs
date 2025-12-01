using Microsoft.AspNetCore.Mvc;
using WebQuanLySinhVien.Helpers;
using System.Data;

public class LoginController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(string taikhoan, string matkhau, string site)
    {
        // 1. Lưu site muốn kết nối
        HttpContext.Session.SetString("CurrentSite", site);

        // 2. Kiểm tra SQL (Tránh SQL Injection bằng cách ghép chuỗi đơn giản cho demo, tốt nhất dùng SP)
        string sql = $"SELECT * FROM DANGNHAP WHERE TAIKHOAN='{taikhoan}' AND MATKHAU='{matkhau}'";
        DataRow user = SQLHelper.GetRow(sql, site);

        if (user != null)
        {
            // Đăng nhập thành công -> Lưu Session
            HttpContext.Session.SetString("User", taikhoan);
            HttpContext.Session.SetString("Role", user["CHUCVU"].ToString());

            return RedirectToAction("Index", "Home");
        }
        else
        {
            ViewBag.Error = "Sai tài khoản hoặc mật khẩu!";
            return View("Index");
        }
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }
}