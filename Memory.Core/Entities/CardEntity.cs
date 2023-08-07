using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Memory.Core.Entities
{
  public class CardEntity
  {
    public CardEntity(int cardId, int pairId, int row, int column, BitmapImage openImage, BitmapImage closedImage)
    {
      CardId = cardId;
      PairId = pairId;
      Row = row;
      Column = column;
      OpenImage = openImage;
      ClosedImage = closedImage;
    }

    /// <summary>
    /// CardId, the id of the current card
    /// </summary>
    public int CardId { get; set; }

    /// <summary>
    /// PairId, the id of the pair
    /// </summary>
    public int PairId { get; set; }

    /// <summary>
    /// IsOpen, is the card opened at the moment
    /// </summary>
    public bool IsOpen { get; set; }

    /// <summary>
    /// Row, The current row the card is in
    /// </summary>
    public int Row { get; set; }

    /// <summary>
    /// Column, The current column the card is in
    /// </summary>
    public int Column { get; set; }

    /// <summary>
    /// OpenImage, the image the card is fill with
    /// </summary>
    public BitmapImage OpenImage { get; set; }

    public BitmapImage ClosedImage { get; set; }
    public bool IsFinished { get; set; }
  }
}
