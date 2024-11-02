using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLiChiTieu
{
    public partial class DangKy : Form
    {
        private SchoolDBEntities dBEntities;
        SchoolDBEntities minh = new SchoolDBEntities();
        public DangKy()
        {
            dBEntities = new SchoolDBEntities();
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string hoTen = txtHoTen.Text;
            string gioiTinh = txtGioiTinh.Text;
            string email = txtEmail.Text;
            string diaChi = txtDiaChi.Text;
            string password = txtPassword.Text;
            string confirmPassword = txtConfirm.Text;

            // Kiểm tra các trường bắt buộc
            if (string.IsNullOrEmpty(hoTen) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Vui lòng điền tất cả các trường bắt buộc.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kiểm tra xác nhận mật khẩu
            if (password != confirmPassword)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kiểm tra email đã tồn tại
            if (dBEntities.Users.Any(u => u.Email == email))
            {
                MessageBox.Show("Email đã tồn tại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Lấy ngày sinh từ DateTimePicker
            DateTime ngaySinh = dpNgaySinh.Value;

            // Mã hóa mật khẩu trước khi lưu vào cơ sở dữ liệu
            string hashedPassword = HashPassword(password);

            // Tạo đối tượng người dùng mới
            var newUser = new User
            {
                HoTen = hoTen,
                GioiTinh = gioiTinh,
                NgaySinh = ngaySinh,
                Email = email,
                DiaChi = diaChi,
                Password = password
            };

            // Thêm người dùng vào cơ sở dữ liệu
            dBEntities.Users.Add(newUser);
            dBEntities.SaveChanges(); // Lưu thay đổi

            MessageBox.Show("Đăng ký thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Trở về form đăng nhập sau khi đăng ký thành công
            this.Hide();
            var loginForm = new DangNhap();
            loginForm.Show();
        }
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
