using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace nhathuoc
{
    public class FrmQuanLyLo : Form
    {
        private readonly string _connStr =
            @"Server=.\SQLEXPRESS;Database=QL_NhaThuoc_HKCARE;Integrated Security=True;TrustServerCertificate=True;";

        private readonly ComboBox _cboThuoc = new();
        private readonly TextBox _txtSoLo = new();
        private readonly DateTimePicker _dtpHanSuDung = new();
        private readonly NumericUpDown _nudSoLuong = new();
        private readonly Button _btnLuuLo = new();
        private readonly Button _btnLamMoi = new();
        private readonly DataGridView _dgvLoThuoc = new();
        private readonly Label _lblTrangThai = new();

        public FrmQuanLyLo()
        {
            BuildUi();
            this.Load += FrmQuanLyLo_Load;
            _cboThuoc.SelectedIndexChanged += (s, e) => TaiDanhSachLoCuaThuocChon();
            _btnLuuLo.Click += BtnLuuLo_Click;
            _btnLamMoi.Click += (s, e) =>
            {
                _txtSoLo.Clear();
                _nudSoLuong.Value = 1;
                _dtpHanSuDung.Value = DateTime.Today.AddMonths(12);
                TaiDanhSachLoCuaThuocChon();
            };
        }

        private void BuildUi()
        {
            this.Text = "HKCARE - Quản lý Lô & Hạn sử dụng";
            this.Size = new Size(860, 520);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = new Font("Segoe UI", 9.5F);

            var lblThuoc = new Label { Text = "Chọn thuốc:", Location = new Point(20, 22), AutoSize = true };
            _cboThuoc.Location = new Point(110, 18);
            _cboThuoc.Size = new Size(340, 28);
            _cboThuoc.DropDownStyle = ComboBoxStyle.DropDownList;

            var lblSoLo = new Label { Text = "Số lô:", Location = new Point(475, 22), AutoSize = true };
            _txtSoLo.Location = new Point(530, 18);
            _txtSoLo.Size = new Size(140, 28);
            _txtSoLo.CharacterCasing = CharacterCasing.Upper;

            var lblHsd = new Label { Text = "Hạn sử dụng:", Location = new Point(20, 62), AutoSize = true };
            _dtpHanSuDung.Location = new Point(110, 58);
            _dtpHanSuDung.Size = new Size(165, 28);
            _dtpHanSuDung.Format = DateTimePickerFormat.Short;
            _dtpHanSuDung.Value = DateTime.Today.AddMonths(12);

            var lblSoLuong = new Label { Text = "Số lượng nhập:", Location = new Point(295, 62), AutoSize = true };
            _nudSoLuong.Location = new Point(400, 58);
            _nudSoLuong.Size = new Size(100, 28);
            _nudSoLuong.Minimum = 1;
            _nudSoLuong.Maximum = 1000000;
            _nudSoLuong.Value = 10;

            _btnLuuLo.Text = "Lưu lô nhập";
            _btnLuuLo.Location = new Point(530, 56);
            _btnLuuLo.Size = new Size(130, 32);

            _btnLamMoi.Text = "Làm mới";
            _btnLamMoi.Location = new Point(670, 56);
            _btnLamMoi.Size = new Size(110, 32);

            _dgvLoThuoc.Location = new Point(20, 105);
            _dgvLoThuoc.Size = new Size(805, 315);
            _dgvLoThuoc.ReadOnly = true;
            _dgvLoThuoc.AllowUserToAddRows = false;
            _dgvLoThuoc.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _dgvLoThuoc.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            _lblTrangThai.Location = new Point(20, 432);
            _lblTrangThai.Size = new Size(805, 35);
            _lblTrangThai.UseMnemonic = false;
            _lblTrangThai.ForeColor = Color.DimGray;

            this.Controls.AddRange(new Control[]
            {
                lblThuoc, _cboThuoc, lblSoLo, _txtSoLo,
                lblHsd, _dtpHanSuDung, lblSoLuong, _nudSoLuong,
                _btnLuuLo, _btnLamMoi, _dgvLoThuoc, _lblTrangThai
            });
        }

        private void FrmQuanLyLo_Load(object? sender, EventArgs e)
        {
            TaiDanhSachThuocCombo();
            TaiDanhSachLoCuaThuocChon();
        }

        private void TaiDanhSachThuocCombo()
        {
            try
            {
                using var conn = new SqlConnection(_connStr);
                using var da = new SqlDataAdapter(
                    "SELECT MaSP, TenThuoc + N' (' + MaSP + N')' AS HienThi FROM Thuoc ORDER BY MaSP", conn);
                var dt = new DataTable();
                da.Fill(dt);

                _cboThuoc.DisplayMember = "HienThi";
                _cboThuoc.ValueMember = "MaSP";
                _cboThuoc.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách thuốc: " + ex.Message, "Lỗi SQL",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TaiDanhSachLoCuaThuocChon()
        {
            string? maSP = _cboThuoc.SelectedValue?.ToString();
            if (string.IsNullOrWhiteSpace(maSP)) return;

            try
            {
                using var conn = new SqlConnection(_connStr);
                using var cmd = new SqlCommand(@"
                    SELECT SoLo, HanSuDung, SoLuong,
                           CASE
                               WHEN HanSuDung < CAST(GETDATE() AS DATE) THEN N'Hết hạn'
                               WHEN HanSuDung <= DATEADD(DAY, 30, CAST(GETDATE() AS DATE)) THEN N'Sắp hết hạn (<=30 ngày)'
                               ELSE N'Còn hạn'
                           END AS TrangThaiHSD
                    FROM LoThuoc
                    WHERE MaSP = @MaSP
                    ORDER BY HanSuDung ASC, SoLo ASC", conn);
                cmd.Parameters.Add("@MaSP", SqlDbType.VarChar, 10).Value = maSP;

                var dt = new DataTable();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                _dgvLoThuoc.DataSource = dt;

                int tongLo = dt.Rows.Count;
                int tongSl = 0;
                foreach (DataRow r in dt.Rows)
                {
                    tongSl += r["SoLuong"] != DBNull.Value ? Convert.ToInt32(r["SoLuong"]) : 0;
                }
                _lblTrangThai.Text = $"Mã thuốc: {maSP} | Số lô: {tongLo} | Tổng số lượng theo lô: {tongSl}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách lô: " + ex.Message, "Lỗi SQL",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnLuuLo_Click(object? sender, EventArgs e)
        {
            string? maSP = _cboThuoc.SelectedValue?.ToString();
            string soLo = (_txtSoLo.Text ?? string.Empty).Trim().ToUpperInvariant();
            DateTime hsd = _dtpHanSuDung.Value.Date;
            int soLuong = (int)_nudSoLuong.Value;

            if (string.IsNullOrWhiteSpace(maSP))
            {
                MessageBox.Show("Vui lòng chọn thuốc.", "Thiếu thông tin",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(soLo))
            {
                MessageBox.Show("Vui lòng nhập số lô.", "Thiếu thông tin",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtSoLo.Focus();
                return;
            }

            if (hsd <= DateTime.Today)
            {
                MessageBox.Show("Hạn sử dụng lô nhập phải lớn hơn ngày hiện tại.", "Hạn sử dụng không hợp lệ",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using var conn = new SqlConnection(_connStr);
                conn.Open();
                using var tran = conn.BeginTransaction();

                string upsertLoSql = @"
                    IF EXISTS (SELECT 1 FROM LoThuoc WHERE MaSP = @MaSP AND SoLo = @SoLo)
                    BEGIN
                        UPDATE LoThuoc
                        SET SoLuong = SoLuong + @SoLuong,
                            HanSuDung = @HanSuDung
                        WHERE MaSP = @MaSP AND SoLo = @SoLo;
                    END
                    ELSE
                    BEGIN
                        INSERT INTO LoThuoc (MaSP, SoLo, HanSuDung, SoLuong)
                        VALUES (@MaSP, @SoLo, @HanSuDung, @SoLuong);
                    END

                    UPDATE Thuoc
                    SET TonKho = ISNULL((SELECT SUM(SoLuong) FROM LoThuoc WHERE MaSP = @MaSP), 0)
                    WHERE MaSP = @MaSP;";

                using var cmd = new SqlCommand(upsertLoSql, conn, tran);
                cmd.Parameters.Add("@MaSP", SqlDbType.VarChar, 10).Value = maSP;
                cmd.Parameters.Add("@SoLo", SqlDbType.NVarChar, 30).Value = soLo;
                cmd.Parameters.Add("@HanSuDung", SqlDbType.Date).Value = hsd;
                cmd.Parameters.Add("@SoLuong", SqlDbType.Int).Value = soLuong;
                cmd.ExecuteNonQuery();

                tran.Commit();

                _txtSoLo.Clear();
                TaiDanhSachLoCuaThuocChon();
                MessageBox.Show("Đã lưu lô nhập và cập nhật tồn kho thành công.", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lưu lô thuốc: " + ex.Message, "Lỗi SQL",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}