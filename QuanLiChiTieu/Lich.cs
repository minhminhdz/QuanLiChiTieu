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
    public partial class Lich : Form
    {
        SchoolDBEntities minh = new SchoolDBEntities();


        public Lich()
        {
            InitializeComponent();
            SharedData.DgvChitieu = dgvChitieu;
        }

        private void Lich_Load(object sender, EventArgs e)
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

            dgvChitieu.DataSource = data.ToList();
            dgvChitieu.Columns["Tgian"].DefaultCellStyle.Format = "dd/MM/yyyy";

            // Đặt thuộc tính Visible của cột Id thành true để hiển thị
            dgvChitieu.Columns["Id"].Visible = true;

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

        private void dgvChitieu_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvChitieu.Rows[e.RowIndex];

                // Gán giá trị cho các TextBox
                txtNote.Text = row.Cells["GhiChu"].Value?.ToString() ?? string.Empty;
                txtTien.Text = row.Cells["Tien"].Value?.ToString() ?? string.Empty;

                // Gán giá trị cho txtTgian
                dpTime.Text = row.Cells["Tgian"].Value?.ToString() ?? string.Empty;

                // Gán giá trị cho cboLoai và cboDanhmuc dựa trên TenLoai và TenDM
                var tenLoai = row.Cells["TenLoai"].Value?.ToString();
                var tenDM = row.Cells["TenDM"].Value?.ToString();

                if (tenLoai != null)
                {
                    var selectedLoai = minh.Loais.FirstOrDefault(l => l.TenLoai == tenLoai);
                    if (selectedLoai != null)
                    {
                        cboLoai.SelectedValue = selectedLoai.MaLoai;
                    }
                }

                if (tenDM != null)
                {
                    var selectedDM = minh.Dmucs.FirstOrDefault(d => d.TenDM == tenDM);
                    if (selectedDM != null)
                    {
                        cboDanhmuc.SelectedValue = selectedDM.MaDM;
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Hoatdong newHoatdong = new Hoatdong
            {
                MaLoai = cboLoai.SelectedValue.ToString(),
                MaDM = cboDanhmuc.SelectedValue.ToString(),
                Tgian = dpTime.Value,
                Tien = double.TryParse(txtTien.Text, out double tien) ? tien : (double?)null,
                GhiChu = txtNote.Text
            };

            minh.Hoatdongs.Add(newHoatdong);
            minh.SaveChanges();

            // Thông báo thêm thành công
            MessageBox.Show("Thêm mới hoạt động chi tiêu thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

            load();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dgvChitieu.SelectedRows.Count > 0)
            {
                // Hiển thị hộp thoại xác nhận
                var result = MessageBox.Show("Bạn có chắc chắn muốn xóa mục này không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Lấy ID của mục đã chọn
                    int id = (int)dgvChitieu.SelectedRows[0].Cells["Id"].Value;
                    var hoatdongToDelete = minh.Hoatdongs.FirstOrDefault(hd => hd.Id == id);

                    if (hoatdongToDelete != null)
                    {
                        minh.Hoatdongs.Remove(hoatdongToDelete);
                        minh.SaveChanges();

                        // Tải lại dữ liệu vào DataGridView
                        load();
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một mục để xóa.");
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dgvChitieu.SelectedRows.Count > 0)
            {
                // Lấy ID của mục đã chọn
                int id = (int)dgvChitieu.SelectedRows[0].Cells["Id"].Value;
                var hoatdongToUpdate = minh.Hoatdongs.FirstOrDefault(hd => hd.Id == id);

                if (hoatdongToUpdate != null)
                {
                    // Cập nhật các thuộc tính của hoạt động
                    hoatdongToUpdate.MaLoai = cboLoai.SelectedValue.ToString();
                    hoatdongToUpdate.MaDM = cboDanhmuc.SelectedValue.ToString();
                    hoatdongToUpdate.Tgian = dpTime.Value; // Gán giá trị DateTime
                    hoatdongToUpdate.Tien = double.TryParse(txtTien.Text, out double tien) ? tien : (double?)null;
                    hoatdongToUpdate.GhiChu = txtNote.Text;

                    // Lưu thay đổi
                    minh.SaveChanges();
                    load(); // Tải lại dữ liệu vào DataGridView
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một mục để chỉnh sửa.");
            }
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            string searchTerm = txtTimkiem.Text.ToLower(); // Lấy giá trị tìm kiếm từ TextBox

            // Lọc dữ liệu theo điều kiện
            var filteredData = from hd in minh.Hoatdongs
                               join loai in minh.Loais on hd.MaLoai equals loai.MaLoai
                               join dm in minh.Dmucs on hd.MaDM equals dm.MaDM
                               where loai.TenLoai.ToLower().Contains(searchTerm) || // Tìm theo Tên Loại
                                     hd.Tien.ToString().Contains(searchTerm) ||  // Tìm theo Tiền
                                     (hd.GhiChu != null && hd.GhiChu.ToLower().Contains(searchTerm)) || // Tìm theo Ghi Chú
                                     dm.TenDM.ToLower().Contains(searchTerm)  // Tìm theo Tên Danh Mục
                               select new
                               {
                                   hd.Id,
                                   TenLoai = loai.TenLoai,
                                   hd.Tgian,
                                   Tien = hd.Tien.HasValue ? hd.Tien.Value : 0,
                                   GhiChu = hd.GhiChu ?? "",
                                   TenDM = dm.TenDM
                               };

            // Cập nhật DataGridView với dữ liệu đã lọc
            dgvChitieu.DataSource = filteredData.ToList();
        }

        private void dgvChitieu_SelectionChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Lấy ngày bắt đầu và ngày kết thúc từ DateTimePicker
            DateTime fromDate = dpToDate.Value.Date;
            DateTime toDate = dpFromDate.Value.Date;

            // Lọc dữ liệu từ minh.Hoatdongs trong khoảng ngày
            var data = from hd in minh.Hoatdongs
                       join loai in minh.Loais on hd.MaLoai equals loai.MaLoai
                       join dm in minh.Dmucs on hd.MaDM equals dm.MaDM
                       where hd.Tgian >= fromDate && hd.Tgian <= toDate
                       select new
                       {
                           hd.Id,
                           TenLoai = loai.TenLoai,
                           hd.Tgian,
                           Tien = hd.Tien.HasValue ? hd.Tien.Value : 0,
                           GhiChu = hd.GhiChu ?? "",
                           TenDM = dm.TenDM
                       };

            // Hiển thị dữ liệu đã lọc vào DataGridView
            dgvChitieu.DataSource = data.ToList();
        }

        private void dpToDate_ValueChanged(object sender, EventArgs e)
        {

        }

        private void dpFromDate_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
