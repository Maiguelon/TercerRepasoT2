namespace turborent;
using System.Text.Json;

public class AccesoADatosVehiculoJson
{
    public List<Vehiculo> CargarVehiculos(string archivo)
    {
        if (!File.Exists(archivo))
        {
            return new List<Vehiculo>();
        }
        string linea = File.ReadAllText(archivo);
        List<Vehiculo> vehiculos = JsonSerializer.Deserialize<List<Vehiculo>>(linea);
        return vehiculos ?? new List<Vehiculo>();
    }

    public void GuardarVehiculos(string archivo, List<Vehiculo> vehiculos)
    {
        var opciones = new JsonSerializerOptions {WriteIndented = true};
        string json = JsonSerializer.Serialize(vehiculos, opciones);
        File.WriteAllText(archivo, json);
    }
}