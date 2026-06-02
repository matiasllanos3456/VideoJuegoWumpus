namespace WinFormsApp1
{
    public partial class Interfaz1 : Form
    {
        // El juego consiste en adivinar los pares de figuras
        // Al seleccionar uno se muestra un simbolo,
        // si al seleccionar otro y este tiene el mismo simbolo
        // se mantienen volteados, de lo contra
        // Aqui van los atributos
        Random random = new Random();

        List<string> icons = new List<string>()
        {
            "!", "!", "N", "N", ",", ",", "k", "k",
            "b", "b", "v", "v", "w", "w", "z", "z"
        };

        // Se guardarán 2 referencias de los labels seleccionados
        // hay que verificar que estos 2 sean iguales,
        // de lo contrario deben volver a su forma inicial
        Label firstClicked = null;
        Label secondClicked = null;

        public Interfaz1()
        {
            InitializeComponent();

            // Se asignan los iconos a los label apenas cargue el formulario
            AssignIconsToSquares();
        }
        // Asignar un icono aleatorio a cada celda
        private void AssignIconsToSquares()
        {
            // The TableLayoutPanel has 16 labels,
            // and the icon list has 16 icons,
            // so an icon is pulled at random from the list
            // and added to each label
            foreach (Control control in tableLayoutPanel1.Controls)
            {
                Label iconLabel = control as Label;
                if (iconLabel != null)
                {
                    // Se obtiene un índice de la lista
                    int randomNumber = random.Next(icons.Count);
                    // Se asigna el elemento al label
                    iconLabel.Text = icons[randomNumber];
                    iconLabel.ForeColor = iconLabel.BackColor;
                    // Por ultimo se borra el icono de la lista para que no salga denuevo
                    icons.RemoveAt(randomNumber);
                }
            }
        }
        // Asignarle la funcionalidad a cada label
        private void label1_Click(object sender, EventArgs e)
        {
            // Comprobar si el temporizador está funcionando
            // he ignorará los demas click para voltear ambos labels
            if (timer1.Enabled == true)
            {
                return;
            }
         
            Label clickedLabel = sender as Label;

            if (clickedLabel != null)
            {
                // If the clicked label is black, the player clicked
                // an icon that's already been revealed --
                // ignore the click
                if (clickedLabel.ForeColor == Color.Black)
                    return;

                if (firstClicked == null)
                {
                    firstClicked = clickedLabel;
                    firstClicked.ForeColor = Color.Black;
                    return;
                }
                // Una vez que el jugador eligió un label, al elegir
                // un segundo label este se asignará automaticamente a la variable
                // de secondClicked he iniciará el contador
                secondClicked = clickedLabel;
                secondClicked.ForeColor = Color.Black;

                timer1.Start();
            }
        }
        // Funcionalidad del temporizador
        // al seleccionar 2 
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();

            if (firstClicked.Text == secondClicked.Text)
            {
                firstClicked = null;
                secondClicked = null;
                return;
            }
            // Hide both icons
            firstClicked.ForeColor = firstClicked.BackColor;
            secondClicked.ForeColor = secondClicked.BackColor;

            // Reset firstClicked and secondClicked 
            // so the next time a label is
            // clicked, the program knows it's the first click
            firstClicked = null;
            secondClicked = null;
        }
    }
}
