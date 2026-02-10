namespace turborent;

public class Alquiler
{
    public int Id {get; set;}
    public int VehiculoId {get; set;}
    public string Cliente {get; set;}
    public DateTime FechaRetiro {get; set;}
    public DateTime FechaDevolucion {get; set;}

    public TimeSpan TiempoAlquilado()
    {
        return FechaDevolucion - FechaRetiro;
    }

    public bool isBetween(DateTime evaluada) // checkea si una fecha está dentro del alquiler
    {
        return (evaluada > FechaRetiro && evaluada < FechaDevolucion);
    }
}