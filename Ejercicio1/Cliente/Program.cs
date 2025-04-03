using System;
using System.Net.Sockets;
using System.Threading;

class Program
{
    static void Main()
    {
        try
        {
            Console.WriteLine("🚦 Conectando al servidor...");
            
            using TcpClient client = new TcpClient("127.0.0.1", 8080);
            NetworkStream stream = client.GetStream();
            var reader = new System.IO.StreamReader(stream);
            var writer = new System.IO.StreamWriter(stream) { AutoFlush = true };

            // 1. Handshake - Recibir ID
            string idMessage = reader.ReadLine();
            if (!idMessage.StartsWith("ID:"))
                throw new Exception("Protocolo inválido");

            int id = int.Parse(idMessage.Split(':')[1]);
                
            // 2. Confirmar handshake
            writer.WriteLine($"ACK:{id}");
            Console.WriteLine($"✅ Conectado como Vehículo #{id}");

            // 3. Simular movimiento
            for (int posicion = 0; posicion <= 100; posicion += 10)
            {
                // Enviar posición actual
                writer.WriteLine($"POS:{posicion}");
                Console.WriteLine($"📤 Enviada posición: {posicion}km");

                // Recibir actualizaciones de otros vehículos
                if (stream.DataAvailable)
                {
                    string update = reader.ReadLine();
                    if (!string.IsNullOrEmpty(update))
                        Console.WriteLine($"📥 Actualización: {update}");
                }

                Thread.Sleep(2000); // Espera 2 segundos
            }

            Console.WriteLine("🏁 Recorrido completado");
        }
        catch (SocketException)
        {
            Console.WriteLine("🔌 Error: No se pudo conectar al servidor");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Error: {ex.Message}");
        }
        finally
        {
            Console.WriteLine("Presiona Enter para salir...");
            Console.ReadLine();
        }
    }
}