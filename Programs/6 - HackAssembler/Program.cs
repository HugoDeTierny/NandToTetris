using System.Reflection;
using System.Text;

namespace HackAssembler
{
    internal class Program
    {
        static void Main(string[] args)
        {

            if(args.Length != 1)
            {
                Console.WriteLine("Usage: HackAssembler <input file>");
                return;
            }
            string inputFile = args[0] + ".asm";
            if (!File.Exists(inputFile))
            {
                Console.WriteLine($"Error: File '{inputFile}' does not exist.");
                return;
            }
            List<string> InstructionsLines;
            using (StreamReader reader = new StreamReader(inputFile))
            {
                Parser parser = new Parser(reader);
                InstructionsLines = parser.Parse();
            }
            SymbolTable symbolTable = new SymbolTable(InstructionsLines);
            Code code = new Code(symbolTable);
            List<string> strings = new List<string>();
            foreach (string line in symbolTable.newInstructions)
            {
                strings.Add(code.Translate(line));
            }
            using (FileStream fs = File.Create(args[0] + ".hack"))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    int count = 1;
                    foreach (string line in strings)
                    {
                        if(count == strings.Count())
                            writer.Write(line);
                        else writer.WriteLine(line);
                        count++;
                    }
                }
            }

                Console.WriteLine("Success !");
        }
    }
}
// Parser
//          Read each Line, yield return a list without comments, line jump, empty lines, white spaces
// SymbolTable : Manage the symbols table first read we will add the symbols (xxx)
// Code 
//          Read the code, translate the instruction to binary, and write to a file
// A a instruction translator
// 
// A c instruction translator

// Main Initialize the io process, override existing file