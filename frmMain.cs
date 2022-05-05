using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CaptchaBypassTest
{
    public partial class frmMain : Form
    {
        frmBrowser wb = null;
        bool retried = false;

        public frmMain()
        {
            InitializeComponent();
        }

        private void btnBypass_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                retried = false;
                wb = new frmBrowser(txtSearch.Text); //Se envía parámetros de búsqueda.
                wb.FormClosed += Wb_FormClosed;
                wb.Show();
            }
            else
            {
                MessageBox.Show("Debe Ingresar un valor!", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSearch.Focus();
            }
        }

        private void Wb_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (wb.schoolDataList != null && wb.schoolDataList.Count > 0 && wb.schoolDataList.FirstOrDefault().Number > 0)
            {
                gcPeople.DataSource = wb.schoolDataList;
            }
            else
            {
                if (!retried)
                {
                    retried = true;
                    wb = new frmBrowser(txtSearch.Text); //Se envía parámetros de búsqueda y retorna modelo de personas.
                    wb.FormClosed += Wb_FormClosed;
                    wb.Show();
                }
                else
                {
                    gcPeople.DataSource = null;
                    MessageBox.Show("No se han encontrado datos, por favor cambie de parámetro o intente más tarde!", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
    }
}
