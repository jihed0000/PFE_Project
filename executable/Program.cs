using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PFEProject;

namespace executable
{
    class Program
    {
        static void Main(string[] args)
        {

           string  ex = args[0];
            ex = ex.Replace(args[2], args[3]);
            Face f = new Face(IoManager.ReadObj(args[0]), IoManager.TbndParser(args[1]));
            //  MainScreen.Faces.Add(f);
          
            f.PreProcessing();
            f.NewMethodeextraction(100, 0.02f, 50);

        
            double[] d = f.noseLocation;
            string nose = d[0] + " " + d[1] + " " + d[2] + "\n";
            string p = ex.Replace("objects", "features");
            p = p.Replace(".obj", ".nose");
            File.WriteAllText(p, nose);
           
            p = p.Replace(".nose", ".curve");
            f.save_curves(p);
            IoManager.SaveObj(ex, f.PolyData);
            f = null;






        }
    }
}
