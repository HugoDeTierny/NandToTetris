using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackAssembler
{
    internal class Parser
    {
        private StreamReader _reader;
        public Parser(StreamReader reader)
        {
            _reader = reader ?? throw new ArgumentNullException(nameof(reader), "Reader cannot be null.");
        }
        // This class is responsible for parsing the input assembly code.
        internal List<string> Parse()
        {
            List<string> InstructionsLines = new List<string>();
            while (!_reader.EndOfStream)
            {
                string line = _reader.ReadLine() ?? string.Empty;
                if (string.IsNullOrEmpty(line))
                    continue;

                line = line.Split("//", StringSplitOptions.TrimEntries)[0];
                if (string.IsNullOrEmpty(line))
                    continue;
                InstructionsLines.Add(line);
            }
            return InstructionsLines;
        }
    }
}
