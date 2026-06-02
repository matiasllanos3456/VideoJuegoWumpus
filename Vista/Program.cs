using ClassLibrary1;

internal class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        Ambiente a = new Ambiente("Pradera", 100, 200, new Soldado(1,1,100,"Pedro","katana"), new Villano(20,20,150,"Juan","Magia negra"));
    }
}