namespace Aplicacion
{
    partial class Login
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Login));
            button1 = new Button();
            label1 = new Label();
            label2 = new Label();
            button2 = new Button();
            button3 = new Button();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            textBox2 = new TextBox();
            label6 = new Label();
            button4 = new Button();
            button5 = new Button();
            textBox1 = new TextBox();
            SuspendLayout();
            // 
            // button1
            // 
            button1.BackColor = Color.FromArgb(33, 63, 149);
            button1.BackgroundImageLayout = ImageLayout.None;
            button1.FlatAppearance.BorderColor = Color.FromArgb(33, 63, 149);
            button1.FlatAppearance.MouseDownBackColor = Color.FromArgb(33, 63, 149);
            button1.FlatAppearance.MouseOverBackColor = Color.FromArgb(33, 63, 149);
            button1.FlatStyle = FlatStyle.Flat;
            button1.Location = new Point(-3, 0);
            button1.Name = "button1";
            button1.Size = new Size(605, 729);
            button1.TabIndex = 0;
            button1.TabStop = false;
            button1.UseVisualStyleBackColor = false;
            // 
            // label1
            // 
            label1.BackColor = Color.FromArgb(33, 63, 149);
            label1.Font = new Font("Segoe UI", 32.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.White;
            label1.Location = new Point(12, 339);
            label1.Name = "label1";
            label1.Size = new Size(582, 148);
            label1.TabIndex = 1;
            label1.Text = "Sistema Biométrico de \r\nControl de Asistencia";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            label1.Click += label1_Click;
            // 
            // label2
            // 
            label2.BackColor = Color.FromArgb(33, 63, 149);
            label2.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.ForeColor = Color.DarkGray;
            label2.Location = new Point(12, 504);
            label2.Name = "label2";
            label2.Size = new Size(582, 104);
            label2.TabIndex = 2;
            label2.Text = "Gestión institucional avanzada con tecnologia de precision\r\ny seguridad biométrica\r\n\r\n";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // button2
            // 
            button2.BackColor = Color.FromArgb(53, 80, 158);
            button2.BackgroundImage = (Image)resources.GetObject("button2.BackgroundImage");
            button2.BackgroundImageLayout = ImageLayout.Zoom;
            button2.FlatAppearance.BorderColor = Color.FromArgb(53, 80, 158);
            button2.FlatAppearance.MouseDownBackColor = Color.FromArgb(53, 80, 158);
            button2.FlatAppearance.MouseOverBackColor = Color.FromArgb(53, 80, 158);
            button2.FlatStyle = FlatStyle.Flat;
            button2.ForeColor = Color.FromArgb(53, 80, 158);
            button2.Location = new Point(180, 139);
            button2.Name = "button2";
            button2.Size = new Size(190, 190);
            button2.TabIndex = 3;
            button2.TabStop = false;
            button2.UseVisualStyleBackColor = false;
            // 
            // button3
            // 
            button3.BackColor = Color.FromArgb(250, 248, 247);
            button3.FlatAppearance.BorderColor = Color.FromArgb(250, 248, 247);
            button3.FlatAppearance.MouseDownBackColor = Color.FromArgb(250, 248, 247);
            button3.FlatAppearance.MouseOverBackColor = Color.FromArgb(250, 248, 247);
            button3.FlatStyle = FlatStyle.Flat;
            button3.Location = new Point(621, 162);
            button3.Name = "button3";
            button3.Size = new Size(375, 431);
            button3.TabIndex = 4;
            button3.TabStop = false;
            button3.Text = "button3";
            button3.UseVisualStyleBackColor = false;
            button3.Click += button3_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = Color.FromArgb(250, 248, 247);
            label3.Font = new Font("Segoe UI", 22F);
            label3.ForeColor = Color.Black;
            label3.Location = new Point(649, 186);
            label3.Name = "label3";
            label3.Size = new Size(188, 41);
            label3.TabIndex = 5;
            label3.Text = "Iniciar sesión";
            label3.Click += label3_Click;
            // 
            // label4
            // 
            label4.BackColor = Color.FromArgb(250, 248, 247);
            label4.Font = new Font("Segoe UI", 12F);
            label4.ForeColor = Color.Black;
            label4.Location = new Point(649, 227);
            label4.Name = "label4";
            label4.Size = new Size(347, 75);
            label4.TabIndex = 6;
            label4.Text = "Ingrese sus credenciales para acceder al panel de control\r\n";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.BackColor = Color.FromArgb(250, 248, 247);
            label5.Font = new Font("Segoe UI", 12F);
            label5.ForeColor = Color.Black;
            label5.Location = new Point(649, 308);
            label5.Name = "label5";
            label5.Size = new Size(77, 21);
            label5.TabIndex = 7;
            label5.Text = "USUARIO";
            // 
            // textBox2
            // 
            textBox2.BackColor = Color.FromArgb(234, 232, 231);
            textBox2.Cursor = Cursors.IBeam;
            textBox2.Font = new Font("Segoe UI", 10F);
            textBox2.Location = new Point(652, 436);
            textBox2.Margin = new Padding(10, 3, 3, 3);
            textBox2.Name = "textBox2";
            textBox2.PasswordChar = '*';
            textBox2.Size = new Size(307, 25);
            textBox2.TabIndex = 10;
            textBox2.TextChanged += textBox2_TextChanged;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.BackColor = Color.FromArgb(250, 248, 247);
            label6.Font = new Font("Segoe UI", 12F);
            label6.ForeColor = Color.Black;
            label6.Location = new Point(649, 407);
            label6.Name = "label6";
            label6.Size = new Size(111, 21);
            label6.TabIndex = 9;
            label6.Text = "CONTRASEÑA";
            // 
            // button4
            // 
            button4.BackColor = Color.FromArgb(75, 111, 219);
            button4.FlatAppearance.BorderColor = Color.FromArgb(75, 111, 219);
            button4.FlatAppearance.MouseDownBackColor = Color.FromArgb(75, 111, 219);
            button4.FlatAppearance.MouseOverBackColor = Color.FromArgb(33, 63, 149);
            button4.FlatStyle = FlatStyle.Flat;
            button4.Font = new Font("Segoe UI", 12F);
            button4.ForeColor = Color.Black;
            button4.Location = new Point(745, 481);
            button4.Name = "button4";
            button4.Size = new Size(134, 38);
            button4.TabIndex = 11;
            button4.Text = "Iniciar sesión";
            button4.UseVisualStyleBackColor = false;
            button4.Click += button4_Click_1;
            // 
            // button5
            // 
            button5.Font = new Font("Segoe UI", 12F);
            button5.ForeColor = Color.Black;
            button5.Location = new Point(745, 535);
            button5.Name = "button5";
            button5.Size = new Size(134, 38);
            button5.TabIndex = 12;
            button5.Text = "Cerrar";
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // textBox1
            // 
            textBox1.BackColor = Color.FromArgb(234, 232, 231);
            textBox1.Cursor = Cursors.IBeam;
            textBox1.Font = new Font("Segoe UI", 10F);
            textBox1.ForeColor = SystemColors.ActiveCaptionText;
            textBox1.Location = new Point(652, 339);
            textBox1.Margin = new Padding(10, 3, 3, 3);
            textBox1.Name = "textBox1";
            textBox1.PlaceholderText = " Ingrese su usuario";
            textBox1.Size = new Size(307, 25);
            textBox1.TabIndex = 15;
            // 
            // Login
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1008, 729);
            Controls.Add(textBox1);
            Controls.Add(button5);
            Controls.Add(button4);
            Controls.Add(textBox2);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(button3);
            Controls.Add(label1);
            Controls.Add(button2);
            Controls.Add(label2);
            Controls.Add(button1);
            ForeColor = Color.FromArgb(250, 248, 247);
            Name = "Login";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private Label label1;
        private Label label2;
        private Button button2;
        private Button button3;
        private Label label3;
        private Label label4;
        private Label label5;
        private TextBox textBox2;
        private Label label6;
        private Button button4;
        private Button button5;
        private TextBox textBox1;
    }
}
