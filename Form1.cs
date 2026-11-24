using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace nhathuoc
{
    public partial class Form1 : Form
    {
        private readonly string _connStr =
            @"Server=.\SQLEXPRESS;Database=QL_NhaThuoc_HKCARE;Integrated Security=True;TrustServerCertificate=True;";

        public Form1()
        {
            InitializeComponent();

            this.Load += Form1_Load;
            btnTimKiem.Click += (s, e) => TaiDanhSachThuoc();
            btnTaiLai.Click += (s, e) =>
            {
                txtTimKiem.Clear();
                if (cboLoaiKeDon.Items.Count > 0) cboLoaiKeDon.SelectedIndex = 0;
                TaiDanhSachThuoc();
            };
            txtTimKiem.TextChanged += (s, e) => TaiDanhSachThuoc();
            cboLoaiKeDon.SelectedIndexChanged += (s, e) => TaiDanhSachThuoc();
            dgvThuoc.SelectionChanged += DgvThuoc_SelectionChanged;

            int topY = txtTimKiem.Top;
            int h = Math.Max(28, txtTimKiem.Height);
            int gap = 5;

            txtTimKiem.Location = new Point(dgvThuoc.Left, topY);
            txtTimKiem.Size = new Size(140, h);

            cboLoaiKeDon.Location = new Point(txtTimKiem.Right + gap, topY);
            cboLoaiKeDon.Size = new Size(110, h);

            btnTimKiem.Location = new Point(cboLoaiKeDon.Right + gap, topY);
            btnTimKiem.Size = new Size(78, h);

            btnTaiLai.Location = new Point(btnTimKiem.Right + gap, topY);
            btnTaiLai.Size = new Size(68, h);

            var btnQuanLyLo = new Button
            {
                Text = "Quản lý Lô",
                Size = new Size(92, h),
                Location = new Point(btnTaiLai.Right + gap, topY),
                UseMnemonic = false
            };
            btnQuanLyLo.Click += (s, e) =>
            {
                using var frm = new FrmQuanLyLo();
                frm.ShowDialog(this);
                TaiDanhSachThuoc();
            };

            var btnBanHang = new Button
            {
                Text = "Bán hàng",
                Size = new Size(80, h),
                Location = new Point(btnQuanLyLo.Right + gap, topY),
                UseMnemonic = false
            };
            btnBanHang.Click += (s, e) =>
            {
                using var frm = new FrmBanHang();
                frm.ShowDialog(this);
                TaiDanhSachThuoc();
            };

            var btnQuanLyThuoc = new Button
            {
                Text = "Danh mục Thuốc",
                Size = new Size(118, h),
                Location = new Point(btnBanHang.Right + gap, topY),
                UseMnemonic = false
            };
            btnQuanLyThuoc.Click += (s, e) =>
            {
                using var frm = new FrmQuanLyThuoc();
                frm.ShowDialog(this);
                TaiDanhSachThuoc();
            };
            var btnCanhBao = new Button
            {
                Text = "Cảnh báo HSD",
                Size = new Size(105, h),
                Location = new Point(btnQuanLyThuoc.Right + gap, topY),
                UseMnemonic = false
            };
            btnCanhBao.Click += (s, e) =>
            {
                using var frm = new FrmCanhBaoHSD();
                frm.ShowDialog(this);
                TaiDanhSachThuoc();
            };

            this.Controls.Add(btnCanhBao);
            btnCanhBao.BringToFront();

            int canChieuRong = btnCanhBao.Right + 24;
            if (this.ClientSize.Width < canChieuRong)
            {
                this.ClientSize = new Size(canChieuRong, this.ClientSize.Height);
            }
            var btnLichSuHd = new Button
            {
                Text = "Lịch sử HĐ",
                Size = new Size(95, h),
                Location = new Point(btnCanhBao.Right + gap, topY),
                UseMnemonic = false
            };
            btnLichSuHd.Click += (s, e) =>
            {
                using var frm = new FrmLichSuBanHang();
                frm.ShowDialog(this);
                TaiDanhSachThuoc();
            };
            this.Controls.Add(btnLichSuHd);
            btnLichSuHd.BringToFront();

            canChieuRong = btnLichSuHd.Right + 24;
            if (this.ClientSize.Width < canChieuRong)
            {
                this.ClientSize = new Size(canChieuRong, this.ClientSize.Height);
            }



            this.Controls.AddRange(new Control[] { btnQuanLyLo, btnBanHang, btnQuanLyThuoc });
            btnQuanLyLo.BringToFront();
            btnBanHang.BringToFront();
            btnQuanLyThuoc.BringToFront();

             canChieuRong = btnQuanLyThuoc.Right + 24;
            if (this.ClientSize.Width < canChieuRong)
            {
                this.ClientSize = new Size(canChieuRong, this.ClientSize.Height);
            }

        }

        private void Form1_Load(object? sender, EventArgs e)
        {
            lblThongTinLo.UseMnemonic = false;
            if (cboLoaiKeDon.Items.Count > 0 && cboLoaiKeDon.SelectedIndex < 0)
            {
                cboLoaiKeDon.SelectedIndex = 0;
            }
            TaiDanhSachThuoc();
        }

        private void TaiDanhSachThuoc()
        {
            try
            {
                string tuKhoa = (txtTimKiem.Text ?? string.Empty).Trim();
                int locKeDon = -1;

                string chon = cboLoaiKeDon.SelectedItem?.ToString() ?? string.Empty;
                if (chon.Contains("Kê đơn") && !chon.Contains("Không")) locKeDon = 1;
                else if (chon.Contains("Không kê đơn")) locKeDon = 0;

                string sql = @"
                    SELECT
                        t.MaSP,
                        t.TenThuoc,
                        t.DonViTinh,
                        t.GiaBan,
                        t.TonKho,
                        CASE WHEN t.KeDon = 1 THEN N'Kê đơn (Rx)' ELSE N'Không kê đơn (OTC)' END AS LoaiKeDon,
                        t.HinhAnh
                    FROM Thuoc t
                    WHERE (@q = ''
                           OR t.TenThuoc LIKE @like
                           OR t.MaSP LIKE @like
                           OR EXISTS (
                               SELECT 1 FROM LoThuoc l
                               WHERE l.MaSP = t.MaSP AND l.SoLo LIKE @like
                           ))
                      AND (@keDon = -1 OR t.KeDon = @keDon)
                    ORDER BY t.MaSP;";

                using var conn = new SqlConnection(_connStr);
                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@q", SqlDbType.NVarChar, 100).Value = tuKhoa;
                cmd.Parameters.Add("@like", SqlDbType.NVarChar, 102).Value = "%" + tuKhoa + "%";
                cmd.Parameters.Add("@keDon", SqlDbType.Int).Value = locKeDon;

                var dt = new DataTable();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                dgvThuoc.DataSource = dt;
                var colHinhAnh = dgvThuoc.Columns["HinhAnh"];
                if (colHinhAnh is not null)
                {
                    colHinhAnh.Visible = false;
                }

                if (dt.Rows.Count > 0)
                {
                    dgvThuoc.Rows[0].Selected = true;
                    HienThiChiTietThuoc(dt.Rows[0]);
                }
                else
                {
                    picAnhThuoc.Image = null;
                    lblThongTinLo.Text = "Không có thuốc phù hợp.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu thuốc: " + ex.Message, "Lỗi SQL",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvThuoc_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvThuoc.CurrentRow?.DataBoundItem is DataRowView rowView)
            {
                HienThiChiTietThuoc(rowView.Row);
            }
        }

        private string? TimFileAnhThuoc(string hinhAnhRel)
        {
            if (string.IsNullOrWhiteSpace(hinhAnhRel)) return null;

            string normalized = hinhAnhRel.Trim().Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;

            string[] candidates =
            {
                Path.Combine(baseDir, normalized),
                Path.Combine(baseDir, "images", Path.GetFileName(normalized)),
                Path.Combine(baseDir, "..", "..", "..", normalized),
                Path.Combine(baseDir, "..", "..", "..", "images", Path.GetFileName(normalized))
            };

            foreach (string path in candidates)
            {
                string full = Path.GetFullPath(path);
                if (File.Exists(full)) return full;
            }
            return null;
        }

        private void HienThiChiTietThuoc(DataRow row)
        {
            lblThongTinLo.UseMnemonic = false;

            string maSP = row["MaSP"]?.ToString() ?? string.Empty;
            string tenThuoc = row["TenThuoc"]?.ToString() ?? string.Empty;
            string dvt = row["DonViTinh"]?.ToString() ?? string.Empty;
            decimal giaBan = row["GiaBan"] != DBNull.Value ? Convert.ToDecimal(row["GiaBan"]) : 0m;
            int tonKho = row["TonKho"] != DBNull.Value ? Convert.ToInt32(row["TonKho"]) : 0;
            string loaiKeDon = row["LoaiKeDon"]?.ToString() ?? string.Empty;
            string hinhAnhRel = row["HinhAnh"]?.ToString() ?? string.Empty;

            picAnhThuoc.Image = null;
            string? imagePath = TimFileAnhThuoc(hinhAnhRel);
            if (imagePath is not null)
            {
                using var fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
                picAnhThuoc.Image = Image.FromStream(fs);
            }

            var sb = new StringBuilder();
            sb.AppendLine(tenThuoc + " (" + maSP + ")");
            sb.AppendLine("Loại: " + loaiKeDon + " | ĐVT: " + dvt);
            sb.AppendLine("Giá: " + giaBan.ToString("N0") + "đ | Tồn: " + tonKho);
            sb.AppendLine("--------------------------------");
            sb.AppendLine("Danh sách lô & HSD:");

            using var conn = new SqlConnection(_connStr);
            using var cmd = new SqlCommand(@"
                SELECT SoLo, HanSuDung, SoLuong
                FROM LoThuoc
                WHERE MaSP = @MaSP
                ORDER BY HanSuDung ASC", conn);
            cmd.Parameters.Add("@MaSP", SqlDbType.VarChar, 10).Value = maSP;
            conn.Open();

            using var rd = cmd.ExecuteReader();
            bool coLo = false;
            while (rd.Read())
            {
                coLo = true;
                string soLo = rd["SoLo"]?.ToString() ?? string.Empty;
                string hsd = rd["HanSuDung"] != DBNull.Value
                    ? Convert.ToDateTime(rd["HanSuDung"]).ToString("dd/MM/yyyy")
                    : "—";
                int sl = rd["SoLuong"] != DBNull.Value ? Convert.ToInt32(rd["SoLuong"]) : 0;
                sb.AppendLine($"• {soLo}: HSD {hsd} (SL: {sl})");
            }
            if (!coLo)
            {
                sb.AppendLine("Chưa có dữ liệu lô.");
            }

            lblThongTinLo.Text = sb.ToString();
        }
    }
}