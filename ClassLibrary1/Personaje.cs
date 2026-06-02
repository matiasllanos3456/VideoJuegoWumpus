namespace ClassLibrary1
{
    public class Personaje
    {
        public int x, y;
        public string nombre;

        public Personaje(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public void MostrarDatos()
        {
            Console.WriteLine("Posicion: \nx: "+x+"\ny: "+y);
        }
    }
}
