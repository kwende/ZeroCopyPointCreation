using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroCopyPointCreation
{
    public static class CPUComputePoints
    {
        public static Point3D[] ComputePoints(CameraFrame frame, float[] M, float[] b)
        {
            Point3D[] ret = new Point3D[frame.Height * frame.Width]; 

            for (int y = 0, i = 0; y < frame.Height; y++)
            {
                for (int x = 0; x < frame.Width; x++, i++)
                {
                    ushort z = frame.RawData[i];

                    float newX = x * M[0] + y * M[1] + z * M[2] + b[0];
                    float newY = x * M[3] + y * M[4] + z * M[5] + b[1];
                    float newZ = x * M[6] + y * M[7] + z * M[8] + b[2];

                    ret[i] = new Point3D { X = newX, Y = newY, Z = newZ }; 
                }
            }

            return ret; 
        }
    }
}
