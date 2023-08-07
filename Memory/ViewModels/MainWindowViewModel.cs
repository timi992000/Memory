using Memory.Core.Baseclasses;
using Memory.Core.Entities;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
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

    public Canvas GameField
    {
      get => Get<Canvas>();
      set => Set(value);
    }

    private void __Init()
    {
      _CardViewModels = new List<CardViewModel>();
      var images = __GetImagesForCards();


      int cardPairId = 0;
      int cardId = 0;
      int cardCountAsPair = 2;
      foreach (var image in images)
      {
        cardPairId++;

        BitmapImage bitmap = new BitmapImage();
        bitmap.BeginInit();
        bitmap.UriSource = new Uri(image);
        bitmap.EndInit();

        for (int i = 0; i < cardCountAsPair; i++)
        {
          cardId++;
          var cardViewModel = new CardViewModel(
            new CardEntity(cardId, cardPairId, 0, 0, bitmap)
            );
          _CardViewModels.Add(cardViewModel);
        }

      }

      __DrawGameField();
    }

    private void __DrawGameField()
    {
      var canvas = new Canvas();
      var maxRows = Math.Round(Math.Sqrt(_CardViewModels.Count));

      int cardSize = 200;
      int cardSpacing = 50;

      int rowCounter = 0;
      int columnCounter = 0;
      foreach (var cardVm in _CardViewModels)
      {
        if (rowCounter == maxRows)
        {
          columnCounter++;
          rowCounter = 0;
        }

        cardVm.Card.Row = rowCounter;
        cardVm.Card.Column = columnCounter;

        var border = new Border();
        border.Width = cardSize;
        border.Height = cardSize;
        border.BorderBrush = Brushes.Black;
        border.BorderThickness = new System.Windows.Thickness(1);
        border.Child = new Viewbox()
        {
          Width = cardSize,
          Height = cardSize,
          Child = new Button { Content = new Image { Source = cardVm.Card.CardImage } }
        };

        Canvas.SetLeft(border, columnCounter * cardSize);
        Canvas.SetTop(border, rowCounter * cardSize);
        canvas.Children.Add(border);
        rowCounter++;
      }

      GameField = canvas;
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
