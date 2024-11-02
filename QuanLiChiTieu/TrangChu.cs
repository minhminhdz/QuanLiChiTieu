using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLiChiTieu
{
    public partial class TrangChu : Form
    {
        SchoolDBEntities minh = new SchoolDBEntities();
        private SchoolDBEntities1 _context;
        public TrangChu(SchoolDBEntities1 context)
        {
            InitializeComponent();
            _context = context;
            LoadData(); 
        }


        private void btnLich_Click(object sender, EventArgs e)
        {
            Lich lichForm = new Lich();

            lichForm.StartPosition = FormStartPosition.Manual;
            lichForm.Location = new Point(585, 250);
            lichForm.Show();
        }
        private void LoadData()
        {
            var data = from hd in minh.Hoatdongs
                       join loai in minh.Loais on hd.MaLoai equals loai.MaLoai
                       join dm in minh.Dmucs on hd.MaDM equals dm.MaDM
                       select new
                       {
                           hd.Id,
                           TenLoai = loai.TenLoai,
                           hd.Tgian,
                           Tien = hd.Tien.HasValue ? hd.Tien.Value : 0,
                           GhiChu = hd.GhiChu ?? "",
                           TenDM = dm.TenDM
                       };

            dataGridView1.DataSource = data.ToList(); // Gán danh sách cho DataGridView
        }
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                // Duyệt qua từng dòng của DataGridView
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.IsNewRow) continue; // Bỏ qua dòng mới

                    // Lấy giá trị từ từng ô
                    var id = row.Cells["Id"].Value != null ? Convert.ToInt32(row.Cells["Id"].Value) : 0;
                    var tgian = row.Cells["Tgian"].Value != null ? Convert.ToDateTime(row.Cells["Tgian"].Value) : DateTime.Now;
                    var tien = row.Cells["Tien"].Value != null ? Convert.ToDouble(row.Cells["Tien"].Value) : 0;
                    var ghiChu = row.Cells["GhiChu"].Value?.ToString() ?? "";
                    var tenLoai = row.Cells["TenLoai"].Value?.ToString() ?? "";
                    var tenDM = row.Cells["TenDM"].Value?.ToString() ?? "";

                    // Tìm hoạt động trong cơ sở dữ liệu theo ID
                    var hoatdong = minh.Hoatdongs.FirstOrDefault(h => h.Id == id);
                    if (hoatdong == null)
                    {
                        continue; // Bỏ qua dòng này
                    }

                    // Cập nhật các giá trị
                    hoatdong.Tgian = tgian;
                    hoatdong.Tien = tien;
                    hoatdong.GhiChu = ghiChu;

                    // Cập nhật MaLoai và MaDM nếu cần
                    var loai = minh.Loais.FirstOrDefault(l => l.TenLoai == tenLoai);
                    var dm = minh.Dmucs.FirstOrDefault(d => d.TenDM == tenDM);
                    if (loai != null) hoatdong.MaLoai = loai.MaLoai;
                    if (dm != null) hoatdong.MaDM = dm.MaDM;
                }

                // Lưu các thay đổi vào cơ sở dữ liệu
                minh.SaveChanges();

                // Tải lại dữ liệu để cập nhật DataGridView
                load();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra khi cập nhật dữ liệu: " + ex.Message);
            }
        }
        public void load()
        {
            try
            {
                var data = from hd in minh.Hoatdongs
                           join loai in minh.Loais on hd.MaLoai equals loai.MaLoai
                           join dm in minh.Dmucs on hd.MaDM equals dm.MaDM
                           select new
                           {
                               hd.Id,
                               TenLoai = loai.TenLoai,
                               hd.Tgian,
                               Tien = hd.Tien.HasValue ? hd.Tien.Value : 0,
                               GhiChu = hd.GhiChu ?? "",
                               TenDM = dm.TenDM
                           };

                var dataList = data.ToList();
                dataGridView1.DataSource = dataList;


                // Tính tổng tiền cho từng loại
                decimal totalIncome = dataList
                    .Where(item => item.TenLoai == "Thu Nhập")
                    .Sum(item => (decimal)item.Tien); // Chuyển đổi sang decimal

                decimal totalExpense = dataList
                    .Where(item => item.TenLoai == "Chi Tiêu")
                    .Sum(item => (decimal)item.Tien); // Chuyển đổi sang decimal

                decimal totalDiLai = dataList
                    .Where(item => item.TenDM == "Đi Lại") // Tổng tiền từ MaDM là "Đi Lại"
                    .Sum(item => (decimal)item.Tien);

                decimal totalAnUong = dataList
                    .Where(item => item.TenDM == "Ăn Uống") // Tổng tiền từ MaLoai là "Ăn Uống"
                    .Sum(item => (decimal)item.Tien);

                decimal totalBanBe = dataList
                    .Where(item => item.TenDM == "Bạn Bè") // Tổng tiền từ MaLoai là "Bạn Bè"
                    .Sum(item => (decimal)item.Tien);

                decimal totalMuaSam = dataList
                    .Where(item => item.TenDM == "Mua Sắm") // Tổng tiền từ MaLoai là "Mua Sắm"
                    .Sum(item => (decimal)item.Tien);

                decimal totalPhong = dataList
                    .Where(item => item.TenDM == "Phòng") // Tổng tiền từ MaLoai là "Phòng"
                    .Sum(item => (decimal)item.Tien);

                decimal du = totalIncome - totalExpense;
                txtDu.Text = du.ToString();

                // Cập nhật vào các TextBox
                txtThu.Text = totalIncome.ToString();
                txtChi.Text = totalExpense.ToString();

                txtDiLai.Text = totalDiLai.ToString();
                txtAnuong.Text = totalAnUong.ToString();
                txtBanBe.Text = totalBanBe.ToString();
                txtMuaSam.Text = totalMuaSam.ToString();
                txtPhong.Text = totalPhong.ToString();



                // Nếu không có dữ liệu, hiển thị 0
                if (!dataList.Any())
                {
                    txtChi.Text = "0";
                    txtThu.Text = "0";
                    txtDiLai.Text = "0";
                    txtAnuong.Text = "0";
                    txtBanBe.Text = "0";
                    txtMuaSam.Text = "0";
                    txtPhong.Text = "0";
                    txtDu.Text = "0";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra khi tải dữ liệu: " + ex.Message);
            }

        }

        private void TrangChu_Load(object sender, EventArgs e)
        {
            load();
        }

        private void btnCloseChildForm_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnTrangchu_Click(object sender, EventArgs e)
        {
            DangNhap loginForm = new DangNhap();

            loginForm.StartPosition = FormStartPosition.Manual;
            loginForm.Location = new Point(585, 250);
            loginForm.Show();
            this.Hide();
        }

        private void txtDiLai_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtAnuong_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtBanBe_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtMuaSam_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtPhong_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtThu_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtChi_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtDu_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnThongke_Click(object sender, EventArgs e)
        {
            ThongKe lich1Form = new ThongKe();

            lich1Form.StartPosition = FormStartPosition.Manual;
            lich1Form.Location = new Point(585, 250);
            lich1Form.Show();
        }

        private void btnNhap_Click(object sender, EventArgs e)
        {
            NhapVao nhapVaoForm = new NhapVao();

            nhapVaoForm.StartPosition = FormStartPosition.Manual;
            nhapVaoForm.Location = new Point(585, 250); // Tùy chỉnh vị trí nếu cần
            nhapVaoForm.Show();
        }

        private void btnThongtin_Click(object sender, EventArgs e)
        {

                ThongTin TtinForm = new ThongTin(CurrentUser.User); // Truyền thông tin người dùng
                TtinForm.StartPosition = FormStartPosition.Manual;
                TtinForm.Location = new Point(585, 250); // Tùy chỉnh vị trí nếu cần
                TtinForm.Show();
        }
    }
}
