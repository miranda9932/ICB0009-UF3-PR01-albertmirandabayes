using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

class Program
{
    // Lista de clientes conectados (compartida entre hilos)
    static List<Cliente> clientesConectados = new List<Cliente>();
    static int nextId = 1; // Contador para IDs únicos
    static object lockObject = new object(); // Para thread-safety

    static void Main()
    {
        TcpListener server = new TcpListener(IPAddress.Any, 8080);
        server.Start();
        Console.WriteLine("Servidor iniciado en el puerto 8080...");

        while (true)
        {
            TcpClient tcpClient = server.AcceptTcpClient();
            Console.WriteLine("Nuevo cliente conectado.");

            // Asignar ID y dirección (norte/sur) al cliente
            int id;
            string direccion = new Random().Next(0, 2) == 0 ? "Norte" : "Sur";

            lock (lockObject) // Bloqueo para evitar condiciones de carrera
            {
                id = nextId++;
            }

            // Crear objeto Cliente y añadirlo a la lista
            Cliente cliente = new Cliente(id, direccion, tcpClient);
            clientesConectados.Add(cliente);

            // Iniciar hilo para gestionar el cliente
            Thread clientThread = new Thread(() => HandleClient(cliente));
            clientThread.Start();
        }
    }

   static void HandleClient(Cliente cliente)
{
    try
    {
        NetworkStream stream = cliente.TcpClient.GetStream();
        Console.WriteLine($"Vehículo #{cliente.Id} conectado (Dirección: {cliente.Direccion})");

        // Enviar ID y dirección al cliente (formato: "ID:Dirección")
        string mensaje = $"{cliente.Id}:{cliente.Direccion}";
        byte[] buffer = System.Text.Encoding.ASCII.GetBytes(mensaje);
        stream.Write(buffer, 0, buffer.Length);

        // Mantener conexión abierta (para pruebas)
        while (cliente.TcpClient.Connected)
        {
            Thread.Sleep(1000);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error con vehículo #{cliente.Id}: {ex.Message}");
    }
    finally
    {
        clientesConectados.Remove(cliente);
        Console.WriteLine($"Vehículo #{cliente.Id} desconectado.");
    }
}

}

// Clase para almacenar información del cliente 
class Cliente
{
    public int Id { get; }
    public string Direccion { get; }
    public TcpClient TcpClient { get; }

    public Cliente(int id, string direccion, TcpClient tcpClient)
    {
        Id = id;
        Direccion = direccion;
        TcpClient = tcpClient;
    }
}