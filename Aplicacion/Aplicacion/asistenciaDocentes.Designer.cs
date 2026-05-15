namespace Aplicacion
{
    partial class asistenciaDocentes
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(asistenciaDocentes));
            panel1 = new Panel();
            button1 = new Button();
            label1 = new Label();
            button2 = new Button();
            button3 = new Button();
            label4 = new Label();
            timer1 = new System.Windows.Forms.Timer(components);
            label2 = new Label();
            label3 = new Label();
            labelEstado = new Label();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(71, 74, 160);
            panel1.Controls.Add(button1);
            panel1.Controls.Add(label1);
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(1012, 50);
            panel1.TabIndex = 0;
            // 
            // button1
            // 
            button1.BackgroundImage = (Image)resources.GetObject("button1.BackgroundImage");
            button1.BackgroundImageLayout = ImageLayout.Zoom;
            button1.FlatAppearance.BorderColor = Color.FromArgb(71, 74, 160);
            button1.FlatAppearance.MouseDownBackColor = Color.FromArgb(71, 74, 160);
            button1.FlatAppearance.MouseOverBackColor = Color.FromArgb(71, 74, 160);
            button1.FlatStyle = FlatStyle.Flat;
            button1.Location = new Point(12, 3);
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
            label1.ForeColor = SystemColors.Control;
            label1.Location = new Point(0, 0);
            label1.Name = "label1";
            label1.Size = new Size(1012, 50);
            label1.TabIndex = 1;
            label1.Text = "Registrar asistencia";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // button2
            // 
            button2.BackgroundImage = Properties.Resources.icon_huella;
            button2.BackgroundImageLayout = ImageLayout.Zoom;
            button2.FlatAppearance.BorderColor = Color.FromArgb(251, 249, 248);
            button2.FlatAppearance.MouseDownBackColor = Color.FromArgb(251, 249, 248);
            button2.FlatAppearance.MouseOverBackColor = Color.FromArgb(251, 249, 248);
            button2.FlatStyle = FlatStyle.Flat;
            button2.Location = new Point(282, 56);
            button2.Name = "button2";
            button2.Size = new Size(457, 435);
            button2.TabIndex = 4;
            button2.TabStop = false;
            button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.BackColor = Color.FromArgb(47, 74, 160);
            button3.Font = new Font("Segoe UI Semibold", 15F, FontStyle.Bold);
            button3.ForeColor = SystemColors.Control;
            button3.Location = new Point(753, 239);
            button3.Name = "button3";
            button3.Size = new Size(243, 61);
            button3.TabIndex = 5;
            button3.Text = "Registrar asistencia";
            button3.UseVisualStyleBackColor = false;
            button3.Click += button3_Click;
            // 
            // label4
            // 
            label4.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label4.ForeColor = Color.DimGray;
            label4.Location = new Point(753, 145);
            label4.Name = "label4";
            label4.Size = new Size(228, 91);
            label4.TabIndex = 6;
            label4.Text = "Coloque su dedo sobre el lector para validar\r\nsu ingreso a la institución";
            label4.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Interval = 1000;
            timer1.Tick += timer1_Tick;
            // 
            // label2
            // 
            label2.FlatStyle = FlatStyle.Flat;
            label2.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold);
            label2.ForeColor = Color.FromArgb(68, 74, 149);
            label2.Location = new Point(47, 133);
            label2.Name = "label2";
            label2.Size = new Size(255, 32);
            label2.TabIndex = 1;
            label2.Text = "Fecha y hora actual";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            label2.Click += label2_Click;
            // 
            // label3
            // 
            label3.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold);
            label3.ForeColor = Color.FromArgb(68, 74, 149);
            label3.Location = new Point(47, 181);
            label3.Name = "label3";
            label3.Size = new Size(255, 32);
            label3.TabIndex = 2;
            label3.Text = "{Fecha y hora}";
            label3.TextAlign = ContentAlignment.MiddleCenter;
            label3.Click += label3_Click;
            // 
            // labelEstado
            // 
            labelEstado.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold);
            labelEstado.ForeColor = Color.FromArgb(68, 74, 149);
            labelEstado.Location = new Point(366, 408);
            labelEstado.Name = "labelEstado";
            labelEstado.Size = new Size(255, 32);
            labelEstado.TabIndex = 7;
            labelEstado.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // asistenciaDocentes
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(251, 249, 248);
            ClientSize = new Size(1008, 729);
            Controls.Add(labelEstado);
            Controls.Add(label2);
            Controls.Add(label3);
            Controls.Add(label4);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(panel1);
            Name = "asistenciaDocentes";
            StartPosition = FormStartPosition.CenterScreen;
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Label label1;
        private Button button1;
        private Button button2;
        private Button button3;
        private Label label4;
        private System.Windows.Forms.Timer timer1;
        private Label label2;
        private Label label3;
        private Label labelEstado;
    }
}