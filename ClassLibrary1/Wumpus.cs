using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary1
{
    public class Wumpus : Personaje
    {
        public Wumpus(int x, int y) : base(x, y)
        {
        }

        public string EmitirRuido()
        {
            return "* Se oye un gruñido cerca....";
        }
    }
}
