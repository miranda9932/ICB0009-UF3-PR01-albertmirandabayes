using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

public static class NetworkStreamClass
{
    private static readonly Encoding _encoding = Encoding.ASCII;
    private const int BufferSize = 1024;

    public static void EscribirMensajeNetworkStream(NetworkStream stream, string mensaje)
    {
        try
        {
            if (!stream.CanWrite)
                throw new InvalidOperationException("El stream no permite escritura");

            // Formato: [longitud mensaje]:[mensaje]
            string mensajeFormateado = $"{mensaje.Length}:{mensaje}";
            byte[] buffer = _encoding.GetBytes(mensajeFormateado);
            stream.Write(buffer, 0, buffer.Length);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Error escritura] {ex.Message}");
            throw;
        }
    }

    public static string LeerMensajeNetworkStream(NetworkStream stream)
    {
        try
        {
            if (!stream.CanRead)
                throw new InvalidOperationException("El stream no permite lectura");

            // Leer primero la longitud del mensaje
            StringBuilder lengthBuilder = new StringBuilder();
            byte[] singleByte = new byte[1];
            
            while (stream.Read(singleByte, 0, 1) > 0)
            {
                char c = _encoding.GetChars(singleByte)[0];
                if (c == ':') break;
                lengthBuilder.Append(c);
            }

            int length = int.Parse(lengthBuilder.ToString());
            byte[] buffer = new byte[length];
            int bytesRead = 0;

            // Leer el mensaje completo
            while (bytesRead < length)
            {
                bytesRead += stream.Read(buffer, bytesRead, length - bytesRead);
            }

            return _encoding.GetString(buffer, 0, length);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Error lectura] {ex.Message}");
            throw;
        }
    }
}