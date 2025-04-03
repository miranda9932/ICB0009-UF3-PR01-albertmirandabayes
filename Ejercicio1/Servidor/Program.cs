using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Concurrent;
using System.IO;

class Program
{
    private static readonly ConcurrentDictionary<int, Cliente> _clientes = new();
    private static int _nextId = 1;
    private static readonly object _lock = new();

    class Cliente
    {
        public int Id { get; }
        public string Direccion { get; }
        public TcpClient TcpClient { get; }
        public DateTime ConexionTime { get; } = DateTime.Now;

        public Cliente(int id, string direccion, TcpClient client)
        {
            Id = id;
            Direccion = direccion;
            TcpClient = client;
        }
    }

    static void Main()
    {
        try
        {
            var server = new TcpListener(IPAddress.Any, 8080);
            server.Start();
            Console.WriteLine("🟢 Servidor iniciado en puerto 8080");

            while (true)
            {
                var client = server.AcceptTcpClient();
                var id = GenerateId();
                var direccion = new Random().Next(0, 2) == 0 ? "Norte" : "Sur";
                var cliente = new Cliente(id, direccion, client);

                _clientes.TryAdd(id, cliente);
                Console.WriteLine($"🚗 Vehículo #{id} ({direccion}) conectado. Total: {_clientes.Count}");

                new Thread(() => HandleClient(cliente)).Start();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"🔴 Error crítico: {ex.Message}");
        }
    }

    static int GenerateId()
    {
        lock (_lock) return _nextId++;
    }

    static void HandleClient(Cliente cliente)
    {
        try
        {
            using var stream = cliente.TcpClient.GetStream();
            var reader = new StreamReader(stream);
            var writer = new StreamWriter(stream) { AutoFlush = true };

            // Handshake
            writer.WriteLine("ID:" + cliente.Id);
            var ack = reader.ReadLine();
            if (ack != $"ACK:{cliente.Id}")
                throw new Exception("Handshake fallido");

            Console.WriteLine($"🔄 Vehículo #{cliente.Id} autenticado");

            // Bucle principal
            while (cliente.TcpClient.Connected)
            {
                var msg = reader.ReadLine();
                if (string.IsNullOrEmpty(msg)) continue;

                Console.WriteLine($"📩 #{cliente.Id}: {msg}");
                Broadcast($"POS:{cliente.Id}:{msg}", cliente.Id);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Error con #{cliente.Id}: {ex.Message}");
        }
        finally
        {
            RemoveClient(cliente.Id);
        }
    }

    static void Broadcast(string msg, int senderId)
    {
        foreach (var (id, client) in _clientes)
        {
            if (id == senderId) continue;

            try
            {
                var writer = new StreamWriter(client.TcpClient.GetStream()) { AutoFlush = true };
                writer.WriteLine(msg);
            }
            catch
            {
                RemoveClient(id);
            }
        }
    }

    static void RemoveClient(int id)
    {
        if (!_clientes.TryRemove(id, out var cliente)) return;

        try
        {
            cliente.TcpClient.Close();
            Console.WriteLine($"🚪 #{id} desconectado. Restantes: {_clientes.Count}");
        }
        catch { /* Ignorar errores al cerrar */ }
    }
}