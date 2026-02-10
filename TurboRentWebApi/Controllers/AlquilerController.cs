using turborent;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;
namespace TurboRentWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class AlquilerController : ControllerBase
{
    // Declaro las variables que usaré en los Endpoints
    private List<Alquiler> alquileres;
    private List<Vehiculo> vehiculos;
    private AccesoADatosAlquilerJson ADAlquiler;
    private AccesoADatosVehiculoJson ADVhiculo;
    string rutaAlquileres = Path.Combine("Data", "Alquileres.json");
    string rutaVehiculos = Path.Combine("Data", "Vehiculos.json");

    // Constructor
    public AlquilerController()
    {
        ADAlquiler = new AccesoADatosAlquilerJson();
        ADVhiculo = new AccesoADatosVehiculoJson();

        alquileres = ADAlquiler.CargarAlquileres(rutaAlquileres);
        vehiculos = ADVhiculo.CargarVehiculos(rutaVehiculos);
    }

    // ----- GET -----
    [HttpGet("GetAlquileres")]
    public ActionResult<List<Alquiler>> GetAlquileres()
    {
        if (!(alquileres == null))
        {
            return Ok(alquileres);
        }
        return NotFound("No se encontraron alquileres.");
    }

    [HttpGet("GetVehiculos")]
    public ActionResult<List<Vehiculo>> GetVehiculos()
    {
        if (!(vehiculos == null))
        {
            return Ok(vehiculos);
        }
        return NotFound("No se encontraron vehiculos.");
    }

    [HttpGet("GetAlquileresPorId/{idBuscado}")]
    public ActionResult<List<Alquiler>> GetAlquileresPorId(int idBuscado)
    {
        List<Alquiler> Buscados = alquileres.Where(a => // checkeo si no hay?
        a.VehiculoId == idBuscado).ToList();

        return Ok(Buscados);
    }

    [HttpGet("GetVehiculosDisponibles/{dia}")]
    public ActionResult<List<Vehiculo>> GetVehiculosDisponibles(DateTime dia)
    {
        List<Alquiler> Nocumplen = alquileres.Where(a =>
        a.isBetween(dia)).ToList();
        List<Vehiculo> disponibles = new List<Vehiculo>();
        foreach (var c in Nocumplen)
        {
            foreach (var v in vehiculos)
            {
                if (v.Id != c.VehiculoId)
                {
                    disponibles.Add(v);
                }
            }
        }
        return Ok(disponibles);   
    }

    [HttpGet("GetCosto/{idBuscado}/{dias}")]
    public ActionResult<decimal> GetCosto(int idBuscado, int dias)
    {
        // checkeos? como serían?
        decimal monto = 0;
        Vehiculo objetivo = vehiculos.FirstOrDefault(v =>
        v.Id == idBuscado);

        if (objetivo is Auto esAuto)
        {
            monto = esAuto.CalcularCosto(dias);
        }

        if (objetivo is Camioneta esCamioneta)
        {
            monto = esCamioneta.CalcularCosto(dias);
        }
        return Ok(monto);
    }

    // ---- POST -----
    [HttpPost("AgregarAuto")]
    public ActionResult<string> AgregarAuto([FromBody] Auto auto)
    {
        // Validaciones
        if (auto == null)
        {
            return BadRequest("No se recibieron los datos");
        }
        if ((auto.Marca == "") || (auto.Modelo == ""))
        {
            return BadRequest("Marca y Modelo son obligatorios para dar de alta un vehiculo");
        }

        // Id autoincremental
        int nuevoId = vehiculos.Count > 0 ? vehiculos.Max(v => v.Id) + 1 : 1;
        auto.Id = nuevoId;
        vehiculos.Add(auto);
        ADVhiculo.GuardarVehiculos(rutaVehiculos, vehiculos);
        return Created("", auto);
    }

    [HttpPost("AgregarCamioneta")]
    public ActionResult<string> AgregarCamioneta([FromBody] Camioneta camioneta)
    {
        // Validaciones
        if (camioneta == null)
        {
            return BadRequest("No se recibieron los datos");
        }
        if ((camioneta.Marca == "") || (camioneta.Modelo == ""))
        {
            return BadRequest("Marca y Modelo son obligatorios para dar de alta un vehiculo");
        }

        // Id autoincremental
        int nuevoId = vehiculos.Count > 0 ? vehiculos.Max(v => v.Id) + 1 : 1;
        camioneta.Id = nuevoId;
        vehiculos.Add(camioneta);
        ADVhiculo.GuardarVehiculos(rutaVehiculos, vehiculos);
        return Created("", camioneta);
    }

    [HttpPost("AgregarVehiculo")] // Vehiculo generico CONSULTAR
    public ActionResult<string> AgregarVehiculo([FromBody] Vehiculo nuevo)
    {
        // Validaciones
        if (nuevo == null)
        {
            return BadRequest("No se recibieron los datos");
        }
        if ((nuevo.Marca == "") || (nuevo.Modelo == ""))
        {
            return BadRequest("Marca y Modelo son obligatorios para dar de alta un vehiculo");
        }

        // Id autoincremental
        int nuevoId = vehiculos.Count > 0 ? vehiculos.Max(v => v.Id) + 1 : 1;
        nuevo.Id = nuevoId;
        // if (nuevo is Auto auto)
        // {
        //     vehiculos.Add(auto);
        //     ADVhiculo.GuardarVehiculos(rutaVehiculos, vehiculos);
        //     return Created("", auto);
        // }

        // if (nuevo is Camioneta camioneta)
        // {
        //     vehiculos.Add(camioneta);
        //     ADVhiculo.GuardarVehiculos(rutaVehiculos, vehiculos);
        //     return Created("", camioneta);
        // }
        vehiculos.Add(nuevo);
        ADVhiculo.GuardarVehiculos(rutaVehiculos, vehiculos);
        return Created("", nuevo);
    }

    [HttpPost("AgregarAlquiler")]
    public ActionResult<string> AgregarAlquiler([FromBody] Alquiler nuevo)
    {
        // validaciones
        if (nuevo == null)
        {
            return BadRequest("No se recibieron datos del alquiler");
        }
        if (nuevo.FechaRetiro > nuevo.FechaDevolucion)
        {
            return BadRequest("Incongruencia Temporal. No se puede devolver un auto que no se sacó");
        }
        DateTime hoy = DateTime.Today;
        if (nuevo.FechaRetiro < hoy)
        {
            return BadRequest("No se puede alquilar en el pasado");
        }

        // check solapamiento
        List<Alquiler> aRevisar = alquileres.Where(a => // alquileres del mismo vehiculo
        a.Id == nuevo.VehiculoId).ToList();
        bool haySolapamiento = aRevisar.Any(a => // logica
        nuevo.FechaRetiro < a.FechaDevolucion &&
        nuevo.FechaDevolucion > a.FechaRetiro);
        if (haySolapamiento)
        {
            return BadRequest("Hay solapamiento por ese vehículo en el tiempo elegido");
        }

        // validaciones de auto y camioneta especificamente
        Vehiculo noDefinido = vehiculos.FirstOrDefault(v => // declaro un padre a definir
        v.Id == nuevo.VehiculoId);

        if (noDefinido is Auto esAuto) // caso auto
        {
            if (esAuto.EsAutomatico == true &&
                (nuevo.TiempoAlquilado().Days > 7))
            {
                return BadRequest("Si es automatico, el alquiler no puede ser mayor a 7 dias");
            }
        }

        if (noDefinido is Camioneta esCamioneta) // caso camioneta
        {
            if (esCamioneta.CapacidadCargaKg > 1000 &&
                nuevo.TiempoAlquilado().Days < 3)
            {
                return BadRequest("El alquiler debe ser al menos de 3 dias para Camionetas con capacidad de más de 1000 kg.");
            }
        }

        // Id autoincremental
        nuevo.Id = alquileres.Count > 0 ? alquileres.Max(a => a.Id) + 1 : 1;

        // Añado el Alquiler
        alquileres.Add(nuevo);
        ADAlquiler.GuardarAlquileres(rutaAlquileres, alquileres);
        return Created("", nuevo);
    }
}