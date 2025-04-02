using System;
using System.Net.Sockets;

class Program
{
    static void Main()
    {
        TcpClient client = new TcpClient("127.0.0.1", 8080);
        Console.WriteLine("Conectado al servidor.");

        // Handshake inicial (Etapa 6)
        NetworkStream stream = client.GetStream();
        byte[] buffer = System.Text.Encoding.ASCII.GetBytes("INICIO");
        stream.Write(buffer, 0, buffer.Length);

        client.Close();
    }
}