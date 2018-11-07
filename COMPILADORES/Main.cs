using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace COMPILADORES
{
    public partial class Main : Form
    {
        string instruccionCorrecta = "";
        int tipo = 0;
        public Main()
        {
            InitializeComponent();
        }

        SqlConnection conn;

        analizador.AnalisisSintactico ana = new analizador.AnalisisSintactico();

        

        private void mostrarErrores(int op, ref ArrayList errores)
        {
            if (op == 1) // Si son errores lexicos
            {
                foreach (string s in errores)
                    txtResultado.Text = "La palabra: '" + s + "' no pertenece al lenguaje del parserSQL. \r\n";

            }
            else // Si son errores sintacticos
            {
                foreach (string s in errores)
                txtResultado.Text += s + "\r\n";
            }
        }

        public void ConsultarDatos(SqlCommand cmd)
        {
            SqlDataReader reader = cmd.ExecuteReader();
            Resultado resultado = new Resultado(conexion, txtSQL.Text);
            resultado.Show();
        }

        public void llenarComboBox(){
            System.Data.SqlClient.SqlConnection SqlCon = new System.Data.SqlClient.SqlConnection(txtStringCon.Text);
            SqlCon.Open();

            System.Data.SqlClient.SqlCommand SqlCom = new System.Data.SqlClient.SqlCommand();
            SqlCom.Connection = SqlCon;
            SqlCom.CommandType = CommandType.StoredProcedure;
            SqlCom.CommandText = "sp_databases";

            System.Data.SqlClient.SqlDataReader SqlDR;
            SqlDR = SqlCom.ExecuteReader();

            List<String> listBD = new List<String>();

            comboBox1.Items.Clear();
            while (SqlDR.Read())
            {
                comboBox1.Items.Add(SqlDR.GetValue(0));
            }
        }

        private void Main_Load(object sender, EventArgs e)
        {
            button1.Enabled = false;
            txtResultado.ReadOnly = true;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            Consulta consulta = new Consulta();
            consulta.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //BD ventanaBD = new BD(txtStringCon.Text);
            
            panel_lateral.Width = 200;
            AbrirFormInPanel(new BD(txtStringCon.Text, this));

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string[] part = txtStringCon.Text.Split(';');
            conexion = part[0] + ";Initial Catalog=" + comboBox1.SelectedItem.ToString() + ";" + part[1];
            button1.Enabled = true;
            //MessageBox.Show(part[0]+";Initial Catalog="+comboBox1.SelectedItem.ToString()+";"+part[1]);
        }

        //RESIZE METODO PARA REDIMENCIONAR/CAMBIAR TAMAÑO A FORMULARIO EN TIEMPO DE EJECUCION ----------------------------------------------------------
        private int tolerance = 12;
        private const int WM_NCHITTEST = 132;
        private const int HTBOTTOMRIGHT = 17;
        private Rectangle sizeGripRectangle;

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_NCHITTEST:
                    base.WndProc(ref m);
                    var hitPoint = this.PointToClient(new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16));
                    if (sizeGripRectangle.Contains(hitPoint))
                        m.Result = new IntPtr(HTBOTTOMRIGHT);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }
        //----------------DIBUJAR RECTANGULO / EXCLUIR ESQUINA PANEL 
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            var region = new Region(new Rectangle(0, 0, this.ClientRectangle.Width, this.ClientRectangle.Height));

            sizeGripRectangle = new Rectangle(this.ClientRectangle.Width - tolerance, this.ClientRectangle.Height - tolerance, tolerance, tolerance);

            region.Exclude(sizeGripRectangle);
            this.panel_contenedor.Region = region;
            this.Invalidate();
        }
        //----------------COLOR Y GRIP DE RECTANGULO INFERIOR
        protected override void OnPaint(PaintEventArgs e)
        {
            SolidBrush blueBrush = new SolidBrush(Color.FromArgb(64, 64, 64));
            e.Graphics.FillRectangle(blueBrush, sizeGripRectangle);

            //base.OnPaint(e);
            //ControlPaint.DrawSizeGrip(e.Graphics, Color.Transparent , sizeGripRectangle);
        }



        //METODO PARA ARRASTRAR EL FORMULARIO---------------------------------------------------------------------
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        private void panelBarraTitulo_MouseMove(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }



        //METODO PARA ABRIR FORM DENTRO DE PANEL-----------------------------------------------------
        private void AbrirFormEnPanel<Forms>() where Forms : Form, new()
        {
            Form formulario;
            formulario = panel_contenedor.Controls.OfType<Forms>().FirstOrDefault();

            //si el formulario/instancia no existe, creamos nueva instancia y mostramos
            if (formulario == null)
            {
                formulario = new Forms();
                formulario.TopLevel = false;
                //formulario.FormBorderStyle = FormBorderStyle.None;
                //formulario.Dock = DockStyle.Fill;
                panel_contenedor.Controls.Add(formulario);
                panel_contenedor.Tag = formulario;
                formulario.Show();

                formulario.BringToFront();
                // formulario.FormClosed += new FormClosedEventHandler(CloseForms);               
            }
            else
            {

                //si la Formulario/instancia existe, lo traemos a frente
                formulario.BringToFront();

                //Si la instancia esta minimizada mostramos
                if (formulario.WindowState == FormWindowState.Minimized)
                {
                    formulario.WindowState = FormWindowState.Normal;
                }

            }
        }

        private void AbrirFormInPanel(object formHijo)
        {
            if (this.panel_lateral.Controls.Count > 0)
                this.panel_lateral.Controls.RemoveAt(0);
            Form fh = formHijo as Form;
            fh.TopLevel = false;
            fh.FormBorderStyle = FormBorderStyle.None;
            fh.Dock = DockStyle.Fill;
            this.panel_lateral.Controls.Add(fh);
            this.panel_lateral.Tag = fh;
            fh.Show();
        }

        private void CloseForms(object sender, FormClosedEventArgs e)
        {
            if (Application.OpenForms["Form1"] == null)
                button1.BackColor = Color.FromArgb(4, 41, 68);
            if (Application.OpenForms["Form2"] == null)
                button2.BackColor = Color.FromArgb(4, 41, 68);
            if (Application.OpenForms["Form3"] == null)
                button3.BackColor = Color.FromArgb(4, 41, 68);
        }

        private void btnconn_Click(object sender, EventArgs e)
        {
            string campo = "";
            ana.analisis(txtSQL.Text, ref campo);
            //creando conexion a sql
            conn = new SqlConnection();
            try
            {
                conn.ConnectionString = txtStringCon.Text;
                conn.Open();
                llenarComboBox();
                conn.Close();

                MessageBox.Show("Prueba de conexión exitosa", "Informe", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception)
            {
                MessageBox.Show("Prueba de conexión fallida", "Informe", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //String de conexion
        String conexion;
        private void button1_Click_1(object sender, EventArgs e)
        {
            txtResultado.Clear();

            Analisis analisis = new Analisis();
            ArrayList palabrasErroneas = new ArrayList();

            string[] words = analisis.analisisLexico(txtSQL.Text, ref palabrasErroneas);

            if (palabrasErroneas.Count > 0) // Comprobando si se detecto algun error lexico
            {
                mostrarErrores(1, ref palabrasErroneas);
            }
            else
            {
                instruccionCorrecta = analisis.analisiSintatico(ref words, ref palabrasErroneas, ref tipo);
                if (palabrasErroneas.Count > 0) // Comprobando si se detecto algun error sintactico
                {
                    mostrarErrores(2, ref palabrasErroneas);
                }
                else
                {
                    txtResultado.Text = "Se ejecuto correctamente la consulta";
                    //variable que convierte el texto del textbox 
                    string texto = txtSQL.Text;
                    string minus = texto.ToLower();
                    bool s;

                    s = minus.Contains("select");

                    conn.ConnectionString = conexion;
                    conn.Open();

                    //enviando el texto con la variable de la conexion
                    SqlCommand cmd = new SqlCommand(minus, conn);

                    //Ejecuta la linea de comando
                    try
                    {

                        if (s) ConsultarDatos(cmd);
                        else cmd.ExecuteNonQuery();
                    }

                    catch (Exception ex)
                    {
                        txtResultado.Text = ex.Message;
                    }
                    finally
                    {
                        conn.Close();
                    }

                }
            }


            /*
*/

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Application.Exit(); 
        }

        int LX, LY;

        private void iconMaximizar_Click(object sender, EventArgs e)
        {
            //this.WindowState = FormWindowState.Maximized;
            LX = this.Location.X;
            LY = this.Location.Y;
            this.Size = Screen.PrimaryScreen.WorkingArea.Size;
            this.Location = Screen.PrimaryScreen.WorkingArea.Location;
            iconMaximizar.Visible = false;
            iconRestaurar.Visible = true;
        }

        private void iconRestaurar_Click(object sender, EventArgs e)
        {
            //this.WindowState = FormWindowState.Normal;
            this.Size = new Size(900, 556);
            this.Location = new Point(LX, LY);
            iconMaximizar.Visible = true;
            iconRestaurar.Visible = false;
        }

        private void iconMinimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
