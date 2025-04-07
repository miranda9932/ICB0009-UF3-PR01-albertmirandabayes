using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Collections.Concurrent;

class Program
{
    static ConcurrentDictionary<int, Cliente> _clientes = new();
    static Carretera _carretera = new();
    static int _nextId = 1;
    static object _lock = new();

    class Cliente
    {
        public int Id { get; }
        public TcpClient TcpClient { get; }
        public NetworkStream Stream => TcpClient.GetStream();

        public Cliente(int id, TcpClient client)
        {
            Id = id;
            TcpClient = client;
        }
    }

    static void Main()
    {
        var server = new TcpListener(IPAddress.Any, 8080);
        server.Start();
        Console.WriteLine("Servidor iniciado (Ejercicio 2)");

        while (true)
        {
            try
            {
                var client = server.AcceptTcpClient();
                var id = GenerarId();
                var cliente = new Cliente(id, client);
                _clientes.TryAdd(id, cliente);
                Console.WriteLine($"Nuevo cliente conectado con ID: {id}");

                new Thread(() => HandleClient(cliente)).Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al aceptar cliente: {ex.Message}");
            }
        }
    }

    static int GenerarId()
    {
        lock (_lock) return _nextId++;
    }

    static void HandleClient(Cliente cliente)
    {
        try
        {
            // --- Handshake ---
            NetworkStreamClass.LeerMensajeNetworkStream(cliente.Stream); // "INICIO"
            string direccion = new Random().Next(0, 2) == 0 ? "Norte" : "Sur";
            NetworkStreamClass.EscribirMensajeNetworkStream(cliente.Stream, $"{cliente.Id}:{direccion}");
            NetworkStreamClass.LeerMensajeNetworkStream(cliente.Stream); // "ACK:id"

            // --- Recibir vehículo inicial ---
            Vehiculo vehiculo = NetworkStreamClass.LeerDatosVehiculo(cliente.Stream);
            lock (_lock)
            {
                _carretera.Vehiculos.Add(vehiculo);
            }

            Console.WriteLine($"Vehículo #{vehiculo.Id} inició recorrido ({direccion})");

            // --- Bucle principal (ETAPA 3) ---
            while (cliente.TcpClient.Connected)
            {
                var vehiculoActualizado = NetworkStreamClass.LeerDatosVehiculo(cliente.Stream);
                
                // Actualizar posición
                lock (_lock)
                {
                    var vehiculoEnCarretera = _carretera.Vehiculos.First(v => v.Id == vehiculoActualizado.Id);
                    vehiculoEnCarretera.Posicion = vehiculoActualizado.Posicion;
                    vehiculoEnCarretera.ViajeCompletado = vehiculoActualizado.ViajeCompletado;
                }

                Console.WriteLine($"📍 Vehículo #{vehiculo.Id} → km {vehiculoActualizado.Posicion}");

                // Broadcast (ETAPA 3)
                if (DateTime.Now.Second % 2 == 0) // Cada 2 segundos aprox.
                {
                    BroadcastEstado();
                }

                if (vehiculoActualizado.ViajeCompletado) break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error con cliente #{cliente.Id}: {ex.Message}");
        }
        finally
        {
            _clientes.TryRemove(cliente.Id, out _);
            Console.WriteLine($"🚪 Vehículo #{cliente.Id} finalizó recorrido");
        }
    }

    static void BroadcastEstado()
    {
        lock (_lock)
        {
            foreach (var c in _clientes.Values.Where(c => c.TcpClient.Connected))
            {
                try
                {
                    NetworkStreamClass.EscribirDatosCarretera(c.Stream, _carretera);
                }
                catch
                {
                    _clientes.TryRemove(c.Id, out _);
                }
            }
        }
    }
}