using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroCopyPointCreation
{
    public static class MockCamera
    {
        const int Width = 512;
        const int Height = 424;

        public static CameraFrame GetNextFrame()
        {
            Random rand = new Random(1234);
            CameraFrame ret = new CameraFrame()
            {
                Width = Width,
                Height = Height,
                RawData = new ushort[Width * Height]
            };
            for (int c = 0; c < Width * Height; c++)
            {
                ret.RawData[c] = (ushort)(rand.Next() % 5000); 
            }
            return ret; 
        }
    }
}
