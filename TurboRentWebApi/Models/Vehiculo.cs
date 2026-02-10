namespace turborent;
using System.Text.Json.Serialization; 

// Esto permite guardar hijos dentro de una lista del padre
[JsonDerivedType(typeof(Auto), typeDiscriminator: "auto")]
[JsonDerivedType(typeof(Camioneta), typeDiscriminator: "camioneta")]

public abstract class Vehiculo
{
    public int Id {get; set;}
    public string Marca {get; set;}
    public string Modelo {get; set;}
    public decimal TarifaDiaria {get; set;}

    public virtual decimal CalcularCosto(int dias)
    {
        return (dias * TarifaDiaria);
    }
}