using ManagedCuda;
using ManagedCuda.BasicTypes;
using System;

namespace GPU
{
    class Program
    {
        static CudaKernel _addWithCuda;

        static void InitKernels()
        {
            CudaContext cuContext = new CudaContext();
            CUmodule cuModule = cuContext.LoadModule(@"C:\Users\w0ns88\Desktop\pba\pa\p-exam\MatrixMultiplication\CUDA\Debug\kernel.ptx");
            _addWithCuda = new CudaKernel("_Z6kerneliiPi", cuModule, cuContext);
        }

        static Func<int, int, int> cudaAdd = (a, b) =>
        {
            // init output parameters
            CudaDeviceVariable<int> resultDev = 0;
            int resultHost = 0;
            // run CUDA method
            _addWithCuda.Run(a, b, resultDev.DevicePointer);
            // copy return to host
            resultDev.CopyToHost(ref resultHost);
            return resultHost;
        };

        static void Main()
        {
            InitKernels();
            Console.WriteLine(cudaAdd(3, 10));
            Console.ReadKey();
        }
    }
}
