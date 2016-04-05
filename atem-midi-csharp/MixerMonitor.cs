using BMDSwitcherAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace atem_midi_csharp
{
    public delegate void MixerMonitorEventHandler(object sender, object args);

    class MixerMonitor : IBMDSwitcherMixEffectBlockCallback
    {
        public event MixerMonitorEventHandler InTransitionChanged;

        public void PropertyChanged(_BMDSwitcherMixEffectBlockPropertyId propertyId)
        {
            if (propertyId == _BMDSwitcherMixEffectBlockPropertyId.bmdSwitcherMixEffectBlockPropertyIdInTransition && InTransitionChanged != null)
            {
                InTransitionChanged(this, null);
            }
        }
    }
}
