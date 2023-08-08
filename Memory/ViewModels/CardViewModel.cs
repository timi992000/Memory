using Memory.Core.Baseclasses;
using Memory.Core.Entities;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Memory.ViewModels
{
  public class CardViewModel : ViewModelBase
  {
    public CardViewModel(CardEntity card)
    {
      Card = card;
    }

    public Visibility IsVisible => Card.IsFinished ? Visibility.Hidden : Visibility.Visible;

    public CardEntity Card
    {
      get => Get<CardEntity>();
      private set => Set(value);
    }

    public BitmapImage CardContent
    {
      get => Card.IsOpen ? Card.OpenImage : Card.ClosedImage;
    }

    public Thickness CardMargin
    {
      get => Get<Thickness>();
      set => Set(value);
    }

    public double CardSize
    {
      get => Get<double>();
      set => Set(value);
    }
    internal void SetFinished()
    {
      Card.IsOpen = false;
      Card.IsFinished = true;
      OnPropertyChanged(nameof(IsVisible));
    }

    internal void SwapCard()
    {
      if (Card.IsOpen)
        Card.IsOpen = false;
      else
        Card.IsOpen = true;
      OnPropertyChanged(nameof(CardContent));
    }
  }
}
