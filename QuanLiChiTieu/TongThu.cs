using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace QuanLiChiTieu
{
    
    public partial class TongThu : Form
    {
        SchoolDBEntities minh = new SchoolDBEntities();
        public TongThu( )
        {
            InitializeComponent();

        }

        private void btnhienthi_Click(object sender, EventArgs e)
        {
            // Xóa dữ liệu cũ khỏi chart
            chart1.Series.Clear();

            // Tạo một series mới cho biểu đồ
            Series series = new Series("Chi Tiêu");
            series.ChartType = SeriesChartType.Column; // Hoặc Line, Pie tùy chọn

            // Duyệt qua các dòng của dgvChitieu và thêm dữ liệu thuộc mục Chi Tiêu
            foreach (DataGridViewRow row in dgvChitieu.Rows)
            {
                if (row.Cells["TenLoai"].Value != null && row.Cells["TenLoai"].Value.ToString() == "Chi Tiêu" &&
                    row.Cells["Tgian"].Value != null && row.Cells["Tien"].Value != null)
                {
                    string ngay = Convert.ToDateTime(row.Cells["Tgian"].Value).ToString("dd/MM/yyyy");
                    decimal tien;

                    // Kiểm tra và chuyển đổi dữ liệu từ cột Tien sang dạng số
                    if (decimal.TryParse(row.Cells["Tien"].Value.ToString(), out tien))
                    {
                        series.Points.AddXY(ngay, tien);
                    }
                }
            }

            // Thêm series vào chart
            chart1.Series.Add(series);
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void btnthoat_Click(object sender, EventArgs e)
        {
            ThongKe zForm = new ThongKe();
            zForm.StartPosition = FormStartPosition.Manual;
            zForm.Location = new Point(585, 250);
            zForm.Show();
            this.Hide();
        }

        private void TongThu_Load(object sender, EventArgs e)
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
        }
    }
}
