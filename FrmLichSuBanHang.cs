using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace nhathuoc
{
    public class FrmLichSuBanHang : Form
    {
        private readonly string _connStr =
            @"Server=.\SQLEXPRESS;Database=QL_NhaThuoc_HKCARE;Integrated Security=True;TrustServerCertificate=True;";

        private readonly TextBox _txtTimMa = new();
        private readonly Button _btnTim = new();
        private readonly Button _btnTaiLai = new();
        private readonly Label _lblDoanhThu = new();
        private readonly DataGridView _dgvHoaDon = new();
        private readonly DataGridView _dgvChiTiet = new();

        public FrmLichSuBanHang()
        {
            BuildUi();
            this.Load += (s, e) => TaiDanhSachHoaDon();
            _btnTim.Click += (s, e) => TaiDanhSachHoaDon();
            _btnTaiLai.Click += (s, e) =>
            {
                _txtTimMa.Clear();
                TaiDanhSachHoaDon();
            };
            _txtTimMa.TextChanged += (s, e) => TaiDanhSachHoaDon();
            _dgvHoaDon.SelectionChanged += DgvHoaDon_SelectionChanged;
        }

        private void BuildUi()
        {
            this.Text = "HKCARE - Lịch sử Bán hàng";
            this.Size = new Size(980, 580);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = new Font("Segoe UI", 9.5F);

            var lblTim = new Label { Text = "Mã HĐ / Mã đơn Rx:", Location = new Point(20, 22), AutoSize = true };
            _txtTimMa.Location = new Point(150, 18);
            _txtTimMa.Size = new Size(190, 28);

            _btnTim.Text = "Tìm";
            _btnTim.Location = new Point(350, 17);
            _btnTim.Size = new Size(75, 30);

            _btnTaiLai.Text = "Tải lại";
            _btnTaiLai.Location = new Point(433, 17);
            _btnTaiLai.Size = new Size(85, 30);

            _lblDoanhThu.Location = new Point(535, 22);
            _lblDoanhThu.Size = new Size(410, 24);
            _lblDoanhThu.UseMnemonic = false;
            _lblDoanhThu.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);

            _dgvHoaDon.Location = new Point(20, 60);
            _dgvHoaDon.Size = new Size(925, 250);
            _dgvHoaDon.ReadOnly = true;
            _dgvHoaDon.AllowUserToAddRows = false;
            _dgvHoaDon.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _dgvHoaDon.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            var lblChiTiet = new Label { Text = "Chi tiết hóa đơn:", Location = new Point(20, 320), AutoSize = true };
            _dgvChiTiet.Location = new Point(20, 344);
            _dgvChiTiet.Size = new Size(925, 175);
            _dgvChiTiet.ReadOnly = true;
            _dgvChiTiet.AllowUserToAddRows = false;
            _dgvChiTiet.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _dgvChiTiet.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            this.Controls.AddRange(new Control[]
            {
                lblTim, _txtTimMa, _btnTim, _btnTaiLai, _lblDoanhThu,
                _dgvHoaDon, lblChiTiet, _dgvChiTiet
            });
        }

        private void TaiDanhSachHoaDon()
        {
            try
            {
                string q = (_txtTimMa.Text ?? string.Empty).Trim();
                string sql = @"
                    SELECT
                        h.MaHD,
                        h.NgayLap,
                        ISNULL(kh.HoTen, N'Khách vãng lai') AS KhachHang,
                        h.HinhThucThanhToan,
                        ISNULL(h.MaKM, N'—') AS MaKM,
                        ISNULL(h.MaDonThuoc, N'—') AS MaDonThuoc,
                        h.TamTinh,
                        h.GiamGia,
                        h.TongTien
                    FROM dbo.HoaDon h
                    LEFT JOIN dbo.KhachHang kh ON kh.MaKH = h.MaKH
                    WHERE (@q = '' OR h.MaHD LIKE @like OR ISNULL(h.MaDonThuoc, '') LIKE @like)
                    ORDER BY h.NgayLap DESC;";

                using var conn = new SqlConnection(_connStr);
                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@q", SqlDbType.NVarChar, 60).Value = q;
                cmd.Parameters.Add("@like", SqlDbType.NVarChar, 62).Value = "%" + q + "%";

                var dt = new DataTable();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                _dgvHoaDon.DataSource = dt;

                decimal tongDoanhThu = 0m;
                foreach (DataRow r in dt.Rows)
                {
                    if (r["TongTien"] != DBNull.Value)
                    {
                        tongDoanhThu += Convert.ToDecimal(r["TongTien"]);
                    }
                }
                _lblDoanhThu.Text = $"Số hóa đơn: {dt.Rows.Count} | Tổng doanh thu: {tongDoanhThu:N0}đ";

                if (dt.Rows.Count > 0)
                {
                    string? maHdDau = dt.Rows[0]["MaHD"]?.ToString();
                    if (!string.IsNullOrWhiteSpace(maHdDau)) TaiChiTietHoaDon(maHdDau);
                }
                else
                {
                    _dgvChiTiet.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải lịch sử bán hàng: " + ex.Message, "Lỗi SQL",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvHoaDon_SelectionChanged(object? sender, EventArgs e)
        {
            if (_dgvHoaDon.CurrentRow?.DataBoundItem is DataRowView rv)
            {
                string? maHD = rv.Row["MaHD"]?.ToString();
                if (!string.IsNullOrWhiteSpace(maHD))
                {
                    TaiChiTietHoaDon(maHD);
                }
            }
        }

        private void TaiChiTietHoaDon(string maHD)
        {
            try
            {
                using var conn = new SqlConnection(_connStr);
                using var cmd = new SqlCommand(@"
                    SELECT ct.MaSP, t.TenThuoc, ct.SoLuong, ct.DonGia, ct.ThanhTien
                    FROM dbo.ChiTietHoaDon ct
                    INNER JOIN dbo.Thuoc t ON t.MaSP = ct.MaSP
                    WHERE ct.MaHD = @MaHD
                    ORDER BY ct.Id ASC;", conn);
                cmd.Parameters.Add("@MaHD", SqlDbType.VarChar, 20).Value = maHD;

                var dt = new DataTable();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                _dgvChiTiet.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải chi tiết hóa đơn: " + ex.Message, "Lỗi SQL",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}