using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Oswietlenie
{
    abstract class Mesh
    {
        protected int n = 10;       //ilosc segmentow
        protected int r = 20;       //promien
        protected Point3DCollection points;
        protected Int32Collection triangleIndices;

        public virtual int Radius
        {
            get
            { 
                return r; 
            }
            set 
            { 
                r = value; 
                CalculateGeometry(); 
            }
        }
        public virtual int Separators
        {
            get
            {
                return n;
            }
            set
            {
                n = value;
                CalculateGeometry();
            }
        }
        public virtual Point3DCollection Points
        {
            get 
            {
                return points;
            }
        }
        public virtual Int32Collection TriangleIndices
        {
            get
            {
                return triangleIndices;
            }
        }

        protected abstract void CalculateGeometry();
    }
}
