using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Sanford.Multimedia.Midi;

using BMDSwitcherAPI;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace atem_midi_csharp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private IBMDSwitcherDiscovery switcherDiscovery;
        private IBMDSwitcher switcher;
        private IBMDSwitcherMixEffectBlock mixEffectBlock1;

        private MixerMonitor monitor = new MixerMonitor();

        private bool invertSlider = false;
        private bool isMapping = false;

        private List<MixerInput> inputList = new List<MixerInput>();

        private InputDevice indev;

        public MainWindow()
        {
            InitializeComponent();
            _BMDSwitcherConnectToFailure failReason = 0;
            switcherDiscovery = new CBMDSwitcherDiscovery();
            try
            {
                switcherDiscovery.ConnectTo("10.11.12.21", out switcher, out failReason);
            }
            catch (COMException)
            {
                // An exception will be thrown if ConnectTo fails. For more information, see failReason.
                switch (failReason)
                {
                    case _BMDSwitcherConnectToFailure.bmdSwitcherConnectToFailureNoResponse:
                        MessageBox.Show("No response from Switcher", "Error");
                        break;
                    case _BMDSwitcherConnectToFailure.bmdSwitcherConnectToFailureIncompatibleFirmware:
                        MessageBox.Show("Switcher has incompatible firmware", "Error");
                        break;
                    default:
                        MessageBox.Show("Connection failed for unknown reason", "Error");
                        break;
                }
            }

            mixEffectBlock1 = null;

            IBMDSwitcherMixEffectBlockIterator meIterator = null;
            IntPtr meIteratorPtr;
            Guid meIteratorIID = typeof(IBMDSwitcherMixEffectBlockIterator).GUID;
            switcher.CreateIterator(ref meIteratorIID, out meIteratorPtr);
            if (meIteratorPtr != null)
            {
                meIterator = (IBMDSwitcherMixEffectBlockIterator)Marshal.GetObjectForIUnknown(meIteratorPtr);
            }

            if (meIterator != null)
            {
                meIterator.Next(out mixEffectBlock1);
            }

            if (mixEffectBlock1 == null)
            {
                MessageBox.Show("Unexpected: Could not get first mix effect block", "Error");
            }

            IBMDSwitcherInput currentInput = null;
            IBMDSwitcherInputIterator inputIterator = null;
            IntPtr inputIteratorPtr;
            Guid inputIteratorIID = typeof(IBMDSwitcherInputIterator).GUID;
            switcher.CreateIterator(ref inputIteratorIID, out inputIteratorPtr);
            if(inputIteratorPtr != null)
            {
                inputIterator = (IBMDSwitcherInputIterator)Marshal.GetObjectForIUnknown(inputIteratorPtr);
            }

            if(inputIterator != null)
            {
                inputIterator.Next(out currentInput);
                while(currentInput != null)
                {
                    MixerInput input = new MixerInput(currentInput);
                    inputList.Add(input);
                    inputIterator.Next(out currentInput);
                }
            }

            mixEffectBlock1.AddCallback(monitor);
            monitor.InTransitionChanged += new MixerMonitorEventHandler((s, a) => this.Dispatcher.Invoke((Action)(() => InTransitionChanged(s, a))));

            indev = new Sanford.Multimedia.Midi.InputDevice(3);
            indev.ChannelMessageReceived += Indev_ChannelMessageReceived;
            indev.StartRecording();

            mappingList.ItemsSource = inputList;
        }

        private void InTransitionChanged(object sender, object args)
        {
            int pinTransition;

            mixEffectBlock1.GetFlag(_BMDSwitcherMixEffectBlockPropertyId.bmdSwitcherMixEffectBlockPropertyIdInTransition, out pinTransition);

            if (pinTransition == 0)
            {
                //invertSlider = !invertSlider;
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void Indev_ChannelMessageReceived(object sender, ChannelMessageEventArgs e)
        {
            if (isMapping)
            {
                ((MixerInput)mappingList.SelectedValue).mapping = new MixerInput.MidiMapping(e.Message.Command, e.Message.Data1);
                stopMapping();
            }
            else
            {
                foreach(MixerInput inpt in inputList)
                {
                    if(inpt.mapping != null && inpt.mapping.Command == e.Message.Command && inpt.mapping.Data1 == e.Message.Data1)
                    {
                        long id;
                        inpt.input.GetInputId(out id);
                        mixEffectBlock1.SetInt(_BMDSwitcherMixEffectBlockPropertyId.bmdSwitcherMixEffectBlockPropertyIdPreviewInput, id);
                    }
                }
            }
            if(e.Message.Command == ChannelCommand.Controller && e.Message.Data1 == 0)
            {
                float val = ((float)e.Message.Data2) / 127f;
                if (invertSlider)
                {
                    val = 1.0f - val;
                }
                if (val == 1.0f)
                {
                    invertSlider = !invertSlider;
                }
                mixEffectBlock1.SetFloat(_BMDSwitcherMixEffectBlockPropertyId.bmdSwitcherMixEffectBlockPropertyIdTransitionPosition, val);
            }
        }

        private void mapButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isMapping)
            {
                startMapping();
            } else
            {
                stopMapping();
            }
        }

        private void stopMapping()
        {
            isMapping = false;
            mappingList.IsEnabled = true;
            ICollectionView view = CollectionViewSource.GetDefaultView(inputList);
            view.Refresh();
        }

        private void startMapping()
        {
            isMapping = true;
            mappingList.IsEnabled = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            indev.StopRecording();
            indev.Close();
        }
    }
}
