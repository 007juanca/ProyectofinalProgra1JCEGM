using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace JuancaProyecto
{
    public partial class MainWindow : Window
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private DataTable departamentosTable;
        private DataRowView selectedRow;

        public MainWindow()
        {
            InitializeComponent();
            LoadDepartamentos();
        }

        private SqlConnection CrearConexion()
        {
            return new SqlConnection(connectionString);
        }

        private void LoadDepartamentos()
        {
            try
            {
                using (SqlConnection connection = CrearConexion())
                {
                    string query = "SELECT ID, Nombre, Ubicacion, Cantidad, Descripcion FROM Departamentos";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);

                    departamentosTable = new DataTable();
                    adapter.Fill(departamentosTable);

                    DepartamentosDataGrid.ItemsSource = departamentosTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los departamentos: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DepartamentosDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DepartamentosDataGrid.SelectedItem != null)
            {
                selectedRow = (DataRowView)DepartamentosDataGrid.SelectedItem;

                TxtIDValue.Text = selectedRow["ID"].ToString();
                TxtNombre.Text = selectedRow["Nombre"].ToString();
                TxtUbicacion.Text = selectedRow["Ubicacion"].ToString();
                TxtCantidad.Text = selectedRow["Cantidad"].ToString();
                TxtDescripcion.Text = selectedRow["Descripcion"].ToString();
            }
        }

        private void BtnActualizar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (selectedRow != null)
                {
                    using (SqlConnection connection = CrearConexion())
                    {
                        string query = "UPDATE Departamentos SET Nombre = @nombre, Ubicacion = @ubicacion, Cantidad = @cantidad, Descripcion = @descripcion WHERE ID = @id";

                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@nombre", TxtNombre.Text);
                        command.Parameters.AddWithValue("@ubicacion", TxtUbicacion.Text);

                        int cantidad;
                        if (int.TryParse(TxtCantidad.Text, out cantidad))
                            command.Parameters.AddWithValue("@cantidad", cantidad);
                        else
                            command.Parameters.AddWithValue("@cantidad", DBNull.Value);

                        command.Parameters.AddWithValue("@descripcion", TxtDescripcion.Text);

                        int id = (int)selectedRow["ID"];
                        command.Parameters.AddWithValue("@id", id);

                        connection.Open();
                        command.ExecuteNonQuery();

                        LoadDepartamentos();
                        ClearInputs();
                    }
                }
                else
                {
                    MessageBox.Show("No se ha seleccionado ningún departamento.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar los datos: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (selectedRow != null)
                {
                    MessageBoxResult result = MessageBox.Show("¿Está seguro de que desea eliminar este departamento?", "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        using (SqlConnection connection = CrearConexion())
                        {
                            string query = "DELETE FROM Departamentos WHERE ID = @id";
                            SqlCommand command = new SqlCommand(query, connection);
                            int id = (int)selectedRow["ID"];
                            command.Parameters.AddWithValue("@id", id);

                            connection.Open();
                            command.ExecuteNonQuery();

                            LoadDepartamentos();
                            ClearInputs();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("No se ha seleccionado ningún departamento.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar el departamento: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection connection = CrearConexion())
                {
                    string query = "INSERT INTO Departamentos (Nombre, Ubicacion, Cantidad, Descripcion) VALUES (@nombre, @ubicacion, @cantidad, @descripcion)";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@nombre", TxtNombre.Text);
                    command.Parameters.AddWithValue("@ubicacion", TxtUbicacion.Text);

                    int cantidad;
                    if (int.TryParse(TxtCantidad.Text, out cantidad))
                        command.Parameters.AddWithValue("@cantidad", cantidad);
                    else
                        command.Parameters.AddWithValue("@cantidad", DBNull.Value);

                    command.Parameters.AddWithValue("@descripcion", TxtDescripcion.Text);

                    connection.Open();
                    command.ExecuteNonQuery();

                    LoadDepartamentos();
                    ClearInputs();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar el departamento: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearInputs()
        {
            TxtIDValue.Text = "";
            TxtNombre.Text = "";
            TxtUbicacion.Text = "";
            TxtCantidad.Text = "";
            TxtDescripcion.Text = "";
            selectedRow = null;
        }
    }
}