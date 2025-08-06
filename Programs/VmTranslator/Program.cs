namespace VmTranslator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            args = [ @"Test\test1" ];
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: HackAssembler <input file>");
                return;
            }
            string inputFile = args[0] + ".vm";
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
            using (FileStream fs = File.Create(args[0] + ".asm"))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    CodeWriter parser = new CodeWriter(writer);
                    foreach(Command command in InstructionsLines)
                    {
                        parser.Write(command);
                    }
                }
            }

            Console.WriteLine("Success !");
        }
    }
}
