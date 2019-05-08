using Panacea.Core;
using Panacea.Multilinguality;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panacea.Modules.ModernUi.ViewModels
{
    public class CharmsBarViewModel: PropertyChangedBase
    {
        private readonly PanaceaServices _core;

        public CharmsBarViewModel(PanaceaServices core)
        {
            _core = core;
        }

        public float SpeakersLevel { get; set; }


        public string User { get; set; }


    }
}
