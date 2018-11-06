using System;
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
    public partial class Resultado : Form
    {
        string cadenaConexión, cadenaSQL;
        public Resultado(string con, string sql)
        {
            cadenaConexión = con;
            cadenaSQL = sql;
            InitializeComponent();
        }

        private void Resultado_Load(object sender, EventArgs e)
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter(cadenaSQL, cadenaConexión);
            SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);
            
            
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(cadenaConexión);
            this.Text = builder.InitialCatalog;

            // Populate a new data table and bind it to the BindingSource.
            DataTable table = new DataTable();
            table.Locale = System.Globalization.CultureInfo.InvariantCulture;
            dataAdapter.Fill(table);

            // Resize the DataGridView columns to fit the newly loaded content.
            dbGridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader);
            // you can make it grid readonly.
            dbGridView.ReadOnly = true;

            // finally bind the data to the grid
            dbGridView.DataSource = table;
        }
    }
}
