// Ejercicio2/Servidor/Program.cs
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Collections.Concurrent;

class Program
{
    static ConcurrentDictionary<int, Cliente> _clientes = new();
    static Carretera _carretera = new();
    static int _nextId = 1;

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
                var id = _nextId++;
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

    static void HandleClient(Cliente cliente)
    {
        try
        {
            Console.WriteLine($"Procesando cliente #{cliente.Id}");
            
            // --- Handshake (Ejercicio 1) ---
            string mensajeInicio = NetworkStreamClass.LeerMensajeNetworkStream(cliente.Stream);
            Console.WriteLine($"Cliente #{cliente.Id} envió: {mensajeInicio}");
            
            string direccion = new Random().Next(0, 2) == 0 ? "Norte" : "Sur";
            string respuesta = $"{cliente.Id}:{direccion}";
            Console.WriteLine($"Enviando a cliente #{cliente.Id}: {respuesta}");
            NetworkStreamClass.EscribirMensajeNetworkStream(cliente.Stream, respuesta);
            
            string ack = NetworkStreamClass.LeerMensajeNetworkStream(cliente.Stream);
            Console.WriteLine($"Cliente #{cliente.Id} confirmó: {ack}");

            // --- Nuevo en Ejercicio 2 ---
            Console.WriteLine($"Esperando datos del vehículo del cliente #{cliente.Id}");
            Vehiculo vehiculo = NetworkStreamClass.LeerDatosVehiculo(cliente.Stream);
            Console.WriteLine($"Recibido vehículo #{vehiculo.Id} (Pos: {vehiculo.Posicion}km, Dir: {vehiculo.Direccion})");
            
            _carretera.Vehiculos.Add(vehiculo);
            Console.WriteLine($"Vehículo #{vehiculo.Id} añadido a la carretera");

            // Bucle principal (se implementará en Etapa 3)
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error detallado con cliente #{cliente.Id}:");
            Console.WriteLine($"Tipo de error: {ex.GetType().Name}");
            Console.WriteLine($"Mensaje: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            _clientes.TryRemove(cliente.Id, out _);
        }
    }
}