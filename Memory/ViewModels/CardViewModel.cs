using Memory.Core.Baseclasses;
using Memory.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Memory.ViewModels
{
  internal class CardViewModel : ViewModelBase
  {
    public CardViewModel(CardEntity card)
    {
      Card = card;
    }

    public CardEntity Card
    {
      get => Get<CardEntity>();
      private set => Set(value);
    }

    public Border Border
    {
      get => Get<Border>();
      set => Set(value);
    }

    internal void SwapCard(System.Windows.Controls.Image image)
    {
      if(Card.IsOpen)
      {
        image.Source = Card.ClosedImage;
        Card.IsOpen = false;
      }
      else
      {
        image.Source = Card.OpenImage;
        Card.IsOpen = true;
      }
    }
  }
}
