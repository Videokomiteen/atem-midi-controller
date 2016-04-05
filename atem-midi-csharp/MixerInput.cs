using BMDSwitcherAPI;
using Sanford.Multimedia.Midi;

namespace atem_midi_csharp
{
    class MixerInput
    {
        private bool isMapping = false;
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
        public MidiMapping mapping; 

        public MixerInput(IBMDSwitcherInput inpt)
        {
            input = inpt;
        }

        public override string ToString()
        {
            return LongName;
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
        }

        public void OnMidi(object sender, ChannelMessageEventArgs e)
        {
            if (e.Message.Command == mapping.Command && e.Message.Data1 == mapping.Data1)
            {
                
            }
        }
    }
}