using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using WebQuanLySinhVien.Helpers;

namespace WebQuanLySinhVien.Controllers
{
    public class HomeController : Controller
    {
        // ==========================================
        // 1. QUẢN LÝ SINH VIÊN
        // ==========================================
        public IActionResult Index(string site = "", bool all = false)
        {
            if (HttpContext.Session.GetString("User") == null) return RedirectToAction("Index", "Login");

            // Xử lý chuyển Site
            string currentSite = HttpContext.Session.GetString("CurrentSite");
            if (!string.IsNullOrEmpty(site))
            {
                currentSite = site;
                HttpContext.Session.SetString("CurrentSite", site);
            }
            if (string.IsNullOrEmpty(currentSite))
            {
                currentSite = "CNTT";
                HttpContext.Session.SetString("CurrentSite", "CNTT");
            }

            ViewBag.CurrentSite = currentSite;
            ViewBag.IsViewAll = all; // Giữ trạng thái nút gạt

            // Logic Xem: all=true (Toàn trường - View), all=false (Khoa nhà - Table)
            string query = all ? "SELECT * FROM v_SINHVIEN_ALL" : "SELECT * FROM SINHVIEN";
            DataTable dt = SQLHelper.GetData(query, currentSite);

            return View(dt);
        }

        [HttpPost]
        public IActionResult ThemSV(string masv, string hoten, string malop, string makh)
        {
            string currentSite = HttpContext.Session.GetString("CurrentSite") ?? "CNTT";
            SqlParameter[] p = {
                new SqlParameter("@MASV", masv), new SqlParameter("@HOTEN", hoten),
                new SqlParameter("@MALOP", malop), new SqlParameter("@MAKH", makh)
            };
            SQLHelper.ExecuteSP("SP_THEMSINHVIEN", p, currentSite);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult XoaSV(string masv, string makh)
        {
            string currentSite = HttpContext.Session.GetString("CurrentSite") ?? "CNTT";
            SqlParameter[] p = { new SqlParameter("@MASV", masv), new SqlParameter("@MAKH", makh) };
            SQLHelper.ExecuteSP("SP_XOASINHVIEN", p, currentSite);
            return RedirectToAction("Index");
        }

        // --- CHỨC NĂNG SỬA SINH VIÊN ---
        [HttpGet]
        public IActionResult SuaSV(string id)
        {
            if (HttpContext.Session.GetString("User") == null) return RedirectToAction("Index", "Login");
            string site = HttpContext.Session.GetString("CurrentSite") ?? "CNTT";
            // Lấy từ View toàn cục để tìm thấy SV dù ở site khác
            DataTable dt = SQLHelper.GetData($"SELECT * FROM v_SINHVIEN_ALL WHERE MASV = '{id}'", site);
            if (dt.Rows.Count > 0) return View(dt.Rows[0]);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult LuuSuaSV(string masv, string hoten, string malop, string makh)
        {
            string site = HttpContext.Session.GetString("CurrentSite") ?? "CNTT";
            SqlParameter[] p = {
                new SqlParameter("@MASV", masv), new SqlParameter("@HOTEN", hoten),
                new SqlParameter("@MALOP", malop), new SqlParameter("@MAKH", makh)
            };
            SQLHelper.ExecuteSP("SP_SUASINHVIEN", p, site);
            return RedirectToAction("Index");
        }

        // ==========================================
        // 2. QUẢN LÝ LỚP HỌC
        // ==========================================
        public IActionResult Lop(bool all = false)
        {
            if (HttpContext.Session.GetString("User") == null) return RedirectToAction("Index", "Login");
            string currentSite = HttpContext.Session.GetString("CurrentSite") ?? "CNTT";
            ViewBag.CurrentSite = currentSite;
            ViewBag.IsViewAll = all;

            string query = all ? "SELECT * FROM v_LOP_ALL" : "SELECT * FROM LOP";
            DataTable dt = SQLHelper.GetData(query, currentSite);
            return View(dt);
        }

        [HttpPost]
        public IActionResult ThemLop(string malop, string tenlop, string makh)
        {
            string currentSite = HttpContext.Session.GetString("CurrentSite") ?? "CNTT";
            SqlParameter[] p = {
                new SqlParameter("@MALOP", malop), new SqlParameter("@TENLOP", tenlop), new SqlParameter("@MAKH", makh)
            };
            SQLHelper.ExecuteSP("SP_THEMLOP", p, currentSite);
            return RedirectToAction("Lop");
        }

        [HttpPost]
        public IActionResult XoaLop(string malop, string makh)
        {
            string currentSite = HttpContext.Session.GetString("CurrentSite") ?? "CNTT";
            SqlParameter[] p = { new SqlParameter("@MALOP", malop), new SqlParameter("@MAKH", makh) };
            SQLHelper.ExecuteSP("SP_XOALOP", p, currentSite);
            return RedirectToAction("Lop");
        }

        // --- CHỨC NĂNG SỬA LỚP ---
        [HttpGet]
        public IActionResult SuaLop(string id)
        {
            if (HttpContext.Session.GetString("User") == null) return RedirectToAction("Index", "Login");
            string site = HttpContext.Session.GetString("CurrentSite") ?? "CNTT";
            DataTable dt = SQLHelper.GetData($"SELECT * FROM v_LOP_ALL WHERE MALOP = '{id}'", site);
            if (dt.Rows.Count > 0) return View(dt.Rows[0]);
            return RedirectToAction("Lop");
        }

        [HttpPost]
        public IActionResult LuuSuaLop(string malop, string tenlop, string makh)
        {
            string site = HttpContext.Session.GetString("CurrentSite") ?? "CNTT";
            SqlParameter[] p = {
                new SqlParameter("@MALOP", malop), new SqlParameter("@TENLOP", tenlop), new SqlParameter("@MAKH", makh)
            };
            SQLHelper.ExecuteSP("SP_SUALOP", p, site);
            return RedirectToAction("Lop");
        }

        // ==========================================
        // 3. QUẢN LÝ MÔN HỌC
        // ==========================================
        public IActionResult MonHoc(bool all = false)
        {
            if (HttpContext.Session.GetString("User") == null) return RedirectToAction("Index", "Login");
            string currentSite = HttpContext.Session.GetString("CurrentSite") ?? "CNTT";
            ViewBag.CurrentSite = currentSite;
            ViewBag.IsViewAll = all;

            string query = all ? "SELECT * FROM v_MONHOC_ALL" : "SELECT * FROM MONHOC";
            DataTable dt = SQLHelper.GetData(query, currentSite);
            return View(dt);
        }

        [HttpPost]
        public IActionResult ThemMonHoc(string mamh, string tenmh, string makh)
        {
            string currentSite = HttpContext.Session.GetString("CurrentSite") ?? "CNTT";
            SqlParameter[] p = {
                new SqlParameter("@MAMH", mamh), new SqlParameter("@TENMH", tenmh), new SqlParameter("@MAKH", makh)
            };
            SQLHelper.ExecuteSP("SP_THEMMONHOC", p, currentSite);
            return RedirectToAction("MonHoc");
        }

        [HttpPost]
        public IActionResult XoaMonHoc(string mamh, string makh)
        {
            string currentSite = HttpContext.Session.GetString("CurrentSite") ?? "CNTT";
            SqlParameter[] p = { new SqlParameter("@MAMH", mamh), new SqlParameter("@MAKH", makh) };
            SQLHelper.ExecuteSP("SP_XOAMONHOC", p, currentSite);
            return RedirectToAction("MonHoc");
        }

        // --- CHỨC NĂNG SỬA MÔN HỌC ---
        [HttpGet]
        public IActionResult SuaMonHoc(string id)
        {
            if (HttpContext.Session.GetString("User") == null) return RedirectToAction("Index", "Login");
            string site = HttpContext.Session.GetString("CurrentSite") ?? "CNTT";
            DataTable dt = SQLHelper.GetData($"SELECT * FROM v_MONHOC_ALL WHERE MAMH = '{id}'", site);
            if (dt.Rows.Count > 0) return View(dt.Rows[0]);
            return RedirectToAction("MonHoc");
        }

        [HttpPost]
        public IActionResult LuuSuaMonHoc(string mamh, string tenmh, string makh)
        {
            string site = HttpContext.Session.GetString("CurrentSite") ?? "CNTT";
            SqlParameter[] p = {
                new SqlParameter("@MAMH", mamh), new SqlParameter("@TENMH", tenmh), new SqlParameter("@MAKH", makh)
            };
            SQLHelper.ExecuteSP("SP_SUAMONHOC", p, site);
            return RedirectToAction("MonHoc");
        }

        // ==========================================
        // 4. QUẢN LÝ ĐIỂM
        // ==========================================
        public IActionResult Diem(bool all = false)
        {
            if (HttpContext.Session.GetString("User") == null) return RedirectToAction("Index", "Login");
            string currentSite = HttpContext.Session.GetString("CurrentSite") ?? "CNTT";
            ViewBag.CurrentSite = currentSite;
            ViewBag.IsViewAll = all;

            // 1. Lấy dữ liệu điểm
            string query = all ? "SELECT * FROM v_DIEM_ALL" : "SELECT * FROM DIEM";
            DataTable dt = SQLHelper.GetData(query, currentSite);

            // 2. Lấy danh sách Môn học (để hiện ComboBox nhập điểm)
            // Luôn lấy toàn cục để có đủ môn
            DataTable dtMonHoc = SQLHelper.GetData("SELECT * FROM v_MONHOC_ALL", currentSite);
            ViewBag.ListMonHoc = dtMonHoc;

            return View(dt);
        }

        [HttpPost]
        public IActionResult ThemDiem(string masv, string mamh, double diemso)
        {
            string currentSite = HttpContext.Session.GetString("CurrentSite") ?? "CNTT";
            try
            {
                SqlParameter[] p = {
                    new SqlParameter("@MASV", masv), new SqlParameter("@MAMH", mamh), new SqlParameter("@DIEM", diemso)
                };
                SQLHelper.ExecuteSP("SP_THEMDIEM", p, currentSite);
                TempData["Success"] = "Thêm điểm thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi: " + ex.Message;
            }
            return RedirectToAction("Diem");
        }

        [HttpPost]
        public IActionResult XoaDiem(string masv, string mamh)
        {
            string currentSite = HttpContext.Session.GetString("CurrentSite") ?? "CNTT";
            try
            {
                SqlParameter[] p = { new SqlParameter("@MASV", masv), new SqlParameter("@MAMH", mamh) };
                SQLHelper.ExecuteSP("SP_XOADIEM", p, currentSite);
                TempData["Success"] = "Xóa điểm thành công!";
            }
            catch (Exception ex) { TempData["Error"] = "Lỗi: " + ex.Message; }
            return RedirectToAction("Diem");
        }

        // --- CHỨC NĂNG SỬA ĐIỂM ---
        [HttpGet]
        public IActionResult SuaDiem(string masv, string mamh)
        {
            if (HttpContext.Session.GetString("User") == null) return RedirectToAction("Index", "Login");
            string site = HttpContext.Session.GetString("CurrentSite") ?? "CNTT";
            DataTable dt = SQLHelper.GetData($"SELECT * FROM v_DIEM_ALL WHERE MASV='{masv}' AND MAMH='{mamh}'", site);
            if (dt.Rows.Count > 0) return View(dt.Rows[0]);
            return RedirectToAction("Diem");
        }

        [HttpPost]
        public IActionResult LuuSuaDiem(string masv, string mamh, double diemso)
        {
            string site = HttpContext.Session.GetString("CurrentSite") ?? "CNTT";
            try
            {
                SqlParameter[] p = {
                    new SqlParameter("@MASV", masv), new SqlParameter("@MAMH", mamh), new SqlParameter("@DIEM", diemso)
                };
                SQLHelper.ExecuteSP("SP_SUADIEM", p, site);
                TempData["Success"] = "Sửa điểm thành công!";
            }
            catch (Exception ex) { TempData["Error"] = "Lỗi: " + ex.Message; }
            return RedirectToAction("Diem");
        }
    }
}