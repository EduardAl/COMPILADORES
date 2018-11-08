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
    public partial class Estructura : Form
    {
        string strCon;
        public Estructura(string con)
        {
            strCon = con;
            InitializeComponent();
        }

        List<TreeViewItem> treeViewList;
        private void BD_Load(object sender, EventArgs e)
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

                cmd.CommandText = "SELECT * FROM "+list+".INFORMATION_SCHEMA.TABLES;";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = SqlCon;
                reader = cmd.ExecuteReader();

                treeViewList.Add(new TreeViewItem(){ ParentID = 0, ID = id, Text = list}); 
                while (reader.Read())
                {
                    treeViewList.Add(new TreeViewItem() { ParentID = id, ID = id+10000, Text = reader.GetString(2) }); 
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
    }
}
