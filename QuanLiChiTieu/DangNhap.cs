using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLiChiTieu
{
    public partial class DangNhap : Form
    {
        SchoolDBEntities minh = new SchoolDBEntities();
        public DangNhap()
        {
            InitializeComponent();

        }

        private void btnDangnhap_Click(object sender, EventArgs e)
        {
            string connectionString = "Data Source=DESKTOP-65842VU\\SQLEXPRESS;Initial Catalog=SchoolDB;Integrated Security=True";
            string email = txtTK.Text;  // Lấy email từ trường txtTK
            string password = txtMK.Text;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                // Sử dụng tham số @Email thay vì @Username
                string query = "SELECT COUNT(1) FROM Users WHERE Email=@Email AND Password=@Password";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Password", password);

                int count = (int)cmd.ExecuteScalar();

                if (count == 1)
                {
                    MessageBox.Show("Đăng nhập thành công!");

                    // Khởi tạo context và truyền vào TrangChu
                    SchoolDBEntities1 context = new SchoolDBEntities1();
                    TrangChu trangChuForm = new TrangChu(context);
                    trangChuForm.Show();

                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Email hoặc mật khẩu không đúng.");
                }
            }
        }

        private void btnDangKy_Click(object sender, EventArgs e)
        {
            DangKy dangkyForm = new DangKy();
            dangkyForm.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnDangKy1_Click(object sender, EventArgs e)
        {
            DangKy dangkyForm = new DangKy();
            dangkyForm.Show();
        }
    }
}
