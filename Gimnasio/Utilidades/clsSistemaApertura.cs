using Gimnasio.Socios;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace Gimnasio.Utilidades
{
    public class clsSistemaApertura
    {

        public bool comprobarSocio(string returnData)
        {
            #region Variables
            bool result = false;
            int idSocio = Convert.ToInt32(returnData);
            DateTime fecha = DateTime.Now;

            clsSocioMembresia infoMembresiaSocio = new clsSocioMembresia();
            Socios.clsSocio oSocio = new Socios.clsSocio();

            DataGridView dgvListaInfo = new DataGridView();
            DataGridView dgvListaRegistro = new DataGridView();

            string membresia = string.Empty;
            DateTime fechaVencimiento = DateTime.Now;
            #endregion

            #region Get Datos
            //Recoger datos
            if (infoMembresiaSocio.getDatos(dgvListaInfo, idSocio))
            {
                DataTable dataSource = (DataTable)dgvListaInfo.DataSource;
                DataRow dataRow = dataSource.Rows[0];
                membresia = dataRow["Membresia"].ToString();
                fechaVencimiento = Convert.ToDateTime(dataRow["Vencimiento"]);
            }
            else
            {
               MessageBox.Show(infoMembresiaSocio.getError());
            }

            //recoger datos registro
            if (oSocio.getDatosRptRegistro(dgvListaRegistro, fecha))
            {
                DataTable dataSource = (DataTable)dgvListaRegistro.DataSource;
                for (int i = 0; i < dataSource.Rows.Count; i++)
                {
                    var a = dataSource.Rows[i];
                }
                DataRow dataRow = dataSource.Rows[0];
                //TODO procesar los datos de registro comprobando si el usuario ha entrado en el dia de hoy mas de dos veces
            }
            else
            {
                MessageBox.Show(oSocio.getError());
            }
            #endregion

            if (comprobarSociosAdministradores(membresia))
            {
                result = true;
            }
            else if (comprobarFechaVencimiento(fechaVencimiento))
            {
                //TODO: Comprobar bono solo mañana
                if (comprobarMembresia(membresia))
                {
                    if (comprobarRegistroDiario())
                    {
                        result = true;
                    }
                    else
                    {
                        //TODO: Mostrar al usuario que el socio ya ha completado los registros diarios posibles.
                        result = false;
                    }
                }
                else
                {
                    //TODO: Mostrar al usuario que el socio tiene bono de mañanas y no puede entrar de tardes
                    result = false;
                }
            }
            else
            {
                result = false;
            }
            return result;
        }

        public bool abrir()
        {   //TODO: Configurar la ip y el puerto donde queremos enviar el mensaje
            //TODO: Configurar el mensaje que queremos enviar
            try
            {
                #region TCP
                //string requestUrl = "http://192.168.0.36/";
                //WebRequest request = HttpWebRequest.Create(requestUrl);
                //request.Method = "POST";
                //request.ContentType = "application/x-www-form-urlencoded";
                //string postData = "myparam1=myvalue1&myparam2=myvalue2";
                //using (var writer = new StreamWriter(request.GetRequestStream()))
                //{
                //    writer.Write(postData);
                //}
                //string responseFromRemoteServer;
                //using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                //{
                //    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                //    {
                //        responseFromRemoteServer = reader.ReadToEnd();
                //    }
                //}
                #endregion

                #region UPD
                //long ip = 1921680102;
                //IPAddress ip = new IPAddress(ip);
                //string port = "8080";
                //string mensaje = "LFT";

                //UdpClient udpClient = new UdpClient();
                //udpClient.Connect(ip, Convert.ToInt16(port));
                //Byte[] senddata = Encoding.ASCII.GetBytes(mensaje);
                //udpClient.Send(senddata, senddata.Length);

                MessageBox.Show("Puerta Abierta");
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return true;
        }


        #region Private methods 
        private bool comprobarSociosAdministradores(string membresia)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(membresia))
            {
                if (membresia.ToLower() == "infinito")
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            else
            {
                //TODO: Completar el error: el usuario no tiene membresia
                //MessageBox.Show(infoMembresiaSocio.getError());
            }
            return result;
        }

        private bool comprobarFechaVencimiento(DateTime fechaVencimiento)
        {
            bool result = false;

            int relation = DateTime.Compare(fechaVencimiento, DateTime.Now);
            if (relation > 0)
            {
                result = true;
            }
            else if (relation == 0)
            {
                result = true;
            }
            else
            {
                result = false;
            }

            return result;
        }

        private bool comprobarMembresia(string membresia)
        {
            bool result = false;

            if (!string.IsNullOrEmpty(membresia))
            {
                if (membresia.ToLower() == "mañana")
                {
                    //TODO: Completar lógica solo mañana
                    int  hora = DateTime.Now.Hour;
                    int min = DateTime.Now.Minute;
                    int sec = DateTime.Now.Minute;

                    result = true;
                    //result = false;
                }
                else
                {
                    result = true;
                }
            }
            else
            {
                //TODO: Completar el error: el usuario no tiene membresia
                //MessageBox.Show(infoMembresiaSocio.getError());
            }
            return result;
        }

        private bool comprobarRegistroDiario()
        {

            //if (!ExpresionesRegulares.RegEX.isNumber(txtClave.Text.ToString()))
            //{
            //    MessageBox.Show("La clave es numerica, debes introducir solo numeros");
            //    return;
            //}
           
           
            //TODO: Recoger datos registro
            return true;
        }
        #endregion
    }
}
