using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace nhathuoc
{
    public class FrmQuanLyThuoc : Form
    {
        private readonly string _connStr =
            @"Server=.\SQLEXPRESS;Database=QL_NhaThuoc_HKCARE;Integrated Security=True;TrustServerCertificate=True;";

        private readonly TextBox _txtMaSP = new();
        private readonly TextBox _txtTenThuoc = new();
        private readonly TextBox _txtDonViTinh = new();
        private readonly NumericUpDown _nudGiaBan = new();
        private readonly CheckBox _chkKeDon = new();
        private readonly TextBox _txtHinhAnh = new();
        private readonly Button _btnLuu = new();
        private readonly Button _btnTaoMoi = new();
        private readonly DataGridView _dgvDanhSach = new();

        public FrmQuanLyThuoc()
        {
            BuildUi();
            this.Load += (s, e) => TaiDanhSachThuoc();
            _dgvDanhSach.SelectionChanged += DgvDanhSach_SelectionChanged;
            _btnTaoMoi.Click += (s, e) => XoaTrangNhap();
            _btnLuu.Click += BtnLuu_Click;
        }

        private void BuildUi()
        {
            this.Text = "HKCARE - Quản lý Danh mục Thuốc";
            this.Size = new Size(900, 540);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = new Font("Segoe UI", 9.5F);

            var lblMa = new Label { Text = "Mã SP:", Location = new Point(20, 22), AutoSize = true };
            _txtMaSP.Location = new Point(95, 18);
            _txtMaSP.Size = new Size(120, 28);
            _txtMaSP.CharacterCasing = CharacterCasing.Upper;

            var lblTen = new Label { Text = "Tên thuốc:", Location = new Point(235, 22), AutoSize = true };
            _txtTenThuoc.Location = new Point(315, 18);
            _txtTenThuoc.Size = new Size(290, 28);

            var lblDvt = new Label { Text = "ĐVT:", Location = new Point(625, 22), AutoSize = true };
            _txtDonViTinh.Location = new Point(665, 18);
            _txtDonViTinh.Size = new Size(195, 28);

            var lblGia = new Label { Text = "Giá bán:", Location = new Point(20, 62), AutoSize = true };
            _nudGiaBan.Location = new Point(95, 58);
            _nudGiaBan.Size = new Size(150, 28);
            _nudGiaBan.Maximum = 100000000;
            _nudGiaBan.Increment = 1000;
            _nudGiaBan.Value = 10000;

            _chkKeDon.Text = "Thuốc kê đơn (Rx)";
            _chkKeDon.Location = new Point(265, 62);
            _chkKeDon.AutoSize = true;

            var lblAnh = new Label { Text = "Hình ảnh:", Location = new Point(420, 62), AutoSize = true };
            _txtHinhAnh.Location = new Point(495, 58);
            _txtHinhAnh.Size = new Size(365, 28);

            _btnLuu.Text = "Lưu thuốc";
            _btnLuu.Location = new Point(20, 102);
            _btnLuu.Size = new Size(120, 32);

            _btnTaoMoi.Text = "Nhập mới";
            _btnTaoMoi.Location = new Point(150, 102);
            _btnTaoMoi.Size = new Size(110, 32);

            _dgvDanhSach.Location = new Point(20, 146);
            _dgvDanhSach.Size = new Size(840, 335);
            _dgvDanhSach.ReadOnly = true;
            _dgvDanhSach.AllowUserToAddRows = false;
            _dgvDanhSach.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _dgvDanhSach.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            this.Controls.AddRange(new Control[]
            {
                lblMa, _txtMaSP, lblTen, _txtTenThuoc, lblDvt, _txtDonViTinh,
                lblGia, _nudGiaBan, _chkKeDon, lblAnh, _txtHinhAnh,
                _btnLuu, _btnTaoMoi, _dgvDanhSach
            });
        }

        private void TaiDanhSachThuoc()
        {
            try
            {
                using var conn = new SqlConnection(_connStr);
                using var da = new SqlDataAdapter(
                    "SELECT MaSP, TenThuoc, DonViTinh, GiaBan, TonKho, KeDon, HinhAnh FROM Thuoc ORDER BY MaSP",
                    conn);
                var dt = new DataTable();
                da.Fill(dt);
                _dgvDanhSach.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh mục thuốc: " + ex.Message, "Lỗi SQL",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvDanhSach_SelectionChanged(object? sender, EventArgs e)
        {
            if (_dgvDanhSach.CurrentRow?.DataBoundItem is not DataRowView rv) return;
            DataRow r = rv.Row;

            _txtMaSP.Text = r["MaSP"]?.ToString() ?? string.Empty;
            _txtTenThuoc.Text = r["TenThuoc"]?.ToString() ?? string.Empty;
            _txtDonViTinh.Text = r["DonViTinh"]?.ToString() ?? string.Empty;
            _nudGiaBan.Value = r["GiaBan"] != DBNull.Value
                ? Math.Clamp(Convert.ToDecimal(r["GiaBan"]), _nudGiaBan.Minimum, _nudGiaBan.Maximum)
                : 0;
            _chkKeDon.Checked = r["KeDon"] != DBNull.Value && Convert.ToBoolean(r["KeDon"]);
            _txtHinhAnh.Text = r["HinhAnh"]?.ToString() ?? string.Empty;
        }

        private void XoaTrangNhap()
        {
            _txtMaSP.Clear();
            _txtTenThuoc.Clear();
            _txtDonViTinh.Clear();
            _nudGiaBan.Value = 10000;
            _chkKeDon.Checked = false;
            _txtHinhAnh.Clear();
            _txtMaSP.Focus();
        }

        private void BtnLuu_Click(object? sender, EventArgs e)
        {
            string maSP = (_txtMaSP.Text ?? string.Empty).Trim().ToUpperInvariant();
            string tenThuoc = (_txtTenThuoc.Text ?? string.Empty).Trim();
            string dvt = (_txtDonViTinh.Text ?? string.Empty).Trim();
            decimal giaBan = _nudGiaBan.Value;
            int keDon = _chkKeDon.Checked ? 1 : 0;
            string hinhAnh = (_txtHinhAnh.Text ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(maSP) || string.IsNullOrWhiteSpace(tenThuoc) || string.IsNullOrWhiteSpace(dvt))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Mã SP, Tên thuốc và Đơn vị tính.", "Thiếu thông tin",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string sql = @"
                    IF EXISTS (SELECT 1 FROM Thuoc WHERE MaSP = @MaSP)
                    BEGIN
                        UPDATE Thuoc
                        SET TenThuoc = @TenThuoc,
                            DonViTinh = @DonViTinh,
                            GiaBan = @GiaBan,
                            KeDon = @KeDon,
                            HinhAnh = @HinhAnh
                        WHERE MaSP = @MaSP;
                    END
                    ELSE
                    BEGIN
                        INSERT INTO Thuoc (MaSP, TenThuoc, DonViTinh, GiaBan, TonKho, KeDon, HinhAnh)
                        VALUES (@MaSP, @TenThuoc, @DonViTinh, @GiaBan, 0, @KeDon, @HinhAnh);
                    END";

                using var conn = new SqlConnection(_connStr);
                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@MaSP", SqlDbType.VarChar, 10).Value = maSP;
                cmd.Parameters.Add("@TenThuoc", SqlDbType.NVarChar, 200).Value = tenThuoc;
                cmd.Parameters.Add("@DonViTinh", SqlDbType.NVarChar, 50).Value = dvt;
                cmd.Parameters.Add("@GiaBan", SqlDbType.Decimal).Value = giaBan;
                cmd.Parameters.Add("@KeDon", SqlDbType.Bit).Value = keDon;
                cmd.Parameters.Add("@HinhAnh", SqlDbType.NVarChar, 255).Value =
                    string.IsNullOrWhiteSpace(hinhAnh) ? DBNull.Value : hinhAnh;

                conn.Open();
                cmd.ExecuteNonQuery();

                TaiDanhSachThuoc();
                MessageBox.Show("Đã lưu thông tin thuốc thành công.", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lưu thuốc: " + ex.Message, "Lỗi SQL",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}