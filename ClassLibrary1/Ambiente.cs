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
        private Random random = new Random();

        public Ambiente(string nombre, Protagonista protagonista, Wumpus wumpus) 
        {
            this.nombre = nombre;
            this.Dimensiones = new TipoElemento[5,5];
            this.protagonista = protagonista;
            this.wumpus = wumpus;
            // Habrán 3 objetos pozo
            this.Pozos = new Pozo[3];
            // Habrá un solo oro
            this.PosicionOro = new int[2];
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
        public void IniciarJuego()
        {
            // Instanciará a las demás clases
            // Se llamará al metodo generar pozos y se colocarán en la matriz Dimensiones
            // Lo mismo para el Wumpus y el oro
            // El protagonista partirá en la posición 0,0
        }

        // Los botones de movimiento no interactuarán directamente con el metodo
        public void SimularTurno(int x, int y)
        {
            // Se llamará al metodo Moverse() del protagonista
            // Se tendrá cuidado de que no se salga del tablero
            // Si el protagonista encuentra el oro y se devuelve a la
            // posición 0,0 el juego finalizará
            if (x == 0) // Se asume que se presiono a arriba o abajo
            {
                // Si con movimiento se sale del tablero no se realizará el movimiento
                if (protagonista.y + y > Dimensiones.GetLength(1) || protagonista.y - y < Dimensiones.GetLength(1))
                {
                    Console.WriteLine("Movimiento invalido");
                } else
                {
                    
                }
            } else // Se asume lo contrario
            {
                if (protagonista.x + x > Dimensiones.GetLength(0) || protagonista.x - x < Dimensiones.GetLength(0))
                {
                    Console.WriteLine("Movimiento invalido");
                }
                else
                {

                }
            }

        }
        
    }
}
