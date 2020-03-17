using System;
using System.Collections.Generic;

namespace Random.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
			float zero = 0.0F;
			float val = 10.0F;
			float result = val / zero;

			// Printing result 
			Console.WriteLine(zero / val);
			Console.WriteLine(Double.IsInfinity(result));

			Console.ReadLine();
		}


    }
}
