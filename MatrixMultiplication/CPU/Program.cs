using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CPU
{
    class Program
    {
        private enum MatrixType { A, B };
        private enum Opperations { Naive, CacheFriendly, ParallelNaive, ParallelCacheFriendly };

        private const int N = 1000;

        static void Main()
        {
            Console.WriteLine("Initializing matrices A and B...\r\n");
            double[][] A = InitializeMatrix(N, N, MatrixType.A);
            double[][] B = InitializeMatrix(N, N, MatrixType.B);

            Console.WriteLine("Choose operation:\r\n1: Naive\r\n2: Cache Friendly\r\n3: Parallel Naive\r\n4: Parallel Cache Friendly\r\n");
            
            while (true)
            {
                var option = Console.ReadKey(true);
                if (option.Key.Equals(ConsoleKey.D1) ||
                    option.Key.Equals(ConsoleKey.D2) ||
                    option.Key.Equals(ConsoleKey.D3) ||
                    option.Key.Equals(ConsoleKey.D4))
                {
                    try
                    {
                        Console.WriteLine("Calculating Matrix Multiplication on C = A * B:");
                        Operation(A, B, UserOption(option.Key));
                        break;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        break;
                    }
                }
                else
                {
                    Console.WriteLine("Not a valid input. Please pick an operation by typing 1-4.");
                }
            }
        }

        private static Opperations UserOption(ConsoleKey input)
        {
            switch (input)
            {
                case ConsoleKey.D1:
                    return Opperations.Naive;
                case ConsoleKey.D2:
                    return Opperations.CacheFriendly;
                case ConsoleKey.D3:
                    return Opperations.ParallelNaive;
                case ConsoleKey.D4:
                    return Opperations.ParallelCacheFriendly;
                default:
                    return Opperations.Naive;
            }
        }

        private static void Operation(double[][] A, double[][] B, Opperations option)
        {
            switch (option)
            {
                case Opperations.Naive:
                    NaiveMatrixMultiplication(A, B);
                    break;
                case Opperations.CacheFriendly:
                    CacheFriendlyMatrixMultiplication(A, B);
                    break;
                case Opperations.ParallelNaive:
                    ParallelNaiveMatrixMultiplication(A, B);
                    break;
                case Opperations.ParallelCacheFriendly:
                    ParallelCacheFriendlyMatrixMultiplication(A, B);
                    break;
                default:
                    NaiveMatrixMultiplication(A, B);
                    break;
            }
        }

        private static double[][] InitializeMatrix(int rows, int cols, MatrixType type)
        {
            double[][] tmp = new double[rows][];
            for (int i = 0; i < N; i++)
            {
                tmp[i] = new double[cols];
                for (int j = 0; j < cols; j++)
                    tmp[i][j] = (type == MatrixType.A ? i + 1 : j + 1);
            }
            return tmp;
        }

        private static void NaiveMatrixMultiplication(double[][] A, double[][] B)
        {
            Console.Write("Naive Matrix Multiplication:");
            Stopwatch stopwatch = Stopwatch.StartNew();

            // init Matrix C
            int rows = A.Length;
            int cols = B[0].Length;

            // Jagged Array
            double[][] C = new double[rows][];
            for (int row = 0; row < rows; row++)
                C[row] = new double[cols];

            // Matrix Multiplication
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    // Set C's values
                    //C[i][j] = 0.0;

                    for (int k = 0; k < N; k++)
                    {
                        C[i][j] = (A[i][k] * B[k][j]);
                    }
                }
            }

            stopwatch.Stop();
            var time = stopwatch.ElapsedMilliseconds;

            Console.WriteLine(" Elapsed Time = {0:f3} seconds.", time / 1000d);
        }

        private static void CacheFriendlyMatrixMultiplication(double[][] A, double[][] B)
        {
            Console.Write("Chache Friendly Matrix Multiplication:");
            Stopwatch stopwatch = Stopwatch.StartNew();

            // init Matrix C
            int rows = A.Length;
            int cols = B[0].Length;

            // Jagged Array
            double[][] C = new double[rows][];
            for (int row = 0; row < rows; row++)
                C[row] = new double[cols];

            // Matrix Multiplication
            for (int i = 0; i < N; i++)
            {
                for (int k = 0; k < N; k++)
                {
                    // Set C's values
                    //C[i][j] = 0.0;

                    for (int j = 0; j < N; j++)
                    {
                        C[i][j] = (A[i][k] * B[k][j]);
                    }
                }
            }

            stopwatch.Stop();
            var time = stopwatch.ElapsedMilliseconds;

            Console.WriteLine(" Elapsed Time = {0:f3} seconds.", time / 1000d);
        }

        private static void ParallelNaiveMatrixMultiplication(double[][] A, double[][] B)
        {
            Console.Write("Parallel Naive Matrix Multiplication:");
            Stopwatch stopwatch = Stopwatch.StartNew();

            // Init Matrix C
            int rows = A.Length;
            int cols = B[0].Length;
            int intermediate = A[0].Length;

            // Jagged Array
            double[][] C = new double[rows][];
            for (int row = 0; row < rows; row++)
                C[row] = new double[cols];
            
            // Matrix Multiplication
            Parallel.For(0, N, (i) =>
            {
                for (int j = 0; j < intermediate; j++)
                {
                    for (int k = 0; k < cols; k++)
                    {
                        C[i][j] += (A[i][k] * B[k][j]);
                    }
                }
            });

            stopwatch.Stop();
            long time = stopwatch.ElapsedMilliseconds;

            Console.WriteLine(" Elapsed Time = {0:f3} seconds.", time / 1000d);
        }

        private static void ParallelCacheFriendlyMatrixMultiplication(double[][] A, double[][] B)
        {
            Console.Write("Parallel Cache Friendly Matrix Multiplication:");
            Stopwatch stopwatch = Stopwatch.StartNew();

            // Init Matrix C
            int rows = A.Length;
            int cols = B[0].Length;
            int intermediate = A[0].Length;

            // Jagged Array
            double[][] C = new double[rows][];
            for (int row = 0; row < rows; row++)
                C[row] = new double[cols];

            // Matrix Multiplication
            Parallel.For(0, N, (i) =>
            {
                for (int k = 0; k < intermediate; k++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        C[i][j] += (A[i][k] * B[k][j]);
                    }
                }
            });

            stopwatch.Stop();
            long time = stopwatch.ElapsedMilliseconds;

            Console.WriteLine(" Elapsed Time = {0:f3} seconds.", time / 1000d);
        }
    }
}
