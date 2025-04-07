public class Vehiculo
{
    public int Id { get; set; }
    public int Posicion { get; set; }
    public required string Direccion { get; set; } // "Norte" o "Sur"
    public bool ViajeCompletado { get; set; }
}

public class Carretera
{
    public List<Vehiculo> Vehiculos { get; set; } = new();
}
