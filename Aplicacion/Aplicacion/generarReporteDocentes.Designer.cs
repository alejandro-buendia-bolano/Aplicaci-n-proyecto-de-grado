namespace Aplicacion
{
    partial class generarReporteDocentes
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(generarReporteDocentes));
            panel1 = new Panel();
            button1 = new Button();
            label1 = new Label();
            dataGridView1 = new DataGridView();
            button3 = new Button();
            button2 = new Button();
            comboBox3 = new ComboBox();
            comboBox2 = new ComboBox();
            comboBox1 = new ComboBox();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(47, 74, 160);
            panel1.Controls.Add(button1);
            panel1.Controls.Add(label1);
            panel1.Location = new Point(1, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(1012, 50);
            panel1.TabIndex = 10;
            // 
            // button1
            // 
            button1.BackgroundImage = (Image)resources.GetObject("button1.BackgroundImage");
            button1.BackgroundImageLayout = ImageLayout.Zoom;
            button1.FlatAppearance.BorderColor = Color.FromArgb(47, 74, 160);
            button1.FlatAppearance.MouseDownBackColor = Color.FromArgb(47, 74, 160);
            button1.FlatAppearance.MouseOverBackColor = Color.FromArgb(47, 74, 160);
            button1.FlatStyle = FlatStyle.Flat;
            button1.ForeColor = Color.FromArgb(47, 74, 160);
            button1.Location = new Point(9, 3);
            button1.Name = "button1";
            button1.Size = new Size(66, 44);
            button1.TabIndex = 1;
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label1
            // 
            label1.Dock = DockStyle.Fill;
            label1.Font = new Font("Segoe UI Semibold", 15.75F, FontStyle.Bold);
            label1.ForeColor = Color.White;
            label1.Location = new Point(0, 0);
            label1.Name = "label1";
            label1.Size = new Size(1012, 50);
            label1.TabIndex = 1;
            label1.Text = "Generar reporte";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(26, 179);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.Size = new Size(939, 439);
            dataGridView1.TabIndex = 19;
            // 
            // button3
            // 
            button3.BackColor = Color.FromArgb(235, 233, 231);
            button3.Font = new Font("Segoe UI", 11F);
            button3.Location = new Point(862, 96);
            button3.Name = "button3";
            button3.Size = new Size(123, 48);
            button3.TabIndex = 18;
            button3.Text = "Exportar PDF";
            button3.UseVisualStyleBackColor = false;
            button3.Click += button3_Click;
            // 
            // button2
            // 
            button2.BackColor = Color.FromArgb(235, 233, 231);
            button2.Font = new Font("Segoe UI", 11F);
            button2.Location = new Point(713, 96);
            button2.Name = "button2";
            button2.Size = new Size(123, 49);
            button2.TabIndex = 17;
            button2.Text = "Generar reporte";
            button2.UseVisualStyleBackColor = false;
            button2.Click += button2_Click;
            // 
            // comboBox3
            // 
            comboBox3.BackColor = Color.FromArgb(235, 233, 231);
            comboBox3.Font = new Font("Segoe UI", 10F);
            comboBox3.FormattingEnabled = true;
            comboBox3.Location = new Point(477, 120);
            comboBox3.Name = "comboBox3";
            comboBox3.Size = new Size(181, 25);
            comboBox3.TabIndex = 16;
            comboBox3.Text = "Seleccione una semana";
            comboBox3.SelectedIndexChanged += comboBox3_SelectedIndexChanged;
            // 
            // comboBox2
            // 
            comboBox2.BackColor = Color.FromArgb(235, 233, 231);
            comboBox2.Font = new Font("Segoe UI", 10F);
            comboBox2.FormattingEnabled = true;
            comboBox2.Location = new Point(274, 120);
            comboBox2.Name = "comboBox2";
            comboBox2.Size = new Size(152, 25);
            comboBox2.TabIndex = 15;
            comboBox2.Text = "Seleccione un mes";
            comboBox2.SelectedIndexChanged += comboBox2_SelectedIndexChanged;
            // 
            // comboBox1
            // 
            comboBox1.BackColor = Color.FromArgb(235, 233, 231);
            comboBox1.Font = new Font("Segoe UI", 10F);
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(32, 120);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(173, 25);
            comboBox1.TabIndex = 14;
            comboBox1.Text = " Seleccione un docente";
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label4.ForeColor = Color.DimGray;
            label4.Location = new Point(477, 96);
            label4.Name = "label4";
            label4.Size = new Size(151, 21);
            label4.TabIndex = 13;
            label4.Text = "RANGO DE TIEMPO";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.ForeColor = Color.DimGray;
            label3.Location = new Point(274, 96);
            label3.Name = "label3";
            label3.Size = new Size(42, 21);
            label3.TabIndex = 12;
            label3.Text = "MES";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.ForeColor = Color.DimGray;
            label2.Location = new Point(32, 96);
            label2.Name = "label2";
            label2.Size = new Size(80, 21);
            label2.TabIndex = 11;
            label2.Text = "DOCENTE";
            // 
            // generarReporteDocentes
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1008, 729);
            Controls.Add(panel1);
            Controls.Add(dataGridView1);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(comboBox3);
            Controls.Add(comboBox2);
            Controls.Add(comboBox1);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Name = "generarReporteDocentes";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "generarReporteDocentes";
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panel1;
        private Button button1;
        private Label label1;
        private DataGridView dataGridView1;
        private Button button3;
        private Button button2;
        private ComboBox comboBox3;
        private ComboBox comboBox2;
        private ComboBox comboBox1;
        private Label label4;
        private Label label3;
        private Label label2;
    }
}