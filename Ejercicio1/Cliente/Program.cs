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

        // Paso 1: Enviar "INICIO"
        NetworkStreamClass.EscribirMensajeNetworkStream(stream, "INICIO");
        Console.WriteLine("Iniciando handshake...");

        // Paso 2: Recibir ID y dirección
        string datosServidor = NetworkStreamClass.LeerMensajeNetworkStream(stream);
        string[] partes = datosServidor.Split(':');
        int id = int.Parse(partes[0]);
        string direccion = partes[1];
        
        Console.WriteLine($"Asignado ID: {id}, Dirección: {direccion}");

        // Paso 3: Confirmar ID al servidor
        NetworkStreamClass.EscribirMensajeNetworkStream(stream, id.ToString());
        Console.WriteLine("Handshake completado exitosamente!");

        // Aquí iría el resto de la lógica del cliente...
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

}