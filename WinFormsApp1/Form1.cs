using ClassLibrary1;
using static System.Net.WebRequestMethods;

namespace WinFormsApp1
{
    public partial class Interfaz1 : Form
    {
        // Declaramos los nuevos componentes del juego
        private Panel panelControles;
        private Button btnArriba, btnAbajo, btnIzquierda, btnDerecha;
        private TextBox txtBitacora;
        private Ambiente ambiente;

        public Interfaz1()
        {
            InitializeComponent();
            // Instanciar el ambiente y generar el mapa
            // Aparte dentro de la clase se instanciará al protagonista y al wumpus
            ambiente = new Ambiente("Mundo de wumpus");
            ambiente.GenerarMapa();
            // Reorganizamos y creamos los elementos dinámicamente
            ConfigurarDiseñoYControles();
        }

        private void ConfigurarDiseñoYControles()
        {
            // 1. Quitamos el Dock Fill al tablero para darle su propio espacio a la izquierda
            tableLayoutPanel1.Dock = DockStyle.Left;
            tableLayoutPanel1.Width = 500; // El tablero ocupará 500px de ancho

            // 2. Creamos un panel contenedor para los controles a la derecha (los 300px restantes)
            panelControles = new Panel();
            panelControles.Location = new Point(500, 0);
            panelControles.Size = new Size(300, 500);
            panelControles.BackColor = SystemColors.ControlLight;
            this.Controls.Add(panelControles);

            // 3. Creamos la caja de texto (Bitácora de mensajes)
            txtBitacora = new TextBox();
            txtBitacora.Multiline = true;
            txtBitacora.ReadOnly = true;
            txtBitacora.ScrollBars = ScrollBars.Vertical;
            txtBitacora.Location = new Point(15, 20);
            txtBitacora.Size = new Size(260, 180);
            txtBitacora.Font = new Font("Arial", 10F, FontStyle.Regular);
            txtBitacora.Text = "¡Bienvenido al Mundo de Wumpus!\r\nTu aventura comienza en (0,0).\r\n";
            panelControles.Controls.Add(txtBitacora);

            // 4. Creamos los 4 botones de movimiento dispuestos en forma de cruz (Cruceta/D-Pad)
            btnArriba = new Button() { Text = "w", Location = new Point(110, 230), Size = new Size(60, 40) };
            btnIzquierda = new Button() { Text = "a", Location = new Point(40, 275), Size = new Size(60, 40) };
            btnAbajo = new Button() { Text = "s", Location = new Point(110, 275), Size = new Size(60, 40) };
            btnDerecha = new Button() { Text = "d", Location = new Point(180, 275), Size = new Size(60, 40) };

            // 5. Asignamos los manejadores de eventos a los botones
            btnArriba.Click += (s, e) => IntentarMover(0, -1);   // En WinForms, restar en Y sube visualmente
            btnAbajo.Click += (s, e) => IntentarMover(0, 1);      // Sumar en Y baja visualmente
            btnIzquierda.Click += (s, e) => IntentarMover(-1, 0);
            btnDerecha.Click += (s, e) => IntentarMover(1, 0);

            // 6. Agregamos los botones al panel de control
            panelControles.Controls.Add(btnArriba);
            panelControles.Controls.Add(btnIzquierda);
            panelControles.Controls.Add(btnAbajo);
            panelControles.Controls.Add(btnDerecha);
        }

        /// <summary>
        /// Procesa el movimiento y valida los límites de tu matriz de 4x4
        /// </summary>
        private void IntentarMover(int desX, int desY)
        {
            string mensaje = ambiente.SimularTurno(desX, desY);

            RegistrarMensaje(mensaje);
            if (ambiente.Finalizado)
            {
                RegistrarMensaje("Haz ganado.... Juego terminado");
                DesactivarControles();
            }
            if (!ambiente.protagonista.Estado)
            {
                RegistrarMensaje("Haz muerto..... Juego terminado");
                DesactivarControles();
            }
            // Se debe actualizar el mapa con la posicion actual del protagonista
            ActualizarMapa(ambiente.protagonista.x, ambiente.protagonista.y);
            
        }
        private void RegistrarMensaje(string mensaje)
            // Se registra el mensaje en el textBox
        {
            txtBitacora.AppendText(Environment.NewLine + mensaje);

            // Auto-scroll hacia abajo para leer siempre el último mensaje
            txtBitacora.SelectionStart = txtBitacora.Text.Length;
            txtBitacora.ScrollToCaret();
        }
        // Desactivar los controles cuando el juego finalize
        private void DesactivarControles()
        {
            btnArriba.Enabled = false;
            btnAbajo.Enabled = false;
            btnIzquierda.Enabled = false;
            btnDerecha.Enabled = false;
        }
        private void ActualizarMapa(int xActual, int yActual)
        {
            // Buscamos el Label exacto que está en esa coordenada de la interfaz
            Label lblCasilla = tableLayoutPanel1.GetControlFromPosition(xActual,yActual) as Label;
            // Falta dejar la casilla anterior del protagonista como "vista" ----------------------------

            if (lblCasilla != null)
            {
                lblCasilla.Text = "j";
                lblCasilla.BackColor = Color.LightGreen; // Color distintivo para el jugador
                lblCasilla.ForeColor = Color.Black;

                //lblCasillaAnterior.Text = "."; // La casilla anterior queda expuesta
                //lblCasilla.BackColor = Color.White; // Color de casilla descubierta
                //lblCasilla.ForeColor = Color.LightGray;
            }
        }

        private void label1_Click(object sender, EventArgs e) { }
        private void timer1_Tick(object sender, EventArgs e) { }
    }
}

    

