﻿using Cloo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroCopyPointCreation
{
    public static class GPUComputePoints
    {
        private static bool _initialized = false;
        private static ComputePlatform _integratedIntelGPUPlatform;
        private static ComputeContext _context;
        private static ComputeCommandQueue _commandQueue;
        private static ComputeProgram _program;
        private static ComputeKernel _kernel;
        private static ComputeBuffer<ushort> _inputBuffer;
        private static ComputeBuffer<Point3D> _outputBuffer;
        private static Point3D[] _ret;

        private static void OneTimeSetup(CameraFrame frame)
        {
            if(!_initialized)
            {
                // get the intel integrated GPU
                _integratedIntelGPUPlatform = ComputePlatform.Platforms.Where(n => n.Name.Contains("Intel")).First();

                // create the compute context. 
                _context = new ComputeContext(
                    ComputeDeviceTypes.Gpu, // use the gpu
                    new ComputeContextPropertyList(_integratedIntelGPUPlatform), // use the intel openCL platform
                    null,
                    IntPtr.Zero);

                // the command queue is the, well, queue of commands sent to the "device" (GPU)
                _commandQueue = new ComputeCommandQueue(
                    _context, // the compute context
                    _context.Devices[0], // first device matching the context specifications
                    ComputeCommandQueueFlags.None); // no special flags

                string kernelSource = null;
                using (StreamReader sr = new StreamReader("kernel.cl"))
                {
                    kernelSource = sr.ReadToEnd();
                }

                // create the "program"
                _program = new ComputeProgram(_context, new string[] { kernelSource });

                // compile. 
                _program.Build(null, null, null, IntPtr.Zero);
                _kernel = _program.CreateKernel("ComputePoints");

                _inputBuffer = new ComputeBuffer<ushort>(_context,
                    ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer,
                    frame.RawData);
                _kernel.SetMemoryArgument(0, _inputBuffer);

                _ret = new Point3D[frame.Width * frame.Height];
                _outputBuffer = new ComputeBuffer<Point3D>(_context,
                    ComputeMemoryFlags.WriteOnly | ComputeMemoryFlags.UseHostPointer,
                    _ret);
                _kernel.SetMemoryArgument(1, _outputBuffer);

                _initialized = true; 
            }
        }

        public static Point3D[] ComputePoints(CameraFrame frame, float[] M, float[] b)
        {
            OneTimeSetup(frame);

            IntPtr retPtr2 = _commandQueue.Map(
                _inputBuffer,
                true,
                ComputeMemoryMappingFlags.Write,
                0,
                frame.RawData.Length, null);

            _commandQueue.Unmap(_inputBuffer, ref retPtr2, null);

            ComputeBuffer<float> mBuffer = new ComputeBuffer<float>(_context,
                ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer,
                M);
            _kernel.SetMemoryArgument(2, mBuffer);

            ComputeBuffer<float> bBuffer = new ComputeBuffer<float>(_context,
                ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer,
                b);
            _kernel.SetMemoryArgument(3, bBuffer);
            _kernel.SetValueArgument<int>(4, frame.Width);

            _commandQueue.Execute(_kernel,
                new long[] { 0 },
                new long[] { frame.Width, frame.Height },
                null, null);

            IntPtr retPtr = _commandQueue.Map(
                _outputBuffer,
                true,
                ComputeMemoryMappingFlags.Read,
                0,
                _ret.Length, null);

            _commandQueue.Unmap(_outputBuffer, ref retPtr, null);

            mBuffer.Dispose();
            bBuffer.Dispose(); 

            return _ret; 
        }
    }
}
