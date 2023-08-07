using Memory.Core.Baseclasses;
using Memory.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

  }
}
