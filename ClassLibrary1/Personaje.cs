using System.Text.Json.Serialization;

namespace ClassLibrary1
{
    public class Personaje
    {
        public int x { get; set; }
        public int y { get; set; }

        public Personaje(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        [JsonConstructor]
        public Personaje()
        {
        }
    }
}
