namespace turborent;
using System.Text.Json;

public class AccesoADatosAlquilerJson
{
    public List<Alquiler> CargarAlquileres(string archivo)
    {
        if (!File.Exists(archivo))
        {
            return new List<Alquiler>();
        }
        string linea = File.ReadAllText(archivo);
        List<Alquiler> alquileres = JsonSerializer.Deserialize<List<Alquiler>>(linea);
        return alquileres ?? new List<Alquiler>();
    }

    public void GuardarAlquileres(string archivo, List<Alquiler> alquileres)
    {
        var opciones = new JsonSerializerOptions {WriteIndented = true};
        string json = JsonSerializer.Serialize(alquileres, opciones);
        File.WriteAllText(archivo, json);
    }
}