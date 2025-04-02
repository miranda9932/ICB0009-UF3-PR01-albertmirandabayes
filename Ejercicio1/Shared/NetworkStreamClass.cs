using System;
using System.IO;
using System.Net.Sockets;

public static class NetworkStreamClass
{
    public static void EscribirMensajeNetworkStream(NetworkStream stream, string mensaje)
    {
        try
        {
            byte[] buffer = System.Text.Encoding.ASCII.GetBytes(mensaje);
            stream.Write(buffer, 0, buffer.Length);
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Error al escribir: {ex.Message}");
        }
    }

    public static string LeerMensajeNetworkStream(NetworkStream stream)
    {
        try
        {
            byte[] buffer = new byte[256];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            return System.Text.Encoding.ASCII.GetString(buffer, 0, bytesRead);
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Error al leer: {ex.Message}");
            return null;
        }
    }
}