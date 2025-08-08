using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace VmTranslator
{
    internal class CodeWriter
    {
        private Stack<CallerEntity> CallerStack = new Stack<CallerEntity>();
        private int CallIndex = 0;
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
        private string FileName;
        public CodeWriter(StreamWriter writer) {
            _writer = writer;
            setValueToAdress("SP", 256);
            CommandString = "@START";
            CommandString = "0;JMP";
            SetBasicCommand();

        }
        public void ProgramStart()
        {
            CommandString = "(START)";
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
            FileName = command.FileName;
            // On écrie l'entéte sous forme de commentaire pour debug
            _writer.WriteLine(command.CommandText);
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
                case CommandType.C_LABEL:
                    writeLabel(command.Arg1);
                    break;
                case CommandType.C_GOTO:
                    writeGoTo(command.Arg1);
                    break;
                case CommandType.C_IF:
                    writeIf(command.Arg1);
                    break;
                case CommandType.C_FUNCTION:
                    writeFunction(command.Arg1, command.Arg2);
                    break;
                case CommandType.C_RETURN:
                    WriteReturn();
                    break;
                case CommandType.C_CALL:
                    writeCall(command.Arg1, command.Arg2);
                    break;
            }
        }

        private void WritePop(string RegisterName, int offset)
        {
            // 1 Get and stock pointer address
            GetAddress(RegisterName, offset, FileName);
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
                GetAddress(RegisterName, offset, FileName);

                //2 On récupére la valeur
                CommandString = "@R13";
                CommandString = "A=M";
                CommandString = "D=M";
            }
            WritePush();
        }
        /// <summary>
        /// Push the already obtained D value onto the stack
        /// </summary>
        private void WritePush()
        {
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
        private void GetAddress(string RegisterName, int offset, string FileName)
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
                    CommandString = "@" + FileName +"." + offset;
                    CommandString = "D=A";
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
       int operationCounter = 0;

        private void ArithmetiqueOperation(string  operation)
        {
            switch (operation)
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

                    // Store value to jump in R13
                    CommandString = $"@ENDOPERATION:{operationCounter}";
                    CommandString = "D=A";
                    CommandString = "@R13";
                    CommandString = "M=D";

                    // Set SP backward and get address
                    CommandString = "@SP";
                    CommandString = "// AM=M-1";
                    CommandString = "AM=M-1";
                   
                    // Get M value
                    CommandString = "D=M";
                    // Set SP backward and get address
                    CommandString = "@SP";
                    CommandString = "AM=M-1";

                    // Compare
                    CommandString = "D=M-D";

                    // Jump
                    CommandString = "@TRUE";
                    CommandString = "D;JEQ";
                    CommandString = "@FALSE";
                    CommandString = "0;JMP";
                    // Mark end of operation
                    CommandString = $"(ENDOPERATION:{operationCounter})";
                    operationCounter++;

                    break;
                case "gt":
                    // Store value to jump in R13
                    CommandString = $"@ENDOPERATION:{operationCounter}";
                    CommandString = "D=A";
                    CommandString = "@R13";
                    CommandString = "M=D";

                    // Set SP backward and get address
                    CommandString = "@SP";
                    CommandString = "// AM=M-1";
                    CommandString = "AM=M-1";

                    // Get M value
                    CommandString = "D=M";
                    // Set SP backward and get address
                    CommandString = "@SP";
                    CommandString = "AM=M-1";

                    // Compare
                    CommandString = "D=M-D";

                    // Jump
                    CommandString = "@TRUE";
                    CommandString = "D;JGT";
                    CommandString = "@FALSE";
                    CommandString = "0;JMP";
                    // Mark end of operation
                    CommandString = $"(ENDOPERATION:{operationCounter})";
                    operationCounter++;
                    break;
                case "lt":
                    // Store value to jump in R13
                    CommandString = $"@ENDOPERATION:{operationCounter}";
                    CommandString = "D=A";
                    CommandString = "@R13";
                    CommandString = "M=D";

                    // Set SP backward and get address
                    CommandString = "@SP";
                    CommandString = "// AM=M-1";
                    CommandString = "AM=M-1";

                    // Get M value
                    CommandString = "D=M";
                    // Set SP backward and get address
                    CommandString = "@SP";
                    CommandString = "AM=M-1";

                    // Compare
                    CommandString = "D=M-D";

                    // Jump
                    CommandString = "@TRUE";
                    CommandString = "D;JLT";
                    CommandString = "@FALSE";
                    CommandString = "0;JMP";
                    // Mark end of operation
                    CommandString = $"(ENDOPERATION:{operationCounter})";
                    operationCounter++;
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

        /// <summary>
        /// Label command
        /// </summary>
        /// <param name="label"></param>
        private void writeLabel(string label)
        {
            if (CurrentFonction != string.Empty)
            {
                label = CurrentFonction + "$" + label;
            }
            label = FileName + "." + label;
            CommandString ="(" + label + ")";
        }
        /// <summary>
        /// Go To Command
        /// </summary>
        /// <param name="label"></param>
        private void writeGoTo(string label)
        {
            if(CurrentFonction != string.Empty)
            {
                label = CurrentFonction + "$" + label; 
            }
            label = FileName + "." + label;
            CommandString = "@" + label;
            CommandString = "0;JMP";
        }
        /// <summary>
        /// If-Goto command (we jump if previous value > 0)
        /// </summary>
        /// <param name="label"></param>
        private void writeIf(string label)
        {
            if (CurrentFonction != string.Empty)
            {
                label = CurrentFonction + "$" + label;
            }
            label = FileName + "." + label;

            CommandString = "@SP";
            CommandString = "AM=M-1";
            CommandString = "D=M";
            CommandString = "@" + label;
            CommandString = "D;JGT";
        }
        

        private string CurrentFonction = string.Empty;

        private void writeFunction(string functionName, int numVars)
        {
            CommandString = "@SP";
            CommandString = "A=M";
            for (int i = 0;  i < numVars; i++)
            {

                CommandString = "M=0";
                CommandString = "A=A+1";

            }

            CurrentFonction = functionName;
            CallIndex = 0;
            CommandString = "(" + FileName + "." + functionName + ")";


        }

        private void writeCall(string functionName, int numVars)
        {
            string returnAddress = FileName + "." + CurrentFonction + "$ret." + CallIndex;
            CallerStack.Push(new CallerEntity()
            {
                Caller = CurrentFonction,
                index = CallIndex,
            });
            // push return address
            CommandString = "@" + returnAddress;
            CommandString = "D=A";
            WritePush();

            // push LCL
            CommandString = "@LCL";
            CommandString = "D=M";
            WritePush();

            // push ARG
            CommandString = "@ARG";
            CommandString = "D=M";
            WritePush();

            // push THIS
            CommandString = "@THIS";
            CommandString = "D=M";
            WritePush();

            // push THAT
            CommandString = "@THAT";
            CommandString = "D=M";
            WritePush();

            // ARG = SP - 5 - numvars
            CommandString = "@SP";
            CommandString = "D=M";
            int jumpvalue = 5 + numVars;
            CommandString = "@" + jumpvalue;
            CommandString = "D=D-A";
            CommandString = "@ARG";
            CommandString = "M=D";

            // LCL = SP
            CommandString = "@SP";
            CommandString = "D=M";
            CommandString = "@LCL";
            CommandString = "M=D";

            // goto function
            writeGoTo(functionName);

            // Declare label of return address
            CommandString = "(" + returnAddress + ")";
            CallIndex++;
        }

        private void WriteReturn()
        {
            string endFrameName = FileName + "." + CurrentFonction + "." + "endframe";
            string returnAddresseName = FileName + "." + CurrentFonction + "." + "returnAddress";
            // Tmp variable endframe stock LCL
            CommandString = "@LCL";
            CommandString = "D=M";
            CommandString = "@" + endFrameName;
            CommandString = "M=D";

            // get and stock the return address
            CommandString = "@5";
            CommandString = "A=D-A";
            CommandString = "A=M";
            CommandString = "D=M";
            CommandString = "@" + returnAddresseName;
            CommandString = "M=D";

            // *ARG = pop()
            WritePop("argument", 0);

            // SP = ARG + 1
            CommandString = "@ARG";
            CommandString = "D=M+1";
            CommandString = "@SP";
            CommandString = "M=D";

            // THAT = *(endframe-1)
            CommandString = "@" + endFrameName;
            CommandString = "A=A-1";
            CommandString = "D=M";
            CommandString = "@THAT";
            CommandString = "M=D";

            // THIS = *(endframe-2)
            CommandString = "@" + endFrameName;
            CommandString = "A=A-1";
            CommandString = "A=A-1";
            CommandString = "D=M";
            CommandString = "@THIS";
            CommandString = "M=D";

            // ARG = *(endframe-3)
            CommandString = "@" + endFrameName;
            CommandString = "A=A-1";
            CommandString = "A=A-1";
            CommandString = "A=A-1";
            CommandString = "D=M";
            CommandString = "@ARG";
            CommandString = "M=D";

            // LCL = *(endframe-4)
            CommandString = "@" + endFrameName;
            CommandString = "A=A-1";
            CommandString = "A=A-1";
            CommandString = "A=A-1";
            CommandString = "A=A-1";
            CommandString = "D=M";
            CommandString = "@LCL";
            CommandString = "M=D";

            // GOTO ret adress
            CallerEntity callerEntity = CallerStack.Pop();

            CurrentFonction = callerEntity.Caller;
            CallIndex = callerEntity.index;

            CommandString = "@" + FileName + "." + callerEntity.Caller + "$ret." + callerEntity.index; ;
            CommandString = "0;JMP";


        }
    }
}
