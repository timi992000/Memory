using Memory.Core.Baseclasses;
using Memory.Core.Entities;
using Memory.Core.Extender;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Memory.ViewModels
{
  internal class MainWindowViewModel : ViewModelBase
  {
    private List<CardViewModel> _CardViewModels;
    public MainWindowViewModel()
    {
      __Init();
    }

    public object GameField
    {
      get
      {
        return new Viewbox()
        {
          Child = Get<Grid>()
        };
      }
      set => Set(value);
    }

    public int OpenedCards => _CardViewModels.Count(c => c.Card.IsOpen && !c.Card.IsFinished);

    private void __Init()
    {
      _CardViewModels = new List<CardViewModel>();
      var images = __GetImagesForCards();


      int cardPairId = 0;
      int cardId = 0;
      int cardCountAsPair = 2;

      BitmapImage closedImage = new BitmapImage();
      closedImage.BeginInit();
      closedImage.UriSource = new Uri(@"C:\Git\Memory\Memory.Core\Resources\Memory_ClosedBackground.png");
      closedImage.EndInit();

      foreach (var image in images)
      {
        cardPairId++;

        BitmapImage openImage = new BitmapImage();
        openImage.BeginInit();
        openImage.UriSource = new Uri(image);
        openImage.EndInit();

        for (int i = 0; i < cardCountAsPair; i++)
        {
          cardId++;
          var cardViewModel = new CardViewModel(
            new CardEntity(cardId, cardPairId, 0, 0, openImage, closedImage)
            );
          _CardViewModels.Add(cardViewModel);
        }

      }

      __DrawGameField();
    }

    private void __DrawGameField()
    {
      var grid = new Grid();
      var maxRows = Math.Round(Math.Sqrt(_CardViewModels.Count));
      for (int i = 0; i < maxRows; i++)
      {
        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
      }
      grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

      int cardSize = 200;
      int cardSpacing = 10;

      int rowCounter = 0;
      int columnCounter = 0;
      _CardViewModels.Shuffle();
      foreach (var cardVm in _CardViewModels)
      {
        if (rowCounter == maxRows)
        {
          columnCounter++;
          grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
          rowCounter = 0;
        }

        cardVm.Card.Row = rowCounter;
        cardVm.Card.Column = columnCounter;

        var border = new Border();
        border.Margin = new Thickness(0, 0, cardSpacing, cardSpacing);
        border.Width = cardSize;
        border.Height = cardSize;
        border.BorderBrush = Brushes.Black;
        border.BorderThickness = new System.Windows.Thickness(1);
        cardVm.Border = border;
        __GetCard(cardSize, cardVm);

        Grid.SetColumn(border, columnCounter);
        Grid.SetRow(border, rowCounter);
        grid.Children.Add(border);
        rowCounter++;
      }

      GameField = grid;
    }

    private static void __GetCard(int cardSize, CardViewModel cardVm)
    {
      var image = new Image { Source = cardVm.Card.ClosedImage };
      cardVm.Border.Child = new Viewbox()
      {
        Width = cardSize,
        Height = cardSize,
        Child = image
      };
      image.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) =>
      {
        var mainVm = image.DataContext as MainWindowViewModel;
        if (cardVm.Card.IsOpen || mainVm.OpenedCards < 2)
        {
          cardVm.SwapCard(image);
          mainVm.__CheckAndHandleCardPair();
        }
      };
    }

    private void __CheckAndHandleCardPair()
    {
      if (OpenedCards != 2)
        return;

      var openCards = _CardViewModels.Where(c => c.Card.IsOpen);

      if (openCards.Select(c => c.Card.PairId).Distinct().Count() == 1)
      {
        //Pair
        foreach (var openCard in openCards)
        {
          openCard.Border.Visibility = Visibility.Hidden;
          openCard.Card.IsOpen = false;
          openCard.Card.IsFinished = true;
        }
      }


    }

    private string[] __GetImagesForCards()
    {
      var dlg = new OpenFileDialog();
      dlg.Multiselect = true;
      dlg.Filter = "Image files (*.png;*.jpeg)|*.png;*.jpeg|All files (*.*)|*.*";

      if (dlg.ShowDialog() == true)
      {
        return dlg.FileNames;
      }
      return Array.Empty<string>();
    }
  }
}
