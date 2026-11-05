namespace Aplicacion
{
    partial class AsistenciaEstudiantes
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AsistenciaEstudiantes));
            panel1 = new Panel();
            button1 = new Button();
            label1 = new Label();
            label3 = new Label();
            timer1 = new System.Windows.Forms.Timer(components);
            label2 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            comboBox1 = new ComboBox();
            button2 = new Button();
            Estado = new Label();
            btnDetener = new Button();
            dgvLista = new DataGridView();
            btnMarcarID = new Button();
            lblContador = new Label();
            txtIdManual = new TextBox();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvLista).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(47, 74, 160);
            panel1.Controls.Add(button1);
            panel1.Controls.Add(label1);
            panel1.Location = new Point(-2, -2);
            panel1.Name = "panel1";
            panel1.Size = new Size(1012, 50);
            panel1.TabIndex = 0;
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
            label1.Font = new Font("Segoe UI Semibold", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = SystemColors.Info;
            label1.Location = new Point(0, 0);
            label1.Name = "label1";
            label1.Size = new Size(1012, 50);
            label1.TabIndex = 1;
            label1.Text = "Registrar asistencia";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            label3.Font = new Font("Segoe UI", 24F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.ForeColor = Color.FromArgb(17, 49, 136);
            label3.Location = new Point(608, 140);
            label3.Name = "label3";
            label3.Size = new Size(318, 93);
            label3.TabIndex = 4;
            label3.Text = "Configuración del registro";
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Interval = 1000;
            timer1.Tick += timer1_Tick;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.ForeColor = Color.FromArgb(68, 74, 149);
            label2.Location = new Point(80, 150);
            label2.Name = "label2";
            label2.Size = new Size(226, 32);
            label2.TabIndex = 5;
            label2.Text = "Fecha y hora actual";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label4.ForeColor = Color.FromArgb(68, 74, 149);
            label4.Location = new Point(80, 192);
            label4.Name = "label4";
            label4.Size = new Size(169, 32);
            label4.TabIndex = 6;
            label4.Text = "{Fecha y hora}";
            // 
            // label5
            // 
            label5.Font = new Font("Segoe UI", 12F);
            label5.Location = new Point(608, 261);
            label5.Name = "label5";
            label5.Size = new Size(285, 92);
            label5.TabIndex = 7;
            label5.Text = "Seleccione un grado antes de proceder con el escaneo biometrico del estudiante\r\n";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label6.ForeColor = SystemColors.ControlDarkDark;
            label6.Location = new Point(608, 363);
            label6.Name = "label6";
            label6.Size = new Size(193, 21);
            label6.TabIndex = 8;
            label6.Text = "SELECCIONE UN GRADO";
            // 
            // comboBox1
            // 
            comboBox1.BackColor = Color.FromArgb(234, 232, 231);
            comboBox1.FlatStyle = FlatStyle.Flat;
            comboBox1.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            comboBox1.ForeColor = SystemColors.InactiveCaptionText;
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(608, 414);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(285, 28);
            comboBox1.TabIndex = 9;
            comboBox1.Text = "   Seleccionar grado académico";
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            // 
            // button2
            // 
            button2.BackColor = Color.FromArgb(47, 74, 160);
            button2.FlatAppearance.BorderColor = Color.Black;
            button2.FlatAppearance.MouseDownBackColor = Color.FromArgb(47, 74, 160);
            button2.FlatAppearance.MouseOverBackColor = Color.FromArgb(47, 74, 160);
            button2.FlatStyle = FlatStyle.Flat;
            button2.Font = new Font("Segoe UI Semibold", 15F, FontStyle.Bold);
            button2.ForeColor = SystemColors.Control;
            button2.Location = new Point(476, 541);
            button2.Name = "button2";
            button2.Size = new Size(209, 80);
            button2.TabIndex = 10;
            button2.Text = "Registrar asistencia por huella";
            button2.UseVisualStyleBackColor = false;
            button2.Click += button2_Click;
            // 
            // Estado
            // 
            Estado.AutoSize = true;
            Estado.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            Estado.ForeColor = Color.FromArgb(68, 74, 149);
            Estado.Location = new Point(85, 261);
            Estado.Name = "Estado";
            Estado.Size = new Size(221, 25);
            Estado.TabIndex = 12;
            Estado.Text = "• ESTADO DEL SISTEMA";
            // 
            // btnDetener
            // 
            btnDetener.BackColor = Color.FromArgb(47, 74, 160);
            btnDetener.FlatAppearance.BorderColor = Color.Black;
            btnDetener.FlatAppearance.MouseDownBackColor = Color.FromArgb(47, 74, 160);
            btnDetener.FlatAppearance.MouseOverBackColor = Color.FromArgb(47, 74, 160);
            btnDetener.FlatStyle = FlatStyle.Flat;
            btnDetener.Font = new Font("Segoe UI Semibold", 15F, FontStyle.Bold);
            btnDetener.ForeColor = SystemColors.Control;
            btnDetener.Location = new Point(161, 541);
            btnDetener.Name = "btnDetener";
            btnDetener.Size = new Size(212, 80);
            btnDetener.TabIndex = 13;
            btnDetener.Text = "Detener asistencia";
            btnDetener.UseVisualStyleBackColor = false;
            btnDetener.Click += btnDetener_Click;
            // 
            // dgvLista
            // 
            dgvLista.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvLista.Location = new Point(302, 66);
            dgvLista.Name = "dgvLista";
            dgvLista.Size = new Size(435, 407);
            dgvLista.TabIndex = 14;
            dgvLista.Visible = false;
            dgvLista.CellContentClick += dataGridView1_CellContentClick;
            // 
            // btnMarcarID
            // 
            btnMarcarID.BackColor = Color.FromArgb(47, 74, 160);
            btnMarcarID.FlatAppearance.BorderColor = Color.Black;
            btnMarcarID.FlatAppearance.MouseDownBackColor = Color.FromArgb(47, 74, 160);
            btnMarcarID.FlatAppearance.MouseOverBackColor = Color.FromArgb(47, 74, 160);
            btnMarcarID.FlatStyle = FlatStyle.Flat;
            btnMarcarID.Font = new Font("Segoe UI Semibold", 15F, FontStyle.Bold);
            btnMarcarID.ForeColor = SystemColors.Control;
            btnMarcarID.Location = new Point(768, 541);
            btnMarcarID.Name = "btnMarcarID";
            btnMarcarID.Size = new Size(209, 80);
            btnMarcarID.TabIndex = 15;
            btnMarcarID.Text = "Registrar asistencia por ID";
            btnMarcarID.UseVisualStyleBackColor = false;
            btnMarcarID.Click += btnMarcarID_Click;
            // 
            // lblContador
            // 
            lblContador.AutoSize = true;
            lblContador.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblContador.ForeColor = Color.FromArgb(68, 74, 149);
            lblContador.Location = new Point(476, 496);
            lblContador.Name = "lblContador";
            lblContador.Size = new Size(0, 21);
            lblContador.TabIndex = 16;
            lblContador.Visible = false;
            // 
            // txtIdManual
            // 
            txtIdManual.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txtIdManual.Location = new Point(317, 436);
            txtIdManual.Name = "txtIdManual";
            txtIdManual.Size = new Size(140, 29);
            txtIdManual.TabIndex = 17;
            txtIdManual.Visible = false;
            txtIdManual.TextChanged += txtIdManual_TextChanged;
            txtIdManual.KeyDown += txtIdManual_KeyDown;
            // 
            // AsistenciaEstudiantes
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(246, 243, 242);
            ClientSize = new Size(1008, 729);
            Controls.Add(txtIdManual);
            Controls.Add(lblContador);
            Controls.Add(btnMarcarID);
            Controls.Add(dgvLista);
            Controls.Add(btnDetener);
            Controls.Add(Estado);
            Controls.Add(button2);
            Controls.Add(comboBox1);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label2);
            Controls.Add(label3);
            Controls.Add(panel1);
            Name = "AsistenciaEstudiantes";
            StartPosition = FormStartPosition.CenterScreen;
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvLista).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panel1;
        private Button button1;
        private Label label1;
        private Label label3;
        private System.Windows.Forms.Timer timer1;
        private Label label2;
        private Label label4;
        private Label label5;
        private Label label6;
        private ComboBox comboBox1;
        private Button button2;
        private Label Estado;
        private Button btnDetener;
        private DataGridView dgvLista;
        private Button btnMarcarID;
        private Label lblContador;
        private TextBox txtIdManual;
    }
}