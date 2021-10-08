using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class User
{
  public float distanceFromKinect;
  public float xDirection;
}

public class DataProcessor : MonoBehaviour
{
  private const int MAX_USERS = 6;
  private List<User> users = new List<User>();
  private int numUsers = 0;

  private const float MOVE_THRESHOLD = Constants.ROOM_LENGTH / 20f;

  // The previously recorded nearest distance to AI
  private float lastNearestDistance = Constants.ROOM_LENGTH;
  private float distanceCoveredThisFrame = 0;
  private float movementScore = 0;
  private const float MOVEMENT_SCORE_EASER = -0.6f;
  private const float MAX_DISTANCE_COVERABLE = 30f;

  private FuzzyLogic fuzzyLogic = null;
  private AI ai = null;

  private void Awake()
  {
    fuzzyLogic = GetComponent<FuzzyLogic>();
    ai = GetComponent<AI>();

    for (int i = 0; i < MAX_USERS; ++i)
    {
      users.Add(new User());
    }
  }

  private void Update()
  {
    float nearestDistance = Constants.ROOM_LENGTH;
    float avrgDirection = 0f;
    float weightedAvrg = 0f;
    int numWeights = 0;

    for (int i = 0; i < numUsers; ++i)
    {
      nearestDistance = users[i].distanceFromKinect < nearestDistance ? users[i].distanceFromKinect : nearestDistance;
      avrgDirection += users[i].xDirection;

      // Invert so closer distance has higher value
      float invertedDistance = Constants.ROOM_LENGTH - users[i].distanceFromKinect;
      int weight = (int)(invertedDistance / 500f) + 1;
      numWeights += weight;

      // Every 500mm adds one weight
      weightedAvrg += invertedDistance * weight;
    }

    if (numUsers > 0)
    {
      avrgDirection /= numUsers;

      weightedAvrg /= numUsers;

      Debug.Log(weightedAvrg);
    }

    if (numUsers == 0)
    {
      lastNearestDistance = nearestDistance;
    }

    distanceCoveredThisFrame = lastNearestDistance - nearestDistance;
    lastNearestDistance = nearestDistance;

    movementScore += CalculateMovementScoreThisFrame();
    movementScore = Mathf.Clamp(movementScore, -750f, 1200f);

    EaseMovementScore();

    ai.SetCrowdAverageDirection(avrgDirection);
    fuzzyLogic.SetValues(numUsers, nearestDistance, weightedAvrg, movementScore);
  }

  private float CalculateMovementScoreThisFrame()
  {
    // Clamp the distance covered from -20 to 30, representing max 35mm per frame (1.8m/s) 
    distanceCoveredThisFrame = Mathf.Clamp(distanceCoveredThisFrame, -25f, 32f);

    // If moving towards AI, distance covered is positive, else negative. Negative has a different graph to return smaller negative values instead
    //if (distanceCoveredThisFrame >= 0)
    //{
    // y = 1.09 ^ x * 0.1 - 0.1, y = movement score, x = distance covered this frame
    //return Mathf.Pow(1.173346f, distanceCoveredThisFrame) * 0.1f - 0.1f;
    return Mathf.Pow(1.136f, distanceCoveredThisFrame) * 0.3f - 0.3f;
    //}

    //else
    //{
    //  // y = -1.1 ^ -x * 0.1 + 0.1, y = movement score, x = distance covered this frame
    //  return -Mathf.Pow(1f, -distanceCoveredThisFrame) * 0.1f + 0.1f;
    //}
  }

  private void EaseMovementScore()
  {
    movementScore += MOVEMENT_SCORE_EASER;

    if (movementScore < 0)
    {
      movementScore = 0;
    }
  }

  public void ReceiveKinectData(string input)
  {
    if (input.Length == 0)
    {
      return;
    }

    //Debug.Log("-----------------------------------------");
    string[] sortedInput = input.Split(',');

    if (sortedInput.Length % 2 != 0)
    {
      numUsers = 0;
      //print("Wrong input sent: " + sortedInput.Length + " " + input);
    }

    else
    {
      numUsers = (int)(sortedInput.Length / 2f);

      for (int i = 0; i < sortedInput.Length; i++)
      {
        int x = i / 2;
        int y = i % 2;

        CultureInfo newCulture = new CultureInfo("en-US");

        float tempVal = float.Parse(sortedInput[i], newCulture);

        switch (y)
        {
          case 0:
            users[x].distanceFromKinect = tempVal;
            break;
          case 1:
            users[x].xDirection = tempVal;
            break;
        }
      }
    }
  }
}
