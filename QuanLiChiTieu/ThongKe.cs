using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;


namespace QuanLiChiTieu
{
    public partial class ThongKe : Form
    {
        SchoolDBEntities minh = new SchoolDBEntities();

        public ThongKe()
        {
            InitializeComponent();


        }

        private void button1_Click(object sender, EventArgs e)
        {
            TongThu TtForm = new TongThu();

            TtForm.StartPosition = FormStartPosition.Manual;
            TtForm.Location = new Point(585, 250);
            TtForm.Show();
            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            TongChi TcForm = new TongChi();

            TcForm.StartPosition = FormStartPosition.Manual;
            TcForm.Location = new Point(585, 250);
            TcForm.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Tạo một đối tượng Excel
            Excel.Application excelApp = new Excel.Application();
            excelApp.Visible = false; // Ẩn Excel trong khi xuất file

            // Tạo một workbook và worksheet mới
            Excel.Workbook workbook = excelApp.Workbooks.Add();
            Excel.Worksheet worksheet = (Excel.Worksheet)workbook.Worksheets[1];

            // Thêm tiêu đề cho các cột
            worksheet.Cells[1, 1] = "ID";
            worksheet.Cells[1, 2] = "Tên Loại";
            worksheet.Cells[1, 3] = "Thời Gian";
            worksheet.Cells[1, 4] = "Tiền";
            worksheet.Cells[1, 5] = "Ghi Chú";
            worksheet.Cells[1, 6] = "Tên DM";

            // Xuất dữ liệu từ DataGridView (dgvChitieu)
            int rowIndex = 2; // Bắt đầu từ hàng thứ hai vì hàng đầu tiên là tiêu đề
            foreach (DataGridViewRow row in dgvChitieu.Rows)
            {
                // Kiểm tra xem hàng có hợp lệ không
                if (!row.IsNewRow)
                {
                    worksheet.Cells[rowIndex, 1] = row.Cells["Id"].Value; // Cột ID
                    worksheet.Cells[rowIndex, 2] = row.Cells["TenLoai"].Value; // Cột Tên Loại
                    worksheet.Cells[rowIndex, 3] = row.Cells["Tgian"].Value; // Cột Thời Gian
                    worksheet.Cells[rowIndex, 4] = row.Cells["Tien"].Value; // Cột Tiền
                    worksheet.Cells[rowIndex, 5] = row.Cells["GhiChu"].Value; // Cột Ghi Chú
                    worksheet.Cells[rowIndex, 6] = row.Cells["TenDM"].Value; // Cột Tên DM
                    rowIndex++;
                }
            }

            // Lưu file Excel
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel Files|*.xlsx";
            saveFileDialog.Title = "Lưu file Excel";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                workbook.SaveAs(saveFileDialog.FileName);
            }

            // Đóng workbook và Excel
            workbook.Close();
            excelApp.Quit();

            // Giải phóng tài nguyên
            System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);

            MessageBox.Show("Xuất file Excel thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ThongKe_Load(object sender, EventArgs e)
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
