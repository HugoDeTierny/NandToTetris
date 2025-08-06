using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private string _CommandString;
        private string CommandString
        {
            get => _CommandString;
            set
            {
                _CommandString = value;
                _writer.WriteLine(value);
            }
        }

        public CodeWriter(StreamWriter writer) {
            _writer = writer;
            setValueToAdress("SP", 256);
            SetBasicCommand();

        }
        // END Jump, write loop to end
        public void Close()
        {
            CommandString = "(END)";
            CommandString = "@END";
            CommandString = "0;JMP";
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
                    WritePush(command.Arg1, command.Arg2);
                    break;
                case CommandType.C_POP:
                    WritePop(command.Arg1, command.Arg2);
                    break;
            }
        }

        private void WritePop(string RegisterName, int offset)
        {
            // 1 Get and stock pointer address
            GetAddress(RegisterName, offset);
            // 2 Get and stock stack pile value
            CommandString = "@SP";
            CommandString = "M=M-1";
            CommandString = "A=M";
            CommandString = "D=M";
            // 3 get and access address
            CommandString = "@R13";
            CommandString = "A=M";
            // 4 store value
            CommandString = "M=D";
        }
        private void WritePush(string RegisterName, int offset)
        {
            if (RegisterName == "constant")
            {
                CommandString = "@" + offset;
                CommandString = "D=A";
            }
            else
            {
                // 1 get register address in R13
                GetAddress(RegisterName, offset);

                //2 On récupére la valeur
                CommandString = "@R13";
                CommandString = "A=M";
                CommandString = "D=M";
            }
            // 3 write value in stack
            CommandString = "@SP";
            CommandString = "A=M";
            CommandString = "M=D";

            // 4 incrémente SP
            CommandString = "@SP";
            CommandString = "M=M+1";
        }
        /// <summary>
        /// Stock l'addresse du registre dans la variable R13
        /// </summary>
        /// <param name="RegisterName"></param>
        /// <param name="offset"></param>
        private void GetAddress(string RegisterName, int offset)
        {
            // To do revoir static
            switch (RegisterName)
            {
                case "local":
                    CommandString = "@" + offset;
                    CommandString = "D=A";
                    CommandString = "@LCL";
                    CommandString = "D=D+M";
                    break;
                case "argument":
                    CommandString = "@" + offset;
                    CommandString = "D=A";
                    CommandString = "@ARG";
                    CommandString = "D=D+M";
                    break;
                case "this":
                    CommandString = "@" + offset;
                    CommandString = "D=A";
                    CommandString = "@THIS";
                    CommandString = "D=D+M";
                    break;
                case "that":
                    CommandString = "@" + offset;
                    CommandString = "D=A";
                    CommandString = "@THAT";
                    CommandString = "D=D+M";
                    break;
                case "temp":
                    CommandString = "@" + offset;
                    CommandString = "D=A";
                    CommandString = "@5";
                    CommandString = "D=D+A";
                    break;
                case "static":
                    CommandString = "@" + offset;
                    CommandString = "D=A";
                    CommandString = "@16";
                    CommandString = "D=D+A";
                    break;
                case "pointer":
                    if (offset == 0)
                        CommandString = "@THIS";
                    else if (offset == 1)
                        CommandString = "@THAT";
                    CommandString = "D=A";
                    break;
            }
            CommandString = "@R13";
            CommandString = "M=D";
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
            pointerName = pointerName.ToUpper();
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
        /*
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
                    else if (command.Arg1 == "static")
                    {
                        Command = "@static." + command.Arg2;
                        _writer.WriteLine(Command);
                        Command = "D=M";
                        _writer.WriteLine(Command);
                    }
                    else if (command.Arg1 == "pointer")
                    {
                        if (command.Arg2 == 0)
                        {
                            Command = "@THIS";
                            _writer.WriteLine(Command);

                            Command = "D=M";
                            _writer.WriteLine(Command);
                        }
                        else if (command.Arg2 == 1)
                        {
                            Command = "@THAT";
                            _writer.WriteLine(Command);

                            Command = "D=M";
                            _writer.WriteLine(Command);
                        }
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
                    GoToPointerAddress("SP", -1);
                    Command = "D=M";
                    _writer.WriteLine(Command);
                    if (command.Arg1 == "static")
                    {
                        Command = "@static." + command.Arg2;
                        _writer.WriteLine(Command);
                        Command = "M=D";
                        _writer.WriteLine(Command);
                    }
                    else if (command.Arg1 == "pointer")
                    {
                        if (command.Arg2 == 0)
                        {
                            Command = "@THIS";
                            _writer.WriteLine(Command);

                            Command = "M=D";
                            _writer.WriteLine(Command);
                        }
                        else if (command.Arg2 == 1)
                        {
                            Command = "@THAT";
                            _writer.WriteLine(Command);

                            Command = "M=D";
                            _writer.WriteLine(Command);
                        }
                    }
                    else
                    {
                        GoToPointerAddress(command.Arg1.ToUpper(), command.Arg2);
                        Command = "M=D";
                        _writer.WriteLine(Command);
                    }

                    Command = "@SP";
                    _writer.WriteLine(Command);
                    Command = "M=M-1";
                    _writer.WriteLine(Command);
                    break;
            }
        }
        */

        int operationCounter = 0;

        private void ArithmetiqueOperation(string  operation)
        {
            string command;
            switch(operation)
            {
                case "add":
                    GoToPointerAddress("SP", -1);
                    CommandString = "D=M";

                    CommandString = "A=A-1";

                    CommandString = "M=D+M";

                    CommandString = "@SP";
                    
                    CommandString = "M=M-1";                    
                    break;
                case "sub":
                    GoToPointerAddress("SP", -1);
                    CommandString = "D=M";

                    CommandString = "A=A-1";
                    CommandString = "M=M-D";
                    
                    CommandString = "@SP";
                    CommandString = "M=M-1";
                    break;
                case "neg":
                    GoToPointerAddress("SP", -1);
                    CommandString = "M=-M";
                    break;
                case "eq":
                    CommandString = "@SP";
                    CommandString = "AM=M-1";
                    //CommandString = "A=M";
                    CommandString = "D=M";
                    CommandString = "@SP";
                    CommandString = "AM=M-1";
                    CommandString = "D=M-D";



                    CommandString = "@TRUE";
                    CommandString = "D;JEQ";
                    CommandString = "@FALSE";
                    CommandString = "0;JMP";

                    

                    break;
                case "gt":
                    break;
                case "lt":
                    break;
                case "and":
                    GoToPointerAddress("SP", -1);
                    CommandString = "D=M";

                    CommandString = "A=A-1";
                    CommandString = "M=D&M";

                    CommandString = "@SP";
                    CommandString = "M=M-1";
                    break;
                case "or":
                    GoToPointerAddress("SP", -1);
                    CommandString = "D=M";

                    CommandString = "A=A-1";
                    CommandString = "M=D|M";

                    CommandString = "@SP";
                    CommandString = "M=M-1";
                    break;
                case "not":
                    GoToPointerAddress("SP", -1);
                    CommandString = "M=!M";
                    break;
            }
        }

        private void SetBasicCommand()
        {
            // Write -1 (true) to current stack pointer, incrémente sp counter, jump to R13 Value to continue
            CommandString = "(TRUE)";
            CommandString = "@SP";
            CommandString = "A=M";
            CommandString = "M=-1";
            CommandString = "@SP";
            CommandString = "M=M+1";
            CommandString = "@R13";
            CommandString = "A=M";
            CommandString = "0;JMP";


            // Write 0 (false) to current stack pointer, increment sp counter, jump to R13 value to continue
            CommandString = "(FALSE)";
            CommandString = "@SP";
            CommandString = "A=M";
            CommandString = "M=0";
            CommandString = "@SP";
            CommandString = "M=M+1";
            CommandString = "@R13";
            CommandString = "A=M";
            CommandString = "0;JMP";
        }
    }
}
