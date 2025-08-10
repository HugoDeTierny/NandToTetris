using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace VmTranslator
{
    internal class VMTranslator
    {
        static void Main(string[] args)
        {
            List<string> FileList = new List<string>();
            bool isFile = false;
            //args = args.Append("StaticsTest").ToArray();
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
                isFile = true;
                FileList.Add(inputFile);
            }
            List<Command> InstructionsLines = new List<Command>();

            foreach (string file in FileList)
            {
                string fileName = file.Split(Path.DirectorySeparatorChar).Last();
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
            if (!isFile)
            {
                outputfile = args[0] +  Path.DirectorySeparatorChar + outputfile;
            }
            using (FileStream fs = File.Create(outputfile))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    CodeWriter parser = new CodeWriter(writer);
                    parser.ProgramStart(isFile);
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
