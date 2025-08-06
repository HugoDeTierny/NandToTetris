using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackAssembler
{
    internal class SymbolTable
    {
        int i = 16;
        Dictionary<string, int> symbols = new Dictionary<string, int>()
        {
            {"R0", 0 },
            {"R1", 1 },
            {"R2", 2 },
            {"R3", 3 },
            {"R4", 4 },
            {"R5", 5 },
            {"R6", 6 },
            {"R7", 7 },
            {"R8", 8 },
            {"R9", 9 },
            {"R10", 10 },
            {"R11", 11 },
            {"R12", 12 },
            {"R13", 13 },
            {"R14", 14 },
            {"R15", 15 },
            {"SP", 0 },
            {"LCL", 1 },
            {"ARG", 2 },
            {"THIS", 3 },
            {"THAT", 4 },
            {"SCREEN", 16384 },
            {"KBD", 24576 },
        };

        internal List<String> newInstructions = new List<string>();

        internal SymbolTable(List<string> instructions)
        {
            int currentLine = 0;
            foreach (string instruction in instructions)
            {
                if (instruction.First() == '(' && instruction.Last() == ')')
                {
                    string label = instruction.Substring(1, instruction.Length - 2);
                    symbols[label] = currentLine;
                }
                else
                {
                    currentLine++;
                    newInstructions.Add(instruction);
                }
            }
        }

        internal int getLine(string symbol)
        {
            if(!symbols.TryGetValue(symbol, out int line))
            {
                line = i;
                symbols[symbol] = i;
                i++;
            }
            return line;
        }
    }
}
