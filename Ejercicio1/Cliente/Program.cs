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

            // Recibir ID y dirección del servidor
            byte[] buffer = new byte[256];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string respuesta = System.Text.Encoding.ASCII.GetString(buffer, 0, bytesRead);
            
            string[] datos = respuesta.Split(':');
            Console.WriteLine($"ID asignado: {datos[0]}, Dirección: {datos[1]}");

            // Mantener conexión (para pruebas)
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

