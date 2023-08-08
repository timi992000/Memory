using Memory.Core.Baseclasses;
using Memory.Core.Entities;
using Memory.Core.Extender;
using Memory.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Memory.ViewModels
{
  public class MainWindowViewModel : ViewModelBase
  {
    private List<CardViewModel> _CardViewModels;
    public MainWindowViewModel()
    {
      __Init(21);
    }

    public Viewbox GameField
    {
      get => Get<Viewbox>();
      set => Set(value);
    }

    public int OpenedCards => _CardViewModels.Count(c => c.Card.IsOpen && !c.Card.IsFinished);

    internal void SwapCardIfPossible(CardViewModel cardVm)
    {
      __CheckAndHandleCardPair(cardVm);
    }

    public void Execute_SwitchAll()
    {
      var openCards = _CardViewModels.Where(c => c.Card.IsOpen);
      if (openCards.Count() == 0)
        openCards = _CardViewModels;
      foreach (var openCard in openCards)
        openCard.SwapCard();
    }

    public void Execute_NewImages()
    {
      __Init(20);
    }

    private void __Init(int pairs)
    {
      bool letCardsOpenAfterInit = _CardViewModels != null && _CardViewModels.All(c => c.Card.IsOpen);
      _CardViewModels = new List<CardViewModel>();

      int cardPairId = 0;
      int cardId = 0;
      int cardCountAsPair = 2;
      int cardSize = 200;
      var cardSpacing = new Thickness(0, 0, 10, 10);

      BitmapImage closedImage = null;
      var assembly = Assembly.Load("Memory.Core");
      if (assembly != null)
      {
        var stream = assembly.GetManifestResourceStream("Memory.Core.Resources.Memory_ClosedBackground.png");
        if (stream != null)
        {
          closedImage = new BitmapImage();
          closedImage.BeginInit();
          closedImage.StreamSource = stream;
          closedImage.CacheOption = BitmapCacheOption.OnLoad;
          closedImage.EndInit();
          closedImage.Freeze();
        }
      }
      if (closedImage == null)
        closedImage = new BitmapImage();

      for (int pairCount = 0; pairCount < pairs; pairCount++)
      {
        cardPairId++;

        BitmapImage openImage = new BitmapImage();
        openImage.BeginInit();
        openImage.UriSource = new Uri(@$"https://picsum.photos/200?random{cardPairId}", UriKind.Absolute);
        openImage.EndInit();

        for (int i = 0; i < cardCountAsPair; i++)
        {
          cardId++;
          var cardViewModel = new CardViewModel(
            new CardEntity(cardId, cardPairId, 0, 0, openImage, closedImage))
          {
            CardMargin = cardSpacing,
            CardSize = cardSize,
          };
          if (letCardsOpenAfterInit)
            cardViewModel.SwapCard();
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
        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
      grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

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
        grid.Children.Add(new CardView(rowCounter, columnCounter, cardVm, this));
        rowCounter++;
      }
      GameField = new Viewbox { Child = grid };
    }

    private void __CheckAndHandleCardPair(CardViewModel cardVm)
    {
      if (OpenedCards < 2 || (OpenedCards == 2 && cardVm.Card.IsOpen))
        cardVm.SwapCard();

      if (OpenedCards != 2)
        return;

      var openCards = _CardViewModels.Where(c => c.Card.IsOpen);

      if (openCards.Select(c => c.Card.PairId).Distinct().Count() == 1)
      {
        //Pair
        foreach (var openCard in openCards)
          openCard.SetFinished();
      }
    }
  }
}
