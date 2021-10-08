using UnityEngine;

public class FuzzySet_LeftShoulder : FuzzySet
{
  private readonly double peakPoint;
  private readonly double leftOffset;
  private readonly double rightOffset;

  /********************************
   *  THE SHAPE OF THIS FUZZY SET
   *  lo <------->     peak <-> ro
   *   __________________
   *                     \
   *                      \
   *                       \
   *                        \
   *                         \
   *                          \
   *                          
   ********************************/

  public FuzzySet_LeftShoulder(double peak, double LeftOffset, double RightOffset) : base( ((peak - LeftOffset) + peak) / 2)
  {
    peakPoint = peak;
    leftOffset = LeftOffset;
    rightOffset = RightOffset;
  }

  // Calculates the degree of membership for a particular value
  public override double CalculateDOM(double val)
  {
    // Case where the triangle's left or right offsets are zero to prevent divide by zero errors
    if (Mathf.Approximately((float)rightOffset, 0) && Mathf.Approximately((float)peakPoint, (float)val))
    {
      return 1.0;
    }

    // Find DOM if left of center
    if ( (val < peakPoint) && (val >= peakPoint - leftOffset) )
    {
      return 1.0;
    }

    // Find DOM if right of center
    else if ( (val >= peakPoint) && (val < peakPoint + rightOffset) )
    {
      double grad = 1.0 / -rightOffset;

      return grad * (val - peakPoint) + 1.0;
    }

    // Out of range of this FLV, return 0
    else return 0;
  }
}
