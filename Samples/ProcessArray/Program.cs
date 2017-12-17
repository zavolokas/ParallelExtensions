using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArrayProcessing;
using BenchmarkDotNet.Running;

namespace ProcessArray
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<ArrayProcessor>();
        }
    }
}
