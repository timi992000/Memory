using MahApps.Metro.Controls;
using Memory.Core.Baseclasses;
using Memory.Core.Entities;
using Memory.Core.Extender;
using Memory.Entities;
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
    private Queue<Player> _playerQueue = new Queue<Player>();
    private MetroWindow _metroWindow;
    public MainWindowViewModel(MetroWindow metroWindow)
    {
      _metroWindow = metroWindow;
      PairCount = 15;
      PlayerCount = 2;
      __Init();
    }

    public Viewbox GameField
    {
      get => Get<Viewbox>();
      set => Set(value);
    }

    public int PairCount
    {
      get => Get<int>();
      set => Set(value);
    }

    public int PlayerCount
    {
      get => Get<int>();
      set => Set(value);
    }

    public Player CurrentPlayer
    {
      get => Get<Player>();
      set => Set(value);
    }

    [DevDependsUpon(nameof(CurrentPlayer))]
    public string CurrentPlayerText => $"{CurrentPlayer?.Name}'s  Turn ({CurrentPlayer?.PairCount} Pairs)";

    public int OpenedCards => _CardViewModels.Count(c => c.Card.IsOpen && !c.Card.IsFinished);

    internal void SwapCardIfPossible(CardViewModel cardVm)
    {
      __CheckAndHandleCardPair(cardVm);
    }

    public void Refresh()
    {
      __Init();
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
      __Init();
    }

    private void __Init()
    {
      __ResetAndPreparePlayers();
      bool letCardsOpenAfterInit = _CardViewModels != null && _CardViewModels.All(c => c.Card.IsOpen);
      _CardViewModels = new List<CardViewModel>();

      int cardPairId = 0;
      int cardId = 0;
      int cardCountAsPair = 2;
      int cardSize = 400;
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

      for (int pairCount = 0; pairCount < PairCount; pairCount++)
      {
        cardPairId++;

        BitmapImage openImage = new BitmapImage();
        openImage.BeginInit();
        openImage.UriSource = new Uri(@$"https://picsum.photos/{cardSize}?random{cardPairId}", UriKind.Absolute);
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

    private void __ResetAndPreparePlayers()
    {
      _playerQueue.Clear();
      for (int i = 0; i < PlayerCount; i++)
      {
        var player = new Player()
        {
          Name = $"Player {i + 1}",
          PairCount = 0,
        };
        _playerQueue.Enqueue(player);
      }
      __NextPlayer();
    }

    private void __NextPlayer()
    {
      CurrentPlayer = _playerQueue?.Dequeue();
      _playerQueue?.Enqueue(CurrentPlayer);
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
      //none or one card is open, so swap the other card
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
        CurrentPlayer.PairCount++;
        OnPropertyChanged(nameof(CurrentPlayerText));
      }
      else
      {
        //Not pair, next player
        __NextPlayer();
      }

      __CheckWin();

    }

    private void __CheckWin()
    {
      if (_CardViewModels.All(c => c.Card.IsFinished))
      {
        var playerOrderedByPairs = _playerQueue.OrderByDescending(p => p.PairCount).ToList();
        var resultList = string.Join("\n", playerOrderedByPairs.Select(p => $"{playerOrderedByPairs.IndexOf(p) + 1}. {p.Name} {p.PairCount} Pairs"));
        ShowMessage($"{CurrentPlayer?.Name} won with {CurrentPlayer?.PairCount} pairs\n{resultList}", _metroWindow);
      }
    }
  }
}
