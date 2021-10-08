using UnityEngine;

public class FuzzyLogic : MonoBehaviour
{
  private FuzzyModule fuzzyModule;

  private double crowdSize = 0.0, avrgDistToAI = 0, distToAI = 5000.0, closingRate = 0.0;

  private AI ai;

  private void Start()
  {
    ai = GetComponent<AI>();

    fuzzyModule = GetComponent<FuzzyModule>();

    FuzzyVariable crowdSize = fuzzyModule.CreateFLV("Crowd Size");

    FzSet crowdEmpty = new FzSet(crowdSize.AddLeftShoulderSet("CrowdEmpty",
      0,
      1,
      3)
    );

    FzSet crowdSparse = new FzSet(crowdSize.AddTriangularSet("CrowdSparse",
      1,
      3,
      5)
    );

    FzSet crowdPacked = new FzSet(crowdSize.AddRightShoulderSet("CrowdPacked",
      3,
      5,
      6)
    );

    FuzzyVariable distToAI = fuzzyModule.CreateFLV("Dist To AI");

    FzSet distNear = new FzSet(distToAI.AddLeftShoulderSet("DistNear",
      0,
      8500,
      16500)
    );

    FzSet distMedium = new FzSet(distToAI.AddTriangularSet("DistMedium",
      8000,
      15000,
      22500)
    );

    FzSet distFar = new FzSet(distToAI.AddRightShoulderSet("DistFar",
      12500,
      22500,
      30000)
    );

    FuzzyVariable avrgDistToAI = fuzzyModule.CreateFLV("Average Dist To AI");

    FzSet avrgDistFar = new FzSet(avrgDistToAI.AddLeftShoulderSet("AvrgDistNear",
      0,
      2125,
      3500)
    );

    FzSet avrgDistMedium = new FzSet(avrgDistToAI.AddTriangularSet("AvrgDistMedium",
      1875,
      4375,
      8000)
    );

    FzSet avrgDistNear = new FzSet(avrgDistToAI.AddRightShoulderSet("AvrgDistFar",
      6250,
      8125,
      10000)
    );

    FuzzyVariable crowdClosingRate = fuzzyModule.CreateFLV("Crowd Closing Rate");

    FzSet closingRateRetreating = new FzSet(crowdClosingRate.AddLeftShoulderSet("ClosingRateRetreating",
      -250,
      -200,
      -100)
    );

    FzSet closingRateWithdrawing = new FzSet(crowdClosingRate.AddTriangularSet("ClosingRateWithdrawing",
      -200,
      -100,
      0)
    );

    FzSet closingRateStill = new FzSet(crowdClosingRate.AddTriangularSet("ClosingRateStill",
      -100,
      0,
      150)
    );

    FzSet closingRateApproaching = new FzSet(crowdClosingRate.AddTriangularSet("ClosingRateApproaching",
      0,
      150,
      400)
    );

    FzSet closingRateAdvancing = new FzSet(crowdClosingRate.AddTriangularSet("ClosingRateAdvancing",
      150,
      450,
      700)
    );

    FzSet closingRateRushing = new FzSet(crowdClosingRate.AddRightShoulderSet("ClosingRateRushing",
      450,
      700,
      800)
    );

    FuzzyVariable anxietyState = fuzzyModule.CreateFLV("Anxiety State");

    FzSet anxietyCalm = new FzSet(anxietyState.AddLeftShoulderSet("AnxietyCalm",
      0,
      0,
      15)
    );

    FzSet anxietyUneasy = new FzSet(anxietyState.AddTriangularSet("AnxietyUneasy",
      5,
      25,
      45)
    );

    FzSet anxietyNervous = new FzSet(anxietyState.AddTriangularSet("AnxietyNervous",
      20,
      50,
      75)
    );

    FzSet anxietyDistressed = new FzSet(anxietyState.AddTriangularSet("AnxietyDistressed",
      50,
      80,
      95)
    );

    FzSet anxietyPanicked = new FzSet(anxietyState.AddRightShoulderSet("AnxietyPanicked",
      80,
      100,
      100)
    );

    FuzzyVariable agitationState = fuzzyModule.CreateFLV("Agitation State");

    FzSet agitationMeek = new FzSet(agitationState.AddLeftShoulderSet("AgitationMeek",
      0,
      0,
      25)
    );

    FzSet agitationAnnoyed = new FzSet(agitationState.AddTriangularSet("AgitationAnnoyed",
      5,
      25,
      50)
    );

    FzSet agitationAgitated = new FzSet(agitationState.AddTriangularSet("AgitationAgitated",
      30,
      50,
      80)
    );

    FzSet agitationAggressive = new FzSet(agitationState.AddTriangularSet("AgitationAggressive",
      50,
      80,
      95)
    );

    FzSet agitationEnraged = new FzSet(agitationState.AddRightShoulderSet("AgitationEnraged",
      80,
      100,
      100)
    );

    // Anxiety rules
    //fuzzyModule.AddRule(new FzAND(crowdEmpty, avrgDistFar), anxietyCalm);
    //fuzzyModule.AddRule(new FzAND(crowdEmpty, avrgDistMedium), anxietyUneasy);
    //fuzzyModule.AddRule(new FzAND(crowdEmpty, avrgDistNear), anxietyNervous);
    //fuzzyModule.AddRule(new FzAND(crowdSparse, avrgDistFar), anxietyUneasy);
    //fuzzyModule.AddRule(new FzAND(crowdSparse, avrgDistMedium), anxietyNervous);
    //fuzzyModule.AddRule(new FzAND(crowdSparse, avrgDistNear), anxietyDistressed);
    //fuzzyModule.AddRule(new FzAND(crowdPacked, avrgDistFar), anxietyNervous);
    //fuzzyModule.AddRule(new FzAND(crowdPacked, avrgDistMedium), anxietyDistressed);
    //fuzzyModule.AddRule(new FzAND(crowdPacked, avrgDistNear), anxietyPanicked);
    fuzzyModule.AddRule(new FzAND(crowdEmpty, distFar, avrgDistFar), anxietyCalm);
    fuzzyModule.AddRule(new FzAND(crowdEmpty, distMedium, avrgDistFar), anxietyCalm);
    fuzzyModule.AddRule(new FzAND(crowdEmpty, distMedium, avrgDistMedium), anxietyUneasy);
    fuzzyModule.AddRule(new FzAND(crowdEmpty, distNear, avrgDistFar), anxietyUneasy);
    fuzzyModule.AddRule(new FzAND(crowdEmpty, distNear, avrgDistMedium), anxietyNervous);
    fuzzyModule.AddRule(new FzAND(crowdEmpty, distNear, avrgDistNear), anxietyNervous);

    fuzzyModule.AddRule(new FzAND(crowdSparse, distFar, avrgDistFar), anxietyUneasy);
    fuzzyModule.AddRule(new FzAND(crowdSparse, distMedium, avrgDistFar), anxietyUneasy);
    fuzzyModule.AddRule(new FzAND(crowdSparse, distMedium, avrgDistMedium), anxietyNervous);
    fuzzyModule.AddRule(new FzAND(crowdSparse, distNear, avrgDistFar), anxietyNervous);
    fuzzyModule.AddRule(new FzAND(crowdSparse, distNear, avrgDistMedium), anxietyNervous);
    fuzzyModule.AddRule(new FzAND(crowdSparse, distNear, avrgDistNear), anxietyDistressed);

    fuzzyModule.AddRule(new FzAND(crowdPacked, distFar, avrgDistFar), anxietyNervous);
    fuzzyModule.AddRule(new FzAND(crowdPacked, distMedium, avrgDistFar), anxietyDistressed);
    fuzzyModule.AddRule(new FzAND(crowdPacked, distMedium, avrgDistMedium), anxietyDistressed);
    fuzzyModule.AddRule(new FzAND(crowdPacked, distNear, avrgDistFar), anxietyDistressed);
    fuzzyModule.AddRule(new FzAND(crowdPacked, distNear, avrgDistMedium), anxietyPanicked);
    fuzzyModule.AddRule(new FzAND(crowdPacked, distNear, avrgDistNear), anxietyPanicked);

    // Agitation rules
    fuzzyModule.AddRule(new FzAND(closingRateApproaching, distFar), agitationMeek);
    fuzzyModule.AddRule(new FzAND(closingRateApproaching, distMedium), agitationAnnoyed);
    fuzzyModule.AddRule(new FzAND(closingRateApproaching, distNear), agitationAgitated);
    fuzzyModule.AddRule(new FzAND(closingRateAdvancing, distFar), agitationAnnoyed);
    fuzzyModule.AddRule(new FzAND(closingRateAdvancing, distMedium), agitationAgitated);
    fuzzyModule.AddRule(new FzAND(closingRateAdvancing, distNear), agitationAggressive);
    fuzzyModule.AddRule(new FzAND(closingRateRushing, distFar), agitationAgitated);
    fuzzyModule.AddRule(new FzAND(closingRateRushing, distMedium), agitationAggressive);
    fuzzyModule.AddRule(new FzAND(closingRateRushing, distNear), agitationEnraged);
    fuzzyModule.AddRule(new FzAND(closingRateStill, distFar), agitationMeek);
    fuzzyModule.AddRule(new FzAND(closingRateStill, distMedium), agitationMeek);
    fuzzyModule.AddRule(new FzAND(closingRateStill, distNear), agitationAnnoyed);
  }

  private void Update()
  {
    double anxiety = CalculateAnxiety();
    double agitation = CalculateAgitation();
    //Debug.Log("Anxiety: " + crowdSize + " " + avrgDistToAI + " " + anxiety);
    //Debug.Log("Agitation: " + distToAI + " " + closingRate + " " + agitation);

    ai.SetAnxiety((float)anxiety);
    ai.SetAgitation((float)agitation);
  }
  
  public void SetValues(double size, double dist, double avrgDist, double speed)
  {
    crowdSize = size;
    distToAI = dist;
    avrgDistToAI = avrgDist;
    closingRate = speed;
  }

  private double CalculateAnxiety()
  {
    fuzzyModule.Fuzzify("Crowd Size", crowdSize);
    fuzzyModule.Fuzzify("Dist To AI", distToAI);
    fuzzyModule.Fuzzify("Average Dist To AI", avrgDistToAI);

    return fuzzyModule.DeFuzzify("Anxiety State");
  }

  private double CalculateAgitation()
  {
    fuzzyModule.Fuzzify("Dist To AI", distToAI);
    fuzzyModule.Fuzzify("Crowd Closing Rate", closingRate);

    return fuzzyModule.DeFuzzify("Agitation State");
  }
}
