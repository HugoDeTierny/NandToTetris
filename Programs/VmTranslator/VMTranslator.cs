using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VmTranslator
{
    internal class VMTranslator
    {
        static void Main(string[] args)
        {
            List<string> FileList = new List<string>();
            args = args.Append("NestedCall").ToArray();
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: HackAssembler <input file>");
                return;
            }
            string inputFile = args[0];
            if (!File.Exists(inputFile))
            {
                if (!Directory.Exists(inputFile))
                {
                    Console.WriteLine($"Error: File '{inputFile}' does not exist.");
                    return;
                }
                foreach(string file in Directory.GetFiles(inputFile))
                {
                    string[] tmpField = file.Split(".");
                    if(tmpField.Length > 1 && tmpField[tmpField.Length-1] == "vm")
                        FileList.Add(file);
                }

            }
            else
            {
                FileList.Add(inputFile);
            }
            List<Command> InstructionsLines = new List<Command>();

            foreach (string file in FileList)
            {
                string fileName = file.Split("\\").Last();
                using (StreamReader reader = new StreamReader(file))
                {
                    Parser parser = new Parser(reader);
                    parser.SetFileName(fileName);
                    while (parser.HasMoreCommand())
                    {
                        parser.Advance();
                        InstructionsLines.Add(parser.GetCommand());
                    }
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
