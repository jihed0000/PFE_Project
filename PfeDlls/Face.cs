#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Kitware.VTK;

#endregion

namespace PFEProject
{
    public class Face
    {
        private const int Nb = 50;
        private const double Epsilon = 0.001;
        private readonly List<vtkActor> _actors = new List<vtkActor>();
        private readonly double[][] _matrix = new double[85][];
        private vtkActor _actor = vtkActor.New();
        private List<Curve> _collectionCurves = new List<Curve>();
        private vtkPolyDataMapper _mapper = vtkPolyDataMapper.New();
        private vtkPolyData _polyData = new vtkPolyData();
        private vtkRenderWindow _renderWindow = new vtkGenericOpenGLRenderWindow();
        private RenderWindowControl _renderWindowControl1 = new RenderWindowControl();
        private vtkRenderer _renderer = new vtkOpenGLRenderer();
        public double nbrecourbes = 100;

        public Face()
        {
        }

        public Face(vtkPolyData polydata)
        {
            PolyData = polydata;
        }

        public Face(vtkPolyData polydata, double[][] matrix)
        {
            PolyData = polydata;
            _matrix = matrix;
            noseLocation = Extract_nose_location();
        }

        public List<Curve> CollectionCurves
        {
            get { return _collectionCurves; }
            set { _collectionCurves = value; }
        }

        public vtkPolyDataMapper Mapper
        {
            get { return _mapper; }
            set { _mapper = value; }
        }

        public double[] noseLocation { get; set; }

        public vtkActor Actor
        {
            get { return _actor; }
            set { _actor = value; }
        }

        public vtkRenderWindow RenderWindow
        {
            get { return _renderWindow; }
            set { _renderWindow = value; }
        }

        public vtkRenderer Renderer
        {
            get { return _renderer; }
            set { _renderer = value; }
        }

        public vtkPolyData PolyData
        {
            get { return _polyData; }
            set { _polyData = value; }
        }

        public RenderWindowControl Render(RenderWindowControl renderWindowControl1)
        {
            _renderWindow = renderWindowControl1.RenderWindow;
            _renderer = _renderWindow.GetRenderers().GetFirstRenderer();
            Mapper.SetInput(_polyData);
            Actor.SetMapper(Mapper);
            _renderer.SetBackground(0.2, 0.3, 0.4);
            _renderer.AddActor(Actor);
            _renderer.ResetCamera();
            renderWindowControl1.Refresh();
            _renderWindowControl1 = renderWindowControl1;
            return renderWindowControl1;
        }


        public void RenderLandmarks(int i)
        {
            var _vtkSphere = vtkSphereSource.New();
            _vtkSphere.SetRadius(5);

            _vtkSphere.SetCenter(_matrix[i][0], _matrix[i][1], _matrix[i][2]);
            RenderSingleLandmarkObject(_vtkSphere);
        }

        public void Renderlandmarklocation(double[] location)
        {
            var _vtkSphere = vtkSphereSource.New();
            _vtkSphere.SetRadius(2);

            _vtkSphere.SetCenter(location[0], location[1], location[2]);
            RenderSingleLandmarkObject(_vtkSphere);
        }

        public void RenderSingleLandmarkObject(vtkSphereSource sphere)
        {
            if (_renderer.GetActors().GetNumberOfItems() != 1)
            {
                _renderer.RemoveActor(_renderer.GetActors().GetLastActor());
            }
            sphere.Update();
            var mapper = vtkPolyDataMapper.New();
            mapper.SetInputConnection(sphere.GetOutputPort());
            var actor = vtkActor.New();
            actor.SetMapper(mapper);

            // vtkRenderer renderer = _renderWindow.GetRenderers().GetFirstRenderer();
            actor.GetProperty().SetColor(1, 0, 0);
            _renderer.AddActor(actor);
            _renderWindow.Render();
            _renderWindowControl1.Refresh();
        }

        public void renderalllandmarks()
        {
            if (_renderer.GetActors().GetNumberOfItems() != 1)
            {
                _renderer.RemoveActor(_renderer.GetActors().GetLastActor());
            }
            for (int i = 1; i < 84; i++)
            {
                var _vtkSphere = vtkSphereSource.New();
                _vtkSphere.SetRadius(2);
                _vtkSphere.SetCenter(_matrix[i][0], _matrix[i][1], _matrix[i][2]);
                var mapper = vtkPolyDataMapper.New();
                mapper.SetInputConnection(_vtkSphere.GetOutputPort());
                var actor = vtkActor.New();
                actor.SetMapper(mapper);
                actor.GetProperty().SetColor(1, 0, 0);
                _actors.Add(actor);
            }
            foreach (var actor in _actors)
            {
                Renderer.AddActor(actor);
            }
            _renderWindow.Render();
        }

        public List<string> AddAllLandmarks()
        {
            var list = new List<string>();
            for (int i = 1; i < 84; i++)
            {
                list.Add(i + " " + _matrix[i][0] + " " + _matrix[i][1] + " " + _matrix[i][2]);
            }
            return list;
        }

        public double[] Extract_nose_location()
        {
            var coordinates = new double[3];
            var pd = new vtkPolyData();
            var sphere = new vtkSphere();
            double maxz;
            coordinates = MathOperations.GetMean(_matrix[42], _matrix[43]);
            sphere.SetRadius(15);
            sphere.SetCenter(coordinates[0], coordinates[1], coordinates[2]);
            var crop = new vtkClipPolyData();
            crop.SetInput(PolyData);
            crop.SetInsideOut(1);
            crop.SetClipFunction(sphere);
            crop.Update();
            pd = crop.GetOutput();
            var maxpoint = new double[3];
            maxpoint = pd.GetPoint(0);
            for (int i = 0; i < pd.GetNumberOfPoints(); i++)
            {
                if (pd.GetPoint(i)[2] > maxpoint[2])
                    maxpoint = pd.GetPoint(i);
            }


            return maxpoint;
        }

        public void rendernoselocation()
        {
            if (noseLocation == null) Extract_nose_location();
            Renderlandmarklocation(noseLocation);
        }

        public vtkPolyData PreProcessing()
        {
            var sphere = new vtkSphere();

            sphere.SetRadius(MathOperations.GetRadiuis(MathOperations.GetMean(_matrix[42], _matrix[43]), _matrix[83]));

            sphere.SetCenter(noseLocation[0], noseLocation[1], noseLocation[2]);

            var crop = new vtkClipPolyData();
            crop.SetInput(PolyData);
            crop.SetInsideOut(1);
            crop.SetClipFunction(sphere);
            crop.Update();
            PolyData = crop.GetOutput();

            var smoother = new vtkSmoothPolyDataFilter();
            smoother.SetInput(PolyData);
            smoother.SetNumberOfIterations(10);
            smoother.Update();
            PolyData = smoother.GetOutput();
            var translate = new vtkTransform();
            //if (MainScreen.Faces.Count != 0)
            //    PolyData = ICP(MainScreen.Faces[0]);


            var angels = new double[4];
            angels = MathOperations.YawPitchRolltoXYZ(_matrix[0][1], _matrix[0][0], _matrix[0][2]);
            translate.RotateWXYZ(angels[3], angels[0], angels[1], angels[2]);
            var transformfilter = new vtkTransformPolyDataFilter();
            transformfilter.SetInput(PolyData);
            transformfilter.SetTransform(translate);
            transformfilter.Update();
            PolyData = transformfilter.GetOutput();
            return PolyData;
        }

        public vtkPolyData ICP(Face refface)
        {
            var icp = new vtkIterativeClosestPointTransform();
            icp.SetSource(PolyData);
            icp.SetTarget(refface.PolyData);
            icp.StartByMatchingCentroidsOn();
            icp.SetMeanDistanceModeToRMS();
            icp.SetCheckMeanDistance(1);
            icp.SetMaximumMeanDistance(0.003);
            icp.SetMaximumNumberOfIterations(50);
            icp.SetMaximumNumberOfLandmarks(50);
            icp.GetLandmarkTransform().SetModeToRigidBody();
            icp.Modified();
            icp.Update();
            var filter = new vtkTransformPolyDataFilter();
            filter.SetInput(PolyData);
            filter.SetTransform(icp);
            filter.Modified();
            filter.Update();
            PolyData = filter.GetOutput();
            return PolyData;
        }

        public int NewMethodeextraction(int nbsections, float paramSpline, int N)
        {
            var nose1 = new double[3];
            nose1 = noseLocation;
            double theta = 0;
            var planhoriz = new vtkPlane();
            planhoriz.SetOrigin(nose1[0], nose1[1], nose1[2]);
            planhoriz.SetNormal(0, 1, 0);
            var demi_vis_inf = new vtkClipPolyData();
            var demi_vis_sup = new vtkClipPolyData();
            demi_vis_sup.SetInput(PolyData);
            demi_vis_sup.SetClipFunction(planhoriz);
            demi_vis_sup.SetInsideOut(0);
            demi_vis_sup.Update();
            demi_vis_inf.SetInput(PolyData);
            demi_vis_inf.SetClipFunction(planhoriz);
            demi_vis_inf.SetInsideOut(1);
            demi_vis_inf.Update();
            var vis_sup = new vtkPolyData();
            var vis_inf = new vtkPolyData();
            vis_sup = demi_vis_sup.GetOutput();
            vis_inf = demi_vis_inf.GetOutput();
            var planvert = new vtkPlane();
            planvert.SetOrigin(nose1[0], nose1[1], nose1[2]);
            planvert.SetNormal(Math.Cos(theta), Math.Sin(theta), 0);

            double anglep1 = Epsilon;

            var transfp1 = new vtkTransform();
            transfp1.RotateWXYZ(anglep1, 0, 0, 1);


            var cutter1 = new vtkCutter();
            cutter1.SetInput(vis_sup);
            cutter1.SetCutFunction(planvert);

            var cutter2 = new vtkCutter();
            cutter2.SetInput(vis_inf);
            cutter2.SetCutFunction(planvert);

            var stripper1 = new vtkStripper();
            stripper1.SetInput(cutter1.GetOutput());

            var stripper2 = new vtkStripper();
            stripper2.SetInput(cutter2.GetOutput());

            var spline1 = new vtkSplineFilter();
            spline1.SetInput(stripper1.GetOutput());
            spline1.SetSubdivideToLength();
            spline1.SetLength(0.01);
            var spline2 = new vtkSplineFilter();
            spline2.SetInput(stripper2.GetOutput());
            spline2.SetSubdivideToLength();
            spline2.SetLength(0.01);

            var ptMask1 = new vtkMaskPoints();
            ptMask1.SetInput(spline1.GetOutput()); //spline1.GetOutput()
            ptMask1.SetMaximumNumberOfPoints(N); //5  400
            ptMask1.GenerateVerticesOn(); // very important to see the points and

            var ptMask2 = new vtkMaskPoints();
            ptMask2.SetInput(spline2.GetOutput()); //spline2.GetOutput()
            ptMask2.SetMaximumNumberOfPoints(N); //5  400		
            ptMask2.GenerateVerticesOn(); // very important to see the points and

            int debut2;
            int vrif;
            int vrif1;
            double angle = 180/(double) nbsections;


            var appcourebsousechantillone1 = new vtkAppendPolyData();
            var appcourebsousechantillone2 = new vtkAppendPolyData();

            var new_normal = new double[4];
            vtkPoints Points3d, Points3d1, ptordonne1, ptordonne2;
            vtkCellArray Vertices, Vertices1, cells1, cells2;
            vtkPolyLine polyLine1, polyLine2;
            vtkPointLocator PointTree, PointTree1;
            vtkPolyData polydata, polydata1, polyDataordonne1, polyDataordonne2;


            var demi_profil_sup = new List<Curve>();
            var demi_profil_inf = new List<Curve>();

//_________________________________________DEBUT DE LA BOUCLE FOR___________________________________________________________________
//__________________________________________________________________________________________________________________________________

            for (int step = 0; step < nbsections; step++)
            {
                //// Code works but need Update + DeppCopy
                cutter1.Update();
                cutter2.Update();

                //cout<<"nb pt cutter1:			"<<cutter1.GetOutput().GetNumberOfPoints()<<endl;
                //cout<<"nb pt cutter2:			"<<cutter2.GetOutput().GetNumberOfPoints()<<endl;
                //cout<<"step:			"<<step<<endl;
                if ((cutter1.GetOutput().GetNumberOfPoints() == 0) || (cutter2.GetOutput().GetNumberOfPoints() == 0))
                    return 0;

                //_______  vtkStripper generate poly-lines from lines and segments ___         
                stripper1.Update();
                stripper2.Update();
                if ((stripper1.GetOutput().GetNumberOfPoints() == 0) || (stripper2.GetOutput().GetNumberOfPoints() == 0))
                    return 0;
                //_______ vtkSplineFilter pour le rechantillonage ____________
                spline1.Update();
                spline2.Update();
                if ((spline1.GetOutput().GetNumberOfPoints() < 100) || (spline2.GetOutput().GetNumberOfPoints() < 100))
                    return 0;

                //_____________________ downsampling ____________________________					
                // Create the tree
                /* vtkSmartPointer<vtkKdTreePointLocator> kDTree =  vtkSmartPointer<vtkKdTreePointLocator>::New();
					kDTree.SetDataSet(spline1.GetOutput());
				    kDTree.BuildLocator();
					 vtkSmartPointer<vtkKdTreePointLocator> kDTree2 =  vtkSmartPointer<vtkKdTreePointLocator>::New();
					kDTree2.SetDataSet(spline2.GetOutput());
				    kDTree2.BuildLocator();
					// Find the closest points to TestPoint
				  vtkIdType idn1 = kDTree.FindClosestPoint(nose1);
				   vtkIdType idn2 = kDTree.FindClosestPoint(nose1);*/


                int pas_sous_echant = spline1.GetOutput().GetNumberOfPoints()/N;
                ptMask1.SetOffset(0);
                ptMask1.SetOnRatio(pas_sous_echant);
                ptMask1.Modified();
                ptMask1.Update();

                int pas_sous_echant2 = spline2.GetOutput().GetNumberOfPoints()/N;
                ptMask2.SetOffset(0);
                ptMask2.SetOnRatio(pas_sous_echant2);
                ptMask2.Modified();
                ptMask2.Update();
                //__________________________________________________________________________________________
                //---------------------------------Ordonnancement---------------------------------------------------

                Points3d = new vtkPoints();
                polydata = new vtkPolyData();
                Vertices = new vtkCellArray();
                var pid = new int[1];
                for (int i = 0; i < ptMask1.GetOutput().GetNumberOfPoints(); i++)
                {
                    pid[0] = Points3d.InsertNextPoint(ptMask1.GetOutput().GetPoints().GetPoint(i)[0],
                        ptMask1.GetOutput().GetPoints().GetPoint(i)[1], ptMask1.GetOutput().GetPoints().GetPoint(i)[2]);
                    var hanss = GCHandle.Alloc(pid, GCHandleType.Pinned);
                    var pr = hanss.AddrOfPinnedObject();
                    Vertices.InsertNextCell(1, pr);
                }

                polydata.SetPoints(Points3d);
                polydata.SetVerts(Vertices);

                int ptId_nose1 = 0;
                var Result = new vtkIdList();
                PointTree = new vtkPointLocator();
                PointTree.SetDataSet(polydata);
                var h = GCHandle.Alloc(polydata.GetPoints().GetBounds(), GCHandleType.Pinned);
                var pointr = h.AddrOfPinnedObject();

                PointTree.InitPointInsertion(polydata.GetPoints(), pointr);
                PointTree.BuildLocator();
                vrif = PointTree.IsInsertedPoint(nose1[0], nose1[1], nose1[2]);
                PointTree.FindClosestNPoints(N, nose1[0], nose1[1], nose1[2], Result);

                //____________________________________________________________________________________

                Points3d1 = new vtkPoints();
                polydata1 = new vtkPolyData();
                Vertices1 = new vtkCellArray();
                var pid1 = new int[1];
                for (int i = 0; i < ptMask2.GetOutput().GetNumberOfPoints(); i++)
                {
                    pid1[0] = Points3d1.InsertNextPoint(ptMask2.GetOutput().GetPoints().GetPoint(i)[0],
                        ptMask2.GetOutput().GetPoints().GetPoint(i)[1], ptMask2.GetOutput().GetPoints().GetPoint(i)[2]);
                    var hh = GCHandle.Alloc(pid1, GCHandleType.Pinned);
                    var prr = hh.AddrOfPinnedObject();

                    Vertices1.InsertNextCell(1, prr);
                }

                polydata1.SetPoints(Points3d1);
                polydata1.SetVerts(Vertices1);

                var Result1 = new vtkIdList();
                int ptId_nose2 = 0;
                PointTree1 = new vtkPointLocator();
                PointTree1.SetDataSet(polydata1);
                var hhh = GCHandle.Alloc(polydata1.GetPoints().GetBounds(), GCHandleType.Pinned);
                var pa = hhh.AddrOfPinnedObject();
                PointTree1.InitPointInsertion(polydata1.GetPoints(), pa);
                //PointTree1.InsertUniquePoint(nose1,ptId_nose2);
                PointTree1.BuildLocator();
                vrif1 = PointTree1.IsInsertedPoint(nose1[0], nose1[1], nose1[2]);
                PointTree1.FindClosestNPoints(N, nose1[0], nose1[1], nose1[2], Result1);


                //________________________________Construct vtkpoints ordered ___________________________________________
                //________________________________ ordered sup ___________________________________________
                // Create a vtkPoints object and store the points in it

                ptordonne1 = new vtkPoints();

                int debut1;
                if (vrif < 0)
                {
                    ptordonne1.InsertNextPoint(nose1[0], nose1[1], nose1[2]);
                    debut1 = 1;
                }
                else debut1 = 0;
                for (int i = debut1; i < N; i++)
                    ptordonne1.InsertNextPoint(ptMask1.GetOutput().GetPoint(Result.GetId(i))[0],
                        ptMask1.GetOutput().GetPoint(Result.GetId(i))[1],
                        ptMask1.GetOutput().GetPoint(Result.GetId(i))[2]);

                polyLine1 = new vtkPolyLine();
                polyLine1.GetPointIds().SetNumberOfIds(N);
                for (int i = 0; i < N; i++)
                    polyLine1.GetPointIds().SetId(i, i);
                // Create a cell array to store the lines in and add the lines to it
                cells1 = new vtkCellArray();
                cells1.InsertNextCell(polyLine1);
                // Create a polydata to store everything in
                polyDataordonne1 = new vtkPolyData();
                // Add the points to the dataset
                polyDataordonne1.SetPoints(ptordonne1);
                // Add the lines to the dataset
                polyDataordonne1.SetLines(cells1);

                //________________________________ ordered inf ___________________________________________
                //problem-------------------------------------------------------------------
                ptordonne2 = new vtkPoints();
                if (vrif1 < 0)
                {
                    ptordonne2.InsertNextPoint(nose1[0], nose1[1], nose1[2]);
                    debut2 = 1;
                }
                else debut2 = 0;
                for (int i = debut2; i < N; i++)
                    ptordonne2.InsertNextPoint(ptMask2.GetOutput().GetPoint(Result1.GetId(i))[0],
                        ptMask2.GetOutput().GetPoint(Result1.GetId(i))[1],
                        ptMask2.GetOutput().GetPoint(Result1.GetId(i))[2]);
                polyLine2 = new vtkPolyLine();
                polyLine2.GetPointIds().SetNumberOfIds(N);
                for (int i = 0; i < N; i++)
                    polyLine2.GetPointIds().SetId(i, i);
                // Create a cell array to store the lines in and add the lines to it
                cells2 = new vtkCellArray();
                cells2.InsertNextCell(polyLine2);
                // Create a polydata to store everything in
                polyDataordonne2 = new vtkPolyData();
                // Add the points to the dataset
                polyDataordonne2.SetPoints(ptordonne2);
                // Add the lines to the dataset
                polyDataordonne2.SetLines(cells2);
                //cout<<"*********************** iccccccccccccccccccciiiiiiiiiiiiiiiiiiiiii******"<<endl;
//-------------------------------------Mettre les info dans la methode collection curves ---------------------------------------------------------


                // this.Collection_curves.push_back(tmp_result1);
                Curve cSup;

                cSup = new Curve(polyDataordonne1);
                cSup.beta_rep = polyDataordonne1.GetPoints();
                cSup.q_rep = cSup.Curve2q(cSup.beta_rep, Nb - 1);

                Curve cInf;
                cInf = new Curve(polyDataordonne2);
                cInf.beta_rep = polyDataordonne2.GetPoints();
                cInf.q_rep = cInf.Curve2q(cInf.beta_rep, Nb - 1);
                demi_profil_sup.Add(cSup);
                demi_profil_inf.Add(cInf);
                //rotation of the cutting plane around normal
                var ham = GCHandle.Alloc(planvert.GetNormal(), GCHandleType.Pinned);
                var prt = ham.AddrOfPinnedObject();
                var hamm = GCHandle.Alloc(new_normal, GCHandleType.Pinned);
                var prt2 = hamm.AddrOfPinnedObject();
                planvert.SetNormal(Math.Cos(theta), Math.Sin(theta), 0);
                theta += (2*Math.PI)/nbrecourbes;
                //   planvert.SetNormal(new_normal[0], new_normal[1], new_normal[2]);
            }
            /*       		cutter1.Delete();cutter2.Delete();stripper1.Delete();stripper1.Delete();stripper2.Delete();spline1.Delete();spline2.Delete();ptMask1.Delete();
		ptMask2.Delete();Points3d.Delete(); polydata.Delete();Vertices.Delete();Points3d1.Delete();
		polydata1.Delete();	Vertices1.Delete();polyDataordonne1.Delete();polyDataordonne1.Delete();ptordonne1.Delete();polyLine1.Delete();cells1.Delete();
		ptordonne2.Delete();polyLine2.Delete();cells2.Delete(); */


//__________________________________________________________________________________________________________________________________


            for (int i = 0; i < demi_profil_sup.Count; i++)
            {
                CollectionCurves.Add(demi_profil_sup[i]);
            }

            for (int i = 0; i < demi_profil_inf.Count; i++)
            {
                CollectionCurves.Add(demi_profil_inf[i]);
            }

            //demi_profil_sup.Curve::~Curve;
            //demi_profil_inf.Curve::~Curve;
            /*for(int i=0;i<this.Collection_curves.size();i++)
	{
		cout<<"nb(r de point beta ["<<i<<"]"<<this.Collection_curves[i].beta_rep.GetNumberOfPoints()<<endl;
		cout<<"nb(r de point q ["<<i<<"]"<<this.Collection_curves[i].q_rep.GetNumberOfPoints()<<endl;
	}*/

            return 1;
        }

        public void save_curves(string str)
        {
            string s = "";
            foreach (var c in CollectionCurves)
            {
                for (int i = 0; i < c.beta_rep.GetNumberOfPoints(); i++)
                {
                    s += c.beta_rep.GetPoint(i)[0] + "\t" + c.beta_rep.GetPoint(i)[1] + "\t" + c.beta_rep.GetPoint(i)[2] +
                         "\n";
                }
            }
            File.WriteAllText(str, s);
        }

        public void myextraction()
        {
            double theta = Math.PI/2;
            var plan = new vtkPlane();
            plan.SetOrigin(noseLocation[0], noseLocation[1], noseLocation[2]);
            plan.SetNormal(Math.Cos(theta), Math.Sin(theta), 0);
            var trans = new vtkTransform();

            var c = new vtkCutter();
            c.SetCutFunction(plan);
            c.SetInput(PolyData);

            var filter = new vtkTransformPolyDataFilter();
            filter.SetTransform(trans);
            filter.SetInput(c.GetOutput());
            c.Update();
            filter.Update();
            var vis_d = new vtkPolyData();
            vis_d = filter.GetOutput();

            var inputactor = new vtkActor();
            var inputmapper = vtkPolyDataMapper.New();
            if (_renderer.GetActors().GetNumberOfItems() != 1)
            {
                _renderer.RemoveActor(_renderer.GetActors().GetLastActor());
            }
            inputmapper.SetInput(vis_d);
            inputactor.SetMapper(inputmapper);
            inputactor.GetProperty().SetColor(1, 0, 0);
            Renderer.AddActor(inputactor);

            _renderWindow.Render();
        }

        public void affichage_courbe()
        {
            NewMethodeextraction((int) nbrecourbes/2, 0.01f, 50);
            var face_adjusted = vtkPolyDataMapper.New();
            face_adjusted.SetInput(PolyData);
            face_adjusted.ScalarVisibilityOff();
            var face_actor = new vtkActor();
            face_actor.SetMapper(face_adjusted);
            face_actor.GetProperty().SetColor(1, 0, 0);
            _renderWindow.GetRenderers().GetFirstRenderer().AddActor(face_actor);
            for (int kk = 0; kk < CollectionCurves.Count; kk++)
            {
                var inputdata = new vtkPolyData();
                var inputactor = new vtkActor();
                var inputmapper = vtkPolyDataMapper.New();
                var lines = new vtkCellArray();
                inputdata.SetPoints(CollectionCurves[kk].beta_rep);
                lines.InsertNextCell(inputdata.GetNumberOfPoints());
                for (int i = 0; i < inputdata.GetNumberOfPoints(); i++)
                    lines.InsertCellPoint(i);
                inputdata.SetLines(lines);
                //  lines.FastDelete();

                inputmapper.SetInput(inputdata);
                inputactor.SetMapper(inputmapper);
                inputactor.GetProperty().SetLineWidth(2);
                inputactor.GetProperty().SetColor(0, 1, 0);

                _actors.Add(inputactor);
            }
            if (_renderer.GetActors().GetNumberOfItems() != 1)
            {
                _renderer.RemoveActor(_renderer.GetActors().GetLastActor());
            }
            foreach (var actor in _actors)
            {
                Renderer.AddActor(actor);
            }
            _renderWindow.Render();
        }

        public vtkRenderer affiche_DSF(Face face)
        {
            PreProcessing();
            face.PreProcessing();
            NewMethodeextraction((int) nbrecourbes/2, 0.01f, 50);
            face.NewMethodeextraction((int) nbrecourbes/2, 0.01f, 50);
            int taille_courbe = CollectionCurves[0].beta_rep.GetNumberOfPoints();
            var points = new vtkPoints();
            var colors = new vtkUnsignedCharArray();
            int nbre_points = CollectionCurves.Count*
                              CollectionCurves[0].beta_rep.GetNumberOfPoints();
            colors.SetNumberOfComponents(3);
            double valeur_color;
            var scalars = new vtkIntArray();
            var couleurs = new double[5000];
            int cpt;
            var lut = new vtkLookupTable();
            lut.SetNumberOfColors(256);
            lut.SetHueRange(0.667, 0);
            lut.Build();
            var pts = new vtkPoints();
            var ca = new vtkCellArray();
            var pd = new vtkPolyData();
            var scalaires = new vtkIntArray();
            vtkDataArray vects = new vtkDoubleArray();


            int kk = 0;
            int jj = 0;
            for (int i = 0; i < CollectionCurves.Count; i++)
            {
                CollectionCurves[i].calculer_alpha_point(face.CollectionCurves[i]);
            }

            for (kk = 0; kk < CollectionCurves.Count; kk ++)
            {
                for (int i = 0; i < CollectionCurves[0].beta_rep.GetNumberOfPoints(); i++)
                {
                    pts.InsertNextPoint(CollectionCurves[kk].beta_rep.GetPoint(i)[0],
                        CollectionCurves[kk].beta_rep.GetPoint(i)[1],
                        CollectionCurves[kk].beta_rep.GetPoint(i)[2]);
                    ca.InsertNextCell(1);
                    ca.InsertCellPoint(jj);


                    scalaires.InsertNextTuple1(MainScreen.Sc[jj]);
                    // vects.InsertNextTuple3(MainScreen.Vc[jj].X, MainScreen.Vc[jj].Y, MainScreen.Vc[jj].Z);

                    jj++;
                }
            }
            pd.SetPoints(pts);
            pd.SetVerts(ca);
            pd.GetPointData().SetScalars(scalaires);


            var t = new vtkDelaunay2D();
            t.SetInput(pd);
            t.Update();
            var s = new vtkSmoothPolyDataFilter();
            s.SetInput(t.GetOutput());
            s.SetNumberOfIterations(0);
            s.Update();
            var arw = new vtkArrowSource();
            arw.SetTipLength(0.3);
            arw.SetTipResolution(18);
            arw.SetTipRadius(0.2);
            var glyp = new vtkGlyph3D();
            glyp.SetSource(arw.GetOutput());
            glyp.SetInput(s.GetOutput());
            glyp.SetInputArrayToProcess(0, 0, 0, 0, "scalaires");
            glyp.SetInputArrayToProcess(1, 0, 0, 0, "vects");
            glyp.SetVectorModeToUseVector();
            glyp.SetScaleFactor(1);
            glyp.SetColorModeToColorByVector();
            glyp.SetScaleModeToScaleByVector();
            glyp.OrientOn();
            glyp.Update();
            var m = vtkPolyDataMapper.New();
            m.SetLookupTable(lut);
            m.SetInput(s.GetOutput());
            m.SetScalarRange(MainScreen.Sc.Min(), MainScreen.Sc.Max());

            var colorbar = new vtkScalarBarActor();
            colorbar.SetLookupTable(lut);
            colorbar.SetWidth(0.05);
            colorbar.SetPosition(0.95, 0.1);
            colorbar.SetMaximumNumberOfColors(256);
            colorbar.SetNumberOfLabels(4);
            colorbar.PickableOff();
            colorbar.VisibilityOn();

            var a = new vtkActor();
            a.GetProperty().SetSpecular(0.3);
            a.GetProperty().SetSpecularPower(3);
            a.GetProperty().SetAmbient(0.1);
            a.GetProperty().SetDiffuse(0.8);
            a.GetProperty().SetInterpolationToGouraud();
            a.SetMapper(m);
            Renderer.AddActor(a);


            var m1 = vtkPolyDataMapper.New();

            m1.SetInput(glyp.GetOutput());

            // vtkActor a2 = new vtkActor();
            // a2.SetMapper(m1);
            //Renderer.AddActor(a2);
            Renderer.SetBackground(1, 1, 1);
            Renderer.AddActor(colorbar);
            return Renderer;
        }

        public vtkRenderer affiche_DSF(Face face, List<double> val)
        {
            //this.PreProcessing();
            //face.PreProcessing();
            //this.NewMethodeextraction((int)nbrecourbes / 2, 0.01f, 50);
            //face.NewMethodeextraction((int)nbrecourbes / 2, 0.01f, 50);
            int taille_courbe = CollectionCurves[0].beta_rep.GetNumberOfPoints();
            var points = new vtkPoints();
            var colors = new vtkUnsignedCharArray();
            int nbre_points = CollectionCurves.Count*
                              CollectionCurves[0].beta_rep.GetNumberOfPoints();
            colors.SetNumberOfComponents(3);
            double valeur_color;
            var scalars = new vtkIntArray();
            var couleurs = new double[5000];
            int cpt;
            var lut = new vtkLookupTable();
            lut.SetNumberOfColors(256);
            lut.SetHueRange(0.667, 0);

            //      lut.SetValueRange(val.Min(), val.Max());
            lut.Build();
            var pts = new vtkPoints();
            var ca = new vtkCellArray();
            var pd = new vtkPolyData();
            var scalaires = new vtkIntArray();
            vtkDataArray vects = new vtkDoubleArray();
            vtkRenderer ren = new vtkOpenGLRenderer();

            int kk = 0;
            int jj = 0;
            for (int i = 0; i < CollectionCurves.Count; i++)
            {
                CollectionCurves[i].calculer_alpha_point(face.CollectionCurves[i]);
            }

            for (kk = 0; kk < CollectionCurves.Count; kk++)
            {
                for (int i = 0; i < CollectionCurves[0].beta_rep.GetNumberOfPoints(); i++)
                {
                    pts.InsertNextPoint(CollectionCurves[kk].beta_rep.GetPoint(i)[0],
                        CollectionCurves[kk].beta_rep.GetPoint(i)[1],
                        CollectionCurves[kk].beta_rep.GetPoint(i)[2]);
                    ca.InsertNextCell(1);
                    ca.InsertCellPoint(jj);


                    scalaires.InsertNextTuple1(255 - 255*val[jj]);
                    // vects.InsertNextTuple3(MainScreen.Vc[jj].X, MainScreen.Vc[jj].Y, MainScreen.Vc[jj].Z);

                    jj++;
                }
            }
            pd.SetPoints(pts);
            pd.SetVerts(ca);
            pd.GetPointData().SetScalars(scalaires);


            var t = new vtkDelaunay2D();
            t.SetInput(pd);
            t.Update();
            var s = new vtkSmoothPolyDataFilter();
            s.SetInput(t.GetOutput());
            s.SetNumberOfIterations(0);
            s.Update();

            var m = vtkPolyDataMapper.New();
            m.SetLookupTable(lut);
            m.SetScalarRange(val.Min(), val.Max());
            m.SetInput(s.GetOutput());

            var p = new vtkTextProperty();
            p.SetColor(1, 0, 0);
            p.SetFontSize(32);
            var colorbar = new vtkScalarBarActor();
            colorbar.SetLookupTable(lut);
            colorbar.SetWidth(0.05);
            colorbar.SetPosition(0.95, 0.1);
            colorbar.SetMaximumNumberOfColors(256);
            colorbar.SetNumberOfLabels(4);
            colorbar.SetLabelTextProperty(p);
            colorbar.PickableOff();
            colorbar.VisibilityOn();
            colorbar.SetOrientation(1);
            var a = new vtkActor();
            a.GetProperty().SetSpecular(0.3);
            a.GetProperty().SetSpecularPower(3);
            a.GetProperty().SetAmbient(0.1);
            a.GetProperty().SetDiffuse(0.8);
            a.GetProperty().SetInterpolationToGouraud();
            a.SetMapper(m);
            ren.AddActor(a);


            // vtkActor a2 = new vtkActor();
            // a2.SetMapper(m1);
            //Renderer.AddActor(a2);
            ren.SetBackground(1, 1, 1);
            ren.AddActor(colorbar);
            return ren;
        }
    }
}