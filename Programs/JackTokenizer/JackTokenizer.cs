using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JackTokenizer
{
    internal class JackTokenizer
    {
        private StreamReader _reader;
        public JackTokenizer(StreamReader reader)
        {
            _reader = reader ?? throw new ArgumentNullException(nameof(reader), "Reader cannot be null.");
        }
    }
}
