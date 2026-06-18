using ClassLibrary1;
using static System.Net.WebRequestMethods;
using Persistencia;
namespace WinFormsApp1
{
    public partial class Interfaz1 : Form
    {
        // Declaramos los nuevos componentes del juego
        private Panel panelControles;
        private Button btnArriba, btnAbajo, btnIzquierda, btnDerecha;
        private TextBox txtBitacora;
        private Ambiente ambiente;
        // Guarda true si la casilla ya fue descubierta
        private bool[,] casillasVisitadas = new bool[4, 4];

        public Interfaz1()
        {
            InitializeComponent();
            // Instanciar el ambiente y generar el mapa
            // Aparte dentro de la clase se instanciará al protagonista y al wumpus
            ambiente = new Ambiente("Mundo de wumpus");
            ambiente.GenerarMapa();
            // Aquí parte el jugador
            casillasVisitadas[0, 0] = true;
            // Reorganizamos y creamos los elementos dinámicamente
            ConfigurarDiseñoYControles();
        }

        private void ConfigurarDiseñoYControles()
        {
            // Se quita el Dock Fill al tablero para darle su propio espacio a la izquierda
            tableLayoutPanel1.Dock = DockStyle.Left;
            tableLayoutPanel1.Width = 500; 

            // Se crea un panel contenedor para los controles a la derecha 
            panelControles = new Panel();
            panelControles.Location = new Point(500, 0);
            panelControles.Size = new Size(300, 500);
            panelControles.BackColor = SystemColors.ControlLight;
            this.Controls.Add(panelControles);

            // Se crea una caja de texto (Bitácora de mensajes)
            txtBitacora = new TextBox();
            txtBitacora.Multiline = true;
            txtBitacora.ReadOnly = true;
            txtBitacora.ScrollBars = ScrollBars.Vertical;
            txtBitacora.Location = new Point(15, 20);
            txtBitacora.Size = new Size(260, 180);
            txtBitacora.Font = new Font("Arial", 10F, FontStyle.Regular);
            txtBitacora.Text = "¡Bienvenido al Mundo de Wumpus!\r\nTu aventura comienza en (0,0).\r\n";
            panelControles.Controls.Add(txtBitacora);

            // Se crean 4 botones de movimiento en forma de cruz 
            btnArriba = new Button() { Text = "w", Location = new Point(110, 230), Size = new Size(60, 40) };
            btnIzquierda = new Button() { Text = "a", Location = new Point(40, 275), Size = new Size(60, 40) };
            btnAbajo = new Button() { Text = "s", Location = new Point(110, 275), Size = new Size(60, 40) };
            btnDerecha = new Button() { Text = "d", Location = new Point(180, 275), Size = new Size(60, 40) };

            // Se asignan los eventos de cada boton
            btnArriba.Click += (s, e) => IntentarMover(0, -1);  
            btnAbajo.Click += (s, e) => IntentarMover(0, 1);      
            btnIzquierda.Click += (s, e) => IntentarMover(-1, 0);
            btnDerecha.Click += (s, e) => IntentarMover(1, 0);

            // Se agregan los botones al panel de control
            panelControles.Controls.Add(btnArriba);
            panelControles.Controls.Add(btnIzquierda);
            panelControles.Controls.Add(btnAbajo);
            panelControles.Controls.Add(btnDerecha);

            // Botones para guardar y cargar partida
            Button btnGuardar = new Button();
            btnGuardar.Text = "Guardar Partida";
            btnGuardar.Location = new Point(15, 380); 
            btnGuardar.Size = new Size(120, 30);
            // Llama al metodo de guardarPartida dentro de la Persistencia
            btnGuardar.Click += (s, e) => {
                bool exito = Persistencia.Partida.GuardarPartida(this.ambiente);
                if (exito)
                {
                    RegistrarMensaje("Partida guardada con éxito.");
                }
                else
                {
                    RegistrarMensaje("Error al guardar la partida.");
                }
            };
            panelControles.Controls.Add(btnGuardar);

            Button btnCargar = new Button();
            btnCargar.Text = "Cargar Partida";
            btnCargar.Location = new Point(145, 380);
            btnCargar.Size = new Size(120, 30);
            btnCargar.Click += (s, e) => {
                // Este metodo llamara a la clase "Partida"
                EjecutarCargaDePartida();
            };
            panelControles.Controls.Add(btnCargar);
        }
        private void EjecutarCargaDePartida()
        {
            // Si los botones estaban desactivados se activan nuevamente
            if(btnArriba.Enabled == false)
            {
                btnArriba.Enabled = true;
                btnAbajo.Enabled = true;
                btnIzquierda.Enabled = true;
                btnDerecha.Enabled = true;
            }
            // Se llama al metodo de CargarPartida que retornará el ambiente guardado
            Ambiente partidaCargada = Persistencia.Partida.CargarPartida();

            if (partidaCargada == null)
            {
                RegistrarMensaje("No se encontró ninguna partida guardada");
                return;
            }

            // Reemplazamos el ambiente viejo por el cargado
            this.ambiente = partidaCargada;

            // Limpiamos e informamos en la interfaz
            txtBitacora.Clear();
            RegistrarMensaje("Partida cargada con éxito. Retomando aventura...");

            // Redibujamos por completo el mapa con las coordenadas guardadas del jugador
            ActualizarMapa(this.ambiente.protagonista.x, this.ambiente.protagonista.y);

            // Falta actualizar la posicion de los objetos Pozo, Wumpus y el oro
 
        }
        private void IntentarMover(int desX, int desY)
        {
            string mensaje = ambiente.SimularTurno(desX, desY);

            RegistrarMensaje(mensaje);
            // Se validan las condiciones de termino
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
        // Al cargar una nueva partida se ejecutará este metodo con la posicion guardada del jugador
        private void ActualizarMapa(int xActual, int yActual)
        {
            // La casilla actual del jugador queda como visitada
            casillasVisitadas[xActual, yActual] = true;

            for (int col = 0; col < 4; col++)      
            {
                for (int fila = 0; fila < 4; fila++)  
                {
                    // Buscamos el Label dinámicamente en esa coordenada
                    Label lblCasilla = tableLayoutPanel1.GetControlFromPosition(col, fila) as Label;

                    if (lblCasilla != null)
                    {
                        // La casilla actual del jugador quedá con el icono y el fondo verde
                        if (col == xActual && fila == yActual)
                        {
                            lblCasilla.Text = "j";
                            lblCasilla.BackColor = Color.LightGreen;
                            lblCasilla.ForeColor = Color.Black;
                        }
                        // Las casillas ya visitadas quedan con un fondo blanco y sin icono
                        else if (casillasVisitadas[col, fila] == true)
                        {
                            lblCasilla.Text = ""; 
                            lblCasilla.BackColor = Color.White; 
                        }
                        // Las casillas que aún no se conocen quedan con un fondo azulado 
                        else
                        {
                            lblCasilla.Text = "";
                        }
                    }
                }
            }
        }

        private void label1_Click(object sender, EventArgs e) { }
        private void timer1_Tick(object sender, EventArgs e) { }
    }
}

    

