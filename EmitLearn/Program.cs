﻿using System;

namespace EmitLearn
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
               // HelloWord.Hello();
                //Fibonacci.CalcRun();
                Calc.Run();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Console.ReadKey();
        }
    }
}
