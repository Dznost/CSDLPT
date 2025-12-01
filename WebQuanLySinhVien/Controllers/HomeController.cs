using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using WebQuanLySinhVien.Helpers;

namespace WebQuanLySinhVien.Controllers
{
    public class HomeController : Controller
    {
        // ============================================================
        // 1. QUẢN LÝ SINH VIÊN
        // ============================================================
        public IActionResult Index(string site = "", bool all = false)
        {
            if (HttpContext.Session.GetString("User") == null) return RedirectToAction("Index", "Login");

            // 1. Xử lý chuyển Site (Giữ kết nối khi F5)
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

            // 2. Logic Xem: all=true (Toàn trường - View), all=false (Khoa nhà - Table)
            string query = all ? "SELECT * FROM v_SINHVIEN_ALL" : "SELECT * FROM SINHVIEN";
            DataTable dt = SQLHelper.GetData(query, currentSite);

            return View(dt);
        }

        [HttpPost]
        // [QUAN TRỌNG] Đã bỏ tham số 'masv' vì hệ thống tự sinh
        public IActionResult ThemSV(string hoten, string malop, string makh)
        {
            string currentSite = HttpContext.Session.GetString("CurrentSite") ?? "CNTT";

            try
            {
                SqlParameter[] p = {
                    // Truyền chuỗi rỗng để SQL tự tính toán mã (VD: 25CNTT00001)
                    new SqlParameter("@MASV", ""),
                    new SqlParameter("@HOTEN", hoten),
                    new SqlParameter("@MALOP", malop),
                    new SqlParameter("@MAKH", makh)
                };

                SQLHelper.ExecuteSP("SP_THEMSINHVIEN", p, currentSite);

                TempData["Success"] = $"Thêm sinh viên thành công! Mã SV đã được hệ thống tự tạo.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult XoaSV(string masv, string makh)
        {
            string currentSite = HttpContext.Session.GetString("CurrentSite") ?? "CNTT";
            try
            {
                SqlParameter[] p = { new SqlParameter("@MASV", masv), new SqlParameter("@MAKH", makh) };
                SQLHelper.ExecuteSP("SP_XOASINHVIEN", p, currentSite);
                TempData["Success"] = "Xóa thành công!";
            }
            catch (Exception ex) { TempData["Error"] = "Lỗi: " + ex.Message; }
            return RedirectToAction("Index");
        }

        // --- SỬA SINH VIÊN ---
        [HttpGet]
        public IActionResult SuaSV(string id)
        {
            if (HttpContext.Session.GetString("User") == null) return RedirectToAction("Index", "Login");
            string site = HttpContext.Session.GetString("CurrentSite") ?? "CNTT";

            DataTable dt = SQLHelper.GetData($"SELECT * FROM v_SINHVIEN_ALL WHERE MASV = '{id}'", site);
            if (dt.Rows.Count > 0) return View(dt.Rows[0]);

            TempData["Error"] = "Không tìm thấy sinh viên!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult LuuSuaSV(string masv, string hoten, string malop, string makh)
        {
            string site = HttpContext.Session.GetString("CurrentSite") ?? "CNTT";
            try
            {
                SqlParameter[] p = {
                    new SqlParameter("@MASV", masv), new SqlParameter("@HOTEN", hoten),
                    new SqlParameter("@MALOP", malop), new SqlParameter("@MAKH", makh)
                };
                SQLHelper.ExecuteSP("SP_SUASINHVIEN", p, site);
                TempData["Success"] = "Cập nhật thông tin thành công!";
            }
            catch (Exception ex) { TempData["Error"] = "Lỗi: " + ex.Message; }
            return RedirectToAction("Index");
        }

        // ============================================================
        // 2. QUẢN LÝ LỚP HỌC
        // ============================================================
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
            try
            {
                SqlParameter[] p = {
                    new SqlParameter("@MALOP", malop), new SqlParameter("@TENLOP", tenlop), new SqlParameter("@MAKH", makh)
                };
                SQLHelper.ExecuteSP("SP_THEMLOP", p, currentSite);
                TempData["Success"] = "Thêm lớp thành công!";
            }
            catch (Exception ex) { TempData["Error"] = "Lỗi: " + ex.Message; }
            return RedirectToAction("Lop");
        }

        [HttpPost]
        public IActionResult XoaLop(string malop, string makh)
        {
            string currentSite = HttpContext.Session.GetString("CurrentSite") ?? "CNTT";
            try
            {
                SqlParameter[] p = { new SqlParameter("@MALOP", malop), new SqlParameter("@MAKH", makh) };
                SQLHelper.ExecuteSP("SP_XOALOP", p, currentSite);
                TempData["Success"] = "Xóa lớp thành công!";
            }
            catch (Exception ex) { TempData["Error"] = "Lỗi: " + ex.Message; }
            return RedirectToAction("Lop");
        }

        // --- SỬA LỚP ---
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
            try
            {
                SqlParameter[] p = {
                    new SqlParameter("@MALOP", malop), new SqlParameter("@TENLOP", tenlop), new SqlParameter("@MAKH", makh)
                };
                SQLHelper.ExecuteSP("SP_SUALOP", p, site);
                TempData["Success"] = "Sửa lớp thành công!";
            }
            catch (Exception ex) { TempData["Error"] = "Lỗi: " + ex.Message; }
            return RedirectToAction("Lop");
        }

        // ============================================================
        // 3. QUẢN LÝ MÔN HỌC
        // ============================================================
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
            try
            {
                SqlParameter[] p = {
                    new SqlParameter("@MAMH", mamh), new SqlParameter("@TENMH", tenmh), new SqlParameter("@MAKH", makh)
                };
                SQLHelper.ExecuteSP("SP_THEMMONHOC", p, currentSite);
                TempData["Success"] = "Thêm môn học thành công!";
            }
            catch (Exception ex) { TempData["Error"] = "Lỗi: " + ex.Message; }
            return RedirectToAction("MonHoc");
        }

        [HttpPost]
        public IActionResult XoaMonHoc(string mamh, string makh)
        {
            string currentSite = HttpContext.Session.GetString("CurrentSite") ?? "CNTT";
            try
            {
                SqlParameter[] p = { new SqlParameter("@MAMH", mamh), new SqlParameter("@MAKH", makh) };
                SQLHelper.ExecuteSP("SP_XOAMONHOC", p, currentSite);
                TempData["Success"] = "Xóa môn học thành công!";
            }
            catch (Exception ex) { TempData["Error"] = "Lỗi: " + ex.Message; }
            return RedirectToAction("MonHoc");
        }

        // --- SỬA MÔN HỌC ---
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
            try
            {
                SqlParameter[] p = {
                    new SqlParameter("@MAMH", mamh), new SqlParameter("@TENMH", tenmh), new SqlParameter("@MAKH", makh)
                };
                SQLHelper.ExecuteSP("SP_SUAMONHOC", p, site);
                TempData["Success"] = "Sửa môn học thành công!";
            }
            catch (Exception ex) { TempData["Error"] = "Lỗi: " + ex.Message; }
            return RedirectToAction("MonHoc");
        }

        // ============================================================
        // 4. QUẢN LÝ ĐIỂM
        // ============================================================
        public IActionResult Diem(bool all = false)
        {
            if (HttpContext.Session.GetString("User") == null) return RedirectToAction("Index", "Login");
            string currentSite = HttpContext.Session.GetString("CurrentSite") ?? "CNTT";
            ViewBag.CurrentSite = currentSite;
            ViewBag.IsViewAll = all;

            // 1. Lấy dữ liệu Bảng Điểm (Xem All hoặc Xem Local)
            string queryDiem = all ? "SELECT * FROM v_DIEM_ALL" : "SELECT * FROM DIEM";
            DataTable dt = SQLHelper.GetData(queryDiem, currentSite);

            // 2. Lấy danh sách Môn học cho ComboBox
            // [SỬA ĐỔI] Chỉ lấy môn học trong bảng MONHOC (Local) để người dùng chỉ chọn được môn khoa mình
            // Nếu muốn xem môn toàn trường để nhập thì đổi thành v_MONHOC_ALL, nhưng ở đây dùng MONHOC để an toàn.
            DataTable dtMonHoc = SQLHelper.GetData("SELECT * FROM MONHOC", currentSite);
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

        // --- SỬA ĐIỂM ---
        [HttpGet]
        public IActionResult SuaDiem(string masv, string mamh)
        {
            if (HttpContext.Session.GetString("User") == null) return RedirectToAction("Index", "Login");
            string site = HttpContext.Session.GetString("CurrentSite") ?? "CNTT";

            // Tìm điểm cũ
            string query = $"SELECT * FROM v_DIEM_ALL WHERE MASV='{masv}' AND MAMH='{mamh}'";
            DataTable dt = SQLHelper.GetData(query, site);

            if (dt.Rows.Count > 0) return View(dt.Rows[0]);

            TempData["Error"] = "Không tìm thấy điểm!";
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