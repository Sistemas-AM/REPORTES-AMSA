﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;
using System.Data.SqlClient;

namespace Cotizacion2022
{
    public partial class CargaCotizacion : MaterialForm
    {

        SqlConnection sqlConnection1 = new SqlConnection(Principal.Variablescompartidas.ReportesAmsa);
        SqlCommand cmd = new SqlCommand();
        SqlDataReader reader;
        DataTable dt = new DataTable();
        int PermiteCerrar = 0;

        public static string foliocot { get; set; }
        public static string ClienteCodigo { get; set; }
        public static string Local { get; set; }
        public static string observaciones { get; set; }

        public static string date1 { get; set; }
        public static string FolioSolo { get; set; }
        public static string TipoPasa { get; set; }


        public CargaCotizacion()
        {
            InitializeComponent();
            MaterialSkinManager materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;

            // Configure color schema
            materialSkinManager.ColorScheme = new ColorScheme(
                Primary.Blue400, Primary.Blue500,
                Primary.Blue500, Accent.LightBlue200,
                TextShade.WHITE
            );
            button1.BackColor = ColorTranslator.FromHtml("#76CA62");
            button2.BackColor = ColorTranslator.FromHtml("#D66F6F");
            dataGridView1.BackgroundColor = ColorTranslator.FromHtml("#CBD0D3");
            cargargrid("Cotizacion");

        }


        private void cargargrid(string tipo)
        {
            //SqlCommand cmd = new SqlCommand(@"
            //with cotiza as (
            //Select FolioCotizacion, FolioCot, Fecha, esLocal, codCliente, observaciones, case when esLocal = 0 then CRAZONSOCIAL
            // when esLocal = 1 then nombre end as nombrecliente from Cotizaciones left join adAMSACONTPAQi.dbo.admClientes
            // on CodCliente = CCODIGOCLIENTE
            // left join ctelocal on CodCliente = CodigoCliente
            // where sucursal = '"+Form1.sucursal+ @"' ),

            //Montos as(
            //select cotizacionesdetalles.foliocotizacion
            //,sum(pro.cprecio1 * cantidad) as ImporteTotal from cotizacionesDetalles 
            //inner join adAMSACONTPAQi.dbo.admproductos as pro
            //on cotizacionesdetalles.codigoproducto = pro.ccodigoproducto
            //group by foliocotizacion)

            //select cotiza.*, cast(round(montos.importetotal,2) as numeric(36,2)) as importeTotal from cotiza left join montos 
            //on cotiza.foliocot = montos.foliocotizacion
            //order by cotiza.fecha desc, importetotal desc", sqlConnection1);


            SqlCommand cmd = new SqlCommand(@"
            
            select FolioCotizacion, FolioCot, Fecha, esLocal, codCliente, observaciones,
            Nombre_Cliente as nombrecliente, tipo, ImporteTotal from cotizaciones where sucursal = '" + Form1.sucursal + @"' and tipo = '" + tipo + @"' and status = 'A' order by id desc, importetotal desc", sqlConnection1);

            SqlDataAdapter da = new SqlDataAdapter(cmd);

            da.Fill(dt);
            dataGridView1.DataSource = dt;
            sqlConnection1.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                foliocot = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["FolioBus"].Value.ToString();
                ClienteCodigo = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["codCliente"].Value.ToString();
                Local = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["esLocal"].Value.ToString();
                observaciones = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["obs"].Value.ToString();
                FolioSolo = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["folio"].Value.ToString();
                date1 = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Fecha"].Value.ToString();
                TipoPasa = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Tipo"].Value.ToString();
                PermiteCerrar = 1;
                this.Close();
            }
            catch (NullReferenceException)
            {

                MessageBox.Show("Selecciona un folio", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foliocot = "";
            ClienteCodigo = "";
            Local = "";
            PermiteCerrar = 1;
            this.Close();
        }

        private void Busqueda_TextChanged(object sender, EventArgs e)
        {
            dt.DefaultView.RowFilter = $"convert(folioCotizacion, System.String) LIKE '%{Busqueda.Text}%' or nombrecliente LIKE '%{Busqueda.Text}%'";
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void dataGridView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                foliocot = dataGridView1.Rows[dataGridView1.CurrentRow.Index-1].Cells["FolioBus"].Value.ToString();
                ClienteCodigo = dataGridView1.Rows[dataGridView1.CurrentRow.Index - 1].Cells["codCliente"].Value.ToString();
                Local = dataGridView1.Rows[dataGridView1.CurrentRow.Index - 1].Cells["esLocal"].Value.ToString();
                observaciones = dataGridView1.Rows[dataGridView1.CurrentRow.Index - 1].Cells["obs"].Value.ToString();
                date1 = dataGridView1.Rows[dataGridView1.CurrentRow.Index - 1].Cells["Fecha"].Value.ToString();
                FolioSolo = dataGridView1.Rows[dataGridView1.CurrentRow.Index - 1].Cells["folio"].Value.ToString();
                TipoPasa = dataGridView1.Rows[dataGridView1.CurrentRow.Index - 1].Cells["Tipo"].Value.ToString();
                PermiteCerrar = 1;
                this.Close();
            }
        }

        private void Busqueda_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                dataGridView1.Select();
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            foliocot = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["FolioBus"].Value.ToString();
            ClienteCodigo = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["codCliente"].Value.ToString();
            Local = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["esLocal"].Value.ToString();
            observaciones = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["obs"].Value.ToString();
            date1 = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Fecha"].Value.ToString();
            FolioSolo = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["folio"].Value.ToString();
            TipoPasa = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Tipo"].Value.ToString();
            PermiteCerrar = 1;
            this.Close();
        }

        private void CargaCotizacion_Load(object sender, EventArgs e)
        {

        }

        private void materialCheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (materialCheckBox1.Checked)
            {
                dt.Rows.Clear();
                cargargrid("Venta");
            }
            else
            {
                dt.Rows.Clear();
                cargargrid("Cotizacion");
            }
        }

        private void CargaCotizacion_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (PermiteCerrar == 0)
            {
                e.Cancel = true;
            }
        }
    }
}
