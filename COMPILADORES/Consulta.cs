using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace COMPILADORES
{
    public partial class Consulta : Form
    {
        public Consulta()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (txtWhere.Text != "")
            {
                if (txtOrderBy.Text != "")
                {
                    txtConsulta.Text = txtOriginal.Text + " WHERE " + txtWhere.Text + " ORDER BY " + txtOrderBy.Text;
                }
                else
                {
                    txtConsulta.Text = txtOriginal.Text + " WHERE " + txtWhere.Text;
                }
            }
            else 
            {
                if (txtOrderBy.Text != "")
                {
                    txtConsulta.Text = txtOriginal.Text + " ORDER BY " + txtOrderBy.Text;
                }
                else
                {
                    txtConsulta.Text = txtOriginal.Text;
                }
            }

        }
    }
}
