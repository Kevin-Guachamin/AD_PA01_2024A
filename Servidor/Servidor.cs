using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Protocolo;

namespace Servidor
{
    class Servidor
    {
        private static TcpListener escuchador;
        private static Dictionary<string, int> listadoClientes = new Dictionary<string, int>();

        static void Main(string[] args)
        {
            try
            {
                escuchador = new TcpListener(IPAddress.Any, 8081);
                escuchador.Start();
                Console.WriteLine("Servidor iniciado en el puerto 8081...");

                while (true)
                {
                    TcpClient cliente = escuchador.AcceptTcpClient();
                    Console.WriteLine("Cliente conectado desde: " + cliente.Client.RemoteEndPoint);
                    Thread hiloCliente = new Thread(ManipuladorCliente);
                    hiloCliente.Start(cliente);
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Error de socket: " + ex.Message);
            }
            finally
            {
                escuchador?.Stop();
            }
        }

        private static void ManipuladorCliente(object obj)
        {
            TcpClient cliente = (TcpClient)obj;
            NetworkStream flujo = null;

            try
            {
                flujo = cliente.GetStream();
                byte[] bufferRx = new byte[1024];
                int bytesRx;

                while ((bytesRx = flujo.Read(bufferRx, 0, bufferRx.Length)) > 0)
                {
                    string mensaje = Encoding.UTF8.GetString(bufferRx, 0, bytesRx);
                    string direccionCliente = cliente.Client.RemoteEndPoint.ToString();

                    Respuesta respuesta = Protocolos.ResolverPedido(mensaje, direccionCliente, ref listadoClientes);
                    Console.WriteLine($"Pedido: {mensaje} | Respuesta: {respuesta}");

                    byte[] bufferTx = Encoding.UTF8.GetBytes(respuesta.ToString());
                    flujo.Write(bufferTx, 0, bufferTx.Length);
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Error de cliente: " + ex.Message);
            }
            finally
            {
                flujo?.Close();
                cliente?.Close();
            }
        }
    }
}
