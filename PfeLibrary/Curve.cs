using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Media3D;

using Kitware.VTK;

using PfeLibrary;

namespace PFEProject
{
    public class Curve
    {

        /// <summary>
        /// modified for test perpose NB=50
        /// </summary>
        public int Nb = 50;
        public vtkPolyData _vtkPolyData = new vtkPolyData();
        public double alpha_dot_max;
        public string _faceid;
        public Curve(vtkPolyData polyData)
        {
            _vtkPolyData = polyData;
            NbrePts = polyData.GetNumberOfPoints();
            Utile = false;
            q_rep = new vtkPoints();
        }

        public Curve(vtkPolyData polyData , string Faceid)
        {
            _vtkPolyData = polyData;
            NbrePts = polyData.GetNumberOfPoints();
            Utile = false;
            q_rep = new vtkPoints();
            _faceid=Faceid;
        }
        public Curve(vtkPoints pts)
        {
            beta_rep = pts;

            Utile = false;
            q_rep = new vtkPoints();
            q_rep = Curve2q(beta_rep, Nb-1);

        }
        public Curve()
        {
        }

        public vtkPoints q_rep { get; set; }

        public vtkPoints beta_rep { get; set; }

        public bool Utile { get; set; }

        public int NbrePts { get; set; }
        public vtkPoints alpha_pt { get; set; }

        public double DistGeodesicSansPD(Curve c)
        {
            int h;
            double dist = 0;
            for (h = 1; h < Nb; h++)
            {
                var vector1 = new Vector3D(q_rep.GetPoint(h)[0], q_rep.GetPoint(h)[1], q_rep.GetPoint(h)[2]);
                var vector2 = new Vector3D(c.q_rep.GetPoint(h)[0], c.q_rep.GetPoint(h)[1], c.q_rep.GetPoint(h)[2]);

                dist += Vector3D.DotProduct(vector1, vector2);
            }

            float distance = (float) dist/(Nb);
            //  MessageBox.Show(distance.ToString());
            if (Math.Abs(distance) >= 1) return 0;
            return Math.Acos(distance);
        }

        public void write_qcurve_to_disk(string filename)
        {
            string str = string.Empty;
            for (int i = 0; i < q_rep.GetNumberOfPoints(); i++)
            {
                str += q_rep.GetPoint(i)[0] + " " + q_rep.GetPoint(i)[1] + " " + q_rep.GetPoint(i)[2] + '\n';
            }

          
            System.IO.File.WriteAllText(filename, str);
        }
        public void write_betacurve_to_disk(string filename)
        {
            string str = string.Empty;
            for (int i = 0; i < q_rep.GetNumberOfPoints(); i++)
            {
                str += beta_rep.GetPoint(i)[0] + " " + beta_rep.GetPoint(i)[1] + " " + beta_rep.GetPoint(i)[2] + '\n';
            }


            System.IO.File.WriteAllText(filename, str);
        }

        public float distance = 0;
       public  int[] CalculDistGeo(Curve courbe_prob,int pas_dp,int indxx)
{
	double dist=0;
            int[] idx = new int[Nb];

 // The result.
    double[] gamma = new double[Nb];
    double[] gamma_1 = new double[Nb];
	int j,i,k,h,e,indice;


            double[] nose = new double[3];
	for (int mm=0;mm<3;mm++)
		nose[mm] = courbe_prob.beta_rep.GetPoint(0)[mm];

	vtkPoints pts_prob=new vtkPoints();
	pts_prob.SetNumberOfPoints(courbe_prob.beta_rep.GetNumberOfPoints());
	vtkPoints pts_prob_inter=new vtkPoints();
	pts_prob_inter.SetNumberOfPoints(courbe_prob.beta_rep.GetNumberOfPoints());

	/// Extraire forme entree de DP pour Courbe actuelle
	double[] beta11_x = new double[Nb];
	double[] beta11_y = new double[Nb];
	double[] beta11_z = new double[Nb];
	for( j=0;j<Nb;j++)
	{
		beta11_x[j] = this.q_rep.GetPoint(j)[0];
		beta11_y[j] = this.q_rep.GetPoint(j)[1];
		beta11_z[j] = this.q_rep.GetPoint(j)[2];
	}
	
	   qfunction q1 = new qfunction(beta11_x,beta11_y,beta11_z,Nb);
		

	

	/// Extraire forme entree de DP pour Courbe probe
		
	double[] beta12_x = new double[Nb];
	double[] beta12_y = new double[Nb];
	double[] beta12_z = new double[Nb];
	for( j=0;j<Nb;j++)
	{
		beta12_x[j] = courbe_prob.q_rep.GetPoint(j)[0];
		beta12_y[j] = courbe_prob.q_rep.GetPoint(j)[1];
		beta12_z[j] = courbe_prob.q_rep.GetPoint(j)[2];
	}
        qfunction q2 = new qfunction(beta12_x, beta12_y,beta12_z, Nb);
		
	
///////////////////////////


	
	gamma_1 = dp( q1, q2,Nb,pas_dp);




           //string str = string.Empty;
           //foreach (double b in gamma_1)
           //{
           //    str += b.ToString() + '\n';
           //}
           //System.IO.File.WriteAllText("c:\\mygamma1.txt",str);
           gamma = gamma_1;

           ////gamma[0] = 0;
           ////for (int kk = 1; kk < Nb-1; kk++)
           ////    gamma[kk] = gamma_1[kk + 1];
           ////gamma[Nb] = 1;
           //str = string.Empty;
           //foreach (double b in gamma_1)
           //{
           //    str += (b*Nb).ToString() + '\n';
           //}
           //System.IO.File.WriteAllText("c:\\mygamma1nb.txt", str);
           
          
/*cout<<"gama"<<endl;
	for(int hh=0;hh<Nb;hh++)
	cout<<gamma[hh]*Nb<<" . ";*/
	

double result;
double partie_decimale;

	/// Group Action by Gamma /////
		for ( k=0;k<Nb;k++)
		{
			result = gamma[k]*Nb ;
		    idx[k] =(int) Math.Floor(result);
            partie_decimale = result -Math.Floor(result);
            if(partie_decimale>0.5)
                idx[k]++;						
			
			if(idx[k] < 1) idx[k]=1;			//if(idx[k] < 0) idx[k]=0;
			if(idx[k] > Nb) idx[k]=Nb;		//if(idx[k] > Nb-1) idx[k]=Nb-1;	

		    idx[k]--;
		}

        //string st = string.Empty;
        //foreach (int b in idx)
        //{
        //    st += b.ToString() + '\n';
        //}
        //System.IO.File.WriteAllText("c:\\indice.txt", st);

		for ( k=0;k<Nb;k++)
			{
			
				indice = idx[k] ;				
				pts_prob_inter.SetPoint(k,courbe_prob.beta_rep.GetPoint(indice)[0],courbe_prob.beta_rep.GetPoint(indice)[1],courbe_prob.beta_rep.GetPoint(indice)[2]);

			}	

			/////////////////////////////						
			pts_prob = courbe_prob.Curve2q(pts_prob_inter,Nb-1);
			
			//////////////////////////////////////////////////////////////////////////////////////////
   		
			for(h=0 ; h<Nb; h++)
			{
				for(e=0;e<3;e++)
				{	
					dist += pts_prob.GetPoint(h)[e] * q1.operation(e,h);					
				}
			}
			distance = (float)dist / (float) Nb;
			//cout<<"distance "<<distance<<endl;
		//	distance = (float) Math.Acos(distance);	//cout<<"dist "<<resultat<<endl;

            if (Math.Abs(distance) >= 1) distance=0;
            else distance = (float)Math.Acos(distance);
			
           Face.dist_table[indxx] = distance;
	return idx;

}
        public void calculer_alpha_point(Curve c,int indxx)
        {
            c.alpha_dot_max = 0;
            
            int taille = c.q_rep.GetNumberOfPoints();
            var alpha_dot = new vtkPoints();
            alpha_dot.SetNumberOfPoints(taille);
            double theta = Face.dist_table[indxx];
            double w_x = 0, w_y = 0, w_z = 0, module_alpha_dot;
            for (int i = 0; i < taille; i++)
            {
                if (theta == 0)
                {
                    module_alpha_dot = 0;
                }
                else
                {
                    w_x = (theta/Math.Sin(theta))*(q_rep.GetPoint(i)[0] - Math.Cos(theta)*c.q_rep.GetPoint(i)[0]);
                    w_y = (theta/Math.Sin(theta))*(q_rep.GetPoint(i)[1] - Math.Cos(theta)*c.q_rep.GetPoint(i)[1]);
                    w_z = (theta/Math.Sin(theta))*(q_rep.GetPoint(i)[2] - Math.Cos(theta)*c.q_rep.GetPoint(i)[2]);

                    module_alpha_dot = Math.Sqrt(w_x*w_x + w_y*w_y + w_z*w_z);
                }
                alpha_dot.SetPoint(i, w_x, w_y, w_z);
               SharedValues.Sc.Add(module_alpha_dot);
               SharedValues.Vc.Add(new Vector3D(w_x, w_y, w_z));
                if (module_alpha_dot > c.alpha_dot_max)
                {
                    c.alpha_dot_max = module_alpha_dot;
                }
            }
            c.alpha_pt = alpha_dot;
        }
    
        public vtkPoints Curve2q(vtkPoints beta, double delta)
        {
            var q = new vtkPoints();
            q.SetNumberOfPoints(beta.GetNumberOfPoints());
            int i, j, n = Nb, k = 3, nb_colons = Nb, nb_lignes = 3, kl, lk;
            kl = Nb - 1;
            lk = Nb - 2;
            double len, norme, aa, bb;
            var inter = new double[3];
            for (j = 0; j < Nb; j++)
            {
                q.SetPoint(j, beta.GetPoint(j)[0], beta.GetPoint(j)[1], beta.GetPoint(j)[2]);
            }

            for (i = 0; i < 3; i++)
            {
                aa = beta.GetPoint(1)[i];
                bb = beta.GetPoint(0)[i];
                inter[i] = (aa - bb)*delta;
            }
            q.SetPoint(0, inter[0], inter[1], inter[2]);
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
                inter[i] = (aa - bb)*(delta);
            }
            q.SetPoint(kl, inter[0], inter[1], inter[2]);

            len = 0;
            for (j = 0; j < Nb; j++)
            {
                norme = 0;
                for (i = 0; i < 3; i++)
                {
                    norme += Math.Pow(q.GetPoint(j)[i], 2);
                }
                norme = Math.Sqrt(norme);
                len += norme;
            }
            len = len/Nb;
            for (j = 0; j < Nb; j++)
            {
                norme = 0;
                for (i = 0; i < 3; i++)
                {
                    if (i == 0) q.SetPoint(j, q.GetPoint(j)[0]/len, q.GetPoint(j)[1], q.GetPoint(j)[2]);
                    if (i == 1) q.SetPoint(j, q.GetPoint(j)[0], q.GetPoint(j)[1]/len, q.GetPoint(j)[2]);
                    if (i == 2) q.SetPoint(j, q.GetPoint(j)[0], q.GetPoint(j)[1], q.GetPoint(j)[2]/len);
                    norme += q.GetPoint(j)[i]*q.GetPoint(j)[i];
                }
                norme = Math.Sqrt(norme);
                double LL = Math.Sqrt(norme);
                if (LL > 0.0001) q.SetPoint(j, q.GetPoint(j)[0]/LL, q.GetPoint(j)[1]/LL, q.GetPoint(j)[2]/LL);
                q.SetPoint(j, q.GetPoint(j)[0]*LL, q.GetPoint(j)[1]*LL, q.GetPoint(j)[2]*LL);
            }
            return q;
        }

       public  double[] dp(qfunction q1, qfunction q2, int n, int dp_rad)
        {
            int NL = q1.size();
            double s = (double) (NL - 1)/(double) (n - 1);
            double[] E = new double[n*n];
            E[0] = 0;
            for (int i = 1; i < n*n; E[i++] = 1E9) ;
            int[] P = new int[n*n];
            double[] gamma=new double[Nb];
            for (int j = 1; j < n; j++)
            {
                for (int i = 1; i < n; i++)
                {
                    double best = 1E9;
                    for (int k = Math.Max(i-dp_rad,0); k < i; k++)
                    {
                        for (int l = Math.Max(j-dp_rad,0); l < j; l++)
                        {
                            if ((k==0 && l>0)||(k>0 && l==0)) continue;
                            double m = ((double) (j - l))/((double) (i - k));
                            double rootm = Math.Sqrt(m);
                            double curE = E[l*n + k];
                            for(int t=k;t<=i;t++)
                            {
                                int indx1 = nearbyint(t*s);
                                int indx2 = nearbyint((m*(t - k) + l)*s);
                                double dx = q1.operation(0, indx1) - q2.operation(0, indx2)*rootm;
                                double dy = q1.operation(1, indx1) - q2.operation(1, indx2) * rootm;
                                double dz = q1.operation(2, indx1) - q2.operation(2, indx2) * rootm;
                                curE += (dx*dx + dy*dy + dz*dz)/n;


                            }
                            if(curE <best)
                            {
                                E[j*n + i] = curE;
                                P[j*n + i] = l*n + k;
                                best = curE;

                            }
                        }
                        
                    }

                }
                
            }
            int hh, ee;
            gamma[0]=0;
            gamma[n - 1] = 1;
            int x2 = n-1  ;
            int y2 = n-1 ;
            while (x2>0)
            {
                int p = P[y2*n + x2];
                int x1 = p%(n);
                int y1 = p/(n);
                double m = (double) (y2 - y1)/(double) (x2 - x1);
                for (int t = x1; t < x2; t++)
                {
                    gamma[t] = (m*(t - x1) + y1)/(double) (n - 1);
                }
                x2 = x1;
                y2 = y1;
            }
            return gamma;
        }

        private int nearbyint(double r)
        {
            int a = (int) (r/1);
            int s = a;
            if ((r + 1 - a) > (r - a)) s = a + 1;
            return s;
        }

        public int[] elastique_curve(Curve c)
        {
            int[] tab = new int[50];
            double tamp=0;
            double tamp2 = 99;
            int index=0;
            for (int i = 0; i <tab.Length; i++)
            {
                tab[i] = 0;
            }

            tab[0] = 0;
            tab[49] = 49;
            int limit=6;
            for (int h = 1; h < Nb-1; h++)
            {
                var vector1 = new Vector3D(q_rep.GetPoint(h)[0], q_rep.GetPoint(h)[1], q_rep.GetPoint(h)[2]);
                if (h > 50 - limit) limit = 50 - h;
                for (int i = 0; i < limit; i++)
                {
                    var vector2 = new Vector3D(c.q_rep.GetPoint(h+i)[0], c.q_rep.GetPoint(h+i)[1], c.q_rep.GetPoint(h+i)[2]);
                   tamp = Math.Sqrt(Math.Pow((vector1.X - vector2.X),2) + Math.Pow((vector1.Y - vector2.Y),2) + Math.Pow((vector1.Z - vector2.Z),2));
                 //  tamp = Math.Sqrt(Math.Pow((vector2.X - vector1.X), 2) + Math.Pow((vector2.Y - vector1.Y), 2) + Math.Pow((vector2.Z - vector1.Z), 2));
                   // tamp = Vector3D.DotProduct(vector1, vector2);
                    if (tamp<tamp2)
                    {
                        index = h + i;
                    }
                    tamp2 = tamp;
                }
                tab[h] = index;
                tamp2 = 99;

                // dist += Vector3D.DotProduct(vector1, vector2);
            }



            string st = string.Empty;
            foreach (int b in tab)
            {
                st += b.ToString() + '\n';
            }
            //System.IO.File.WriteAllText("c:\\indice.txt", st);


            return tab;

        }
    }
}