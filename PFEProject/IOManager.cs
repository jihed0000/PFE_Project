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

    }
}
