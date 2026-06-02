using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary1
{
    public class Protagonista : Personaje
    {
        public bool EncontroOro { get; set; }
        // Si está vivo el estado será true, de lo contrario false
        public bool Estado { get; set; }
        public Protagonista(int x, int y) : base(x,y)
        {
            // El jugador parte en la casilla 0,0
            this.x = 0;
            this.y = 0;
            EncontroOro = false;
            Estado = true;
        }
        public void Moverse(int x, int y)
        {
            if (x == 0)
            {
                this.y += y;
            }
            else
            {
                this.x += x;
            }
            MostrarDatos();
        }

    }
}
