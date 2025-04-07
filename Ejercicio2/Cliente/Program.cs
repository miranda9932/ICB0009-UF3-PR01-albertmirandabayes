using System.Net.Sockets;

class Program
{
    static bool _conectado = true;

    static void Main()
    {
        try
        {
            Console.WriteLine("Conectando al servidor...");
            using TcpClient client = new TcpClient("localhost", 8080);
            NetworkStream stream = client.GetStream();

            // --- Handshake ---
            NetworkStreamClass.EscribirMensajeNetworkStream(stream, "INICIO");
            string[] datos = NetworkStreamClass.LeerMensajeNetworkStream(stream).Split(':');
            int id = int.Parse(datos[0]);
            string direccion = datos[1];
            NetworkStreamClass.EscribirMensajeNetworkStream(stream, $"ACK:{id}");

            // --- Crear vehículo ---
            var vehiculo = new Vehiculo 
            {
                Id = id,
                Posicion = 0,
                Direccion = direccion,
                ViajeCompletado = false
            };

            // --- Hilo para recibir updates (ETAPA 3) ---
            new Thread(() => 
            {
                while (_conectado)
                {
                    try
                    {
                        var carretera = NetworkStreamClass.LeerDatosCarretera(stream);
                        MostrarEstado(carretera);
                    }
                    catch { /* Ignorar errores al cerrar */ }
                }
            }).Start();

            // --- Simulación de movimiento (ETAPA 3) ---
            for (int pos = 0; pos <= 100; pos += 5)
            {
                vehiculo.Posicion = pos;
                NetworkStreamClass.EscribirDatosVehiculo(stream, vehiculo);
                Console.WriteLine($"⏩ Enviada posición: {pos}km");
                Thread.Sleep(1500); // 1.5 segundos entre movimientos

                if (pos == 100) vehiculo.ViajeCompletado = true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            _conectado = false;
            Console.WriteLine("Viaje completado. Presiona cualquier tecla para salir...");
            Console.ReadKey();
        }
    }

    static void MostrarEstado(Carretera carretera)
    {
        Console.WriteLine("\n=== TRÁFICO ACTUAL ===");
        foreach (var v in carretera.Vehiculos)
        {
            string estado = v.ViajeCompletado ? "COMPLETADO" : $"{v.Posicion}km";
            Console.WriteLine($"🚙 #{v.Id} ({v.Direccion}): {estado}");
        }
        Console.WriteLine("----------------------\n");
    }
}