using System.Collections.Generic;

class FzAND : FuzzyTerm
{
  private List<FuzzyTerm> terms = new List<FuzzyTerm>();

  // Copy constructor
  public FzAND(FzAND fa)
  {
    foreach (FuzzyTerm fuzzyTerm in fa.terms)
    {
      terms.Add(fuzzyTerm.Clone());
    }
  }

  // Virtual constructor
  public override FuzzyTerm Clone()
  {
    return new FzAND(this);
  }

  // Constructor using two terms
  public FzAND(FuzzyTerm op1, FuzzyTerm op2)
  {
    terms.Add(op1.Clone());
    terms.Add(op2.Clone());
  }

  // Constructor using three terms
  public FzAND(FuzzyTerm op1, FuzzyTerm op2, FuzzyTerm op3)
  {
    terms.Add(op1.Clone());
    terms.Add(op2.Clone());
    terms.Add(op3.Clone());
  }

  public override double GetDOM()
  {
    double smallest = double.MaxValue;

    foreach (FuzzyTerm fuzzyTerm in terms)
    {
      if (fuzzyTerm.GetDOM() < smallest)
      {
        smallest = fuzzyTerm.GetDOM();
      }
    }

    return smallest;
  }

  public override void ORwithDOM(double value)
  {
    foreach (FuzzyTerm fuzzyTerm in terms)
    {
      fuzzyTerm.ORwithDOM(value);
    }
  }

  public override void ClearDOM()
  {
    foreach (FuzzyTerm fuzzyTerm in terms)
    {
      fuzzyTerm.ClearDOM();
    }
  }
}