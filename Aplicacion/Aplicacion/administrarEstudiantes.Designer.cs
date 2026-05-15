namespace Aplicacion
{
    partial class administrarEstudiantes
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(administrarEstudiantes));
            panel1 = new Panel();
            label1 = new Label();
            button1 = new Button();
            dataGridView1 = new DataGridView();
            button2 = new Button();
            button3 = new Button();
            label2 = new Label();
            label3 = new Label();
            primerNombre = new TextBox();
            segundoNombre = new TextBox();
            primerApellido = new TextBox();
            segundoApellido = new TextBox();
            idEstudiante = new TextBox();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            label7 = new Label();
            estado = new Label();
            registraHuella = new Button();
            button5 = new Button();
            agregar = new Button();
            eliminar = new Button();
            actualizar = new Button();
            verRegistros = new Button();
            grados = new CheckedListBox();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(47, 74, 160);
            panel1.Controls.Add(label1);
            panel1.Location = new Point(-5, -1);
            panel1.Name = "panel1";
            panel1.Size = new Size(1012, 50);
            panel1.TabIndex = 0;
            // 
            // label1
            // 
            label1.Dock = DockStyle.Fill;
            label1.Font = new Font("Segoe UI Semibold", 15.75F, FontStyle.Bold);
            label1.ForeColor = Color.White;
            label1.Location = new Point(0, 0);
            label1.Name = "label1";
            label1.Size = new Size(1012, 50);
            label1.TabIndex = 2;
            label1.Text = "ADMINISTRAR ESTUDIANTES";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // button1
            // 
            button1.BackColor = Color.FromArgb(47, 74, 160);
            button1.BackgroundImage = (Image)resources.GetObject("button1.BackgroundImage");
            button1.BackgroundImageLayout = ImageLayout.Stretch;
            button1.FlatAppearance.BorderColor = Color.FromArgb(47, 74, 160);
            button1.FlatAppearance.MouseDownBackColor = Color.FromArgb(47, 74, 160);
            button1.FlatAppearance.MouseOverBackColor = Color.FromArgb(47, 74, 160);
            button1.FlatStyle = FlatStyle.Flat;
            button1.Location = new Point(9, 3);
            button1.Name = "button1";
            button1.Size = new Size(66, 44);
            button1.TabIndex = 1;
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(12, 494);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.Size = new Size(972, 223);
            dataGridView1.TabIndex = 2;
            // 
            // button2
            // 
            button2.BackColor = Color.White;
            button2.FlatAppearance.BorderColor = Color.LightGray;
            button2.FlatAppearance.MouseDownBackColor = Color.White;
            button2.FlatAppearance.MouseOverBackColor = Color.White;
            button2.FlatStyle = FlatStyle.Flat;
            button2.Location = new Point(12, 55);
            button2.Name = "button2";
            button2.Size = new Size(552, 310);
            button2.TabIndex = 3;
            button2.UseVisualStyleBackColor = false;
            // 
            // button3
            // 
            button3.BackColor = Color.White;
            button3.FlatAppearance.BorderColor = Color.LightGray;
            button3.FlatAppearance.MouseDownBackColor = Color.White;
            button3.FlatAppearance.MouseOverBackColor = Color.White;
            button3.FlatStyle = FlatStyle.Flat;
            button3.Location = new Point(653, 55);
            button3.Name = "button3";
            button3.Size = new Size(310, 310);
            button3.TabIndex = 4;
            button3.UseVisualStyleBackColor = false;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.White;
            label2.FlatStyle = FlatStyle.Flat;
            label2.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.ForeColor = Color.DimGray;
            label2.Location = new Point(40, 73);
            label2.Name = "label2";
            label2.Size = new Size(114, 17);
            label2.TabIndex = 5;
            label2.Text = "PRIMER NOMBRE";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = Color.White;
            label3.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.ForeColor = Color.DimGray;
            label3.Location = new Point(332, 73);
            label3.Name = "label3";
            label3.Size = new Size(128, 17);
            label3.TabIndex = 6;
            label3.Text = "SEGUNDO NOMBRE";
            // 
            // primerNombre
            // 
            primerNombre.BackColor = Color.FromArgb(248, 250, 252);
            primerNombre.Font = new Font("Segoe UI", 10F);
            primerNombre.ForeColor = Color.DimGray;
            primerNombre.Location = new Point(40, 105);
            primerNombre.Name = "primerNombre";
            primerNombre.Size = new Size(203, 25);
            primerNombre.TabIndex = 7;
            primerNombre.Text = "EJ. JUAN";
            // 
            // segundoNombre
            // 
            segundoNombre.BackColor = Color.FromArgb(248, 250, 252);
            segundoNombre.Font = new Font("Segoe UI", 10F);
            segundoNombre.ForeColor = Color.DimGray;
            segundoNombre.Location = new Point(332, 105);
            segundoNombre.Name = "segundoNombre";
            segundoNombre.Size = new Size(203, 25);
            segundoNombre.TabIndex = 8;
            segundoNombre.Text = "EJ. DAVID";
            // 
            // primerApellido
            // 
            primerApellido.BackColor = Color.FromArgb(248, 250, 252);
            primerApellido.Font = new Font("Segoe UI", 10F);
            primerApellido.ForeColor = Color.DimGray;
            primerApellido.Location = new Point(40, 200);
            primerApellido.Name = "primerApellido";
            primerApellido.Size = new Size(203, 25);
            primerApellido.TabIndex = 9;
            primerApellido.Text = "EJ. PEREZ";
            // 
            // segundoApellido
            // 
            segundoApellido.BackColor = Color.FromArgb(248, 250, 252);
            segundoApellido.Font = new Font("Segoe UI", 10F);
            segundoApellido.ForeColor = Color.DimGray;
            segundoApellido.Location = new Point(332, 200);
            segundoApellido.Name = "segundoApellido";
            segundoApellido.Size = new Size(203, 25);
            segundoApellido.TabIndex = 10;
            segundoApellido.Text = "EJ. RODRIGUEZ";
            // 
            // idEstudiante
            // 
            idEstudiante.BackColor = Color.FromArgb(248, 250, 252);
            idEstudiante.Font = new Font("Segoe UI", 10F);
            idEstudiante.ForeColor = Color.DimGray;
            idEstudiante.Location = new Point(40, 296);
            idEstudiante.Name = "idEstudiante";
            idEstudiante.Size = new Size(203, 25);
            idEstudiante.TabIndex = 11;
            idEstudiante.Text = "0-000-000-000";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.BackColor = Color.White;
            label4.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            label4.ForeColor = Color.DimGray;
            label4.Location = new Point(40, 169);
            label4.Name = "label4";
            label4.Size = new Size(118, 17);
            label4.TabIndex = 13;
            label4.Text = "PRIMER APELLIDO";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.BackColor = Color.White;
            label5.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            label5.ForeColor = Color.DimGray;
            label5.Location = new Point(332, 169);
            label5.Name = "label5";
            label5.Size = new Size(132, 17);
            label5.TabIndex = 14;
            label5.Text = "SEGUNDO APELLIDO";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.BackColor = Color.White;
            label6.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            label6.ForeColor = Color.DimGray;
            label6.Location = new Point(40, 265);
            label6.Name = "label6";
            label6.Size = new Size(189, 17);
            label6.TabIndex = 15;
            label6.Text = "NÚMERO DE IDENTIFICACION";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.BackColor = Color.White;
            label7.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            label7.ForeColor = Color.DimGray;
            label7.Location = new Point(332, 245);
            label7.Name = "label7";
            label7.Size = new Size(53, 17);
            label7.TabIndex = 16;
            label7.Text = "GRADO";
            // 
            // estado
            // 
            estado.BackColor = Color.White;
            estado.Font = new Font("Segoe UI", 10F);
            estado.ForeColor = Color.DimGray;
            estado.Location = new Point(729, 275);
            estado.Name = "estado";
            estado.Size = new Size(167, 30);
            estado.TabIndex = 17;
            estado.Text = "• ESTADO DEL SISTEMA";
            // 
            // registraHuella
            // 
            registraHuella.AutoSize = true;
            registraHuella.BackColor = Color.FromArgb(47, 74, 160);
            registraHuella.FlatAppearance.BorderColor = Color.Black;
            registraHuella.FlatAppearance.MouseDownBackColor = Color.FromArgb(44, 53, 155);
            registraHuella.FlatAppearance.MouseOverBackColor = Color.FromArgb(44, 53, 155);
            registraHuella.FlatStyle = FlatStyle.Flat;
            registraHuella.Font = new Font("Segoe UI", 10F);
            registraHuella.Location = new Point(725, 318);
            registraHuella.Name = "registraHuella";
            registraHuella.Size = new Size(171, 31);
            registraHuella.TabIndex = 18;
            registraHuella.Text = "REGISTRAR HUELLA";
            registraHuella.UseVisualStyleBackColor = false;
            registraHuella.Click += registraHuella_Click_1;
            // 
            // button5
            // 
            button5.BackColor = Color.White;
            button5.BackgroundImage = Properties.Resources.icon_huella;
            button5.BackgroundImageLayout = ImageLayout.Stretch;
            button5.FlatAppearance.BorderColor = Color.White;
            button5.FlatAppearance.MouseDownBackColor = Color.White;
            button5.FlatAppearance.MouseOverBackColor = Color.White;
            button5.FlatStyle = FlatStyle.Flat;
            button5.Location = new Point(690, 84);
            button5.Name = "button5";
            button5.Size = new Size(259, 189);
            button5.TabIndex = 19;
            button5.UseVisualStyleBackColor = false;
            // 
            // agregar
            // 
            agregar.AutoSize = true;
            agregar.BackColor = Color.FromArgb(76, 175, 80);
            agregar.FlatAppearance.BorderColor = Color.FromArgb(76, 175, 80);
            agregar.FlatAppearance.MouseDownBackColor = Color.FromArgb(95, 195, 99);
            agregar.FlatAppearance.MouseOverBackColor = Color.FromArgb(95, 195, 99);
            agregar.FlatStyle = FlatStyle.Flat;
            agregar.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            agregar.ForeColor = Color.White;
            agregar.Location = new Point(44, 401);
            agregar.Name = "agregar";
            agregar.Size = new Size(172, 50);
            agregar.TabIndex = 20;
            agregar.Text = "AGREGAR";
            agregar.UseVisualStyleBackColor = false;
            agregar.Click += agregar_Click_1;
            // 
            // eliminar
            // 
            eliminar.BackColor = Color.FromArgb(244, 67, 54);
            eliminar.FlatAppearance.BorderColor = Color.FromArgb(244, 67, 54);
            eliminar.FlatAppearance.MouseDownBackColor = Color.FromArgb(244, 83, 71);
            eliminar.FlatAppearance.MouseOverBackColor = Color.FromArgb(244, 83, 71);
            eliminar.FlatStyle = FlatStyle.Flat;
            eliminar.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            eliminar.ForeColor = Color.White;
            eliminar.Location = new Point(288, 401);
            eliminar.Name = "eliminar";
            eliminar.Size = new Size(172, 50);
            eliminar.TabIndex = 21;
            eliminar.Text = "ELIMINAR";
            eliminar.UseVisualStyleBackColor = false;
            eliminar.Click += eliminar_Click_1;
            // 
            // actualizar
            // 
            actualizar.BackColor = Color.FromArgb(252, 211, 77);
            actualizar.FlatAppearance.BorderColor = Color.FromArgb(252, 211, 77);
            actualizar.FlatAppearance.MouseDownBackColor = Color.FromArgb(248, 212, 96);
            actualizar.FlatAppearance.MouseOverBackColor = Color.FromArgb(248, 212, 96);
            actualizar.FlatStyle = FlatStyle.Flat;
            actualizar.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            actualizar.ForeColor = Color.FromArgb(47, 74, 160);
            actualizar.Location = new Point(532, 401);
            actualizar.Name = "actualizar";
            actualizar.Size = new Size(172, 50);
            actualizar.TabIndex = 22;
            actualizar.Text = "ACTUALIZAR";
            actualizar.UseVisualStyleBackColor = false;
            actualizar.Click += actualizar_Click_1;
            // 
            // verRegistros
            // 
            verRegistros.BackColor = Color.FromArgb(3, 169, 244);
            verRegistros.FlatAppearance.BorderColor = Color.FromArgb(3, 169, 244);
            verRegistros.FlatAppearance.MouseDownBackColor = Color.FromArgb(11, 159, 226);
            verRegistros.FlatAppearance.MouseOverBackColor = Color.FromArgb(11, 159, 226);
            verRegistros.FlatStyle = FlatStyle.Flat;
            verRegistros.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            verRegistros.ForeColor = Color.White;
            verRegistros.Location = new Point(776, 401);
            verRegistros.Name = "verRegistros";
            verRegistros.Size = new Size(172, 50);
            verRegistros.TabIndex = 23;
            verRegistros.Text = "VER REGISTROS";
            verRegistros.UseVisualStyleBackColor = false;
            verRegistros.Click += verRegistros_Click_1;
            // 
            // grados
            // 
            grados.FormattingEnabled = true;
            grados.Location = new Point(332, 267);
            grados.Name = "grados";
            grados.Size = new Size(222, 76);
            grados.TabIndex = 24;
            // 
            // administrarEstudiantes
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(251, 249, 248);
            ClientSize = new Size(1008, 729);
            Controls.Add(grados);
            Controls.Add(verRegistros);
            Controls.Add(actualizar);
            Controls.Add(eliminar);
            Controls.Add(agregar);
            Controls.Add(button5);
            Controls.Add(registraHuella);
            Controls.Add(estado);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(idEstudiante);
            Controls.Add(segundoApellido);
            Controls.Add(primerApellido);
            Controls.Add(segundoNombre);
            Controls.Add(primerNombre);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(dataGridView1);
            Controls.Add(button1);
            Controls.Add(panel1);
            Name = "administrarEstudiantes";
            StartPosition = FormStartPosition.CenterScreen;
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
        private Button button2;
        private Button button3;
        private Label label2;
        private Label label3;
        private TextBox primerNombre;
        private TextBox segundoNombre;
        private TextBox primerApellido;
        private TextBox segundoApellido;
        private TextBox idEstudiante;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label estado;
        private Button registraHuella;
        private Button button5;
        private Button agregar;
        private Button eliminar;
        private Button actualizar;
        private Button verRegistros;
        private CheckedListBox grados;
    }
}