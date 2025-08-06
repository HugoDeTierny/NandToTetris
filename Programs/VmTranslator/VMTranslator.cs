using System;
using System.Collections.Generic;
using System.IO;

namespace VmTranslator
{
    internal class VMTranslator
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: HackAssembler <input file>");
                return;
            }
            string inputFile = args[0];
            if (!File.Exists(inputFile))
            {
                Console.WriteLine($"Error: File '{inputFile}' does not exist.");
                return;
            }
            List<Command> InstructionsLines = new List<Command>();
            using (StreamReader reader = new StreamReader(inputFile))
            {
                Parser parser = new Parser(reader);
                while (parser.HasMoreCommand())
                {
                    parser.Advance();
                    InstructionsLines.Add(parser.GetCommand());
                }
            }
            string outputfile = args[0].Split('.')[0] + ".asm";
            using (FileStream fs = File.Create(outputfile))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    CodeWriter parser = new CodeWriter(writer);
                    parser.ProgramStart();
                    foreach(Command command in InstructionsLines)
                    {
                        parser.Write(command);
                    }
                    parser.Close();
                }
            }

            Console.WriteLine("Success !");
        }
    }
}
