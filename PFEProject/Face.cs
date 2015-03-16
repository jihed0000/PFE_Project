using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kitware.VTK;

namespace PFEProject
{
  public class Face
    {
        vtkPolyData _polyData = new vtkPolyData();
        vtkPolyDataMapper _mapper = vtkPolyDataMapper.New();
        vtkActor _actor = vtkActor.New();
        vtkRenderWindow _renderWindow = new vtkGenericOpenGLRenderWindow();
        vtkRenderer _renderer = new vtkOpenGLRenderer();


      public Face(vtkPolyData polydata)
      {
          this._polyData = polydata;
      }




















    }
}
