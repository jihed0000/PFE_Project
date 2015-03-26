using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Hosting;
using System.Text;
using System.Threading.Tasks;
using Kitware.VTK;

namespace PFEProject
{
  public  class Curve
    {
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
            for (h = 0; h < Nb; h++)
            {
                int e;
                for (e = 0; e < 3; e++)
                {
                    dist += this.q_rep.GetPoint(h)[e]*c.q_rep.GetPoint(h)[e];
                }
            }
            float distance = (float) dist/(float) Nb;

            if (Math.Abs(distance) >= 1) return 0;
            return Math.Acos(distance);

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
                q.SetPoint(j, beta.GetPoint(j)[0], beta.GetPoint(j)[1], beta.GetPoint(j)[2]);
            }
            for (i = 0; i < 3; i++)
            {
                aa = beta.GetPoint(kl)[i];
                bb = beta.GetPoint(lk)[i];
                inter[i] = (aa - bb) * (delta);
            }
            q.SetPoint(kl,inter[0],inter[1],inter[2]);

            len = 0;
            for ( j = 0; j < Nb; j++)
            {
                norme = 0;
                for ( i = 0; i < 3; i++)
                {
                    if(i==0)q.SetPoint(j,q.GetPoint(j)[0]/len,q.GetPoint(j)[1],q.GetPoint(j)[2]);
                    if (i == 1) q.SetPoint(j, q.GetPoint(j)[0], q.GetPoint(j)[1] / len, q.GetPoint(j)[2]);
                    if (i == 2) q.SetPoint(j, q.GetPoint(j)[0], q.GetPoint(j)[1], q.GetPoint(j)[2] / len);
                    norme += Math.Pow(q.GetPoint(j)[i], 2);

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
      public int Nb = 25;

        public bool Utile { get; set; }

        public int NbrePts { get; set; }

    }
}
