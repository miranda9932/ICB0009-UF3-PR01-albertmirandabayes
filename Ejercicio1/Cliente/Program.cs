using System;
using System.Net.Sockets;

class Program
{
    static void Main()
    {
        try
        {
            TcpClient client = new TcpClient("127.0.0.1", 8080);
            NetworkStream stream = client.GetStream();

            // Recibir ID y dirección usando NetworkStreamClass
            string respuesta = NetworkStreamClass.LeerMensajeNetworkStream(stream);
            string[] datos = respuesta.Split(':');
            Console.WriteLine($"ID asignado: {datos[0]}, Dirección: {datos[1]}");

            // Enviar mensaje de prueba al servidor
            NetworkStreamClass.EscribirMensajeNetworkStream(stream, "¡Conectado correctamente!");

            Console.WriteLine("Presiona Enter para salir...");
            Console.ReadLine();
            
            client.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
