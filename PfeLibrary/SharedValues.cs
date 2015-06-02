using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace PfeLibrary
{
    public static class SharedValues
    {
        public static List<double> Sc = new List<double>();
        public static List<Vector3D> Vc = new List<Vector3D>();
        public static Dictionary<int, int> AUS = new Dictionary<int, int>();

        public static Dictionary<string, Dictionary<int, List<int>>> AUCollection =
            new Dictionary<string, Dictionary<int, List<int>>>();

    }
}