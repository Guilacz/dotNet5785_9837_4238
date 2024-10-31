using System;


namespace Stage0 
{
    partial class Program
    {
        private static void Main(string[] args)
        {
            //Console.WriteLine("Hello, World!");
            Welcome9837();
            Welcome4238();
            Console.ReadKey();


        }
        static partial void Welcome4238();
        private static void Welcome9837()
        {
            Console.Write("Enter your name: ");
            string name = Console.ReadLine();
            Console.WriteLine("{0}, welcome to my first application", name);
        }
        

    }
}

