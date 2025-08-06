using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VmTranslator
{
    internal class Command
    {
        public string CommandText { get; set; }
        public CommandType Type { get; set; }
        public string Arg1 { get; set; }
        public int Arg2 { get; set; }

        public Command(string commandText, CommandType type, string arg1, int arg2)
        {
            this.CommandText = commandText;
            Type = type;
            Arg1 = arg1;
            Arg2 = arg2;
        }
    }
}
