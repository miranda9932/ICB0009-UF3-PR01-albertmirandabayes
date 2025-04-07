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
        Console.WriteLine("🟢 Servidor iniciado");

        while (true)
        {
            try
            {
                var client = server.AcceptTcpClient();
                var id = GenerarId();
                var cliente = new Cliente(id, client);
                _clientes.TryAdd(id, cliente);
                Console.WriteLine($"🚗 Nuevo cliente conectado - ID: {id}");

                new Thread(() => HandleClient(cliente)).Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error aceptando cliente: {ex.Message}");
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

            // --- Registro inicial ---
            Vehiculo vehiculo = NetworkStreamClass.LeerDatosVehiculo(cliente.Stream);
            lock (_lock)
            {
                _carretera.Vehiculos.Add(vehiculo);
            }
            Console.WriteLine($"📝 Vehículo #{vehiculo.Id} registrado - Dirección: {direccion}");

            // --- Bucle principal ---
            while (cliente.TcpClient.Connected)
            {
                var vehiculoActualizado = NetworkStreamClass.LeerDatosVehiculo(cliente.Stream);
                
                // Actualización segura del estado
                lock (_lock)
                {
                    var vehiculoEnCarretera = _carretera.Vehiculos.First(v => v.Id == vehiculoActualizado.Id);
                    vehiculoEnCarretera.Posicion = vehiculoActualizado.Posicion;
                    vehiculoEnCarretera.ViajeCompletado = vehiculoActualizado.ViajeCompletado;
                    Console.WriteLine($"📡 Actualización - Vehículo #{vehiculo.Id} → km {vehiculoActualizado.Posicion}");
                }

                // Broadcast inmediato
                BroadcastEstado();

                if (vehiculoActualizado.ViajeCompletado) break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Error con cliente #{cliente.Id}: {ex.Message}");
        }
        finally
        {
            RemoverCliente(cliente.Id);
        }
    }

    static void BroadcastEstado()
    {
        Carretera copiaSegura;
        lock (_lock)
        {
            copiaSegura = new Carretera
            {
                Vehiculos = _carretera.Vehiculos.Select(v => new Vehiculo 
                {
                    Id = v.Id,
                    Posicion = v.Posicion,
                    Direccion = v.Direccion,
                    ViajeCompletado = v.ViajeCompletado
                }).ToList()
            };
        }

        foreach (var cliente in _clientes.Values.ToList())
        {
            try
            {
                if (cliente.TcpClient.Connected)
                {
                    NetworkStreamClass.EscribirDatosCarretera(cliente.Stream, copiaSegura);
                    Console.WriteLine($"📤 Estado enviado a cliente #{cliente.Id}");
                }
            }
            catch
            {
                RemoverCliente(cliente.Id);
            }
        }
    }

    static void RemoverCliente(int id)
    {
        if (_clientes.TryRemove(id, out var cliente))
        {
            try
            {
                cliente.TcpClient.Close();
                Console.WriteLine($"🚪 Cliente #{id} desconectado");
            }
            catch { /* Ignorar errores al cerrar */ }
        }
    }
}