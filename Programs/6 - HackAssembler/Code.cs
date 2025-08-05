using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackAssembler
{
    internal class Code(SymbolTable symbolTable)
    {
        SymbolTable _SymbolTable = symbolTable;

        internal string Translate(string Value)
        {
            int lineValue;
            string translatedLine;
            if (Value.First() == '@')
            {
                lineValue = ConvertA(Value);
                translatedLine = Convert.ToString(lineValue, 2);

                while (translatedLine.Length < 16)
                {
                    translatedLine = translatedLine.Insert(0, "0");
                }

            }
            else translatedLine = ConvertC(Value);


            return translatedLine;
        }
        private int ConvertA(string Value)
        {
            String line = Value.Substring(1);
            if(!int.TryParse(line, out int address)){
                address = _SymbolTable.getLine(line);
            }
            return address;
        }
        private string ConvertC(string Value)
        {
            string line;
            string headPart = "111";
            string compPart = "0000000";
            string destPart = "000";
            string jumpPart = "000";

            string[] jmpDef = Value.Split(';');
            if (jmpDef.Length > 1)
            {
                switch (jmpDef[1])
                {
                    case "JGT":
                        jumpPart = "001";
                        break;
                    case "JEQ":
                        jumpPart = "010";
                        break;
                    case "JGE":
                        jumpPart = "011";
                        break;
                    case "JLT":
                        jumpPart = "100";
                        break;
                    case "JNE":
                        jumpPart = "101";
                        break;
                    case "JLE":
                        jumpPart = "110";
                        break;
                    case "JMP":
                        jumpPart = "111";
                        break;
                }
            }
            Value = jmpDef[0];
            jmpDef = Value.Split('=');
            if (jmpDef.Length > 1)
            {
                switch (jmpDef[0])
                {
                    case "M":
                        destPart = "001";
                        break;
                    case "D":
                        destPart = "010";
                        break;
                    case "MD":
                        destPart = "011";
                        break;
                    case "A":
                        destPart = "100";
                        break;
                    case "AM":
                        destPart = "101";
                        break;
                    case "AD":
                        destPart = "110";
                        break;
                    case "AMD":
                        destPart = "111";
                        break;
                }
                Value = jmpDef[1];
            }
            else Value = jmpDef[0];

            switch (Value)
            {
                case "0":
                    compPart = "0101010";
                    break;
                case "1":
                    compPart = "0111111";
                    break;
                case "-1":
                    compPart = "0111010";
                    break;
                case "D":
                    compPart = "0001100";
                    break;
                case "A":
                    compPart = "0110000";
                    break;
                case "!D":
                    compPart = "0001101";
                    break;
                case "!A":
                    compPart = "0110001";
                    break;
                case "D+1":
                    compPart = "0011111";
                    break;
                case "A+1":
                    compPart = "0110111";
                    break;
                case "D-1":
                    compPart = "0001110";
                    break;
                case "A-1":
                    compPart = "0110010";
                    break;
                case "D+A":
                    compPart = "0000010";
                    break;
                case "D-A":
                    compPart = "0010011";
                    break;
                case "A-D":
                    compPart = "0000111";
                    break;
                case "D&A":
                    compPart = "0000000";
                    break;
                case "D|A":
                    compPart = "0010101";
                    break;
                case "M":
                    compPart = "1110000";
                    break;
                case "!M":
                    compPart = "1110001";
                    break;
                case "M+1":
                    compPart = "1110111";
                    break;
                case "M-1":
                    compPart = "1110010";
                    break;
                case "D+M":
                    compPart = "1000010";
                    break;
                case "D-M":
                    compPart = "1010011";
                    break;
                case "M-D":
                    compPart = "1000111";
                    break;
                case "D&M":
                    compPart = "1000000";
                    break;
                case "D|M":
                    compPart = "1010101";
                    break;
            }
            line = headPart + compPart + destPart + jumpPart;
            return line;
        }


    }
}
