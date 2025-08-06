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

        private readonly int Static = 16;

        private readonly int Pointer = 256;
        private readonly int Temp = 256;

        private readonly StreamWriter _writer;
        public CodeWriter(StreamWriter writer) {
            _writer = writer;
            setValueToAdress("SP", 256);
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
                case CommandType.C_POP:
                    WritePushPop(command);
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
            return 0;
        }

        private void setValueToAdress(string addressName, int Value)
        {
            string Command = "@" + Value;
            _writer.WriteLine(Command);

            Command = "D=A";
            _writer.WriteLine(Command);

            Command = "@" + addressName;
            _writer.WriteLine(Command);

            Command = "M=D";
            _writer.WriteLine(Command);
        }

        private void GoToPointerAddress(string pointerName, int offset)
        {

            string Command;
            Command = "@" + pointerName;
            _writer.WriteLine(Command);
            Command = "A=M";
            _writer.WriteLine(Command);

            while(offset > 0)
            {
                Command = "A=A+1";
                _writer.WriteLine(Command);
                offset--;
            }
            while (offset < 0)
            {
                Command = "A=A-1";
                _writer.WriteLine(Command);
                offset++;
            }

        }

        private void WritePushPop(Command command)
        {
            string Command;
            switch (command.Type)
            {
                case CommandType.C_PUSH:

                    if (command.Arg1 == "constant")
                    {
                        Command = "@" + command.Arg2;
                        _writer.WriteLine(Command);

                        Command = "D=A";
                        _writer.WriteLine(Command);
                    }
                    else
                    {
                        GoToPointerAddress(command.Arg1, command.Arg2);
                        Command = "D=M";
                        _writer.WriteLine(Command);
                    }
                    GoToPointerAddress("SP", 0);
                    Command = "M=D";
                    _writer.WriteLine(Command);

                    Command = "@SP";
                    _writer.WriteLine(Command);
                    Command = "M=M+1";
                    _writer.WriteLine(Command);
                    break;
                case CommandType.C_POP:
                    GoToPointerAddress("SP", 0);
                    Command = "D=M";
                    GoToPointerAddress(command.Arg1, command.Arg2);
                    Command = "M=D";
                    _writer.WriteLine(Command);


                    Command = "@SP";
                    _writer.WriteLine(Command);
                    Command = "M=M-1";
                    _writer.WriteLine(Command);
                    break;
            }
        }

        private void ArithmetiqueOperation(string  operation)
        {
            string command;
            switch(operation)
            {
                case "add":
                    GoToPointerAddress("SP", -1);
                    command = "D=M";
                    _writer.WriteLine(command);

                    command = "A=A-1";
                    _writer.WriteLine(command);

                    command = "M=D+M";
                    _writer.WriteLine(command);

                    command = "@SP";
                    _writer.WriteLine(command);
                    command = "M = M-1";
                    _writer.WriteLine(command);
                    
                    break;
                case "sub":
                    GoToPointerAddress("SP", -1);
                    command = "D=M";
                    _writer.WriteLine(command);

                    command = "A=A-1";
                    _writer.WriteLine(command);

                    command = "M=D-M";
                    _writer.WriteLine(command);

                    command = "@SP";
                    _writer.WriteLine(command);
                    command = "M = M-1";
                    _writer.WriteLine(command);
                    break;
                case "neg":
                    GoToPointerAddress("SP", -1);
                    command = "M=-M";
                    _writer.WriteLine(command);
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
