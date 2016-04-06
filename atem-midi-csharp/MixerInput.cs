using BMDSwitcherAPI;
using Sanford.Multimedia.Midi;

namespace atem_midi_csharp
{
    class MixerInput
    {
        public string LongName
        {
            get
            {
                string name;
                input.GetLongName(out name);
                return name;
            }
        }
        public string ShortName
        {
            get
            {
                string name;
                input.GetShortName(out name);
                return name;
            }
        }
        public IBMDSwitcherInput input;
        public MidiMapping mapping = null; 

        public MixerInput(IBMDSwitcherInput inpt)
        {
            input = inpt;
        }

        public override string ToString()
        {
            string mapStr = mapping != null ? mapping.ToString() : "Not mapped";
            return ShortName + " - " + mapStr;
        }

        public class MidiMapping
        {
            public readonly ChannelCommand Command;
            public readonly int Data1;

            public MidiMapping(ChannelCommand command, int data1)
            {
                Command = command;
                Data1 = data1;
            }

            public override string ToString()
            {
                return Command.ToString() + ":" + Data1.ToString();
            }
        }
    }
}