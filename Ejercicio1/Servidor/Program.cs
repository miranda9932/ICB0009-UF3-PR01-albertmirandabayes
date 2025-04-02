using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

class Program
{
    static void Main()
    {
        TcpListener server = new TcpListener(IPAddress.Any, 8080);
        server.Start();
        Console.WriteLine("Servidor iniciado en el puerto 8080...");

        while (true)
        {
            TcpClient client = server.AcceptTcpClient();
            Console.WriteLine("Nuevo cliente conectado.");
            Thread clientThread = new Thread(() => HandleClient(client));
            clientThread.Start();
        }
    }

    static void HandleClient(TcpClient client)
    {
        // Lógica para manejar el cliente (Etapa 2 en adelante)
        Console.WriteLine("Gestionando nuevo vehículo...");
        client.Close();
    }
}