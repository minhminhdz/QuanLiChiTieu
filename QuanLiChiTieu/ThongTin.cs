using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLiChiTieu
{
    public partial class ThongTin : Form
    {
        private User currentUser;
        public ThongTin(User user)
        {
            InitializeComponent();
            currentUser = user;
        }

        private void txtHoTen_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtGioiTinh_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtNgaySinh_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtEmail_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtDiaChi_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtPass_TextChanged(object sender, EventArgs e)
        {

        }

        private void ThongTin_Load(object sender, EventArgs e)
        {
            
        }

        private void btnThongtin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();

            // Kiểm tra xem email có được nhập hay không
            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Vui lòng nhập email.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Tìm kiếm thông tin người dùng dựa trên email
            using (var context = new SchoolDBEntities())
            {
                var user = context.Users.FirstOrDefault(u => u.Email == email);

                if (user != null)
                {
                    // Hiển thị thông tin người dùng
                    txtHoTen.Text = user.HoTen;
                    txtGioiTinh.Text = user.GioiTinh;
                    txtNgaySinh.Text = user.NgaySinh.ToString("dd/MM/yyyy"); // Định dạng ngày tháng
                    txtDiaChi.Text = user.DiaChi;
                    txtPass.Text = user.Password; // Nếu bạn không muốn hiện mật khẩu, hãy thay đổi dòng này
                }
                else
                {
                    MessageBox.Show("Không tìm thấy thông tin người dùng với email này.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
