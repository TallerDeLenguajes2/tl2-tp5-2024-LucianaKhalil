using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SQLite;

//realizo interfaz
public interface IproductoRepository{
    List<Productos> getAll();

    Productos GetById(int idProducto);

    void Create(Productos producto);

    void Update(Productos producto, int id);
    void Delete(int id);
}

public class ProductoRepositorio : IproductoRepository{
     private string cadenaConexion = @"Data Source=db\Tienda.db;Cache=Shared";
    //constructor
    public ProductoRepositorio(string CadenaConexion)
    {
        cadenaConexion = CadenaConexion;
    }
      public List<Productos> getAll()
        {
            var queryString = @"SELECT * FROM Productos;";
            List<Productos> productos = new List<Productos>();
            using (SQLiteConnection connection = new SQLiteConnection(cadenaConexion))
            {
                SQLiteCommand command = new SQLiteCommand(queryString, connection);
                connection.Open();
            
                using(SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var producto = new Productos();
                        producto.IdProducto = Convert.ToInt32(reader["idProducto"]);
                        producto.Descripcion = reader["Descripcion"].ToString();
                        producto.Precio = Convert.ToInt32(reader["Precio"]);
                        productos.Add(producto);//agregar a la lista
                    }
                }
                connection.Close();
            }
            return productos;
        }
    public Productos GetById(int id)
        {
            SQLiteConnection connection = new SQLiteConnection(cadenaConexion);
            var producto = new Productos();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Productos WHERE idProducto = @idProducto";
            command.Parameters.Add(new SQLiteParameter("@idProducto", id));
            connection.Open();
            using(SQLiteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    producto.IdProducto = Convert.ToInt32(reader["idProducto"]);
                    producto.Descripcion = reader["Descripcion"].ToString();
                    producto.Precio = Convert.ToInt32(reader["Precio"]);
                }
            }
            connection.Close();

            return (producto);
        }

    public void Create(Productos producto)
        {
            var query = $"INSERT INTO Productos (Descripcion, Precio) VALUES (@Descripcion, @Precio)";
            using (SQLiteConnection connection = new SQLiteConnection(cadenaConexion))
            {

                connection.Open();
                var command = new SQLiteCommand(query, connection);

                command.Parameters.Add(new SQLiteParameter("@Descripcion", producto.Descripcion));
                command.Parameters.Add(new SQLiteParameter("@Precio", producto.Precio));

                command.ExecuteNonQuery();

                connection.Close();   
            }
        }
    
    public void Update(Productos producto, int id){
        var query =$"UPDATE Productos SET Descripcion = '{producto.Descripcion}', Precio = '{producto.Precio}' WHERE idProducto = '{producto.IdProducto}'";
            using (SQLiteConnection connection = new SQLiteConnection(cadenaConexion))
            {

                connection.Open();
                var command = new SQLiteCommand(query, connection);

                command.Parameters.Add(new SQLiteParameter("@Descripcion", producto.Descripcion));
                command.Parameters.Add(new SQLiteParameter("@Precio", producto.Precio));

                command.ExecuteNonQuery();

                connection.Close();   
            }
    }
    public void Delete(int id){
         var query = "DELETE FROM Productos WHERE idProducto = @idProducto"; // Usar parámetro seguro
            using (SQLiteConnection connection = new SQLiteConnection(cadenaConexion))
            {

                connection.Open();
                var command = new SQLiteCommand(query, connection);

                command.Parameters.Add(new SQLiteParameter("@idProducto", id));
                command.ExecuteNonQuery();
                connection.Close();   
            }
    }

    


}