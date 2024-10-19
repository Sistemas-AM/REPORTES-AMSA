using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Cotizacion2022
{

    public partial class FrmLotesProducto : Form
    {
        // Variable para el código del producto seleccionado
        private string CodP = PantallaProductos.CidProd;

        public FrmLotesProducto()
        {
            InitializeComponent();
            InicializarDataGridView();
            CargarDatosLotes();
        }

        // Método para configurar el DataGridView
        private void InicializarDataGridView()
        {
            MessageBox.Show("El valor de CodP es:" + CodP);
            // Limpiar columnas existentes para evitar duplicados
            dataGridView1.Columns.Clear();

            // Crear y configurar las columnas del DataGridView
            DataGridViewTextBoxColumn codigoProductoColumn = new DataGridViewTextBoxColumn
            {
                HeaderText = "Código Producto",
                Name = "CCODIGOPRODUCTO",
                DataPropertyName = "CCODIGOPRODUCTO",
                ReadOnly = true
            };

            DataGridViewTextBoxColumn numeroLoteColumn = new DataGridViewTextBoxColumn
            {
                HeaderText = "Número de Lote",
                Name = "CNUMEROLOTE",
                DataPropertyName = "CNUMEROLOTE",  // Asegura que esta columna mapea con el campo correcto del DataTable
                ReadOnly = true
            };

            DataGridViewTextBoxColumn nombreProductoColumn = new DataGridViewTextBoxColumn
            {
                HeaderText = "Nombre Producto",
                Name = "CNOMBREPRODUCTO",
                DataPropertyName = "CNOMBREPRODUCTO",
                ReadOnly = true
            };

            DataGridViewTextBoxColumn nombreAlmacenColumn = new DataGridViewTextBoxColumn
            {
                HeaderText = "Nombre Almacén",
                Name = "CNOMBREALMACEN",
                DataPropertyName = "CNOMBREALMACEN",
                ReadOnly = true
            };

            DataGridViewTextBoxColumn existenciaColumn = new DataGridViewTextBoxColumn
            {
                HeaderText = "Existencia",
                Name = "CEXISTENCIA",
                DataPropertyName = "CEXISTENCIA",
                ReadOnly = true
            };

            DataGridViewTextBoxColumn cantidadColumn = new DataGridViewTextBoxColumn
            {
                HeaderText = "Cantidad a Tomar",
                Name = "Cantidad"
            };
            DataGridViewTextBoxColumn totalRestanteColumn = new DataGridViewTextBoxColumn
            {
                HeaderText = "Total Restante",
                Name = "TotalRestante",
                ReadOnly = true
            };

            // Agregar las columnas al DataGridView
            dataGridView1.Columns.Add(codigoProductoColumn);
            dataGridView1.Columns.Add(numeroLoteColumn);
            dataGridView1.Columns.Add(nombreProductoColumn);
            dataGridView1.Columns.Add(nombreAlmacenColumn);
            dataGridView1.Columns.Add(existenciaColumn);
            dataGridView1.Columns.Add(cantidadColumn);
            dataGridView1.Columns.Add(totalRestanteColumn);

            // Configurar evento para validación al finalizar edición de la celda
            dataGridView1.CellEndEdit += DataGridView1_CellEndEdit;
        }

        // Método para cargar los datos desde la base de datos
        private void CargarDatosLotes()
        {
            // Define tu cadena de conexión aquí
            string connectionString = @"Data Source=192.168.1.127\COMPAC;Initial Catalog=adAMSACONTPAQi;Persist Security Info=True;User ID=sa;Password=AdminSql7639!";

            // Define tu consulta SQL con filtro basado en el código de producto
            string query = @"
                SELECT TOP (1000)
                    h.[CCODIGOPRODUCTO],
                    d.[CNUMEROLOTE],
                    h.[CNOMBREPRODUCTO],
                    a.[CNOMBREALMACEN],
                    d.[CEXISTENCIA]
                FROM [adAMSACONTPAQi].[dbo].[admProductos] h
                LEFT JOIN admCapasProducto d ON h.CIDPRODUCTO = d.CIDPRODUCTO
                LEFT JOIN admExistenciaCosto e ON h.CIDPRODUCTO = e.CIDPRODUCTO AND d.CIDALMACEN = e.CIDALMACEN
                LEFT JOIN admAlmacenes a ON a.CIDALMACEN = d.CIDALMACEN AND a.CIDALMACEN = e.CIDALMACEN
                WHERE e.CIDALMACEN IS NOT NULL
                    AND e.CIDEJERCICIO = 7
                    AND h.CTIPOPRODUCTO = 1
                    AND h.CCONTROLEXISTENCIA = 17
                    AND d.CEXISTENCIA != 0
                    AND d.CIDPRODUCTO  = @CodProducto -- Filtro por código de producto
                ORDER BY h.CIDPRODUCTO;
            ";

            // Crear conexión y adaptador para ejecutar la consulta
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open(); // Abrir la conexión
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@CodProducto", CodP); // Usar el parámetro para evitar SQL Injection

                    SqlDataAdapter adapter = new SqlDataAdapter(command); // Crear adaptador de datos
                    DataTable dataTable = new DataTable(); // Crear DataTable para almacenar los resultados
                    adapter.Fill(dataTable); // Llenar el DataTable con los resultados de la consulta

                    // Asignar el DataTable como fuente de datos para el DataGridView
                    dataGridView1.DataSource = dataTable;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar los datos: " + ex.Message);
                }
            }
        }

        // Evento para validar la entrada del usuario en la columna "Cantidad"
        private void DataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridView1.Columns["Cantidad"].Index)
            {
                int cantidadTomar;
                if (int.TryParse(dataGridView1.Rows[e.RowIndex].Cells["Cantidad"].Value?.ToString(), out cantidadTomar))
                {
                    int existencia = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["CEXISTENCIA"].Value);

                    if (cantidadTomar > existencia)
                    {
                        MessageBox.Show("No puede tomar más unidades de las disponibles.");
                        dataGridView1.Rows[e.RowIndex].Cells["Cantidad"].Value = 0;
                        dataGridView1.Rows[e.RowIndex].Cells["TotalRestante"].Value = existencia;
                    }
                    else
                    {
                        int totalRestante = existencia - cantidadTomar;
                        dataGridView1.Rows[e.RowIndex].Cells["TotalRestante"].Value = totalRestante;
                    }
                }
                else
                {
                    MessageBox.Show("Por favor, ingrese un número válido.");
                    dataGridView1.Rows[e.RowIndex].Cells["Cantidad"].Value = 0;
                dataGridView1.Rows[e.RowIndex].Cells["TotalRestante"].Value = dataGridView1.Rows[e.RowIndex].Cells["CEXISTENCIA"].Value;
            }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void FrmLotesProducto_Load(object sender, EventArgs e)
        {

        }
    }
}