using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroCopyPointCreation
{
    public class CameraFrame
    {
        public ushort[] RawData { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
