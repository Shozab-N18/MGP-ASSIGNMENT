using System;
using Forms;
using Processor;

class Program
{
    static void Main(string[] args)
    {
        var processor = new DataProcessor();
        
        try
        {
            processor.ReadData("./practice.dat");
            processor.ProcessData();

            processor.ReadData("./qualifying.dat");
            processor.ProcessData();
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occurred: {e.Message}");
        }
    }
}
