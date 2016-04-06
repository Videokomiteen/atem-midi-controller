using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace atem_midi_csharp
{
    class MixerMapping
    {
        public readonly MidiMapping midi;
        public readonly ATEMMapping atem;

        public MixerMapping(MidiMapping midi, ATEMMapping atem)
        {
            this.midi = midi;
            this.atem = atem;
        }
    }
}
