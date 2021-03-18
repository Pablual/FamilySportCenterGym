using Gimnasio.Socios;
using Gimnasio.Utilidades.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using static Gimnasio.Utilidades.Model.SocioModel;

namespace Gimnasio.Utilidades
{
    public class clsSistemaApertura
    {
        public clsSistemaApertura() { }

        public SocioModel comprobarSocio(string idUsuario)
        {
            #region Variables
            int idSocio = Convert.ToInt32(idUsuario);
            DateTime fecha = DateTime.Now;
            SocioModel currentSocio = new SocioModel();

            clsSocioMembresia infoMembresiaSocio = new clsSocioMembresia();
            Socios.clsSocio oSocio = new Socios.clsSocio();

            DataGridView dgvListaInfoMembresia = new DataGridView();
            DataGridView dgvListaRegistro = new DataGridView();
            DataTable tablaRegistro = new DataTable();
            #endregion

            #region Get Datos
            //Recoger datos del socio
            if (oSocio.getDatos(Convert.ToInt32(idSocio)))
            {
                if (string.IsNullOrEmpty(oSocio.datos.Nombre))
                {
                    MessageBox.Show("El nombre del socio esta vacio");
                    currentSocio.isSocioEnabled = false;
                    return currentSocio;
                }
                if (string.IsNullOrEmpty(oSocio.datos.Paterno))
                {
                    MessageBox.Show("El nombrePaterno del socio esta vacio");
                    currentSocio.isSocioEnabled = false;
                    return currentSocio;
                }
                if (string.IsNullOrEmpty(oSocio.datos.Materno))
                {
                    MessageBox.Show("El nombreMaterno del socio esta vacio");
                    currentSocio.isSocioEnabled = false;
                    return currentSocio;
                }
                currentSocio.idSocio = Convert.ToString(idSocio);
                currentSocio.nombre = oSocio.datos.Nombre;
                currentSocio.nombrePaterno = oSocio.datos.Paterno;
                currentSocio.nombreMaterno = oSocio.datos.Materno;
            }
            else
            {
                MessageBox.Show(oSocio.getError());
                currentSocio.isSocioEnabled = false;
                return currentSocio;
            }
            //Recoger datos de la membresia del socio
            if (infoMembresiaSocio.getDatos(dgvListaInfoMembresia, idSocio))
            {
                DataTable dataSource = (DataTable)dgvListaInfoMembresia.DataSource;
                string membresia = string.Empty;
                string fechaVencimiento = string.Empty;
                if (dataSource.Rows.Count != 0)
                {
                    DataRow dataRow = dataSource.Rows[0];
                    membresia = dataRow["Membresia"].ToString();
                    fechaVencimiento = dataRow["Vencimiento"].ToString();
                }
                else
                {
                    MessageBox.Show("El socio " + idSocio + " no tiene membresia, por favor añada una membresia válida");
                    currentSocio.isSocioEnabled = false;
                    return currentSocio;
                }

                if (!string.IsNullOrEmpty(membresia))
                {
                    currentSocio.membresia = (MembresiaType)Enum.Parse(typeof(MembresiaType), membresia);
                }
                else
                {
                    MessageBox.Show("El socio " + idSocio + " no tiene membresia, por favor añada una membresia válida");
                    currentSocio.isSocioEnabled = false;
                    return currentSocio;
                }

                if (!string.IsNullOrEmpty(fechaVencimiento))
                {
                    currentSocio.vencimiento = Convert.ToDateTime(fechaVencimiento);
                }
                else
                {
                    MessageBox.Show("El socio " + idSocio + " no tiene fecha de vencimiento");
                    currentSocio.isSocioEnabled = false;
                    return currentSocio;
                }
            }
            else
            {
                MessageBox.Show(infoMembresiaSocio.getError());
                currentSocio.isSocioEnabled = false;
                return currentSocio;
            }
            //Recoger datos del registro del socio
            if (oSocio.getDatosRptRegistro(dgvListaRegistro, fecha))
            {
                tablaRegistro = (DataTable)dgvListaRegistro.DataSource;
            }
            else
            {
                MessageBox.Show(oSocio.getError());
                currentSocio.isSocioEnabled = false;
                return currentSocio;
            }
            #endregion

            if (comprobarTarjetaMaestra(currentSocio))
            {
                currentSocio.isSocioEnabled = true;
                currentSocio.exitType = "BTH";
            }
            else if (comprobarSociosAdministradores(currentSocio))
            {
                currentSocio.isSocioEnabled = true;
                currentSocio.exitType = "BTH";
            }
            else if (comprobarMembresiaVisitaSocios(currentSocio))
            {
                if (comprobarRegistroDiario(tablaRegistro, ref currentSocio))
                {
                    currentSocio.isSocioEnabled = true;
                }
                else
                {
                    currentSocio.isSocioEnabled = false;
                    DialogResult result = MessageBox.Show("El socio ya ha completado todos los registros diarios del gimnasio");
                    if (result == DialogResult.OK)
                    {
                        currentSocio.exitType = "NON";
                        enviar(currentSocio);
                    }
                }
            }
            else if (comprobarFechaVencimiento(currentSocio))
            {
                if (comprobarMembresia(currentSocio))
                {
                    if (comprobarRegistroDiario(tablaRegistro, ref currentSocio))
                    {
                        currentSocio.isSocioEnabled = true;
                    }
                    else
                    {
                        currentSocio.isSocioEnabled = false;
                        DialogResult result = MessageBox.Show("El socio ya ha completado todos los registros diarios del gimnasio");
                        if (result == DialogResult.OK)
                        {
                            currentSocio.exitType = "NON";
                            enviar(currentSocio);
                        }
                    }
                }
                else
                {
                    currentSocio.isSocioEnabled = false;
                    DialogResult result = MessageBox.Show("El socio " + currentSocio.idSocio + " tiene bono de medio dia. No puede entrar al gimnasio a partir de las 13 y debe salir antes de la misma");
                    if (result == DialogResult.OK)
                    {
                        currentSocio.exitType = "NON";
                        enviar(currentSocio);
                    }
                }
            }
            else
            {
                currentSocio.isSocioEnabled = false;
            }
            return currentSocio;
        }

        public bool enviar(SocioModel currentSocio)
        {   //TODO: Configurar la ip y el puerto donde queremos enviar el mensaje
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
                string ipAddress = "192.168.1.102";
                IPAddress ip = IPAddress.Parse(ipAddress);
                string port = "8080";
                string mensaje = currentSocio.exitType;

                UdpClient udpClient = new UdpClient();
                udpClient.Connect(ip, Convert.ToInt16(port));
                Byte[] senddata = Encoding.ASCII.GetBytes(mensaje);
                udpClient.Send(senddata, senddata.Length);
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return true;
        }


        #region Private methods 

        private bool comprobarMembresiaVisitaSocios(SocioModel socio)
        {
            bool result = false;

            if (socio.membresia == MembresiaType.Visita)
            {
                result = true;
            }
            else
            {
                result = false;
            }

            return result;
        }
        private bool comprobarSociosAdministradores(SocioModel socio)
        {
            bool result = false;

            if (socio.membresia == MembresiaType.Infinito)
            {
                result = true;
            }
            else
            {
                result = false;
            }

            return result;
        }

        private bool comprobarTarjetaMaestra(SocioModel socio)
        {
            bool result = false;

            if (socio.idSocio == "0000")
            {
                result = true;
            }
            else
            {
                result = false;
            }

            return result;
        }
        

        private bool comprobarFechaVencimiento(SocioModel socio)
        {
            bool result = false;
            DateTime date = DateTime.Now;
            //date = date.AddDays(31);
            //date = date.AddMonths(3);
            //date = date.AddYears(1);

            int relation = DateTime.Compare(socio.vencimiento, date);
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

        private bool comprobarMembresia(SocioModel socio)
        {
            bool result = false;

            if (socio.membresia == MembresiaType.MedioDia)
            {
                int day = DateTime.Now.Day;
                int month = DateTime.Now.Month;
                int year = DateTime.Now.Year;

                DateTime currentTime = DateTime.Now;
                DateTime horaLimiteManana = new DateTime(year, month, day, 12, 59, 59);
                int output = DateTime.Compare(currentTime, horaLimiteManana);

                if (output < 0)
                {
                    result = true;
                }
                else if (output == 0)
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
                result = true;
            }

            return result;
        }

        private bool comprobarRegistroDiario(DataTable dataSource, ref SocioModel socio)
        {
            bool result = false;
            int contadorRegistroUsuario = 0;
            string registroNombre = string.Empty;
            string registroNombrePaterno = string.Empty;
            string regsitroNombreMaterno = string.Empty;

            for (int i = 0; i < dataSource.Rows.Count; i++)
            {
                DataRow registro = dataSource.Rows[i];
                registroNombre = registro["Nombre"].ToString().ToLower();
                registroNombrePaterno = registro["Paterno"].ToString().ToLower();
                regsitroNombreMaterno = registro["Materno"].ToString().ToLower();

                if (registroNombre == socio.nombre.ToLower() && registroNombrePaterno == socio.nombrePaterno.ToLower() && regsitroNombreMaterno == socio.nombreMaterno.ToLower())
                {
                    contadorRegistroUsuario += 1;
                }
            }

            if (socio.membresia == MembresiaType.AnualPlus || socio.membresia == MembresiaType.TrimestralPlus || socio.membresia == MembresiaType.MensualPlus || socio.membresia == MembresiaType.MedioDiaPlus)
            {
                switch (contadorRegistroUsuario)
                {
                    case 0:
                        //EntradaGYM
                        socio.exitType = "LFT";
                        result = true;
                        break;
                    case 1:
                        //SalidaGYM
                        socio.exitType = "RGT";
                        result = true;
                        break;
                    case 2:
                        //EntradaGYM
                        socio.exitType = "LFT";
                        result = true;
                        break;
                    case 3:
                        //SalidaGYM
                        socio.exitType = "RGT";
                        result = true;
                        break;
                    default:
                        result = false;
                        break;
                }
            }
            else
            {
                switch (contadorRegistroUsuario)
                {
                    case 0:
                        //EntradaGYM
                        socio.exitType = "LFT";
                        result = true;
                        break;
                    case 1:
                        //SalidaGYM
                        socio.exitType = "RGT";
                        result = true;
                        break;
                    default:
                        result = false;
                        break;
                }
            }
            return result;
        }
        #endregion
    }
}
