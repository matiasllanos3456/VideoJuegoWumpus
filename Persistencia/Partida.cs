using ClassLibrary1;
using System.Text.Json;
using System.Collections.Generic;
namespace Persistencia
{
    // La siguiente clase tendrá dos funcionalidades
    // 1) Guardar (serializar) el estado actual de la partida
    // La interfaz contará con un boton "Guardar" que hará que el
    // objeto ambiente llame al metodo estatico "GuardarPartida" pasandole sus datos
    // 2) Cargar (deserializar) la ultima partida guardada
    // Al presionar el boton "Cargar" se modificará el estado actual del ambiente
    // reemplazando sus datos con los guardados en formato json

    // Cada acción mostrará un mensaje en pantalla
    // Los metodos deben manejar correctamente las posibles excepciones que surjan
    public class Partida
    {
        public Partida () { }
        // Se define una clase interna que contendrá los datos a serializar
        private class DatosGuardados
        {
            public Ambiente ambienteAGuardar {  get; set; }
            public List<TipoElemento> MapaAGuardar {  get; set; }
            public bool OroRecolectado { get; set; }
        }

        public static bool GuardarPartida(Ambiente ambiente) 
        { 
            try
            {
                if(ambiente == null)
                {
                    return false;
                }
                // Se convertirá la matriz de 4 x 4 a una lista de 16 elementos
                // para poder alamcenarla mas facilmente en un .json
                List<TipoElemento> plano = new List<TipoElemento>();
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        TipoElemento elementoActual = ambiente.ObtenerElemento(i, j);
                        plano.Add(elementoActual);
                        
                    }
                }
                DatosGuardados datos = new DatosGuardados
                {
                    ambienteAGuardar = ambiente,
                    MapaAGuardar = plano,
                    OroRecolectado = ambiente.protagonista.EncontroOro
                };

                var opciones = new JsonSerializerOptions { WriteIndented = true };
                // Se guardará un objeto "DatosGuardados"
                String jsonText = JsonSerializer.Serialize(datos, opciones);
                File.WriteAllText("Registro.json", jsonText);
                return true;
            } catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar: {ex.Message}");
                return false;
            }
        }

        public static Ambiente CargarPartida()
        {
            try
            {
                if(!File.Exists("Registro.json"))
                {
                    return null;
                }
                string jsonText = File.ReadAllText("Registro.json");
                DatosGuardados datos = JsonSerializer.Deserialize<DatosGuardados>(jsonText);
                if(datos == null || datos.ambienteAGuardar == null)
                {
                    return null;
                }
                // datos = {
                //      Ambiente ambienteAGuardar,
                //      List<TipoElemento> MapaAGuardar
                // }
                Ambiente juegoCargado = datos.ambienteAGuardar;
                // Restauración del mapa guardado
                int contador = 0;
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        // Necesitaremos un método público en Ambiente para restaurar la casilla,
                        // por ejemplo: juegoCargado.AsignarElemento(i, j, datos.MapaAplanado[contador]);
                        juegoCargado.AsignarElemento(i, j, datos.MapaAGuardar[contador]);
                        contador++;
                    }
                }
                // Actualizamos la posicion de los objetos y el oro antes de retornarlo
                juegoCargado.SincronizarElementosDespuesDeCargar();
                if (datos.OroRecolectado)
                {
                    juegoCargado.protagonista.EncontroOro = true;
                }
                // Se retorna el nuevo ambiente
                return juegoCargado;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar la partida: {ex.Message}");
                return null;
            }
        }

    }
}
