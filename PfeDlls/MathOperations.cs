using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Kitware.VTK;

namespace PFEProject
{
    public static class MathOperations
    {




        public static double[] YawPitchRolltoXYZ(double yaw,double pitch,double roll)
        {
            double [] result =new double[4];
            double rollovertwo = roll*0.5;
            double sinrollovertwo = Math.Sin(rollovertwo);
            double cosrollovertwo = Math.Cos(rollovertwo);
            double pitchovertwo =  pitch * 0.5;
            double sinpitchlovertwo = Math.Sin( pitchovertwo);
            double cospitchovertwo = Math.Cos( pitchovertwo);
            double ryawovertwo = yaw * 0.5;
            double sinryawovertwo = Math.Sin(ryawovertwo);
            double cosyawlovertwo = Math.Cos(ryawovertwo);

            result[0] = cosyawlovertwo*cospitchovertwo*cosrollovertwo + sinpitchlovertwo*sinryawovertwo*sinrollovertwo;
            result[1] = cosyawlovertwo * cospitchovertwo * sinrollovertwo - sinpitchlovertwo * sinryawovertwo * cosrollovertwo;
            result[2] = cosyawlovertwo * sinpitchlovertwo * cosrollovertwo + cospitchovertwo * sinryawovertwo * sinrollovertwo;
            result[3] = sinryawovertwo*cospitchovertwo*cosrollovertwo-cosyawlovertwo*sinpitchlovertwo*sinrollovertwo;



            return result;

        }

        public static double GetRadiuis(double[] p1,double[] p2)
        {
            return Math.Sqrt(p1.Zip(p2, (a, b) => (a - b)*(a - b)).Sum());
        }
        public static double[] GetMean (double[] p1 , double[] p2)
    {
            double[] result = new double[3];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = (p1[i] + p2[i])/2;
            }
            return result;
    }
        
    }
}
