using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary1
{
    public class Pozo : Personaje
    {
        public Pozo(int x, int y) : base(x, y)
        {
        }
        public void EmitirViento()
        {
            Console.WriteLine("* Se siente una extraña briza");
        }
    }
}
