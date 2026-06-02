using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary1
{
    public class Protagonista : Personaje
    {
        protected bool EncontroOro { get; set; }
        public Protagonista(int x, int y) : base(x,y)
        {
            // El jugador parte en la casilla 0,0
            this.x = 0;
            this.y = 0;
            EncontroOro = false;
        }
        public virtual void Moverse(int x, int y)
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
