using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Media3D;
using Kitware.VTK;
using PfeLibrary;

namespace PFEProject
{
    public partial class DSFview : Form
    {

        public List<double> philist = new List<double>();
        public List<double> thetalist = new List<double>();
        public List<double> phiorthetalist = new List<double>();
        
        public static Face face1 = new Face();
        public static Face face2 = new Face();
        public DSFview()
        {
            InitializeComponent();
        }
        public DSFview(Face f1,Face f2)
        {
            InitializeComponent();
            face1 = f1;
            face2 = f2;
           

        }

        public void affiche()
        {
           
              


                vtkRenderer renderer = new vtkOpenGLRenderer();
                renderer = face2.affiche_DSF(face1);

                //vtkRenderer renderer1 = new vtkOpenGLRenderer();
                //vtkRenderer renderer2 = new vtkOpenGLRenderer();
                //vtkRenderer renderer3 = new vtkOpenGLRenderer();
                //double phi, theta, rho;
                //for (int i = 0; i < SharedValues.Sc.Count; i++)
                //{
                //    rho = SharedValues.Sc[i];
                //    phi = getphi(SharedValues.Vc[i], rho);
                //    theta = gettheta(SharedValues.Vc[i], rho);
                //    philist.Add(phi);
                //    thetalist.Add(theta);

                //    phiorthetalist.Add(phi + theta);

                //}

                //for (int i = 0; i < 4; i++)
                //{
                //    if (i == 2)
                //    {
                //        renderer.SetViewport(xmins[i], ymins[i], xmaxs[i], ymaxs[i]);
                //        dsf.RenderWindow.AddRenderer(renderer);
                //    }

                //    if (i == 3)
                //    {
                //        renderer1 = face2.affiche_DSF(face1, philist);
                //        renderer1.SetViewport(xmins[i], ymins[i], xmaxs[i], ymaxs[i]);
                //        dsf.RenderWindow.AddRenderer(renderer1);
                //    }
                //    if (i == 0)
                //    {
                //        renderer2 = face2.affiche_DSF(face1, thetalist);
                //        renderer2.SetViewport(xmins[i], ymins[i], xmaxs[i], ymaxs[i]);
                //        dsf.RenderWindow.AddRenderer(renderer2);
                //    }
                //    if (i == 1)
                //    {
                //        renderer3 = face2.affiche_DSF(face1, phiorthetalist);

                //        renderer3.SetViewport(xmins[i], ymins[i], xmaxs[i], ymaxs[i]);
                //        dsf.RenderWindow.AddRenderer(renderer3);
                //    }

             dsf.RenderWindow.AddRenderer(renderer);

            dsf.RenderWindow.GetRenderers().GetFirstRenderer().SetBackground(224,224,224);

                

            
                //  dsf.RenderWindow.GetInteractor().Start();
            
        

    }

        public double getphi(Vector3D vect, double rho )
        { double phi =  Math.Acos(vect.Z/rho);
            if (false)
            {
                phi += 2*Math.PI;
            }
                //Math.Acos(vect.X/rho);
            return phi;
        }
        public double gettheta(Vector3D vect, double rho)
        {
            double phi = Math.Atan(vect.Y / vect.X);
            if (false)
            {
                phi += 2 * Math.PI;
            }
            //Math.Acos(vect.X/rho);
            return phi;
        }

        private void dowork(object sender, DoWorkEventArgs e)
        {
            affiche();
        }

        private void complete(object sender, RunWorkerCompletedEventArgs e)
        {

            dsf.RenderWindow.Render();
        }

        private void onload(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }

        


     
    }
}
