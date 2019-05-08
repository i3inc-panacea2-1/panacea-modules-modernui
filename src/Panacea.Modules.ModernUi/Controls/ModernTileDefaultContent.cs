using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Panacea.Modules.ModernUi.Controls
{
    public class ModernTileDefaultContent : Control
    {
        static ModernTileDefaultContent()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ModernTileDefaultContent), new FrameworkPropertyMetadata(typeof(ModernTileDefaultContent)));
        }
    }
}
