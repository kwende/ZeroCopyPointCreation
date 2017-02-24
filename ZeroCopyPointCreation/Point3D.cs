using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroCopyPointCreation
{
    public struct Point3D
    {
        public float X;
        public float Y;
        public float Z; 

        public static bool operator ==(Point3D lhs, Point3D rhs)
        {
            return (lhs.X == rhs.X &&
                lhs.Y == rhs.Y &&
                lhs.Z == rhs.Z); 
        }

        public static bool operator !=(Point3D lhs, Point3D rhs)
        {
            return !(lhs == rhs); 
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"<{this.X}, {this.Y}, {this.Z}>"; 
        }
    }
}
