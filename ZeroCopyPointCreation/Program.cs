using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroCopyPointCreation
{
    class Program
    {
        const int NumberOfIterations = 1000;

        static void Main(string[] args)
        {
            long msForCpu = 0, msForGpu = 0;

            Stopwatch cpuSw = new Stopwatch();
            Stopwatch gpuSw = new Stopwatch();

            Random rand = new Random();
            for (int c = 0; c < NumberOfIterations; c++)
            {
                // build the silly (not meaningful #s used) matrices for the affine transformation. 
                // in production these can change, so I'm simulating that here. 
                float[] sillyM = new float[9];
                float[] sillyb = new float[3];
                for (int d = 0; d < 3; d++)
                {
                    sillyb[d] = rand.Next() % 10 + 1;

                    sillyM[d * 3] = rand.Next() % 10 + 1;
                    sillyM[d * 3 + 1] = rand.Next() % 10 + 1;
                    sillyM[d * 3 + 2] = rand.Next() % 10 + 1;
                }

                CameraFrame currentFrame = MockCamera.GetNextFrame();

                // increment performance stats for GPU point computation. 
                gpuSw.Start();
                Point3D[] gpuGeneratedPoints = GPUComputePoints.ComputePoints(currentFrame, sillyM, sillyb);
                gpuSw.Stop();
                msForGpu += gpuSw.ElapsedMilliseconds;
                gpuSw.Reset();

                // increment performance stats for CPU point computation. 
                cpuSw.Start();
                Point3D[] cpuGeneratedPoints = CPUComputePoints.ComputePoints(currentFrame, sillyM, sillyb);
                cpuSw.Stop();
                msForCpu += cpuSw.ElapsedMilliseconds;
                cpuSw.Reset();

                // verify they're correct. 
                if (gpuGeneratedPoints.Length != cpuGeneratedPoints.Length)
                {
                    throw new Exception("Mismatched number of points.");
                }

                for (int d = 0; d < gpuGeneratedPoints.Length; d++)
                {
                    Point3D gpuGenerated = gpuGeneratedPoints[d];
                    Point3D cpuGenerated = cpuGeneratedPoints[d];

                    if (gpuGenerated != cpuGenerated)
                    {
                        throw new Exception("Points aren't equal.");
                    }
                }
            }

            Console.WriteLine($"CPU: average of {msForCpu / (NumberOfIterations * 1.0f)}ms");
            Console.WriteLine($"GPU: average of {msForGpu / (NumberOfIterations * 1.0f)}ms");
            Console.WriteLine($"GPU is {msForCpu / (msForGpu * 1.0f)}x faster.");
            Console.ReadLine();
        }
    }
}
