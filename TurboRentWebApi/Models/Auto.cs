namespace turborent;

public class Auto : Vehiculo
{
    public int CantidadPuertas {get; set;}
    public bool EsAutomatico {get; set;}

    public override decimal CalcularCosto(int dias) 
    {
        return dias * TarifaDiaria;
    }
}