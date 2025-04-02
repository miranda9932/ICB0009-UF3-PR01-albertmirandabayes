using System;
using System.Net.Sockets;

class Program
{
    static void Main()
    {
        try
        {
            using TcpClient client = new TcpClient("127.0.0.1", 8080);
            NetworkStream stream = client.GetStream();

            // Recibir datos de conexión
            string datos = NetworkStreamClass.LeerMensajeNetworkStream(stream);
            string[] partes = datos.Split(':');
            Console.WriteLine($"Conectado como Vehículo #{partes[0]} (Dirección: {partes[1]})");

            // Enviar confirmación
            NetworkStreamClass.EscribirMensajeNetworkStream(stream, "CONFIRMACION_RECIBIDA");

            // Simular movimiento
            for (int posicion = 0; posicion <= 100; posicion += 10)
            {
                NetworkStreamClass.EscribirMensajeNetworkStream(stream, posicion.ToString());
                string respuesta = NetworkStreamClass.LeerMensajeNetworkStream(stream);
                Console.WriteLine($"Avanzando... Posición: {posicion}km (Servidor: {respuesta})");
                System.Threading.Thread.Sleep(1000);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}