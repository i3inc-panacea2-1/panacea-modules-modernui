using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
//using CoreAudioApi;
using System.Windows.Threading;
using Panacea.Controls;
using Panacea.Core;
using Panacea.Modularity.UiManager;
using Panacea.Multilinguality;

namespace Panacea.Modules.ModernUi.Controls
{
    /// <summary>
    ///     Interaction logic for CharmsBar.xaml
    /// </summary>
    public partial class CharmsBar : UserControl
    {
        public CharmsBar()
        {
            InitializeComponent();
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var res = base.MeasureOverride(constraint);
            if (constraint.Width != double.PositiveInfinity && constraint.Height != double.PositiveInfinity)
                return new Size(res.Width, constraint.Height);

            return res;
        }
    }
}