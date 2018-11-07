using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
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

        //String de conexion
        String conexion;
        private void button1_Click(object sender, EventArgs e)
        {
            if (txtSQL.Text == "")
            {
                MessageBox.Show("Ingrese una consulta", "Informe", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
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
            }

        }

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

        private void button2_Click(object sender, EventArgs e)
        {
            string campo  ="";
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
            if (comboBox1.Items.Count <= 0)
            {
                MessageBox.Show("Necesita una conexion", "Informe", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Seleccione una base de datos", "Informe", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                Consulta consulta = new Consulta();
                consulta.Show();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            BD ventanaBD = new BD(txtStringCon.Text);
            ventanaBD.Show();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string[] part = txtStringCon.Text.Split(';');
            conexion = part[0] + ";Initial Catalog=" + comboBox1.SelectedItem.ToString() + ";" + part[1];
            button1.Enabled = true;
            button3.Enabled = true;
            button2.Enabled = true;
            //MessageBox.Show(part[0]+";Initial Catalog="+comboBox1.SelectedItem.ToString()+";"+part[1]);
        }
    }
}
