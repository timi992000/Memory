using Memory.Dialogs;
using Memory.ViewModels;
using System.ComponentModel;
using System;
using System.Windows;
using MahApps.Metro.Controls;

namespace Memory
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : MetroWindow
  {
    private MainWindowViewModel _ViewModel;
    private StartupDialog _Startup;
    public MainWindow()
    {
      InitializeComponent();
      Closing += __Closing;
      _ViewModel = new MainWindowViewModel(this);
      DataContext = _ViewModel;
      _Startup = new StartupDialog(_ViewModel);
      _Startup.StartRequested += __StartRequested;
      _Startup.ShowWindow();
      Hide();
    }

    private void __StartRequested(object? sender, EventArgs e)
    {
      Show();
    }

    private void __Closing(object? sender, CancelEventArgs e)
    {
      e.Cancel = true;
      Hide();
      _Startup.ShowWindow();
    }
  }
}
