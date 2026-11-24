namespace nhathuoc
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox txtTimKiem;
        private System.Windows.Forms.ComboBox cboLoaiKeDon;
        private System.Windows.Forms.Button btnTimKiem;
        private System.Windows.Forms.Button btnTaiLai;
        private System.Windows.Forms.DataGridView dgvThuoc;
        private System.Windows.Forms.PictureBox picAnhThuoc;
        private System.Windows.Forms.Label lblThongTinLo;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.txtTimKiem = new System.Windows.Forms.TextBox();
            this.cboLoaiKeDon = new System.Windows.Forms.ComboBox();
            this.btnTimKiem = new System.Windows.Forms.Button();
            this.btnTaiLai = new System.Windows.Forms.Button();
            this.dgvThuoc = new System.Windows.Forms.DataGridView();
            this.picAnhThuoc = new System.Windows.Forms.PictureBox();
            this.lblThongTinLo = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvThuoc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picAnhThuoc)).BeginInit();
            this.SuspendLayout();

            // txtTimKiem
            this.txtTimKiem.Location = new System.Drawing.Point(20, 18);
            this.txtTimKiem.Name = "txtTimKiem";
            this.txtTimKiem.Size = new System.Drawing.Size(340, 27);
            this.txtTimKiem.TabIndex = 0;

            // cboLoaiKeDon
            this.cboLoaiKeDon.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLoaiKeDon.FormattingEnabled = true;
            this.cboLoaiKeDon.Items.AddRange(new object[] {
                "Tất cả",
                "Kê đơn (Rx)",
                "Không kê đơn (OTC)"
            });
            this.cboLoaiKeDon.Location = new System.Drawing.Point(372, 18);
            this.cboLoaiKeDon.Name = "cboLoaiKeDon";
            this.cboLoaiKeDon.Size = new System.Drawing.Size(180, 28);
            this.cboLoaiKeDon.TabIndex = 1;

            // btnTimKiem
            this.btnTimKiem.Location = new System.Drawing.Point(564, 17);
            this.btnTimKiem.Name = "btnTimKiem";
            this.btnTimKiem.Size = new System.Drawing.Size(105, 30);
            this.btnTimKiem.TabIndex = 2;
            this.btnTimKiem.Text = "Tìm kiếm";
            this.btnTimKiem.UseVisualStyleBackColor = true;

            // btnTaiLai
            this.btnTaiLai.Location = new System.Drawing.Point(677, 17);
            this.btnTaiLai.Name = "btnTaiLai";
            this.btnTaiLai.Size = new System.Drawing.Size(95, 30);
            this.btnTaiLai.TabIndex = 3;
            this.btnTaiLai.Text = "Tải lại";
            this.btnTaiLai.UseVisualStyleBackColor = true;

            // dgvThuoc
            this.dgvThuoc.AllowUserToAddRows = false;
            this.dgvThuoc.AllowUserToDeleteRows = false;
            this.dgvThuoc.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvThuoc.Location = new System.Drawing.Point(20, 60);
            this.dgvThuoc.MultiSelect = false;
            this.dgvThuoc.Name = "dgvThuoc";
            this.dgvThuoc.ReadOnly = true;
            this.dgvThuoc.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvThuoc.Size = new System.Drawing.Size(752, 500);
            this.dgvThuoc.TabIndex = 4;

            // picAnhThuoc
            this.picAnhThuoc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picAnhThuoc.Location = new System.Drawing.Point(790, 60);
            this.picAnhThuoc.Name = "picAnhThuoc";
            this.picAnhThuoc.Size = new System.Drawing.Size(220, 220);
            this.picAnhThuoc.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picAnhThuoc.TabIndex = 5;
            this.picAnhThuoc.TabStop = false;

            // lblThongTinLo
            this.lblThongTinLo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblThongTinLo.Location = new System.Drawing.Point(790, 294);
            this.lblThongTinLo.Name = "lblThongTinLo";
            this.lblThongTinLo.Padding = new System.Windows.Forms.Padding(10);
            this.lblThongTinLo.Size = new System.Drawing.Size(220, 266);
            this.lblThongTinLo.TabIndex = 6;
            this.lblThongTinLo.Text = "Chọn một dòng thuốc để xem lô và hạn sử dụng.";

            // Form1
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1030, 580);
            this.Controls.Add(this.lblThongTinLo);
            this.Controls.Add(this.picAnhThuoc);
            this.Controls.Add(this.dgvThuoc);
            this.Controls.Add(this.btnTaiLai);
            this.Controls.Add(this.btnTimKiem);
            this.Controls.Add(this.cboLoaiKeDon);
            this.Controls.Add(this.txtTimKiem);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "HKCARE - Quản lý thuốc và Lô HSD";
            ((System.ComponentModel.ISupportInitialize)(this.dgvThuoc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picAnhThuoc)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}