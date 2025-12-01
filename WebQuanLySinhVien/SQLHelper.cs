using System.Data;
using System.Data.SqlClient;

namespace WebQuanLySinhVien.Helpers
{
    public class SQLHelper
    {
        // 1. Lấy chuỗi kết nối theo Site
        public static string GetConnectionString(string siteName)
        {
            string ip = "";
            switch (siteName)
            {
                case "CNTT": ip = "26.124.132.99"; break; // IP Kiên
                case "VT": ip = "26.91.184.231"; break; // IP Nhi
                case "KT": ip = "26.138.92.254"; break; // IP Bảo
                default: ip = "26.124.132.99"; break; // Mặc định
            }
            return $"Data Source={ip},1433;Initial Catalog=QLDSV_PT;User ID=Admin_PT;Password=123456;TrustServerCertificate=True";
        }

        // 2. Lấy dữ liệu dạng Bảng (DataTable) - Dùng cho hiển thị danh sách
        public static DataTable GetData(string query, string siteName)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(GetConnectionString(siteName)))
                {
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    da.Fill(dt);
                }
            }
            catch { /* Xử lý lỗi kết nối nếu cần */ }
            return dt;
        }

        // 3. [MỚI] Lấy 1 dòng dữ liệu (DataRow) - Dùng cho Login, Sửa
        // --> ĐÂY LÀ HÀM BẠN ĐANG THIẾU
        public static DataRow GetRow(string query, string siteName)
        {
            DataTable dt = GetData(query, siteName);
            if (dt.Rows.Count > 0) return dt.Rows[0];
            return null;
        }

        // 4. Thực thi thủ tục (INSERT/UPDATE/DELETE)
        public static void ExecuteSP(string spName, SqlParameter[] p, string siteName)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString(siteName)))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(spName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                if (p != null) cmd.Parameters.AddRange(p);
                cmd.ExecuteNonQuery(); // Nếu lỗi SQL sẽ văng Exception ra để Controller bắt
            }
        }
    }
}