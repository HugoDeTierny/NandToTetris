using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace VmTranslator
{
    internal class CodeWriter
    {
        private int SP = 256;

        private readonly int Static = 16;

        private readonly int Pointer = 256;
        private readonly int Temp = 256;

        private readonly StreamWriter _writer;
        public CodeWriter(StreamWriter writer) {
            _writer = writer;
        }

        public void Write(Command command)
        {
            // On écrie l'entéte sous forme de commentaire pour debug
            _writer.WriteLine(command.CommandText);
            string Command;
            switch (command.Type)
            {
                case CommandType.C_ARITHMETIC:
                    ArithmetiqueOperation(command.Arg1);
                    break;
                case CommandType.C_PUSH:
                    Command = "@" + GetAddress(command.Arg1, command.Arg2);
                    _writer.WriteLine(Command);
                    Command = "D=M";
                    _writer.WriteLine(Command);
                    Command = $"@{SP}";
                    _writer.WriteLine(Command);
                    Command = "M=D";
                    _writer.WriteLine(Command);
                    SP++;
                    break;
                case CommandType.C_POP:
                    Command = $"@{SP}";
                    _writer.WriteLine(Command);
                    Command = "D=M";
                    _writer.WriteLine(Command);
                    Command = "@" + GetAddress(command.Arg1, command.Arg2);
                    _writer.WriteLine(Command);
                    Command = "M=D";
                    _writer.WriteLine(Command);
                    SP--;
                    break;
            }
        }
        private int GetAddress(string registerName, int Position)
        {
            switch ("registerName")
            {
                case "local":
                    break;
                case "argument":
                    break;
                case "this":
                    break;
                case "that":
                    break;
                case "constant":
                    break;
                case "static":
                    break;
                case "pointer":
                    break;
                case "temp":
                    break;
            }
            return SP;
        }

        private void WritePointer(string pointerName, int offset)
        {
            string Command;
            
            Command = "@" + offset;
            _writer.WriteLine(Command);
            Command = "D=A";
            _writer.WriteLine(Command);

            Command = "@" + pointerName;
            _writer.WriteLine(Command);
            Command = "A=D+M";
            _writer.WriteLine(Command);

        }

        private void WritePushPop(Command command)
        {
            string Command;
            switch (command.Type)
            {
                case CommandType.C_PUSH:
                    WritePointer(command.Arg1, command.Arg2);
                    Command = "D=M";
                    _writer.WriteLine(Command);
                    Command = "@" + SP;
                    _writer.WriteLine(Command);
                    Command = "M=D";
                    _writer.WriteLine(Command);
                    SP++;
                    break;
                case CommandType.C_POP:
                    Command = "@" + SP;
                    _writer.WriteLine(Command);
                    Command = "D=M";
                    _writer.WriteLine(Command);
                    // We need to save the value somewhere with this implementation

                    WritePointer(command.Arg1, command.Arg2);
                    Command = "M=D";
                    break;
            }
            WritePointer(command.Arg1, command.Arg2);
        }

        private void ArithmetiqueOperation(string  operation)
        {
            string command;
            switch(operation)
            {
                case "add":
                    SP--;
                    command = $"@{SP}";
                    _writer.WriteLine(command);
                    command = "D=M";
                    _writer.WriteLine(command);
                    SP--;
                    command = $"@{SP}";
                    _writer.WriteLine(command);
                    command = "M=D+M";
                    _writer.WriteLine(command);
                    SP++;
                    break;
                case "sub":
                    SP--;
                    command = $"@{SP}";
                    _writer.WriteLine(command);
                    command = "D=M";
                    _writer.WriteLine(command);
                    SP--;
                    command = $"@{SP}";
                    _writer.WriteLine(command);
                    command = "M=D-M";
                    _writer.WriteLine(command);
                    SP++;
                    break;
                case "neg":
                    SP--;
                    command = $"@{SP}";
                    _writer.WriteLine(command);
                    command = "D=-M";
                    _writer.WriteLine(command);
                    SP++;
                    break;
                case "eq":
                    break;
                case "gt":
                    break;
                case "lt":
                    break;
                case "and":
                    break;
                case "or":
                    break;
                case "not":
                    break;
            }
        }
    }
}
