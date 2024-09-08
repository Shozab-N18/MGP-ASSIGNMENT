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
            processor.ReadData("../MGP Assignment/practice.dat");
            processor.ProcessData();

            processor.ReadData("../MGP Assignment/qualifying.dat");
            processor.ProcessData();
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occurred: {e.Message}");
        }
    }
}
