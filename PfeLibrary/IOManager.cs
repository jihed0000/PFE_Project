using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Media.Media3D;
using Kitware.VTK;
using Microsoft.VisualBasic.FileIO;

namespace PFEProject
{
    public static class IoManager
    {
        private static  vtkRenderer renderer = vtkRenderer.New();
        private static vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
        private static vtkRenderWindow renderWindow = vtkRenderWindow.New();
        private static vtkActor actor = vtkActor.New();
       
        

        public static vtkPolyData ReadObj(string filename)
        {
            vtkOBJReader reader = vtkOBJReader.New();
            reader.SetFileName(filename);
            reader.Update();
            return reader.GetOutput();
        }

        public static vtkPolyData ReadVrml(string filename)
        {
            vtkVRMLImporter reader = vtkVRMLImporter.New();
            reader.SetFileName(filename);
            reader.Read();
            
            return (vtkPolyData) reader.GetRenderer().GetActors().GetLastActor().GetMapper().GetInputAsDataSet();
        }

        public static vtkPolyData readxyz(string filename)
        {
            vtkSimplePointsReader reader = new vtkSimplePointsReader();
            reader.SetFileName(filename);
            reader.Update();
            return reader.GetOutput();
        }
        public static void SaveObj(string filename, vtkPolyData data)
        {
            mapper.SetInput(data);
            actor.SetMapper(mapper);
            renderer.AddActor(actor);
            renderWindow.AddRenderer(renderer);
            vtkOBJExporter exporter = vtkOBJExporter.New();
            exporter.SetInput(renderWindow);
            exporter.SetFilePrefix(filename.Remove(filename.IndexOf('.')));
            exporter.Update();


        }

        public static void SaveVrml(string filename,vtkPolyData data)
        {mapper.SetInput(data);
            actor.SetMapper(mapper);
            renderer.AddActor(actor);
            renderWindow.AddRenderer(renderer);
            vtkVRMLExporter vrmlexporter = vtkVRMLExporter.New();
            vrmlexporter.SetInput(renderWindow);
            vrmlexporter.SetFileName(filename);
            vrmlexporter.Update();
        }
        


        public static double[][] TbndParser(string filename)
        {
            var cultureInfo = new System.Globalization.CultureInfo("en-US");
            int i=0,j = 0;
            var d = new double[85][];
            var parser = new TextFieldParser(filename) {TextFieldType = FieldType.Delimited};
            parser.SetDelimiters(" ");
            while (!parser.EndOfData)
            {
                string[] fields = parser.ReadFields();
                if (fields != null)
                    d[i] = new double[3];
                    foreach (string field in fields)
                    {
                        
                        d[i][j] = double.Parse(field,cultureInfo);
                        j++;
                    }
                j = 0;
                i++;
            }
            
            return d;
        }

        public static double[][] LoadDvf(string filename)
        {
            var cultureInfo = new System.Globalization.CultureInfo("en-US");
            int i = 0, j = 0;
            var d = new double[5000][];
            var parser = new TextFieldParser(filename) { TextFieldType = FieldType.Delimited };
            parser.SetDelimiters(" ");
            while (!parser.EndOfData)
            {
                string[] fields = parser.ReadFields();
                if (fields != null)
                    d[i] = new double[3];
                foreach (string field in fields)
                {

                    d[i][j] = double.Parse(field, cultureInfo);
                    j++;
                }
                j = 0;
                i++;
            }

            return d;
        }

        public static List<Vector3D> LoadDvfdata(string filename)
        {
           List<Vector3D> ls = new List<Vector3D>();
            var cultureInfo = new System.Globalization.CultureInfo("en-US");
            int  j = 0;
            
            var parser = new TextFieldParser(filename) { TextFieldType = FieldType.Delimited };
            parser.SetDelimiters(" ");
            while (!parser.EndOfData)
            {
                string[] fields = parser.ReadFields();
               double[] d = new double[3];
                foreach (string field in fields)
                {

                    d[j] = double.Parse(field);
                    j++;
                }
                j = 0;
                Vector3D v = new Vector3D(d[0], d[1], d[2]);
                ls.Add(v);
               
            }

            return ls;
        }

       
        public static Face filetocurve(string file)
        {
            Face f = new Face();
            
            vtkPolyData polydata = new vtkPolyData();
           vtkPoints pts = new vtkPoints();

            CultureInfo cultureInfo = new System.Globalization.CultureInfo("en-US");
            int i = 0, j = 0;
    
           var parser = new TextFieldParser(file) { TextFieldType = FieldType.Delimited };
           parser.SetDelimiters("\t");
           var ca = new vtkCellArray();
         //   double[] d = new double[3];
            while (!parser.EndOfData)
           {
               string[] fields = parser.ReadFields();
               if (fields != null)


                   pts.InsertNextPoint(double.Parse(fields[0],cultureInfo), double.Parse(fields[1],cultureInfo), double.Parse(fields[2],cultureInfo));
                 //  d[i][j] = double.Parse(field, cultureInfo);
               ca.InsertNextCell(1);
               ca.InsertCellPoint(i);
               i++;
           }
            for (int ii=0; i < 100; i++)
            {f.CollectionCurves[ii] = new Curve();
                for (int jj=0; j < 50; j++)
                { string[] fields = parser.ReadFields();
                f.CollectionCurves[ii].beta_rep.SetPoint(jj, double.Parse(fields[0], cultureInfo), double.Parse(fields[1], cultureInfo), double.Parse(fields[2], cultureInfo));
                }
            }
         //   pd.SetPoints(pts);
       
          //  pd.GetPointData().SetScalars(scalaires);
         
            f.PolyData.SetPoints(pts);
            f.PolyData.SetVerts(ca);





            return f;
        }
        public static Dictionary<int, List<int>> csvparser(string file)
        {int i=0;
            string textline = string.Empty;
            string[] splitline;
            Dictionary<int, List<int>> dict = new Dictionary<int, List<int>>();
           
            if (System.IO.File.Exists(file))
            {
                System.IO.StreamReader cvsreader = new StreamReader(file);

                do
                {
                    List<int> vals = new List<int>();
                    textline = cvsreader.ReadLine();
                    if(textline != string.Empty && i!=0)
                    {
                        int j = 0;
                        splitline = textline.Split(',');
                        int frame = 0;
                        foreach (string s in splitline)
                        {
                           
                            int val=int.Parse(s);
                            if (j == 0)
                            {
                                frame = val;
                            }
                            else
                            {
                                if (val == 1)
                                {
                                    vals.Add(j);
                                }
                                
                            


                            }

                            j++;
                        }
                        dict.Add(frame, vals);
                        
                    }


                    i++;
                } while (cvsreader.Peek() != -1);
            }



            return dict;
        } 

    }
}
