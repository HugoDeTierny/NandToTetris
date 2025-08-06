using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VmTranslator
{
    internal class Parser
    {
        List<string> InstructionsLines = new List<string>();
        public Parser(StreamReader reader)
        {


            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine() ?? string.Empty;
                if (string.IsNullOrEmpty(line))
                    continue;

                line = line.Split("//", StringSplitOptions.TrimEntries)[0];
                if (string.IsNullOrEmpty(line))
                    continue;
                InstructionsLines.Add(line);
            }
        }
        



        private string? CurrentLine = null;
        int? LineNumber = null;
        private string[] LineWords = null;
        public bool HasMoreCommand()
        {
            // Todo ignore empty line and comments
            return LineNumber == null || LineNumber < InstructionsLines.Count;
        }
        public void Advance()
        {
            if (LineNumber == null)
                LineNumber = 0;

            CurrentLine = InstructionsLines[LineNumber.Value];
            LineNumber = LineNumber.Value + 1;

            LineWords = CurrentLine.Split(' ');
        }
        private CommandType currentComandType;
        public Command GetCommand()
        {
            currentComandType = commandType();
            

            return new Command(
                "//" + CurrentLine,
                currentComandType,
                arg1(),
                arg2());
        }
        private CommandType commandType()
        {
            if (LineWords is null || LineWords.Length == 0)
                return CommandType.ERROR;

            switch (LineWords[0])
            {
                case "add":
                case "sub":
                case "neg":
                case "eq":
                case "gt":
                case "lt":
                case "and":
                case "or":
                case "not":
                    return CommandType.C_ARITHMETIC;
                case "pop":
                    return CommandType.C_POP;
                case "push":
                    return CommandType.C_PUSH;
                case "label":
                    return CommandType.C_LABEL;
                case "goto":
                    return CommandType.C_GOTO;
                case "if-goto":
                    return CommandType.C_IF;
                case "function":
                    return CommandType.C_FUNCTION;
                case "call":
                    return CommandType.C_CALL;
                case "return":
                    return CommandType.C_RETURN;                    
            }

            return CommandType.ERROR;
            
        }
        private string arg1()
        {
            if (currentComandType == CommandType.C_RETURN)
                return string.Empty;
            if (currentComandType == CommandType.C_ARITHMETIC)
                return LineWords[0];
            else return LineWords[1];
        }
        CommandType[] Types = { CommandType.C_PUSH, CommandType.C_POP, CommandType.C_FUNCTION, CommandType.C_CALL };
        private int arg2()
        {
            if ( Types.Contains(currentComandType))
                return Convert.ToInt32(LineWords[2]);

            return -1;
        }
        


    }
}
