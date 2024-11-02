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
    public partial class NhapVao : Form
    {
        SchoolDBEntities minh = new SchoolDBEntities();

        public NhapVao()
        {
            InitializeComponent();

        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra giá trị nhập vào
                if (string.IsNullOrEmpty(txtTien.Text) || !double.TryParse(txtTien.Text, out double tienValue))
                {
                    MessageBox.Show("Vui lòng nhập một giá trị hợp lệ cho tiền.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // Dừng xử lý nếu tiền không hợp lệ
                }

                if (cboLoai.SelectedIndex == -1)
                {
                    MessageBox.Show("Vui lòng chọn loại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // Dừng xử lý nếu loại không được chọn
                }

                if (cboDanhmuc.SelectedIndex == -1)
                {
                    MessageBox.Show("Vui lòng chọn danh mục.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // Dừng xử lý nếu danh mục không được chọn
                }

                // Lấy thông tin từ các TextBox và ComboBox
                var newHoatDong = new QuanLiChiTieu.Hoatdong // Đảm bảo đúng namespace
                {
                    MaLoai = cboLoai.SelectedValue.ToString(), // Chuyển giá trị thành chuỗi
                    MaDM = cboDanhmuc.SelectedValue.ToString(), // Chuyển giá trị thành chuỗi
                    Tgian = dpTime.Value,                       // Giá trị thời gian từ DateTimePicker
                    Tien = tienValue,                          // Giá trị tiền tệ đã được kiểm tra
                    GhiChu = txtGhiChu.Text                      // Lấy ghi chú từ TextBox
                };

                // Thêm đối tượng mới vào cơ sở dữ liệu
                minh.Hoatdongs.Add(newHoatDong);
                minh.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu

                // Tải lại dữ liệu để hiển thị trong DataGridView
                load(); // Gọi lại phương thức load để làm mới dữ liệu

                // Xóa các TextBox sau khi thêm thành công
                txtTien.Clear();
                txtGhiChu.Clear();
                cboLoai.SelectedIndex = -1; // Bỏ chọn ComboBox
                cboDanhmuc.SelectedIndex = -1; // Bỏ chọn ComboBox
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void NhapVao_Load(object sender, EventArgs e)
        {
            load();
        }
        public void load()
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



            // Thiết lập nguồn dữ liệu cho ComboBox Loai
            var LoaiList = minh.Loais.ToList();
            cboLoai.DataSource = LoaiList;
            cboLoai.DisplayMember = "TenLoai";
            cboLoai.ValueMember = "MaLoai";

            // Thiết lập nguồn dữ liệu cho ComboBox DanhMuc
            var danhMucList = minh.Dmucs.ToList();
            cboDanhmuc.DataSource = danhMucList;
            cboDanhmuc.DisplayMember = "TenDM";
            cboDanhmuc.ValueMember = "MaDM";

        }
    }
}
