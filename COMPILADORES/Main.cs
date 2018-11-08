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
        String CCadena = "";
        SqlConnection conn;
        analizador.AnalisisSintactico analizer = new analizador.AnalisisSintactico();
        String conexion;

        public Main()
        {
            InitializeComponent();
        }
        public void ConsultarDatos(SqlCommand cmd)
        {
            SqlDataReader reader = cmd.ExecuteReader();
            Resultado resultado = new Resultado(conexion, txtSQL.Text);
            resultado.Show();
        }
        public void LlenarComboBox(){
            System.Data.SqlClient.SqlConnection SqlCon = new System.Data.SqlClient.SqlConnection(CCadena);
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
            //this.Size = new Size(803+213, 527);
            groupBox2.Visible = false;
            groupBox3.Visible = false;
            Ejecucion.Enabled = false;
            txtResultado.ReadOnly = true;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
        }
        private void Ejecición_Click(object sender, EventArgs e)
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
        private void btnGenerarConsulta_Click_1(object sender, EventArgs e)
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
                //this.Size = new Size(803 + 423, 527);
                groupBox2.Visible = true;
                txtOriginal.Clear();
                txtWhere.Clear();
                txtOrderBy.Clear();
                txtConsulta.Clear();
            }
        }



        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string[] part = CCadena.Split(';');
            conexion = part[0] + ";Initial Catalog=" + comboBox1.SelectedItem.ToString() + ";Integrated Security=Yes";
            Ejecucion.Enabled = true;
            DBCatalog.Enabled = true;
            btnGenerarConsulta.Enabled = true;
            //MessageBox.Show(part[0]+";Initial Catalog="+comboBox1.SelectedItem.ToString()+";"+part[1]);
        }

        private void btnGenerar_Click(object sender, EventArgs e)
        {
            if (txtWhere.Text != "")
            {
                if (txtOrderBy.Text != "")
                {
                    txtConsulta.Text = txtOriginal.Text + " WHERE " + txtWhere.Text + " ORDER BY " + txtOrderBy.Text;
                    txtSQL.Text = txtConsulta.Text;
                }
                else
                {
                    txtConsulta.Text = txtOriginal.Text + " WHERE " + txtWhere.Text;
                    txtSQL.Text = txtConsulta.Text;
                }
            }
            else
            {
                if (txtOrderBy.Text != "")
                {
                    txtConsulta.Text = txtOriginal.Text + " ORDER BY " + txtOrderBy.Text;
                    txtSQL.Text = txtConsulta.Text;
                }
                else
                {
                    txtConsulta.Text = txtOriginal.Text;
                    txtSQL.Text = txtConsulta.Text;
                }
            }
        }

        private void Cancelar_Click(object sender, EventArgs e)
        {
            this.Size = new Size(803, 527);
            groupBox2.Visible = false;
            txtConsulta.Text = "";
            txtOrderBy.Text = "";
            txtOriginal.Text = "";
            txtResultado.Text = "";
        }

        private void btnconn_Click(object sender, EventArgs e)
        {
            string campo = "";
            analizer.analisis(txtSQL.Text, ref campo);
            //creando conexion a sql
            conn = new SqlConnection();
            try
            {
                if (((txtUser.Text == "" || txtUser.Text == null) && (txtPassword.Text == "" || txtPassword.Text == null)) || (txtUser.Text == "" || txtUser.Text == null) || (txtPassword.Text == "" || txtPassword.Text == null))
                {
                    conn.ConnectionString = "Data Source=" + txtSorce.Text + ";Integrated Security=True";
                }
                else
                {
                    conn.ConnectionString = "Data Source=" + txtSorce.Text + ";User Id=" + txtUser.Text + ";Password=" + txtPassword.Text;
                }
                CCadena = conn.ConnectionString;
                try
                {
                    conn.Open();
                }
                catch
                {
                    MessageBox.Show(":/");
                }
                LlenarComboBox();
                DBCatalog.Enabled = true;
                conn.Close();

                MessageBox.Show("Prueba de conexión exitosa", "Informe", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception)
            {
                DBCatalog.Enabled = false;
                MessageBox.Show("Prueba de conexión fallida", "Informe", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void DBCatalog_Click_1(object sender, EventArgs e)
        {
            //            Estructura ventanaBD = new Estructura(CCadena);
            BD_Load(CCadena);
            groupBox3.Visible = true;
            //            ventanaBD.Show();
        }
        private void mostrarErrores(int op, ref ArrayList errores)
        {
            if (op == 1) // Si son errores lexicos
            {
                foreach (string s in errores)
                {
                    txtResultado.Text = "La palabra: '" + s + "' no pertenece al lenguaje del parserSQL. \r\n";
                }

            }
            else // Si son errores sintacticos
            {
                foreach (string s in errores)
                {
                    txtResultado.Text += s + "\r\n";
                }
            }
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            txtOriginal.Clear();
            txtWhere.Clear();
            txtOrderBy.Clear();
            txtConsulta.Clear();
        }

        List<TreeViewItem> treeViewList;
        private void BD_Load(string strCon)
        {

            System.Data.SqlClient.SqlConnection SqlCon = new System.Data.SqlClient.SqlConnection(strCon);
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
                listBD.Add(SqlDR.GetString(0));
            }
            SqlCon.Close();

            treeViewList = new List<TreeViewItem>();
            int pi = 0;
            int id = 1;
            foreach (String list in listBD)
            {
                SqlCon.Open();

                SqlCommand cmd = new SqlCommand();
                SqlDataReader reader;

                cmd.CommandText = "SELECT * FROM " + list + ".INFORMATION_SCHEMA.TABLES;";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = SqlCon;
                reader = cmd.ExecuteReader();

                treeViewList.Add(new TreeViewItem() { ParentID = 0, ID = id, Text = list });
                while (reader.Read())
                {
                    treeViewList.Add(new TreeViewItem() { ParentID = id, ID = id + 10000, Text = reader.GetString(2) });
                }
                id++;
                SqlCon.Close();
            }
            PopulateTreeView(0, null);

        }

        private void PopulateTreeView(int parentId, TreeNode parentNode)
        {
            var filteredItems = treeViewList.Where(item =>
                                        item.ParentID == parentId);

            TreeNode childNode;
            foreach (var i in filteredItems.ToList())
            {
                if (parentNode == null)
                    childNode = treeView1.Nodes.Add(i.Text);
                else
                    childNode = parentNode.Nodes.Add(i.Text);
                PopulateTreeView(i.ID, childNode);
            }
        }

        private void btnCerrarBD_Click(object sender, EventArgs e)
        {
            groupBox3.Visible = false;
        }
    }
}
