using System.Collections.Generic;
using UnityEngine;

public class FuzzyModule : MonoBehaviour
{
  // Map of all the fuzzy variables this module uses
  private Dictionary<string, FuzzyVariable> fuzzyVariables = new Dictionary<string, FuzzyVariable>();

  // List containing all the fuzzy rules
  private List<FuzzyRule> rules = new List<FuzzyRule>();

  // Zeroes the DOMs of the consequents of each rule. Used by Defuzzify()
  private void SetConfidencesOfConsequentsToZero()
  {
    foreach (FuzzyRule fuzzyRule in rules)
    {
      fuzzyRule.SetConfidenceOfConsequentToZero();
    }
  }

  // Creates a new "empty" fuzzy variable and returns a reference to it.
  public FuzzyVariable CreateFLV(string varName)
  {
    FuzzyVariable fuzzyVariable = new FuzzyVariable();

    fuzzyVariables.Add(varName, fuzzyVariable);

    return fuzzyVariable;
  }

  public void AddRule(FuzzyTerm antecedent, FuzzyTerm consequence)
  {
    rules.Add(new FuzzyRule(antecedent, consequence));
  }

  public void Fuzzify(string nameOfFLV, double val)
  {
    fuzzyVariables[nameOfFLV].Fuzzify(val);
  }

  public double DeFuzzify(string nameOfFLV)
  {
    //clear the DOMs of all the consequents of all the rules
    SetConfidencesOfConsequentsToZero();

    foreach (FuzzyRule fuzzyRule in rules)
    {
      fuzzyRule.Calculate();
    }

    return fuzzyVariables[nameOfFLV].Defuzzify();
  }
}
