// ************************************************************************
// Practica 07
// Kevin Guachamin, Carlos Benavides
// Fecha de realización: 27/11/2024
// Fecha de entrega: 02/12/2024
// Resultados:
// * El código permite a los usuarios consultar el pico y placa de un vehículo, además de contar el número de consultas realizadas.
// Conclusiones:
// * El uso de la herramienta de GitHub permite manejar una gestión eficiente de las versiones de un proyecto,
// así como también permite el acceso y colaboración en proyectos compartidos por otros usuarios.
// * La reorganización del código refuerza los principios de la programación orientada a objetos mejorando la estructuración del código,
// permitiendo una mayor claridad y comprensión en la lectura del código.
// Recomendaciones
// * Es esencial realizar las pruebas necesarias para verificar que los cambios realizados son correctos,
// para luego subirlos al repositorio garantizando que no se introduzcan errores.
// * Establecer una rama principal (master) permite que exista un historial organizado y limpio,
// así como tener un punto de referencia confiable el cual permita realizar pruebas automáticas, implementaciones, despliegues, entre otras opciones.
// ************************************************************************
using System;
using System.Net.Sockets;
using System.Windows.Forms;
using Protocolo;

namespace Cliente
{
    public partial class FrmValidador : Form
    {
        private TcpClient remoto;
        private NetworkStream flujo;
        private Protocolos protocolo;

        public FrmValidador()
        {
            InitializeComponent();
        }

        private void FrmValidador_Load(object sender, EventArgs e)
        {
            try
            {
                remoto = new TcpClient("127.0.0.1", 8081);
                flujo = remoto.GetStream();
                protocolo = new Protocolos(flujo);
                panPlaca.Enabled = false;

            }
            catch (SocketException ex)
            {
                MessageBox.Show("No se pudo establecer conexión: " + ex.Message, "ERROR");
            }
        }

        private void btnIniciar_Click(object sender, EventArgs e)
        {
            string usuario = txtUsuario.Text;
            string contraseña = txtPassword.Text;

            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(contraseña))
            {
                MessageBox.Show("Se requiere el ingreso de usuario y contraseña", "ADVERTENCIA");
                return;
            }

            try
            {
                var respuesta = protocolo.HazOperacion("INGRESO", new[] { usuario, contraseña });

                if (respuesta.Estado == "OK" && respuesta.Mensaje == "ACCESO_CONCEDIDO")
                {
                    panPlaca.Enabled = true;
                    panLogin.Enabled = false;
                    txtModelo.Focus();

                    MessageBox.Show("Acceso concedido", "INFORMACIÓN");
                }
                else
                {
                    MessageBox.Show("No se pudo ingresar, revise credenciales", "ERROR");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "ERROR");
            }
        }

        private void btnConsultar_Click(object sender, EventArgs e)
        {
            try
            {
                var respuesta = protocolo.HazOperacion("CALCULO", new[] { txtModelo.Text, txtMarca.Text, txtPlaca.Text });
                MessageBox.Show("Respuesta recibida: " + respuesta.Mensaje, "INFORMACIÓN");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "ERROR");
            }
        }

        private void btnNumConsultas_Click(object sender, EventArgs e)
        {
            try
            {
                var respuesta = protocolo.HazOperacion("CONTADOR", new string[0]);
                MessageBox.Show($"Número de consultas: {respuesta.Mensaje}", "INFORMACIÓN");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "ERROR");
            }
        }

        private void FrmValidador_FormClosing(object sender, FormClosingEventArgs e)
        {
            flujo?.Close();
            remoto?.Close();
        }
    }
}
