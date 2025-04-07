// Ejercicio2/Cliente/Program.cs
using System.Net.Sockets;

class Program
{
    static void Main()
    {
        try
        {
            Console.WriteLine("Intentando conectar al servidor...");
            using TcpClient client = new TcpClient("localhost", 8080);
            Console.WriteLine("Conexión establecida con éxito.");
            
            NetworkStream stream = client.GetStream();

            // Handshake
            NetworkStreamClass.EscribirMensajeNetworkStream(stream, "INICIO");
            string[] datos = NetworkStreamClass.LeerMensajeNetworkStream(stream).Split(':');
            int id = int.Parse(datos[0]);
            string direccion = datos[1];
            NetworkStreamClass.EscribirMensajeNetworkStream(stream, $"ACK:{id}");

            // --- Nuevo en Ejercicio 2 ---
            var vehiculo = new Vehiculo 
            {
                Id = id,
                Posicion = 0,
                Direccion = direccion
            };
            NetworkStreamClass.EscribirDatosVehiculo(stream, vehiculo);
            Console.WriteLine($"Vehículo {id} listo. Dirección: {direccion}");

            // Simulación de movimiento (Etapa 3)
        }
        catch (SocketException)
        {
            Console.WriteLine("Error: No se pudo conectar al servidor.");
            Console.WriteLine("Asegúrate de que el servidor esté en ejecución primero (dotnet run en la carpeta Servidor).");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error inesperado: {ex.Message}");
        }
    }
}