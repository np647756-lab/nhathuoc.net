using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace nhathuoc
{
    public class FrmBanHang : Form
    {
        private readonly string _connStr =
            @"Server=.\SQLEXPRESS;Database=QL_NhaThuoc_HKCARE;Integrated Security=True;TrustServerCertificate=True;";

        private readonly ComboBox _cboKenhBan = new();
        private readonly ComboBox _cboThuoc = new();
        private readonly NumericUpDown _nudSoLuong = new();
        private readonly Button _btnThemGio = new();
        private readonly Button _btnXoaDong = new();
        private readonly DataGridView _dgvGioHang = new();
        private readonly CheckBox _chkCoDonThuoc = new();
        private readonly TextBox _txtMaDonThuoc = new();
        private readonly ComboBox _cboKhuyenMai = new();
        private readonly Label _lblTongTien = new();
        private readonly Button _btnThanhToan = new();

        private readonly DataTable _dtGio = new();

        public FrmBanHang()
        {
            KhoiTaoBangGio();
            BuildUi();
            this.Load += FrmBanHang_Load;
            _btnThemGio.Click += BtnThemGio_Click;
            _btnXoaDong.Click += BtnXoaDong_Click;
            _chkCoDonThuoc.CheckedChanged += (s, e) => _txtMaDonThuoc.Enabled = _chkCoDonThuoc.Checked;
            _cboKhuyenMai.SelectedIndexChanged += (s, e) => TinhLaiTongTien();
            _btnThanhToan.Click += BtnThanhToan_Click;
        }

        private void KhoiTaoBangGio()
        {
            _dtGio.Columns.Add("MaSP", typeof(string));
            _dtGio.Columns.Add("TenThuoc", typeof(string));
            _dtGio.Columns.Add("KeDon", typeof(bool));
            _dtGio.Columns.Add("Loai", typeof(string));
            _dtGio.Columns.Add("DonGia", typeof(decimal));
            _dtGio.Columns.Add("SoLuong", typeof(int));
            _dtGio.Columns.Add("ThanhTien", typeof(decimal));
        }

        private void BuildUi()
        {
            this.Text = "HKCARE - Bán hàng POS & Online";
            this.Size = new Size(920, 560);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = new Font("Segoe UI", 9.5F);

            var lblKenh = new Label { Text = "Kênh bán:", Location = new Point(20, 22), AutoSize = true };
            _cboKenhBan.Location = new Point(95, 18);
            _cboKenhBan.Size = new Size(130, 28);
            _cboKenhBan.DropDownStyle = ComboBoxStyle.DropDownList;
            _cboKenhBan.Items.AddRange(new object[] { "POS tại quầy", "Đơn Online" });
            _cboKenhBan.SelectedIndex = 0;

            var lblThuoc = new Label { Text = "Thuốc:", Location = new Point(245, 22), AutoSize = true };
            _cboThuoc.Location = new Point(300, 18);
            _cboThuoc.Size = new Size(310, 28);
            _cboThuoc.DropDownStyle = ComboBoxStyle.DropDownList;

            var lblSl = new Label { Text = "SL:", Location = new Point(625, 22), AutoSize = true };
            _nudSoLuong.Location = new Point(655, 18);
            _nudSoLuong.Size = new Size(70, 28);
            _nudSoLuong.Minimum = 1;
            _nudSoLuong.Maximum = 10000;
            _nudSoLuong.Value = 1;

            _btnThemGio.Text = "Thêm vào giỏ";
            _btnThemGio.Location = new Point(740, 17);
            _btnThemGio.Size = new Size(140, 30);

            _dgvGioHang.Location = new Point(20, 62);
            _dgvGioHang.Size = new Size(860, 290);
            _dgvGioHang.DataSource = _dtGio;
            _dgvGioHang.ReadOnly = true;
            _dgvGioHang.AllowUserToAddRows = false;
            _dgvGioHang.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _dgvGioHang.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            _btnXoaDong.Text = "Xóa dòng chọn";
            _btnXoaDong.Location = new Point(20, 365);
            _btnXoaDong.Size = new Size(125, 30);

            _chkCoDonThuoc.Text = "Có xác nhận đơn thuốc (Rx)";
            _chkCoDonThuoc.Location = new Point(165, 370);
            _chkCoDonThuoc.AutoSize = true;

            var lblDon = new Label { Text = "Mã đơn:", Location = new Point(375, 370), AutoSize = true };
            _txtMaDonThuoc.Location = new Point(435, 366);
            _txtMaDonThuoc.Size = new Size(150, 28);
            _txtMaDonThuoc.Enabled = false;

            var lblKm = new Label { Text = "Khuyến mãi:", Location = new Point(605, 370), AutoSize = true };
            _cboKhuyenMai.Location = new Point(695, 366);
            _cboKhuyenMai.Size = new Size(185, 28);
            _cboKhuyenMai.DropDownStyle = ComboBoxStyle.DropDownList;
            _cboKhuyenMai.Items.AddRange(new object[]
            {
                "Không giảm (0%)",
                "HKCARE5 - Giảm 5%",
                "HKCARE10 - Giảm 10%",
                "MEMBER15 - Giảm 15%"
            });
            _cboKhuyenMai.SelectedIndex = 0;

            _lblTongTien.Location = new Point(20, 415);
            _lblTongTien.Size = new Size(650, 80);
            _lblTongTien.UseMnemonic = false;
            _lblTongTien.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

            _btnThanhToan.Text = "Xác nhận thanh toán";
            _btnThanhToan.Location = new Point(695, 420);
            _btnThanhToan.Size = new Size(185, 42);

            this.Controls.AddRange(new Control[]
            {
                lblKenh, _cboKenhBan, lblThuoc, _cboThuoc, lblSl, _nudSoLuong, _btnThemGio,
                _dgvGioHang, _btnXoaDong, _chkCoDonThuoc, lblDon, _txtMaDonThuoc,
                lblKm, _cboKhuyenMai, _lblTongTien, _btnThanhToan
            });
        }

        private void FrmBanHang_Load(object? sender, EventArgs e)
        {
            TaiDanhSachThuoc();
            var colKeDon = _dgvGioHang.Columns["KeDon"];
            if (colKeDon is not null)
            {
                colKeDon.Visible = false;
            }
            TinhLaiTongTien();
        }

        private void TaiDanhSachThuoc()
        {
            try
            {
                using var conn = new SqlConnection(_connStr);
                using var da = new SqlDataAdapter(@"
                    SELECT MaSP,
                           TenThuoc + N' (' + MaSP + N') - ' +
                           CASE WHEN KeDon = 1 THEN N'Rx' ELSE N'OTC' END +
                           N' - Tồn: ' + CAST(TonKho AS NVARCHAR(20)) AS HienThi
                    FROM Thuoc
                    ORDER BY MaSP", conn);
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

        private void BtnThemGio_Click(object? sender, EventArgs e)
        {
            string? maSP = _cboThuoc.SelectedValue?.ToString();
            int soLuongMua = (int)_nudSoLuong.Value;
            if (string.IsNullOrWhiteSpace(maSP)) return;

            try
            {
                using var conn = new SqlConnection(_connStr);
                using var cmd = new SqlCommand(
                    "SELECT MaSP, TenThuoc, GiaBan, TonKho, KeDon FROM Thuoc WHERE MaSP = @MaSP", conn);
                cmd.Parameters.Add("@MaSP", SqlDbType.VarChar, 10).Value = maSP;
                conn.Open();

                using var rd = cmd.ExecuteReader();
                if (!rd.Read()) return;

                string tenThuoc = rd["TenThuoc"]?.ToString() ?? string.Empty;
                decimal donGia = rd["GiaBan"] != DBNull.Value ? Convert.ToDecimal(rd["GiaBan"]) : 0m;
                int tonKho = rd["TonKho"] != DBNull.Value ? Convert.ToInt32(rd["TonKho"]) : 0;
                bool keDon = rd["KeDon"] != DBNull.Value && Convert.ToBoolean(rd["KeDon"]);

                DataRow? existing = null;
                foreach (DataRow r in _dtGio.Rows)
                {
                    if (string.Equals(r["MaSP"]?.ToString(), maSP, StringComparison.OrdinalIgnoreCase))
                    {
                        existing = r;
                        break;
                    }
                }

                int slMoi = (existing != null ? Convert.ToInt32(existing["SoLuong"]) : 0) + soLuongMua;
                if (slMoi > tonKho)
                {
                    MessageBox.Show($"Số lượng vượt tồn kho hiện tại ({tonKho}).", "Không đủ tồn kho",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (existing != null)
                {
                    existing["SoLuong"] = slMoi;
                    existing["ThanhTien"] = slMoi * donGia;
                }
                else
                {
                    _dtGio.Rows.Add(
                        maSP,
                        tenThuoc,
                        keDon,
                        keDon ? "Kê đơn (Rx)" : "Không kê đơn (OTC)",
                        donGia,
                        soLuongMua,
                        soLuongMua * donGia
                    );
                }

                TinhLaiTongTien();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thêm vào giỏ: " + ex.Message, "Lỗi SQL",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnXoaDong_Click(object? sender, EventArgs e)
        {
            if (_dgvGioHang.CurrentRow?.DataBoundItem is DataRowView rv)
            {
                _dtGio.Rows.Remove(rv.Row);
                TinhLaiTongTien();
            }
        }

        private decimal LayTyLeGiamGia()
        {
            string km = _cboKhuyenMai.SelectedItem?.ToString() ?? string.Empty;
            if (km.Contains("15%")) return 0.15m;
            if (km.Contains("10%")) return 0.10m;
            if (km.Contains("5%")) return 0.05m;
            return 0m;
        }

        private string? LayMaKhuyenMaiChon()
        {
            string km = (_cboKhuyenMai.SelectedItem?.ToString() ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(km) || km.StartsWith("Không giảm", StringComparison.OrdinalIgnoreCase))
                return null;

            int dashIdx = km.IndexOf(" - ", StringComparison.Ordinal);
            string ma = dashIdx > 0 ? km[..dashIdx].Trim() : km;
            return ma.Length > 10 ? ma[..10] : ma;
        }

        private (decimal TamTinh, decimal GiamGia, decimal ThanhTien) TinhCacKhoanTien()
        {
            decimal tamTinh = 0m;
            foreach (DataRow r in _dtGio.Rows)
            {
                tamTinh += r["ThanhTien"] != DBNull.Value ? Convert.ToDecimal(r["ThanhTien"]) : 0m;
            }

            decimal tyLe = LayTyLeGiamGia();
            decimal giamGia = Math.Round(tamTinh * tyLe, 0, MidpointRounding.AwayFromZero);
            decimal thanhTien = tamTinh - giamGia;
            return (tamTinh, giamGia, thanhTien);
        }

        private void TinhLaiTongTien()
        {
            bool coThuocKeDon = false;
            foreach (DataRow r in _dtGio.Rows)
            {
                if (r["KeDon"] != DBNull.Value && Convert.ToBoolean(r["KeDon"]))
                {
                    coThuocKeDon = true;
                    break;
                }
            }

            var (tamTinh, giamGia, thanhTien) = TinhCacKhoanTien();
            var vi = CultureInfo.GetCultureInfo("vi-VN");
            string rxNote = coThuocKeDon ? "Có thuốc kê đơn (Rx) — cần mã đơn" : "Chỉ có thuốc OTC";
            _lblTongTien.Text =
                $"Tạm tính: {tamTinh.ToString("N0", vi)}đ | Giảm giá: -{giamGia.ToString("N0", vi)}đ | " +
                $"Thanh toán: {thanhTien.ToString("N0", vi)}đ\n" +
                $"Kênh: {_cboKenhBan.SelectedItem} | {rxNote}";
        }

        private void BtnThanhToan_Click(object? sender, EventArgs e)
        {
            if (_dtGio.Rows.Count == 0)
            {
                MessageBox.Show("Giỏ hàng đang trống.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool coThuocKeDon = false;
            foreach (DataRow r in _dtGio.Rows)
            {
                if (r["KeDon"] != DBNull.Value && Convert.ToBoolean(r["KeDon"]))
                {
                    coThuocKeDon = true;
                    break;
                }
            }

            if (coThuocKeDon && (!_chkCoDonThuoc.Checked || string.IsNullOrWhiteSpace(_txtMaDonThuoc.Text)))
            {
                MessageBox.Show("Giỏ hàng có thuốc kê đơn (Rx). Vui lòng tích xác nhận đơn thuốc và nhập Mã đơn.",
                    "Yêu cầu đơn thuốc", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var (tamTinh, giamGia, thanhTien) = TinhCacKhoanTien();
            string kenhBan = _cboKenhBan.SelectedItem?.ToString() ?? "POS tại quầy";
            string? maKM = LayMaKhuyenMaiChon();
            string? maDonThuoc = _chkCoDonThuoc.Checked ? _txtMaDonThuoc.Text.Trim() : null;
            if (string.IsNullOrWhiteSpace(maDonThuoc)) maDonThuoc = null;

            using var conn = new SqlConnection(_connStr);
            conn.Open();
            using var tran = conn.BeginTransaction();

            try
            {
                foreach (DataRow r in _dtGio.Rows)
                {
                    string maSP = r["MaSP"]?.ToString() ?? string.Empty;
                    int slMua = Convert.ToInt32(r["SoLuong"]);

                    using var cmdTruLo = new SqlCommand(@"
                        DECLARE @canTru INT = @SoLuongMua;
                        DECLARE @idLo INT, @slLo INT;

                        DECLARE curLo CURSOR LOCAL FAST_FORWARD FOR
                            SELECT Id, SoLuong
                            FROM LoThuoc
                            WHERE MaSP = @MaSP AND SoLuong > 0
                            ORDER BY HanSuDung ASC, Id ASC;

                        OPEN curLo;
                        FETCH NEXT FROM curLo INTO @idLo, @slLo;
                        WHILE @@FETCH_STATUS = 0 AND @canTru > 0
                        BEGIN
                            IF @slLo <= @canTru
                            BEGIN
                                UPDATE LoThuoc SET SoLuong = 0 WHERE Id = @idLo;
                                SET @canTru = @canTru - @slLo;
                            END
                            ELSE
                            BEGIN
                                UPDATE LoThuoc SET SoLuong = SoLuong - @canTru WHERE Id = @idLo;
                                SET @canTru = 0;
                            END
                            FETCH NEXT FROM curLo INTO @idLo, @slLo;
                        END
                        CLOSE curLo;
                        DEALLOCATE curLo;

                        IF @canTru > 0
                            THROW 50001, N'Số lượng lô khả dụng không đủ để xuất bán.', 1;

                        UPDATE Thuoc
                        SET TonKho = ISNULL((SELECT SUM(SoLuong) FROM LoThuoc WHERE MaSP = @MaSP), 0)
                        WHERE MaSP = @MaSP;", conn, tran);

                    cmdTruLo.Parameters.Add("@MaSP", SqlDbType.VarChar, 10).Value = maSP;
                    cmdTruLo.Parameters.Add("@SoLuongMua", SqlDbType.Int).Value = slMua;
                    cmdTruLo.ExecuteNonQuery();
                }

                string maHD = "HD" + DateTime.Now.ToString("yyMMddHHmmss");

                using var cmdHd = new SqlCommand(@"
                    DECLARE @maKmHopLe VARCHAR(10) = NULL;
                    IF @MaKM IS NOT NULL AND EXISTS (SELECT 1 FROM dbo.KhuyenMai WHERE MaKM = @MaKM)
                        SET @maKmHopLe = @MaKM;

                    INSERT INTO dbo.HoaDon
                        (MaHD, NgayLap, MaKH, MaKM, HinhThucThanhToan, TamTinh, GiamGia, TongTien, MaDonThuoc, BacSiKeDon)
                    VALUES
                        (@MaHD, GETDATE(), NULL, @maKmHopLe, @HinhThucThanhToan, @TamTinh, @GiamGia, @TongTien, @MaDonThuoc, NULL);",
                    conn, tran);
                cmdHd.Parameters.Add("@MaHD", SqlDbType.VarChar, 20).Value = maHD;
                cmdHd.Parameters.Add("@MaKM", SqlDbType.VarChar, 10).Value =
                    string.IsNullOrWhiteSpace(maKM) ? DBNull.Value : maKM;
                cmdHd.Parameters.Add("@HinhThucThanhToan", SqlDbType.NVarChar, 30).Value = kenhBan;
                cmdHd.Parameters.Add("@TamTinh", SqlDbType.Decimal).Value = tamTinh;
                cmdHd.Parameters.Add("@GiamGia", SqlDbType.Decimal).Value = giamGia;
                cmdHd.Parameters.Add("@TongTien", SqlDbType.Decimal).Value = thanhTien;
                cmdHd.Parameters.Add("@MaDonThuoc", SqlDbType.NVarChar, 60).Value =
                    string.IsNullOrWhiteSpace(maDonThuoc) ? DBNull.Value : maDonThuoc;
                cmdHd.ExecuteNonQuery();

                foreach (DataRow r in _dtGio.Rows)
                {
                    using var cmdCt = new SqlCommand(@"
                        INSERT INTO dbo.ChiTietHoaDon (MaHD, MaSP, SoLuong, DonGia, ThanhTien)
                        VALUES (@MaHD, @MaSP, @SoLuong, @DonGia, @ThanhTien);",
                        conn, tran);
                    cmdCt.Parameters.Add("@MaHD", SqlDbType.VarChar, 20).Value = maHD;
                    cmdCt.Parameters.Add("@MaSP", SqlDbType.VarChar, 10).Value = r["MaSP"]?.ToString() ?? string.Empty;
                    cmdCt.Parameters.Add("@SoLuong", SqlDbType.Int).Value = Convert.ToInt32(r["SoLuong"]);
                    cmdCt.Parameters.Add("@DonGia", SqlDbType.Decimal).Value = Convert.ToDecimal(r["DonGia"]);
                    cmdCt.Parameters.Add("@ThanhTien", SqlDbType.Decimal).Value = Convert.ToDecimal(r["ThanhTien"]);
                    cmdCt.ExecuteNonQuery();
                }

                tran.Commit();

                _dtGio.Rows.Clear();
                _txtMaDonThuoc.Clear();
                _chkCoDonThuoc.Checked = false;
                _cboKhuyenMai.SelectedIndex = 0;
                TaiDanhSachThuoc();
                TinhLaiTongTien();

                MessageBox.Show($"Đã ghi nhận hóa đơn {maHD}, trừ lô FEFO và cập nhật tồn kho thành công.",
                    "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                try { tran.Rollback(); } catch { }
                MessageBox.Show("Lỗi khi thanh toán: " + ex.Message, "Lỗi SQL",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}