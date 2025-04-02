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
        
        // Paso 1: Esperar mensaje "INICIO" del cliente
        string inicioMsg = NetworkStreamClass.LeerMensajeNetworkStream(stream);
        if (inicioMsg != "INICIO")
        {
            throw new Exception("Handshake fallido: mensaje inicial incorrecto");
        }

        // Paso 2: Enviar ID y dirección
        string datosCliente = $"{cliente.Id}:{cliente.Direccion}";
        NetworkStreamClass.EscribirMensajeNetworkStream(stream, datosCliente);
        Console.WriteLine($"Enviado ID {cliente.Id} a vehículo {cliente.Direccion}");

        // Paso 3: Esperar confirmación del cliente
        string confirmacion = NetworkStreamClass.LeerMensajeNetworkStream(stream);
        if (confirmacion != cliente.Id.ToString())
        {
            throw new Exception($"Handshake fallido: confirmación incorrecta (esperado {cliente.Id}, recibido {confirmacion})");
        }

        Console.WriteLine($"Handshake completado con vehículo #{cliente.Id}");
        
        // Aquí iría el resto de la lógica de comunicación...
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error con vehículo #{cliente.Id}: {ex.Message}");
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