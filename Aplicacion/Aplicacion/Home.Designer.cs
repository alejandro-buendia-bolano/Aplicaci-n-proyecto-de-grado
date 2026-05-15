namespace Aplicacion
{
    partial class Home
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
            label1 = new Label();
            log_out = new Button();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            button1 = new Button();
            label6 = new Label();
            label7 = new Label();
            label8 = new Label();
            timer1 = new System.Windows.Forms.Timer(components);
            button3 = new Button();
            button4 = new Button();
            button5 = new Button();
            label9 = new Label();
            label10 = new Label();
            button6 = new Button();
            label11 = new Label();
            label12 = new Label();
            button7 = new Button();
            button9 = new Button();
            label13 = new Label();
            label14 = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(815, 25);
            label1.Name = "label1";
            label1.Size = new Size(128, 21);
            label1.TabIndex = 0;
            label1.Text = "CERRAR SESIÓN";
            // 
            // log_out
            // 
            log_out.BackgroundImage = Properties.Resources.log_out;
            log_out.BackgroundImageLayout = ImageLayout.Stretch;
            log_out.FlatAppearance.BorderColor = Color.FromArgb(251, 249, 248);
            log_out.FlatAppearance.MouseDownBackColor = Color.FromArgb(251, 249, 248);
            log_out.FlatAppearance.MouseOverBackColor = Color.FromArgb(251, 249, 248);
            log_out.FlatStyle = FlatStyle.Flat;
            log_out.ForeColor = Color.FromArgb(251, 249, 248);
            log_out.Location = new Point(938, 3);
            log_out.Name = "log_out";
            log_out.Size = new Size(66, 68);
            log_out.TabIndex = 1;
            log_out.UseVisualStyleBackColor = true;
            log_out.Click += log_out_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.ForeColor = Color.FromArgb(44, 53, 155);
            label2.Location = new Point(27, 32);
            label2.Name = "label2";
            label2.Size = new Size(292, 21);
            label2.TabIndex = 2;
            label2.Text = "CONTROL DE ACCESO INSTITUCIONAL";
            label2.Click += label2_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 21.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.ForeColor = Color.FromArgb(17, 49, 136);
            label3.Location = new Point(27, 74);
            label3.Name = "label3";
            label3.Size = new Size(0, 40);
            label3.TabIndex = 3;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 21.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label4.ForeColor = Color.FromArgb(17, 49, 136);
            label4.Location = new Point(27, 128);
            label4.Name = "label4";
            label4.Size = new Size(282, 40);
            label4.TabIndex = 4;
            label4.Text = "¿Qué deseas hacer?";
            // 
            // label5
            // 
            label5.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label5.ForeColor = Color.FromArgb(10, 86, 86, 100);
            label5.Location = new Point(27, 194);
            label5.Name = "label5";
            label5.Size = new Size(471, 74);
            label5.TabIndex = 5;
            label5.Text = "Optimización de la gestión académica con control de asistencia biométrico seguro para la comunidad educativa.";
            // 
            // button1
            // 
            button1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            button1.BackColor = Color.FromArgb(234, 232, 231);
            button1.FlatAppearance.BorderColor = Color.FromArgb(17, 49, 136);
            button1.FlatAppearance.MouseDownBackColor = Color.FromArgb(234, 232, 231);
            button1.FlatAppearance.MouseOverBackColor = Color.FromArgb(234, 232, 231);
            button1.FlatStyle = FlatStyle.Flat;
            button1.Location = new Point(743, 74);
            button1.Name = "button1";
            button1.Size = new Size(253, 131);
            button1.TabIndex = 6;
            button1.TabStop = false;
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // label6
            // 
            label6.BackColor = Color.FromArgb(234, 232, 231);
            label6.FlatStyle = FlatStyle.Flat;
            label6.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label6.Location = new Point(763, 94);
            label6.Name = "label6";
            label6.Size = new Size(229, 23);
            label6.TabIndex = 8;
            label6.Text = "HORA Y FECHA ACTUAL";
            // 
            // label7
            // 
            label7.BackColor = Color.FromArgb(234, 232, 231);
            label7.FlatStyle = FlatStyle.Flat;
            label7.Font = new Font("Segoe UI Black", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label7.ForeColor = Color.FromArgb(44, 74, 149);
            label7.Location = new Point(767, 128);
            label7.Name = "label7";
            label7.Size = new Size(212, 30);
            label7.TabIndex = 9;
            label7.Text = "{HORA}";
            label7.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label8
            // 
            label8.BackColor = Color.FromArgb(234, 232, 231);
            label8.FlatStyle = FlatStyle.Flat;
            label8.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label8.ForeColor = Color.FromArgb(44, 74, 149);
            label8.Location = new Point(767, 163);
            label8.Name = "label8";
            label8.Size = new Size(212, 23);
            label8.TabIndex = 10;
            label8.Text = "{FECHA ACTUAL}";
            label8.TextAlign = ContentAlignment.MiddleRight;
            label8.Click += label8_Click;
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Interval = 1000;
            timer1.Tick += timer1_Tick;
            // 
            // button3
            // 
            button3.BackColor = Color.FromArgb(47, 74, 160);
            button3.BackgroundImageLayout = ImageLayout.Stretch;
            button3.FlatAppearance.BorderColor = Color.FromArgb(47, 74, 160);
            button3.FlatAppearance.MouseDownBackColor = Color.FromArgb(47, 74, 160);
            button3.FlatAppearance.MouseOverBackColor = Color.FromArgb(47, 74, 160);
            button3.FlatStyle = FlatStyle.Flat;
            button3.Location = new Point(27, 271);
            button3.Name = "button3";
            button3.Size = new Size(510, 210);
            button3.TabIndex = 11;
            button3.TabStop = false;
            button3.UseVisualStyleBackColor = false;
            // 
            // button4
            // 
            button4.BackColor = Color.LightGray;
            button4.BackgroundImageLayout = ImageLayout.Stretch;
            button4.FlatAppearance.BorderColor = Color.LightGray;
            button4.FlatAppearance.MouseDownBackColor = Color.LightGray;
            button4.FlatAppearance.MouseOverBackColor = Color.LightGray;
            button4.FlatStyle = FlatStyle.Flat;
            button4.Location = new Point(27, 495);
            button4.Name = "button4";
            button4.Size = new Size(454, 183);
            button4.TabIndex = 12;
            button4.TabStop = false;
            button4.UseVisualStyleBackColor = false;
            // 
            // button5
            // 
            button5.BackColor = Color.LightGray;
            button5.FlatAppearance.BorderColor = Color.LightGray;
            button5.FlatAppearance.MouseDownBackColor = Color.LightGray;
            button5.FlatAppearance.MouseOverBackColor = Color.LightGray;
            button5.FlatStyle = FlatStyle.Flat;
            button5.Location = new Point(554, 271);
            button5.Name = "button5";
            button5.Size = new Size(425, 210);
            button5.TabIndex = 13;
            button5.UseVisualStyleBackColor = false;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.BackColor = Color.FromArgb(47, 74, 160);
            label9.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label9.ForeColor = SystemColors.ButtonHighlight;
            label9.Location = new Point(46, 296);
            label9.Name = "label9";
            label9.Size = new Size(292, 30);
            label9.TabIndex = 14;
            label9.Text = "Tomar asistencia estudiantes";
            // 
            // label10
            // 
            label10.BackColor = Color.FromArgb(47, 74, 160);
            label10.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label10.ForeColor = SystemColors.ButtonFace;
            label10.Location = new Point(46, 357);
            label10.Name = "label10";
            label10.Size = new Size(477, 52);
            label10.TabIndex = 15;
            label10.Text = "Agiliza el pase de lista y elimina errores manuales con nuestro sistema de validación por huella digital.";
            // 
            // button6
            // 
            button6.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button6.ForeColor = Color.FromArgb(44, 53, 155);
            button6.Location = new Point(46, 423);
            button6.Name = "button6";
            button6.Size = new Size(147, 39);
            button6.TabIndex = 16;
            button6.Text = "Comenzar registro";
            button6.UseVisualStyleBackColor = true;
            button6.Click += button6_Click;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.BackColor = Color.LightGray;
            label11.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label11.ForeColor = Color.FromArgb(44, 53, 155);
            label11.Location = new Point(582, 296);
            label11.Name = "label11";
            label11.Size = new Size(157, 25);
            label11.TabIndex = 17;
            label11.Text = "Generar reporte";
            // 
            // label12
            // 
            label12.BackColor = Color.LightGray;
            label12.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label12.ForeColor = Color.Black;
            label12.Location = new Point(582, 334);
            label12.Name = "label12";
            label12.Size = new Size(384, 75);
            label12.TabIndex = 18;
            label12.Text = "Genera informes detallados sobre la puntualidad y asistencia basados en los registros biométricos del sistema.";
            // 
            // button7
            // 
            button7.BackColor = Color.FromArgb(44, 53, 155);
            button7.FlatAppearance.BorderColor = Color.FromArgb(44, 53, 155);
            button7.FlatAppearance.MouseOverBackColor = Color.FromArgb(98, 107, 213);
            button7.FlatStyle = FlatStyle.Flat;
            button7.Font = new Font("Segoe UI", 9.75F);
            button7.ForeColor = Color.White;
            button7.Location = new Point(582, 423);
            button7.Name = "button7";
            button7.Size = new Size(147, 39);
            button7.TabIndex = 19;
            button7.Text = "Crear reporte";
            button7.UseVisualStyleBackColor = false;
            button7.Click += button7_Click;
            // 
            // button9
            // 
            button9.BackColor = Color.FromArgb(44, 53, 155);
            button9.FlatAppearance.BorderColor = Color.FromArgb(44, 53, 155);
            button9.FlatAppearance.MouseOverBackColor = Color.FromArgb(98, 107, 213);
            button9.FlatStyle = FlatStyle.Flat;
            button9.Font = new Font("Segoe UI", 9.75F);
            button9.ForeColor = Color.White;
            button9.Location = new Point(46, 608);
            button9.Name = "button9";
            button9.Size = new Size(147, 39);
            button9.TabIndex = 23;
            button9.Text = "Comenzar registro";
            button9.UseVisualStyleBackColor = false;
            button9.Click += button9_Click;
            // 
            // label13
            // 
            label13.BackColor = Color.LightGray;
            label13.Font = new Font("Segoe UI", 11F);
            label13.ForeColor = Color.Black;
            label13.Location = new Point(46, 537);
            label13.Name = "label13";
            label13.Size = new Size(435, 62);
            label13.TabIndex = 22;
            label13.Text = "Portal seguro dedicado para que los miembros de la institución verifiquen su presencia institucional y el cumplimiento de horarios.";
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.BackColor = Color.LightGray;
            label14.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label14.ForeColor = Color.FromArgb(44, 53, 155);
            label14.Location = new Point(46, 506);
            label14.Name = "label14";
            label14.Size = new Size(223, 21);
            label14.TabIndex = 21;
            label14.Text = "Registrar asistencia docente";
            // 
            // Home
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(251, 249, 248);
            ClientSize = new Size(1008, 729);
            Controls.Add(button9);
            Controls.Add(label13);
            Controls.Add(label14);
            Controls.Add(button7);
            Controls.Add(label12);
            Controls.Add(label11);
            Controls.Add(button6);
            Controls.Add(label10);
            Controls.Add(label9);
            Controls.Add(button5);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(label8);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(button1);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(log_out);
            Controls.Add(label1);
            Name = "Home";
            StartPosition = FormStartPosition.CenterScreen;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Button log_out;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Button button1;
        private Label label6;
        private Label label7;
        private Label label8;
        private System.Windows.Forms.Timer timer1;
        private Button button3;
        private Button button4;
        private Button button5;
        private Label label9;
        private Label label10;
        private Button button6;
        private Label label11;
        private Label label12;
        private Button button7;
        private Button button9;
        private Label label13;
        private Label label14;
    }
}