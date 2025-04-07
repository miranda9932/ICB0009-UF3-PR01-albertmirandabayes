using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

public static class NetworkStreamClass
{
    private static readonly Encoding _encoding = Encoding.ASCII;
    private const int BufferSize = 1024;

    public static void EscribirDatosVehiculo(NetworkStream stream, Vehiculo vehiculo)
    {
        try
        {
            string json = JsonSerializer.Serialize(vehiculo);
            Console.WriteLine($"Enviando JSON del vehículo: {json}");
            EscribirMensajeNetworkStream(stream, $"VEH:{json}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al serializar vehículo: {ex.Message}");
            throw;
        }
    }

    public static Vehiculo LeerDatosVehiculo(NetworkStream stream)
    {
        try
        {
            string mensaje = LeerMensajeNetworkStream(stream);
            Console.WriteLine($"Recibido mensaje para vehículo: {mensaje}");
            
            if (!mensaje.StartsWith("VEH:"))
            {
                throw new InvalidOperationException("Formato de mensaje incorrecto. Se esperaba 'VEH:'");
            }

            string json = mensaje.Substring(4); // Eliminar el prefijo "VEH:"
            Console.WriteLine($"JSON del vehículo a deserializar: {json}");
            
            var vehiculo = JsonSerializer.Deserialize<Vehiculo>(json);
            if (vehiculo == null)
            {
                throw new InvalidOperationException("No se pudo deserializar el vehículo");
            }
            
            return vehiculo;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al deserializar vehículo: {ex.Message}");
            throw;
        }
    }

    public static void EscribirDatosCarretera(NetworkStream stream, Carretera carretera)
    {
        string json = JsonSerializer.Serialize(carretera);
        EscribirMensajeNetworkStream(stream, $"CARR:{json}");
    }

    public static Carretera LeerDatosCarretera(NetworkStream stream)
    {
        try
        {
            string mensaje = LeerMensajeNetworkStream(stream);
            Console.WriteLine($"Recibido mensaje para carretera: {mensaje}");
            
            if (!mensaje.StartsWith("CARR:"))
            {
                throw new InvalidOperationException("Formato de mensaje incorrecto. Se esperaba 'CARR:'");
            }

            string json = mensaje.Substring(5); // Eliminar el prefijo "CARR:"
            Console.WriteLine($"JSON de la carretera a deserializar: {json}");
            
            var carretera = JsonSerializer.Deserialize<Carretera>(json);
            if (carretera == null)
            {
                throw new InvalidOperationException("No se pudo deserializar la carretera");
            }
            
            return carretera;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al deserializar carretera: {ex.Message}");
            throw;
        }
    }

    public static void EscribirMensajeNetworkStream(NetworkStream stream, string mensaje)
    {
        try
        {
            if (!stream.CanWrite)
                throw new InvalidOperationException("El stream no permite escritura");

            // Formato: [longitud mensaje]:[mensaje]
            string mensajeFormateado = $"{mensaje.Length}:{mensaje}";
            Console.WriteLine($"Enviando mensaje formateado: {mensajeFormateado}");
            
            byte[] buffer = _encoding.GetBytes(mensajeFormateado);
            stream.Write(buffer, 0, buffer.Length);
            stream.Flush(); // Asegurar que los datos se envían
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
            Console.WriteLine($"Longitud del mensaje a leer: {length}");
            
            byte[] buffer = new byte[length];
            int bytesRead = 0;

            // Leer el mensaje completo
            while (bytesRead < length)
            {
                int read = stream.Read(buffer, bytesRead, length - bytesRead);
                if (read == 0) break; // Fin del stream
                bytesRead += read;
            }

            string mensaje = _encoding.GetString(buffer, 0, length);
            Console.WriteLine($"Mensaje recibido: {mensaje}");
            return mensaje;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Error lectura] {ex.Message}");
            throw;
        }
    }
}