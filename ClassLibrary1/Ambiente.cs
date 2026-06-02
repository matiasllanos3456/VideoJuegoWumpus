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
        // Se manejara un tamaño fijo de 5 * 5
        protected string nombre;
        protected Protagonista protagonista; 
        protected Wumpus wumpus;
        protected TipoElemento[,] Dimensiones;
        protected int[] PosicionOro;
        protected Pozo[] Pozos;
        public bool Finalizado;
        private Random random = new Random();

        public Ambiente(string nombre) 
        {
            this.nombre = nombre;
            Dimensiones = new TipoElemento[5,5];
            protagonista = new Protagonista(0,0);
            wumpus = new Wumpus(0,0);
            // Habrán 3 objetos pozo
            Pozos = new Pozo[3];
            // Habrá un solo oro
            PosicionOro = new int[2];
            Finalizado = false;
        }
        // Se manejará un juego por turnos
        public void GenerarMapa()
        {
            // 1. Limpiar el mapa completo (poner todo en Vacío)
            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 5; y++)
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

            // Casillas para los 3 pozos
            for(int i = 0; i < 3; i++)
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
                x = random.Next(0, 5); // Genera de 0 a 4
                y = random.Next(0, 5);
            }
            // Se repite si la casilla NO está vacía O si es la casilla inicial (0,0)
            while (Dimensiones[x, y] != TipoElemento.Vacio || (x == 0 && y == 0));

            return (x, y);
        }

        // Los botones de movimiento no interactuarán directamente con el metodo
        public void SimularTurno(int x, int y)
        {
            // Se llamará al metodo Moverse() del protagonista
            // Se tendrá cuidado de que no se salga del tablero
            // Si el protagonista encuentra el oro y se devuelve a la
            // posición 0,0 el juego finalizará

            // Si con movimiento se sale del tablero no se realizará el movimiento
            if (protagonista.y + y > Dimensiones.GetLength(1) || protagonista.y - y < Dimensiones.GetLength(1) || protagonista.x + x > Dimensiones.GetLength(0) || protagonista.x - x < Dimensiones.GetLength(0))
            {
                Console.WriteLine("Movimiento invalido");
                return;
            }
            protagonista.Moverse(x, y);
            // Verificar en que casilla a caído y si finaliza o no el juego

            if (protagonista.x == wumpus.x && protagonista.y == wumpus.y){
                // El protagonista muere
                Console.WriteLine("Haz sido deborado por el wumpus");
                protagonista.Estado = false;
                return;
            }
            if (Dimensiones[protagonista.x, protagonista.y] == TipoElemento.Pozo)
            {
                Console.WriteLine("El protagonista ha vaído en un pozo");
                protagonista.Estado = false;
                return;
            }
            if (Dimensiones[protagonista.x, protagonista.y] == TipoElemento.Oro)
            {
                Console.WriteLine("Haz encontrado el oro");
                protagonista.EncontroOro = true;
                return;
            }
            if(protagonista.x == 0 && protagonista.y == 0 && protagonista.EncontroOro == true)
            {
                Console.WriteLine("Haz conseguido escapar con el oro. Felicitaciones!!!");
                // El juego se termina con la victoria del protagonista
                Finalizado = true;
                return;
            }
            // Colocar las advertencias para casillas adyacentes

            if (Dimensiones[protagonista.x + 1, protagonista.y] == TipoElemento.Wumpus
                || Dimensiones[protagonista.x - 1, protagonista.y] == TipoElemento.Wumpus
                || Dimensiones[protagonista.x, protagonista.y + 1] == TipoElemento.Wumpus
                || Dimensiones[protagonista.x, protagonista.y - 1] == TipoElemento.Wumpus)
            {
                wumpus.EmitirRuido();
            }
            if (Dimensiones[protagonista.x + 1, protagonista.y] == TipoElemento.Pozo
                || Dimensiones[protagonista.x - 1, protagonista.y] == TipoElemento.Pozo
                || Dimensiones[protagonista.x, protagonista.y + 1] == TipoElemento.Pozo
                || Dimensiones[protagonista.x, protagonista.y - 1] == TipoElemento.Pozo)
            {
                Pozos[0].EmitirViento();
            }
            if (Dimensiones[protagonista.x + 1, protagonista.y] == TipoElemento.Oro
                || Dimensiones[protagonista.x - 1, protagonista.y] == TipoElemento.Oro
                || Dimensiones[protagonista.x, protagonista.y + 1] == TipoElemento.Oro
                || Dimensiones[protagonista.x, protagonista.y - 1] == TipoElemento.Oro)
            {
                Console.WriteLine("Hay algo brillante cerca");
            }
            // Actualizar el mapa
            // La nueva casilla pasa a estar ocupada por el jugador
            Dimensiones[protagonista.x, protagonista.y] = TipoElemento.Jugador;
            // mientras que la casilla anterior pasa a estar vacía
            Dimensiones[protagonista.x - x, protagonista.y - y] = TipoElemento.Vacio;
        }

    }
}
