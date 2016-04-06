using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace atem_midi_csharp
{
    class ATEMInputMapping : ATEMMapping
    {
        public readonly long inputId;
        public readonly bool program;

        public ATEMInputMapping(long id) : this(id, false) { }

        public ATEMInputMapping(long id, bool pgm)
        {
            inputId = id;
            program = pgm;
        }
    }
}
