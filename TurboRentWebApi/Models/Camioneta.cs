namespace turborent;

public class Camioneta : Vehiculo
{
    public double CapacidadCargaKg {get; set;}

    public override decimal CalcularCosto(int dias)
    {
        decimal monto = base.CalcularCosto(dias);
        if (CapacidadCargaKg > 500)
        {
            monto *= 1.2m;
        } 
        return monto;
    }
}