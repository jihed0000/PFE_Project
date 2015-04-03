using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Hosting;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Media3D;
using Kitware.VTK;

namespace PFEProject
{
  public  class Curve
  {
      public double alpha_dot_max;
        public vtkPolyData _vtkPolyData = new vtkPolyData();
        public Curve(vtkPolyData polyData)
        {
            this._vtkPolyData = polyData;
            NbrePts = polyData.GetNumberOfPoints();
            Utile = false;
            q_rep = new vtkPoints();
        }

        public Curve()
        {
            
        }

        public double DistGeodesicSansPD(Curve c)
        {
          
            int h;
            double dist = 0;
            for (h =1; h < Nb; h++)
            {
                Vector3D vector1 = new Vector3D(q_rep.GetPoint(h)[0], q_rep.GetPoint(h)[1], q_rep.GetPoint(h)[2]);
                Vector3D vector2 = new Vector3D(c.q_rep.GetPoint(h)[0], c.q_rep.GetPoint(h)[1], c.q_rep.GetPoint(h)[2]);
               
                dist += Vector3D.DotProduct(vector1, vector2);

                

            }
            
            float distance = (float) dist/(Nb);
          //  MessageBox.Show(distance.ToString());
         if (Math.Abs(distance) >= 1) return 0;
            return Math.Acos(distance);

        }

      public void calculer_alpha_point(Curve c)
      {
          c.alpha_dot_max = 0;
        
          int taille = c.q_rep.GetNumberOfPoints();
          vtkPoints alpha_dot = new vtkPoints();
          alpha_dot.SetNumberOfPoints(taille);
          double theta = c.DistGeodesicSansPD(this);
          double w_x = 0, w_y = 0, w_z = 0, module_alpha_dot;
          for (int i = 0; i < taille; i++)
          {
              if (theta==0)
              {
                  module_alpha_dot = 0;
              }
              else
              {

                  w_x = (theta / Math.Sin(theta)) * (this.q_rep.GetPoint(i)[0] - Math.Cos(theta) * c.q_rep.GetPoint(i)[0]);
                  w_y = (theta / Math.Sin(theta)) * (this.q_rep.GetPoint(i)[1] - Math.Cos(theta) * c.q_rep.GetPoint(i)[1]);
                  w_z = (theta / Math.Sin(theta)) * (this.q_rep.GetPoint(i)[2] - Math.Cos(theta) * c.q_rep.GetPoint(i)[2]);

                  module_alpha_dot =Math.Sqrt(w_x*w_x + w_y*w_y + w_z*w_z);
              }
              alpha_dot.SetPoint(i,w_x,w_y,w_z);
              MainScreen.Sc.Add(module_alpha_dot);
              MainScreen.Vc.Add(new Vector3D(w_x,w_y,w_z));
              if (module_alpha_dot > c.alpha_dot_max)
              {
                  c.alpha_dot_max = module_alpha_dot;
              }
          }
          c.alpha_pt = alpha_dot;

      }
        public vtkPoints Curve2q(vtkPoints beta, double delta)
        {
            vtkPoints q = new vtkPoints();
            q.SetNumberOfPoints(beta.GetNumberOfPoints());
            int i, j, n = Nb, k = 3, nb_colons = Nb, nb_lignes = 3, kl, lk;
            kl = Nb - 1;
            lk = Nb - 2;
            double len, norme,aa,bb;
            double[] inter = new double[3];
            for (j = 0; j < Nb; j++)
            {
                q.SetPoint(j,beta.GetPoint(j)[0],beta.GetPoint(j)[1],beta.GetPoint(j)[2]);
            }

            for (i = 0; i < 3; i++)
            {
                 aa = beta.GetPoint(1)[i];
                bb = beta.GetPoint(0)[i];
                inter[i] = (aa - bb)*delta;
            }
            q.SetPoint(0,inter[0],inter[1],inter[2]);
            for (j = 1; j < Nb - 1; j++)
            {
                for (i = 0; i < 3; i++)
                {
                    aa = beta.GetPoint(j + 1)[i];
                    bb = beta.GetPoint(j - 1)[i];
                    inter[i] = (aa - bb)*(delta/2);


                }
                q.SetPoint(j, inter[0], inter[1], inter[2]);
            }
            for (i = 0; i < 3; i++)
            {
                aa = beta.GetPoint(kl)[i];
                bb = beta.GetPoint(lk)[i];
                inter[i] = (aa - bb) * (delta);
            }
            q.SetPoint(kl,inter[0],inter[1],inter[2]);

            len = 0;
            for (j = 0; j < Nb; j++)
            {
                norme = 0;
                for ( i = 0; i < 3; i++)
                {
                    norme += Math.Pow(q.GetPoint(j)[i], 2);

                }
                norme = Math.Sqrt(norme);
                len += norme;

            }
            len = len/Nb;
            for ( j = 0; j < Nb; j++)
            {
                norme = 0;
                for ( i = 0; i < 3; i++)
                {
                    if(i==0)q.SetPoint(j,q.GetPoint(j)[0]/len,q.GetPoint(j)[1],q.GetPoint(j)[2]);
                    if (i == 1) q.SetPoint(j, q.GetPoint(j)[0], q.GetPoint(j)[1] / len, q.GetPoint(j)[2]);
                    if (i == 2) q.SetPoint(j, q.GetPoint(j)[0], q.GetPoint(j)[1], q.GetPoint(j)[2] / len);
                    norme += q.GetPoint(j)[i] * q.GetPoint(j)[i];

                }
                norme = Math.Sqrt(norme);
                double LL = Math.Sqrt(norme);
                if(LL>0.0001) q.SetPoint(j,q.GetPoint(j)[0]/LL,q.GetPoint(j)[1]/LL,q.GetPoint(j)[2]/LL);
                q.SetPoint(j, q.GetPoint(j)[0] * LL, q.GetPoint(j)[1] * LL, q.GetPoint(j)[2] * LL);
            }
            return q;
        }

        public vtkPoints q_rep { get; set; }

        public vtkPoints beta_rep { get; set; }
      public int Nb = 50;

        public bool Utile { get; set; }

        public int NbrePts { get; set; }
      public vtkPoints alpha_pt { get; set; }
  }
}
