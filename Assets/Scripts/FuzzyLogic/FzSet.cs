public class FzSet : FuzzyTerm
{
  private FuzzySet set;

  public FzSet(FuzzySet fs)
  {
    set = fs;
  }

  public override FuzzyTerm Clone()
  {
    return new FzSet(set);
  }

  public override double GetDOM()
  {
    return set.GetDOM();
  }

  public override void ClearDOM()
  {
    set.ClearDOM();
  }

  public override void ORwithDOM(double value)
  {
    set.ORwithDOM(value);
  }
}