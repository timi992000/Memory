using Memory.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace Memory.Views
{
  /// <summary>
  /// Interaction logic for CardView.xaml
  /// </summary>
  public partial class CardView : UserControl
  {
    MainWindowViewModel _mainVm;
    CardViewModel _cardVm;
    public CardView(int row, int column, CardViewModel cardVm, MainWindowViewModel mainVm)
    {
      InitializeComponent();
      _mainVm = mainVm;
      _cardVm = cardVm;
      DataContext = cardVm;
      cardVm.Card.Row = row;
      cardVm.Card.Column = column;
      Grid.SetColumn(this, column);
      Grid.SetRow(this, row);
    }

    private void __LeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      _mainVm.SwapCardIfPossible(_cardVm);
    }
  }
}
