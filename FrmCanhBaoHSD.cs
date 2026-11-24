using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace nhathuoc
{
    public class FrmCanhBaoHSD : Form
    {
        private readonly string _connStr =
            @"Server=.\SQLEXPRESS;Database=QL_NhaThuoc_HKCARE;Integrated Security=True;TrustServerCertificate=True;";

        private readonly ComboBox _cboMucCanhBao = new();
        private readonly Button _btnTaiLai = new();
        private readonly Label _lblTongHop = new();
        private readonly DataGridView _dgvCanhBao = new();

        public FrmCanhBaoHSD()
        {
            BuildUi();
            this.Load += (s, e) => TaiDuLieuCanhBao();
            _cboMucCanhBao.SelectedIndexChanged += (s, e) => TaiDuLieuCanhBao();
            _btnTaiLai.Click += (s, e) => TaiDuLieuCanhBao();
        }

        private void BuildUi()
        {
            this.Text = "HKCARE - Cảnh báo Hạn sử dụng & Tồn kho thấp";
            this.Size = new Size(940, 540);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = new Font("Segoe UI", 9.5F);

            var lblLoc = new Label { Text = "Mức cảnh báo:", Location = new Point(20, 22), AutoSize = true };
            _cboMucCanhBao.Location = new Point(115, 18);
            _cboMucCanhBao.Size = new Size(250, 28);
            _cboMucCanhBao.DropDownStyle = ComboBoxStyle.DropDownList;
            _cboMucCanhBao.Items.AddRange(new object[]
            {
                "Sắp hết hạn trong 90 ngày & Đã hết hạn",
                "Sắp hết hạn trong 30 ngày",
                "Đã hết hạn (HSD < hôm nay)",
                "Thuốc tồn kho thấp (<= 10)"
            });
            _cboMucCanhBao.SelectedIndex = 0;

            _btnTaiLai.Text = "Làm mới";
            _btnTaiLai.Location = new Point(380, 17);
            _btnTaiLai.Size = new Size(100, 30);

            _lblTongHop.Location = new Point(495, 22);
            _lblTongHop.Size = new Size(415, 24);
            _lblTongHop.UseMnemonic = false;
            _lblTongHop.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);

            _dgvCanhBao.Location = new Point(20, 60);
            _dgvCanhBao.Size = new Size(885, 420);
            _dgvCanhBao.ReadOnly = true;
            _dgvCanhBao.AllowUserToAddRows = false;
            _dgvCanhBao.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _dgvCanhBao.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            this.Controls.AddRange(new Control[]
            {
                lblLoc, _cboMucCanhBao, _btnTaiLai, _lblTongHop, _dgvCanhBao
            });
        }

        private void TaiDuLieuCanhBao()
        {
            try
            {
                int mode = _cboMucCanhBao.SelectedIndex;
                string sql;

                if (mode == 3)
                {
                    sql = @"
                        SELECT
                            t.MaSP,
                            t.TenThuoc,
                            t.DonViTinh,
                            t.TonKho,
                            CASE WHEN t.KeDon = 1 THEN N'Kê đơn (Rx)' ELSE N'Không kê đơn (OTC)' END AS LoaiThuoc,
                            N'Tồn kho thấp (<= 10)' AS TrangThai
                        FROM Thuoc t
                        WHERE t.TonKho <= 10
                        ORDER BY t.TonKho ASC, t.MaSP ASC;";
                }
                else
                {
                    int soNgay = mode == 1 ? 30 : 90;
                    string dieuKienNgay = mode == 2
                        ? "l.HanSuDung < CAST(GETDATE() AS DATE)"
                        : $"l.HanSuDung <= DATEADD(DAY, {soNgay}, CAST(GETDATE() AS DATE))";

                    sql = $@"
                        SELECT
                            l.MaSP,
                            t.TenThuoc,
                            l.SoLo,
                            l.HanSuDung,
                            DATEDIFF(DAY, CAST(GETDATE() AS DATE), l.HanSuDung) AS SoNgayConLai,
                            l.SoLuong,
                            CASE
                                WHEN l.HanSuDung < CAST(GETDATE() AS DATE) THEN N'Đã hết hạn'
                                WHEN DATEDIFF(DAY, CAST(GETDATE() AS DATE), l.HanSuDung) <= 30 THEN N'Khẩn cấp (<= 30 ngày)'
                                ELSE N'Sắp hết hạn (<= 90 ngày)'
                            END AS TrangThai
                        FROM LoThuoc l
                        INNER JOIN Thuoc t ON t.MaSP = l.MaSP
                        WHERE l.SoLuong > 0 AND {dieuKienNgay}
                        ORDER BY l.HanSuDung ASC, l.MaSP ASC;";
                }

                using var conn = new SqlConnection(_connStr);
                using var da = new SqlDataAdapter(sql, conn);
                var dt = new DataTable();
                da.Fill(dt);

                _dgvCanhBao.DataSource = dt;
                _lblTongHop.Text = $"Số bản ghi cảnh báo: {dt.Rows.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải cảnh báo: " + ex.Message, "Lỗi SQL",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}