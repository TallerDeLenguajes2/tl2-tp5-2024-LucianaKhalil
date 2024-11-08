using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SQLite;

//interfaz
public interface IPresupuestosRepository{
    void Create(Presupuesto presupuesto);
    List<Presupuesto> getAll();

    Presupuesto GetById(int id);
    void Update(int id, Productos producto, int cantidad);
    void Delete(int id);
}

public class PresupuestoRepositorio : IPresupuestosRepository{
     private string cadenaConexion = @"Data Source=db\Tienda.db;Cache=Shared";
    //constructor
    public PresupuestoRepositorio(string CadenaConexion)
    {
        cadenaConexion = CadenaConexion;
    }
      public List<Presupuesto> getAll()
        {
            var queryString = @"SELECT * FROM Presupuestos;";
            List<Presupuesto> presupuestos = new List<Presupuesto>();
            using (SQLiteConnection connection = new SQLiteConnection(cadenaConexion))
            {
                SQLiteCommand command = new SQLiteCommand(queryString, connection);
                connection.Open();
            
                using(SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int idPresupuesto = Convert.ToInt32(reader["idPresupuesto"]);
                        string nombreDestinatario = reader["nombreDestinatario"].ToString();
                        Presupuesto presupuesto = new Presupuesto(idPresupuesto, nombreDestinatario);
                        presupuestos.Add(presupuesto);
                    }
                }
                connection.Close();
            }
            return presupuestos;
        }
    public Presupuesto GetById(int id)
        {
            SQLiteConnection connection = new SQLiteConnection(cadenaConexion);
            Presupuesto presupuesto=null;//inicializo en null

            SQLiteCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Presupuestos WHERE idPresupuesto = @idPresupuesto";
            command.Parameters.Add(new SQLiteParameter("@idPresupuesto", id));
            connection.Open();
            using(SQLiteDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    int idPresupuesto = Convert.ToInt32(reader["idPresupuesto"]);
                    string nombreDestinatario = reader["nombreDestinatario"].ToString();

                    presupuesto = new Presupuesto(idPresupuesto, nombreDestinatario);
                    presupuesto.Detalle = GetPresupuestoDetalles(idPresupuesto);
                }
                connection.Close();

                return (presupuesto);
            }
        }

    public void Create(Presupuesto presupuesto)
    {
        var query = @"INSERT INTO Presupuestos (idPresupuesto, nombreDestinatario, FechaCreacion) 
                    VALUES (@idPresupuesto, @nombreDestinatario, @FechaCreacion)";
        using (SQLiteConnection connection = new SQLiteConnection(cadenaConexion))
        {
            connection.Open();
            var command = new SQLiteCommand(query, connection);

            command.Parameters.Add(new SQLiteParameter("@idPresupuesto", presupuesto.IdPresupuesto));
            command.Parameters.Add(new SQLiteParameter("@nombreDestinatario", presupuesto.NombreDestinatario));
            command.Parameters.Add(new SQLiteParameter("@FechaCreacion", DateTime.Now));

            command.ExecuteNonQuery();
            connection.Close();   
        }
    }

    

    public void Update(int id, Productos producto, int cantidad){
        var query ="UPDATE PresupuestosDetalle SET Cantidad = @Cantidad WHERE idPresupuesto = @idPresupuesto AND idProducto = @idProducto;";
            using (SQLiteConnection connection = new SQLiteConnection(cadenaConexion))
            {

                connection.Open();
                var command = new SQLiteCommand(query, connection);

                command.Parameters.Add(new SQLiteParameter("@idPresupuesto", id));
                command.Parameters.Add(new SQLiteParameter("@idProducto", producto.IdProducto));
                command.Parameters.Add(new SQLiteParameter("@Cantidad", cantidad));

                command.ExecuteNonQuery();

                connection.Close();   
            }
    }
    public void Delete(int id){
        var queryDetalles = "DELETE FROM PresupuestoDetalles WHERE idPresupuesto = @idPresupuesto";
        var queryPresupuesto = "DELETE FROM Presupuestos WHERE id = @idPresupuesto";


            using (SQLiteConnection connection = new SQLiteConnection(cadenaConexion))
            {

                //elimiar detalles primero
                var commandDetalles = new SQLiteCommand(queryDetalles, connection);
                commandDetalles.Parameters.Add(new SQLiteParameter("@idPresupuesto", id));
                commandDetalles.ExecuteNonQuery();
                
                //eliminar presupuesto despues
                var commandPresupuestos = new SQLiteCommand(queryPresupuesto, connection);
                commandPresupuestos.Parameters.Add(new SQLiteParameter("@idPresupuesto", id));


                connection.Close();   
            }
    }

//obtener detalles de presupuesto private List<PresupuestoDetalle> GetPresupuestoDetalles(int id)---------------------

   private List<PresupuestoDetalle> GetPresupuestoDetalles(int id)
{
    List<PresupuestoDetalle> presupuestosDetalles = new List<PresupuestoDetalle>();
    string query = @"SELECT p.idProducto, p.Descripcion, p.Precio, d.Cantidad
                     FROM PresupuestosDetalle d
                     JOIN Productos p ON d.idProducto = p.idProducto
                     WHERE d.idPresupuesto = @idPresupuesto;";

    using (SQLiteConnection connection = new SQLiteConnection(cadenaConexion))
    {
        connection.Open();
        SQLiteCommand command = new SQLiteCommand(query, connection);
        command.Parameters.Add(new SQLiteParameter("@idPresupuesto", id));

        using (SQLiteDataReader reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                // Crear el objeto Producto
                Productos producto = new Productos
                {
                    IdProducto = reader.GetInt32(0),
                    Descripcion = reader.GetString(1),
                    Precio = reader.GetInt32(2)
                };

                // Leer la cantidad
                int cantidad = reader.GetInt32(3);

                // Crear el PresupuestoDetalle y agregarlo a la lista
                presupuestosDetalles.Add(new PresupuestoDetalle(producto, cantidad));
            }
        }
    }
    return presupuestosDetalles;
}

}
