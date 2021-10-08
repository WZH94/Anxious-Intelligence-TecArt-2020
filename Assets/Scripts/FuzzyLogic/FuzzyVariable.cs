using System.Collections.Generic;
using UnityEngine;

public class FuzzyVariable
{
  // Map of the fuzzy sets that comprise this variable
  private Dictionary<string, FuzzySet> memberSets;

  // Min and max values of the range of this variable
  double minRange = 0, maxRange = 0;

  public FuzzyVariable()
  {
    memberSets = new Dictionary<string, FuzzySet>();
  }

  // Called with the upper and lower bound of set each time a new set is added to adjust upper and lower ranges accordingly
  private void AdjustRangeToFit(double min, double max)
  {
    minRange = min < minRange ? min : minRange;
    maxRange = max > maxRange ? max : maxRange;
  }

  // The following methods create instances of the sets named in the method name and adds them to the member set map.
  // Each time a set of any type is added the minRange and maxRange are adjusted accordingly. 
  // All of the methods return a proxy class representing the newly created instance. This proxy set can be used
  // as an operand when creating the rule base.
  public FuzzySet AddLeftShoulderSet(string name, double minBound, double peak, double maxBound)
  {
    FuzzySet_LeftShoulder newSet = new FuzzySet_LeftShoulder(peak, peak - minBound, maxBound - peak);

    memberSets.Add(name, newSet);
    AdjustRangeToFit(minBound, maxBound);

    return newSet;
  }

  public FuzzySet AddRightShoulderSet(string name, double minBound, double peak, double maxBound)
  {
    FuzzySet_RightShoulder newSet = new FuzzySet_RightShoulder(peak, peak - minBound, maxBound - peak);

    memberSets.Add(name, newSet);
    AdjustRangeToFit(minBound, maxBound);

    return newSet;
  }

  public FuzzySet AddTriangularSet(string name, double minBound, double peak, double maxBound)
  {
    FuzzySet_Triangle newSet = new FuzzySet_Triangle(peak, peak - minBound, maxBound - peak);

    memberSets.Add(name, newSet);
    AdjustRangeToFit(minBound, maxBound);

    return newSet;
  }

  // Fuzzify a given value by calculating its DOM in each of this variables' subsets.
  public void Fuzzify(double value)
  {
    if (value >= minRange && value <= maxRange)
    {
      foreach (FuzzySet fuzzySet in memberSets.Values)
      {
        fuzzySet.SetDOM(fuzzySet.CalculateDOM(value));
      }
    }
  }

  // Defuzzify the variable using the MaxAv method
  public double Defuzzify()
  {
    double bottom = 0.0;
    double top = 0.0;

    foreach (FuzzySet fuzzySet in memberSets.Values)
    {
      bottom += fuzzySet.GetDOM();
      top += fuzzySet.GetRepresentativeValue() * fuzzySet.GetDOM();
    }

    if (Mathf.Approximately((float)bottom, 0)) return 0.0;

    return top / bottom;
  }
}