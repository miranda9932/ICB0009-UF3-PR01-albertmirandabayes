using System.Net.Sockets;

class Program
{
    static bool _conectado = true;
    static int _id;

    static void Main()
    {
        try
        {
            Console.WriteLine("🔌 Conectando al servidor...");
            using TcpClient client = new TcpClient("localhost", 8080);
            NetworkStream stream = client.GetStream();

            // --- Handshake ---
            NetworkStreamClass.EscribirMensajeNetworkStream(stream, "INICIO");
            string[] datos = NetworkStreamClass.LeerMensajeNetworkStream(stream).Split(':');
            _id = int.Parse(datos[0]);
            string direccion = datos[1];
            NetworkStreamClass.EscribirMensajeNetworkStream(stream, $"ACK:{_id}");

            // --- Registro inicial ---
            var vehiculo = new Vehiculo 
            {
                Id = _id,
                Posicion = 0,
                Direccion = direccion,
                ViajeCompletado = false
            };
            NetworkStreamClass.EscribirDatosVehiculo(stream, vehiculo);
            Console.WriteLine($"✅ Conectado como Vehículo #{_id} ({direccion})");

            // --- Hilo de recepción ---
            new Thread(() => 
            {
                while (_conectado)
                {
                    try
                    {
                        var carretera = NetworkStreamClass.LeerDatosCarretera(stream);
                        if (carretera != null)
                        {
                            MostrarEstado(carretera);
                        }
                    }
                    catch (IOException)
                    {
                        if (_conectado) Console.WriteLine("🔴 Conexión con el servidor perdida");
                        break;
                    }
                    catch (Exception ex)
                    {
                        if (_conectado) Console.WriteLine($"⚠️ Error recibiendo datos: {ex.Message}");
                    }
                }
            }).Start();

            // --- Simulación de movimiento ---
            SimularViaje(stream, vehiculo);
        }
        catch (SocketException)
        {
            Console.WriteLine("🔴 Error: Servidor no disponible");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Error crítico: {ex.Message}");
        }
        finally
        {
            _conectado = false;
            Console.WriteLine("\n🛑 Programa terminado. Presiona cualquier tecla para salir...");
            Console.ReadKey();
        }
    }

    static void SimularViaje(NetworkStream stream, Vehiculo vehiculo)
    {
        for (int pos = 0; pos <= 100 && _conectado; pos += 5)
        {
            vehiculo.Posicion = pos;
            NetworkStreamClass.EscribirDatosVehiculo(stream, vehiculo);
            Console.WriteLine($"⏩ Enviada posición: {pos}km");
            Thread.Sleep(1500);

            if (pos == 100)
            {
                vehiculo.ViajeCompletado = true;
                NetworkStreamClass.EscribirDatosVehiculo(stream, vehiculo);
                Console.WriteLine("🏁 ¡Viaje completado!");
            }
        }
    }

    static void MostrarEstado(Carretera carretera)
    {
        Console.Clear();
        Console.WriteLine($"=== ESTADO DE LA CARRETERA ===");
        Console.WriteLine($"📊 Total vehículos: {carretera.Vehiculos.Count}");
        
        foreach (var v in carretera.Vehiculos.OrderBy(v => v.Posicion))
        {
            string estado = v.ViajeCompletado ? "✅ COMPLETADO" : $"🚗 km {v.Posicion}";
            string marca = v.Id == _id ? "(TÚ)" : "";
            Console.WriteLine($"#{v.Id} {marca} [{v.Direccion}] ➔ {estado}");
        }

        Console.WriteLine("\n📌 Leyenda: [Norte] → ← [Sur]");
        Console.WriteLine($"\n>> TU POSICIÓN ACTUAL: {carretera.Vehiculos.First(v => v.Id == _id).Posicion}km <<");
    }
}