public class FuzzySet
{
  // Hold the degree of membership in this set of a given value
  protected double DOM;

  // The max of the set's membership function. Ex. if the set is triangular then this will be the peak point of the triangle.
  // If the set is a plateau then this will be the midpoint of the plateau. Value is set in the constructor
  protected double representativeValue;

  public FuzzySet(double repValue)
  {
    DOM = 0;
    representativeValue = repValue;
  }

  // Returns the degree of membership in this set to the given value. NOTE,
  // this does not set m_dDOM to the DOM of the value passed as the parameter.
  // This is because the centroid defuzzification method also uses this method
  // to determine the DOMs of the values it uses as its sample points.
  public virtual double CalculateDOM(double value)
  {
    return 0;
  }

  // If this fuzzy set is part of a consequent FLV (Fuzzy Linguistic Variable) and it is fired by a rule,
  // then this method sets the DOM (aka confidence level) to the max of the parameter value or the set's
  // existing degreeOfMembership value
  public void ORwithDOM(double val)
  {
    if (val > DOM)
    {
      DOM = val;
    }
  }

  /////////////////////
  /// Accessor methods
  public double GetRepresentativeValue()
  {
    return representativeValue;
  }

  public void ClearDOM()
  {
    DOM = 0.0;
  }

  public double GetDOM()
  {
    return DOM;
  }

  public void SetDOM(double val)
  {
    if (val >= 0 && val <= 1)
    {
      DOM = val;
    }
  }
}
