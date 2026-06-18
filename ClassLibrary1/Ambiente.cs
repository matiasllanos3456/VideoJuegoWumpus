using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using static System.Net.Mime.MediaTypeNames;
namespace ClassLibrary1
{
    // Elementos a colocar en la matriz dimensiones
    // Aunque aparecen nombre por debajo actúan como enteros
    // ya que es una enumeración. Vacío -> 0; Jugador -> 1; etc.
    public enum TipoElemento { Vacio, Jugador, Wumpus, Pozo, Oro }
    public class Ambiente
    {
        // Se manejara un tamaño fijo de 4 x 4
        public string nombre { get; private set; }
        // Se necesitará acceder a algunas propiedades del protagonista
        // desde fuera de la clase
        public Protagonista protagonista { get; private set; }
        public Wumpus wumpus { get; private set; }
        private TipoElemento[,] Dimensiones;
        public Pozo[] Pozos { get; private set; }
        public int[] posicionOro { get; private set; }
        // Se leera desde la interfaz pero solo se modificará desde la misma clase
        public bool Finalizado {  get; private set; }
        private Random random = new Random();

        public Ambiente(string nombre) 
        {
            this.nombre = nombre;
            Dimensiones = new TipoElemento[4,4];
            protagonista = new Protagonista(0,0);
            wumpus = new Wumpus(0,0);
            // Habrán 3 objetos pozo
            Pozos = new Pozo[2];
            // La posicion del oro quedara guardada en un array de longitud 2
            posicionOro = new int[2];
            // Habrá un solo oro
            Finalizado = false;
        }
        // Metodo para obtener un elemento dentro de las dimensiones
        public TipoElemento ObtenerElemento(int x, int y)
        {
            return Dimensiones[x,y];
        }
        // Si se carga una nueva partida se debe cambiar de lugar al wumpus, los pozos y el oro
        // a la posición que quedó guardada
        public void AsignarElemento(int x, int y, TipoElemento elemento)
        {
            Dimensiones[x, y] = elemento;
        }
        // Se manejará un juego por turnos
        public void GenerarMapa()
        {
            // 1. Limpiar el mapa completo (poner todo en Vacío)
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    Dimensiones[x, y] = TipoElemento.Vacio;
                }
            }
            // Casilla de aparición del jugador
            Dimensiones[0, 0] = TipoElemento.Jugador;

            // Casilla aleatoria del Wumpus
            (int wumpusX, int wumpusY) = ObtenerCasillaVaciaAleatoria();
            Dimensiones[wumpusX, wumpusY] = TipoElemento.Wumpus;
            // Mover al wumpus a su posicion
            wumpus.x = wumpusX;
            wumpus.y = wumpusY;

            // Casilla del oro
            (int oroX, int oroY) = ObtenerCasillaVaciaAleatoria();
            Dimensiones[oroX, oroY] = TipoElemento.Oro;
            posicionOro[0] = oroX;
            posicionOro[1] = oroY;

            // Casillas para los 2 pozos
            for(int i = 0; i < 2; i++)
            {
                (int pozoX, int pozoY) = ObtenerCasillaVaciaAleatoria();
                Dimensiones[pozoX, pozoY] = TipoElemento.Pozo;
                // Se guardan en el array por si se necesitan despues
                this.Pozos[i] = new Pozo(pozoX, pozoY);

            }
        }
        // Será privada ya que solo la utilizará el método de GenerarMapa()
        private (int x, int y) ObtenerCasillaVaciaAleatoria()
        { // Se retornará una posicion vacía aleatoria
            int x, y;
            do
            {
                // Como la matriz no es muy grande no deberia 
                // generar problemas de rendimiento
                x = random.Next(0, 4); // Genera de 0 a 3
                y = random.Next(0, 4);
            }
            // Se repite si la casilla NO está vacía O si es la casilla inicial (0,0)
            while (Dimensiones[x, y] != TipoElemento.Vacio || (x == 0 && y == 0));

            return (x, y);
        }

        // Los botones de movimiento no interactuarán directamente con el metodo
        public string SimularTurno(int x, int y)
        {
            // Se llamará al metodo Moverse() del protagonista
            // Se tendrá cuidado de que no se salga del tablero
            // Si el protagonista encuentra el oro y se devuelve a la
            // posición 0,0 el juego finalizará

            // Si con movimiento se sale del tablero no se realizará el movimiento
            int nuevaX = protagonista.x + x;
            int nuevaY = protagonista.y + y;

            // Validamos si ese destino está fuera de los límites (0 a 3)
            if (nuevaX < 0 || nuevaX >= Dimensiones.GetLength(0) ||
                nuevaY < 0 || nuevaY >= Dimensiones.GetLength(1))
            {
                return "Movimiento invalido";
            }
            protagonista.Moverse(x, y);
            // Verificar en que casilla a caído y si finaliza o no el juego

            if (protagonista.x == wumpus.x && protagonista.y == wumpus.y){
                // El protagonista muere
                protagonista.Estado = false;
                return "Haz sido deborado por el wumpus";
            }
            if (Dimensiones[protagonista.x, protagonista.y] == TipoElemento.Pozo)
            {
                // El protagonista muere
                protagonista.Estado = false;
                return "El protagonista ha vaído en un pozo";
            }
            if (Dimensiones[protagonista.x, protagonista.y] == TipoElemento.Oro)
            {
                protagonista.EncontroOro = true;
                return "Haz encontrado el oro, escapa rapido!!";
            }
            if(protagonista.x == 0 && protagonista.y == 0 && protagonista.EncontroOro == true)
            {
                // El juego se termina con la victoria del protagonista
                Finalizado = true;
                return "Haz conseguido escapar con el oro. Felicitaciones!!!";
            }
            // Colocar las advertencias para casillas adyacentes
            // No se retornanar de inmediato por si hay mas de un mensaje
            // Ejemplo: arriba del jugador hay un wumpus y a la derecha un pozo
            // se tendrían que mostrar 2 mensajes
            string mensaje = "";
            // Se revisan las casillas adyacentes en busqueda de algun peligro o signos de oro cercanos
            if (RevisarCasillasAdyacentes(protagonista.x, protagonista.y, TipoElemento.Wumpus))
            {
                mensaje += wumpus.EmitirRuido() + "\n";
            }
            if (RevisarCasillasAdyacentes(protagonista.x, protagonista.y, TipoElemento.Pozo))
            {
                if (Pozos[0] != null)
                {
                    mensaje += Pozos[0].EmitirViento() + "\n";
                }
            }
            if (RevisarCasillasAdyacentes(protagonista.x, protagonista.y, TipoElemento.Oro))
            {
                mensaje += "Hay algo brillante cerca\n....";
            }
            // Actualizar el mapa
            // La nueva casilla pasa a estar ocupada por el jugador
            Dimensiones[protagonista.x, protagonista.y] = TipoElemento.Jugador;
            // mientras que la casilla anterior pasa a estar vacía
            Dimensiones[protagonista.x - x, protagonista.y - y] = TipoElemento.Vacio;
            return mensaje +"Casilla segura\n " + protagonista.MostrarDatos();
        }
        private bool RevisarCasillasAdyacentes(int x, int y, TipoElemento elementoBuscado)
        {
            // Definimos las 4 direcciones a revisar: Derecha, Izquierda, Abajo, Arriba
            int[] desX = { 1, -1, 0, 0 };
            int[] desY = { 0, 0, 1, -1 };

            for (int i = 0; i < 4; i++)
            {
                int vecinoX = x + desX[i];
                int vecinoY = y + desY[i];

                // Validamos si la casilla adyacente existe en la matriz
                // para que no salten excepciones de indice fuera de rango
                if (vecinoX >= 0 && vecinoX < Dimensiones.GetLength(0) &&
                    vecinoY >= 0 && vecinoY < Dimensiones.GetLength(1))
                {
                    // Si existe, evisamos qué hay adentro sin peligro de excepción
                    if (Dimensiones[vecinoX, vecinoY] == elementoBuscado)
                    {
                        return true; // Encontró el elemento alrededor
                    }
                }
            }

            return false;
        }
        // Al cargar la partida tambien se deben mover los objetos y el oro a su antigua posicion
        // ya que solo se actualizan las casillas de la matriz, mas no los objetos
        public void SincronizarElementosDespuesDeCargar()
        {
            int indicePozo = 0;
            if (this.wumpus == null)
            {
                this.wumpus = new Wumpus(4,4);
            }

            // Si el arreglo de Pozos llegó como null, se instanciará denuevo
            if (this.Pozos == null)
            {
                this.Pozos = new Pozo[2];
            }

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    // Si encontramos al Wumpus en la matriz, le actualizamos su posición interna
                    if (Dimensiones[i, j] == TipoElemento.Wumpus && this.wumpus != null)
                    {
                        this.wumpus.x = i;
                        this.wumpus.y = j;
                    }
                    // Si encontramos un Pozo, se lo asignamos a nuestro arreglo de Pozos en orden
                    else if (Dimensiones[i, j] == TipoElemento.Pozo && this.Pozos != null && indicePozo < this.Pozos.Length)
                    {
                        if (this.Pozos[indicePozo] != null)
                        {
                            this.Pozos[indicePozo].x = i;
                            this.Pozos[indicePozo].y = j;
                        }
                        indicePozo++;
                    }
                    // Si encontramos la casilla del Oro en la matriz, actualizamos su posicion
                    else if (Dimensiones[i, j] == TipoElemento.Oro && this.posicionOro != null)
                    {
                        // Si el protagonista ya encontró el oro, este no se coloca denuevo
                        if (!protagonista.EncontroOro)
                        {
                            this.posicionOro[0] = i;
                            this.posicionOro[1] = j;
                        } else
                        {
                            // Si ya lo encontró la casilla que antes contenia al oro queda vacía
                            Dimensiones[i, j] = TipoElemento.Vacio;
                        }
                    }
                }
            }
        }
    }
}
