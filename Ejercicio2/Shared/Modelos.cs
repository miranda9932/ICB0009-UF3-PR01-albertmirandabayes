public class Vehiculo
{
    public int Id { get; set; }
    public int Posicion { get; set; }
    public string Direccion { get; set; } // "Norte" o "Sur"
}

public class Carretera
{
    public List<Vehiculo> Vehiculos { get; } = new();
}
