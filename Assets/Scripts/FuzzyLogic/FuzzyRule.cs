public class FuzzyRule
{
  // Usually a composite of several fuzzy sets and operators
  private FuzzyTerm antecedent;
  // Usually a single fuzzy set, but can be several ANDed together
  private FuzzyTerm consequence;

  public FuzzyRule(FuzzyTerm ant, FuzzyTerm con)
  {
    antecedent = ant.Clone();
    consequence = con.Clone();
  }

  public void SetConfidenceOfConsequentToZero()
  {
    consequence.ClearDOM();
  }

  // This method updates the DOM (the confidence) of the consequent term with the DOM of the antecedent term
  public void Calculate()
  {
    consequence.ORwithDOM(antecedent.GetDOM());
  }
}
