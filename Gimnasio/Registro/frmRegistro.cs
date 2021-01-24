using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
namespace Gimnasio.Registro
{
    public partial class frmRegistro : Form
    {
        clsRegistro oRegistro;
        string idUsuario = string.Empty;
        string sourceTrigger = string.Empty;
        public frmRegistro()
        {
            InitializeComponent();
        }
        public frmRegistro(string source, string id)
        {
            idUsuario = id;
            sourceTrigger = source;
            InitializeComponent(source);
        }


        private void frmRegistro_Load(object sender, EventArgs e)
        {
            Utilidades.clsGrafico.centraX(this, panelContenedor);
            Utilidades.clsGrafico.centraX(this, panelEncabezado);
            Utilidades.clsGrafico.centraX(this, panelFoot);
            cargaDatos();
        }

        private void frmRegistro_LoadNFC(object sender, EventArgs e)
        {
            Utilidades.clsGrafico.centraX(this, panelContenedor);
            Utilidades.clsGrafico.centraX(this, panelEncabezado);
            Utilidades.clsGrafico.centraX(this, panelFoot);
            cargaDatosNFC();
        }

        private void cargaDatos()
        {
            Configuracion.clsConfiguracion.getDatos();
            lblNombreGimnacio.Text = Configuracion.clsConfiguracion.datos.NombreGimnacio.ToUpper();
            lblDomicilio.Text = Configuracion.clsConfiguracion.datos.Domicilio;
            lblTelefono.Text = Configuracion.clsConfiguracion.datos.Telefono;
            lblRFC.Text = Configuracion.clsConfiguracion.datos.RFC.ToUpper();
            lblMensaje.Text = Configuracion.clsConfiguracion.datos.Mensaje;
            if (Configuracion.clsConfiguracion.datos.Logo != null)
            {

                MemoryStream stream = new MemoryStream(Configuracion.clsConfiguracion.datos.Logo);
                Bitmap image = new Bitmap(stream);
                pbLogo.Image = image;
            }

            lblFecha.Text = DateTime.Now.ToLongDateString();
            lblHora.Text = DateTime.Now.ToLongTimeString();
        }

        private void cargaDatosNFC()
        {
            Configuracion.clsConfiguracion.getDatos();
            lblNombreGimnacio.Text = Configuracion.clsConfiguracion.datos.NombreGimnacio.ToUpper();
            lblDomicilio.Text = Configuracion.clsConfiguracion.datos.Domicilio;
            lblTelefono.Text = Configuracion.clsConfiguracion.datos.Telefono;
            lblRFC.Text = Configuracion.clsConfiguracion.datos.RFC.ToUpper();
            lblMensaje.Text = Configuracion.clsConfiguracion.datos.Mensaje;
            if (Configuracion.clsConfiguracion.datos.Logo != null)
            {

                MemoryStream stream = new MemoryStream(Configuracion.clsConfiguracion.datos.Logo);
                Bitmap image = new Bitmap(stream);
                pbLogo.Image = image;
            }

            lblFecha.Text = DateTime.Now.ToLongDateString();
            lblHora.Text = DateTime.Now.ToLongTimeString();

            object sender = new object();
            KeyEventArgs a = new KeyEventArgs(Keys.Enter);
            txtClave_KeyDown(sender, a);
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
           
        }

        private void frmRegistro_SizeChanged(object sender, EventArgs e)
        {
            Utilidades.clsGrafico.centraX(this, panelContenedor);
            Utilidades.clsGrafico.centraX(this, panelEncabezado);
            Utilidades.clsGrafico.centraX(this, panelFoot);
        }

        private void txtClave_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                int clave;

                if (!string.IsNullOrEmpty(sourceTrigger) && sourceTrigger == "RegistroNFC")
                {
                    //TODO: Comprobar si sigue el patron que nos gustaria para usuarios especiales. 
                    //if (!ExpresionesRegulares.RegEX.isNumber(idUsuario))
                    //{
                    //    MessageBox.Show("La clave es numerica, debes introducir solo numeros");
                    //    return;
                    //}
                    clave = int.Parse(idUsuario);
                }
                else
                {
                    if (!ExpresionesRegulares.RegEX.isNumber(txtClave.Text.ToString()))
                    {
                        MessageBox.Show("La clave es numerica, debes introducir solo numeros");
                        return;
                    }
                    clave = int.Parse(txtClave.Text.ToString());
                }

                oRegistro = new clsRegistro(clave);
                if (oRegistro.buscaDatos())
                {
                    //cargar datos
                    txtNombre.Text = oRegistro.datos.NombreSocio;
                    txtPaterno.Text = oRegistro.datos.Paterno;
                    txtMaterno.Text = oRegistro.datos.Materno;
                    lblVencimiento.Text = oRegistro.datos.Vencimiento.ToLongDateString();
                   
                    //cargar foto
                    if (oRegistro.datos.foto != null)
                    {
                        MemoryStream stream = new MemoryStream(oRegistro.datos.foto);
                        Bitmap image = new Bitmap(stream);
                        pbFoto.Image = image;
                    }

                    if (!string.IsNullOrEmpty(sourceTrigger) && sourceTrigger == "RegistroNFC")
                    {
                        txtClave.Text = Convert.ToString(clave);
                    }
                    else
                    {
                        txtClave.Text = "";
                    }
                      
                    if (DateTime.Compare(oRegistro.datos.Vencimiento, DateTime.Now) < 0)
                    {
                        lblVencimiento.ForeColor = Color.Red;
                        MessageBox.Show("Se a términado tu membresia");

                    }
                    //si no se a venciudo la registramos REGISTRO
                    else
                    {
                        //si le quedanm pocos dias se le avisa de que ya mero se le acaba su membresia
                        Configuracion.clsConfiguracion.getDatos();
                        if(Configuracion.clsConfiguracion.datos.mensajeVencimiento!=null&&Configuracion.clsConfiguracion.datos.mensajeVencimiento>0)//si tiene abilitado el mensaje
                        if (DateTime.Compare(oRegistro.datos.Vencimiento, DateTime.Now.AddDays(Configuracion.clsConfiguracion.datos.mensajeVencimiento)) < 0)//si ya mero se termina
                        {
                            DateTime newDate = oRegistro.datos.Vencimiento;
                            DateTime oldDate = DateTime.Now;
                            TimeSpan ts = newDate - oldDate;
                            int diasFaltantes=ts.Days;

                            MessageBox.Show("Quedan "+(diasFaltantes)+" dias de tu membresia");
                        }

                        lblVencimiento.ForeColor = Color.Black;
                       
                        if (!oRegistro.addVisita())
                        {
                            MessageBox.Show(oRegistro.error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("No existe un socio con esa clave");
                }
            }
        }

        private void txtClave_TextChanged(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblFecha.Text = DateTime.Now.ToLongDateString();
            lblHora.Text = DateTime.Now.ToLongTimeString();
        }
    }
}
