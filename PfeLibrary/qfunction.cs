using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PfeLibrary
{
    public class qfunction
    {
        private double[] _x;
        private double[] _y;
        private double[] _z;
        private int _len;

        public qfunction(int n)
        {
            if (n > 0)
            {
                this._x = new double[n];
                this._y = new double[n];
                this._z = new double[n];
                this._len = n;
            }
            else
            {
                this._len = 0;
            }
        }

        public qfunction(double[] x, double[] y, double[] z, int n)
        {
            if (x != null & y != null & z != null & n > 0)
            {
                this._x = new double[n];
                this._y = new double[n];
                this._z = new double[n];
                this._len = n;



                this._x = x;
                this._y = y;
                this._z = z;

            }
            else
            {
                this._len = 0;
            }
        }
        public qfunction(qfunction q)
        {
            this._len = q._len;
            if (q._len > 0)
            {
                this._x = new double[q._len];
                this._y = new double[q._len];
                this._z = new double[q._len];
                this._len = q._len;



                this._x = q._x;
                this._y = q._y;
                this._z = q._z;

            }
            
        }
  
        public int size()
        {
            return this._len;
        }

        public double operation(int xyz,int i)
        {
            if (i>=0 && i<this._len)
            {
                if (xyz == 0)
                {
                    return this._x[i];
                }
                else
                {
                    if (xyz == 2) return this._z[i];
                    else return this._y[i];
                }
            }
            return 0;
        }


    }
}
