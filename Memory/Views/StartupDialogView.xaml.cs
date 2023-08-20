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

namespace Memory.Views
{
  /// <summary>
  /// Interaction logic for StartupDialogView.xaml
  /// </summary>
  public partial class StartupDialogView : UserControl
  {
    public event EventHandler StartGameRequested;
    public StartupDialogView()
    {
      InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      StartGameRequested?.Invoke(this, e);
    }
  }
}
