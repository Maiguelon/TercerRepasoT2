# 🚗 Sistema de Alquiler de Vehículos "TurboRent"

## Contexto
La empresa "TurboRent" desea modernizar su sistema. Actualmente manejan dos registros por separado: uno para su **Flota de Vehículos** y otro para los **Alquileres** realizados.
El objetivo es desarrollar una API que permita administrar los vehículos (Autos y Camionetas) y registrar nuevos alquileres validando la disponibilidad y reglas de negocio específicas de cada tipo de vehículo.

**Diferencia Clave:**
A diferencia de sistemas anteriores, los Alquileres **NO** se guardan dentro del Vehículo. Se guardan en un archivo separado (`alquileres.json`) y se relacionan mediante el `VehiculoId`.

---

## 🛠 Modelos Sugeridos

### 1. La Flota (Archivo `vehiculos.json`)
Debe aplicar **Polimorfismo** para distinguir entre Autos y Camionetas.

* **Vehiculo (Clase Base - Abstracta):**
  - Propiedades: `Id`, `Marca`, `Modelo`, `TarifaDiaria` (decimal).
  - **Método Virtual:** `public virtual decimal CalcularCosto(int dias)`
    - *Comportamiento por defecto:* Retorna `dias * TarifaDiaria`.

* **Auto (Derivada):**
  - Propiedades: `CantidadPuertas` (int), `EsAutomatico` (bool).
  - *Comportamiento:* Usa el cálculo de costo base.

* **Camioneta (Derivada):**
  - Propiedades: `CapacidadCargaKg` (double).
  - *Override:* Sobrescribe `CalcularCosto`. Si la carga es mayor a 500kg, se cobra un **20% extra** sobre el total.

### 2. Los Alquileres (Archivo `alquileres.json`)
* **Alquiler:**
  - Propiedades: `Id`, `VehiculoId` (int), `Cliente` (string), `FechaRetiro` (DateTime), `FechaDevolucion` (DateTime).
  - *Nota:* No guarda el objeto vehículo, solo su ID.

---

## 📡 Interfaz de la API (Endpoints)

| Método | Endpoint | Descripción |
| :--- | :--- | :--- |
| **GET** | `/api/vehiculos` | Lista todos los vehículos de la flota. |
| **POST** | `/api/vehiculos` | Da de alta un vehículo (Auto o Camioneta). |
| **POST** | `/api/alquileres` | Crea un nuevo alquiler (Valida disponibilidad en `alquileres.json`). |
| **GET** | `/api/alquileres/vehiculo/{id}` | Lista el historial de alquileres de un vehículo específico. |
| **GET** | `/api/vehiculos/disponibles` | Recibe una fecha y devuelve los vehículos que **no** están alquilados ese día. |
| **GET** | `/api/alquileres/cotizar` | Recibe `VehiculoId` y `Dias`. Devuelve el costo estimado usando el método polimórfico `CalcularCosto`. |

---

## 📋 Validaciones y Reglas de Negocio

### 1. Validaciones de Fechas (DateTime)
* **Coherencia:** `FechaDevolucion` debe ser posterior a `FechaRetiro`.
* **Futuro:** No se permiten alquileres en el pasado (validar contra `DateTime.Now` o `Today`).
* **Solapamiento (Cross-File Check):**
  - Al crear un alquiler para el Vehículo `X`, se debe leer `alquileres.json`, filtrar los alquileres que tengan `VehiculoId == X` y verificar que las fechas no choquen con la nueva solicitud.

### 2. Reglas de Negocio (Polimórficas)
* **Autos:** Si `EsAutomatico` es `true`, el alquiler **no puede superar los 7 días**.
* **Camionetas:** Si `CapacidadCargaKg` > 1000, el alquiler debe ser de **mínimo 3 días**.

---

## 💾 Requerimientos Técnicos

* **Persistencia Relacional:**
  - `vehiculos.json`: Guarda la lista polimórfica de vehículos (`$type`).
  - `alquileres.json`: Guarda la lista plana de alquileres.
* **Manejo de Archivos:** Necesitarás una clase de Acceso a Datos capaz de leer/escribir dos archivos distintos (o dos clases separadas).
* **DateTime:** Uso obligatorio de `DateTime` para cálculos de días (`TimeSpan`) y validaciones.