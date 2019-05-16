using Panacea.Core;
using Panacea.Mvvm;

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
