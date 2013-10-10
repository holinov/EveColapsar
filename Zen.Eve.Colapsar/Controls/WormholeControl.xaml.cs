using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Zen.Eve.Colapsar.Models;

namespace Zen.Eve.Colapsar.Controls
{
    /// <summary>
    /// Interaction logic for WormholeControl.xaml
    /// </summary>
    public partial class WormholeControl : UserControl
    {
        public WormholeControl()
        {
            InitializeComponent();
        }


        public WormholeModel Wormhole
        {
            get { return (WormholeModel)GetValue(WormholeProperty); }
            set { SetValue(WormholeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Wormhole.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WormholeProperty =
            DependencyProperty.Register("Wormhole", typeof(WormholeModel), typeof(WormholeControl), new PropertyMetadata(null));


    }
}
