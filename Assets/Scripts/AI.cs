using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.VFX;
using FMODUnity;

using UnityEngine.UI;

public class AI : MonoBehaviour
{
  private float leftCameraPos;
  private float rightCameraPos;

  private float crowdAverageDirection = 1500f;
  private float perceivedAnxiety = 0;
  private float perceivedAgitation = 0;
  // Used when alarmed
  private float realAnxiety = 0;
  private float realAgitation = 0;

  // agitation && anxiety > 85
  private const float ALARMED_THRESHOLD = 80f;
  private bool alarmed = false;
  private bool pointOfNoReturn = false;
  // After alarmed, lerp back to current values
  private bool coolingDown = false;

  // Lerp amount when cooling down
  private const float COOLING_DOWN_RATE = 0.015f;
  private const float COOLING_DOWN_THRESHOLD = 70f;
  private const float RESTART_THRESHOLD = 30f;
  private const float SHUTDOWN_MIN_TIME = 10f;
  private float shutdownTime;

  private float alarmedStartTime;
  // 20 seconds before it shutsdown
  private const float ALARMED_TIME_THRESHOLD = 20f;

  private bool shutdown = false;

  // We have to map the kinect x position (0 is left 3000 is right) to the actual min and max world position of the AI.
  // This is how much world position it should move per kinect unit
  private float worldToKinectUnit; 

  [SerializeField]
  private VisualEffect[] flares = null;

  [SerializeField]
  private Gradient[] gradientPresets = null;

  [SerializeField]
  private StudioEventEmitter audioEmitter = null;

  [SerializeField]
  private ScreenManager screenManager = null;

  private AttractorMover attractorMover;

  private void Awake()
  {
    Camera camera = Camera.main;
    attractorMover = GetComponentInChildren<AttractorMover>();

    float depth = transform.position.z - camera.transform.position.z;

    leftCameraPos = Camera.main.ScreenToWorldPoint(Vector3.zero + Vector3.forward * depth).x * screenManager.GetXModifier();
    rightCameraPos = -leftCameraPos;

    worldToKinectUnit = ( Mathf.Abs(leftCameraPos) + Mathf.Abs(rightCameraPos) ) / Constants.ROOM_WIDTH;
    // Multiply by this percentage, which dictates the max it can move towards the screen
    worldToKinectUnit *= Constants.AI_MAX_AVOID_PERCENTAGE;
  }

  private void Update()
  {
    float depth = transform.position.z - Camera.main.transform.position.z;
    Debug.Log(Camera.main.ScreenToWorldPoint(transform.position + Vector3.forward * depth));
    if (shutdown)
    {
      if (Time.time - shutdownTime >= SHUTDOWN_MIN_TIME)
      {
        if (realAnxiety < RESTART_THRESHOLD && realAgitation < RESTART_THRESHOLD)
        {
          shutdown = false;
          audioEmitter.SetParameter("Shutdown", 0);
        }
      }

      return;
    }

    audioEmitter.SetParameter("Anxiety", perceivedAnxiety);
    audioEmitter.SetParameter("Agitation", perceivedAgitation);

    AIAvoidanceModule();

    if (!alarmed && realAnxiety > ALARMED_THRESHOLD && realAgitation > ALARMED_THRESHOLD)
    {
      coolingDown = false;
      alarmed = true;
      alarmedStartTime = Time.time;

      attractorMover.StartBouncing();

      audioEmitter.SetParameter("Alarmed", 2);
    }

    SetAnxietyParameters();
    SetAgitationParameters();

    if (alarmed)
    {
      Debug.Log("ALARMED!!!!" + " " + realAnxiety + " " + realAgitation);

      if (!pointOfNoReturn && realAnxiety < COOLING_DOWN_THRESHOLD && realAgitation < COOLING_DOWN_THRESHOLD)
      {
        alarmed = false;
        coolingDown = true;

        attractorMover.StopBouncing();

        audioEmitter.SetParameter("Alarmed", 0);
      }

      if (Time.time - alarmedStartTime >= 19.5f)
      {
        attractorMover.StopBouncing();
        attractorMover.StartIdle();

        pointOfNoReturn = true;
      }

      if (Time.time - alarmedStartTime >= ALARMED_TIME_THRESHOLD)
      {
        Shutdown();
      }
    }
  }

  private void AIAvoidanceModule()
  {
    float worldCrowdAverageDirection = crowdAverageDirection;

    Vector3 newPos = transform.position;
    newPos.x = worldCrowdAverageDirection * worldToKinectUnit;
    // Invert the position since it should be avoiding the crowd instead of following them
    newPos.x *= -1;

    transform.position = Vector3.Lerp(transform.position, newPos, Constants.AI_POSITION_LERP_SPEED);

    if (Mathf.Approximately(transform.position.x, newPos.x))
    {
      transform.position = newPos;
    }

    // Set the particle spawn positions to move with the attractor
    for (int i = 0; i < flares.Length; ++i)
    {
      Vector3 currentPos = flares[i].GetVector3("ParticleSpawnPosition");
      Vector3 spawnPos = currentPos;

      spawnPos.x = newPos.x * Constants.FLARE_PARTICLE_AVOID_MODIFIER;
      currentPos = Vector3.Lerp(currentPos, spawnPos, Constants.AI_POSITION_LERP_SPEED);

      if (Mathf.Approximately(currentPos.x, spawnPos.x))
      {
        flares[i].SetVector3(Constants.FLARE_NAME_PARTICLE_SPAWN_POSITION, spawnPos);
      }

      else
      {
        flares[i].SetVector3(Constants.FLARE_NAME_PARTICLE_SPAWN_POSITION, currentPos);
      }
    }
  }

  private void SetAnxietyParameters()
  {
    // Set the rotation speed and attraction strength of the emittor
    for (int i = 0; i < flares.Length; ++i)
    {
      flares[i].SetFloat(
        Constants.FLARE_NAME_ROTATION_SPEED, 
        Mathf.Lerp(Constants.FLARE_MIN_ROTATION_SPEED, Constants.FLARE_MAX_ROTATION_SPEED, perceivedAnxiety * 0.01f));

      flares[i].SetFloat(
        Constants.FLARE_NAME_LIFETIME,
        Mathf.Lerp(Constants.FLARE_MIN_LIFETIME, Constants.FLARE_MAX_LIFETIME, perceivedAnxiety * 0.01f));

      // Uses y = 1.049551384^x * 0.8 - 0.8, x = anxiety, y = output
      float attractiveStrengthPercentage = Mathf.Pow(1.049551384f, perceivedAnxiety) * 0.8f - 0.8f;

      float alarmedModifer = alarmed ? 
        Mathf.Lerp(
          Constants.FLARE_ALARMED_MIN_ATTRACTIVE_STRENGTH_MULTIPLIER, 
        Constants.FLARE_ALARMED_MAX_ATTRACTIVE_STRENGTH_MULTIPLIER, 
        (Time.time - alarmedStartTime) / ALARMED_TIME_THRESHOLD) : 1f;

      flares[i].SetFloat(
        Constants.FLARE_NAME_ATTRACTIVE_STRENGTH,
        Mathf.Lerp(Constants.FLARE_MIN_ATTRACTIVE_STRENGTH, Constants.FLARE_MAX_ATTRACTIVE_STRENGTH, attractiveStrengthPercentage * 0.01f) * alarmedModifer);
    }

    attractorMover.SetSpeed(Mathf.Lerp(Constants.ATTRACTOR_MIN_SPEED, Constants.ATTRACTOR_MAX_SPEED, perceivedAnxiety * 0.01f));
  }

  private void SetAgitationParameters()
  {
    Gradient agitatedGradient = null;
    float agitationPercentage = perceivedAgitation * 0.01f;

    if (agitationPercentage >= 0f && agitationPercentage < 0.45f)
    {
      agitatedGradient = LerpGradient(gradientPresets[0], gradientPresets[1], agitationPercentage / 0.45f);
    }

    else
    {
      agitatedGradient = LerpGradient(gradientPresets[1], gradientPresets[2], (agitationPercentage - 0.45f) / (1f - 0.45f));
    }

    // Set the rotation speed and attraction strength of the emittor
    for (int i = 0; i < flares.Length; ++i)
    {
      flares[i].SetFloat(
        Constants.FLARE_NAME_PARTICLE_SPAWN_RATE,
        Mathf.Lerp(Constants.FLARE_MIN_PARTICLE_SPAWN_RATE, Constants.FLARE_MAX_PARTICLE_SPAWN_RATE, agitationPercentage));

      flares[i].SetGradient(Constants.FLARE_NAME_PARTICLE_GRADIENT, agitatedGradient);
    }
  }

  private void Shutdown()
  {
    shutdown = true;
    alarmed = false;
    pointOfNoReturn = false;

    attractorMover.StopBouncing();
    attractorMover.StopIdle();

    perceivedAnxiety = 0f;
    perceivedAgitation = 25f;

    SetAnxietyParameters();
    SetAgitationParameters();

    audioEmitter.SetParameter("Shutdown", 2);
    audioEmitter.SetParameter("Alarmed", 0);

    StartCoroutine("DropSpawnRate");

    shutdown = true;
    shutdownTime = Time.time;
  }

  private IEnumerator DropSpawnRate()
  {
    float startTime = Time.time;
    float startSpawnRate = flares[0].GetFloat(Constants.FLARE_NAME_PARTICLE_SPAWN_RATE);
    float duration = 2f;

    for (int i = 0; i < flares.Length; ++i)
    {
      flares[i].SetFloat(Constants.FLARE_NAME_LIFETIME, 5f);
    }

    while (Time.time - startTime < duration)
    {
      for (int i = 0; i < flares.Length; ++i)
      {
        flares[i].SetFloat(Constants.FLARE_NAME_PARTICLE_SPAWN_RATE, startSpawnRate - (  (Time.time - startTime) / duration * startSpawnRate ) );
      }

      yield return 1;
    }

    for (int i = 0; i < flares.Length; ++i)
    {
      flares[i].SetFloat(Constants.FLARE_NAME_PARTICLE_SPAWN_RATE, 0f);
    }
  }

  public void SetCrowdAverageDirection(float crowdAvrg)
  {
    crowdAverageDirection = crowdAvrg;
  }

  public void SetAnxiety(float anxietyInput)
  {
    if (anxietyInput >= 0f && anxietyInput <= 100.0f)
    {
      if (!alarmed)
      {
        if (coolingDown && anxietyInput < perceivedAnxiety)
        {
          realAnxiety = anxietyInput;
          perceivedAnxiety = Mathf.Lerp(perceivedAnxiety, anxietyInput, COOLING_DOWN_RATE);

          //Debug.Log("Cooling down " + realAnxiety + " " + realAgitation);

          if (Mathf.Approximately(perceivedAnxiety, anxietyInput))
          {
            realAnxiety = perceivedAnxiety = anxietyInput;
            coolingDown = false;
          }
        }
        
        else
        {
          realAnxiety = perceivedAnxiety = anxietyInput;
          coolingDown = false;
        }
      }
      
      else
      {
        if (anxietyInput > perceivedAnxiety)
        {
          realAnxiety = perceivedAnxiety = anxietyInput;
        }
        
        else
        {
          realAnxiety = anxietyInput;
        }
      }
    }
  }

  public void SetAgitation(float agitationInput)
  {
    if (agitationInput >= 0f && agitationInput <= 100.0f)
    {
      if (!alarmed)
      {
        if (coolingDown && agitationInput < perceivedAgitation)
        {
          realAgitation = agitationInput;
          perceivedAgitation = Mathf.Lerp(perceivedAgitation, agitationInput, COOLING_DOWN_RATE);

          if (Mathf.Approximately(perceivedAgitation, agitationInput))
          {
            realAgitation = perceivedAgitation = agitationInput;
            coolingDown = false;
          }
        }

        else
        {
          realAgitation = perceivedAgitation = agitationInput;
          coolingDown = false;
        }
      }

      else
      {
        if (agitationInput > perceivedAgitation)
        {
          realAgitation = perceivedAgitation = agitationInput;
        }

        else
        {
          realAgitation = agitationInput;
        }
      }
    }
  }

  // Helper function
  private Gradient LerpGradient(Gradient a, Gradient b, float t)
  {
    //list of all the unique key times
    var keysTimes = new List<float>();

    for (int i = 0; i < a.colorKeys.Length; i++)
    {
      float k = a.colorKeys[i].time;
      if (!keysTimes.Contains(k))
        keysTimes.Add(k);
    }

    for (int i = 0; i < b.colorKeys.Length; i++)
    {
      float k = b.colorKeys[i].time;
      if (!keysTimes.Contains(k))
        keysTimes.Add(k);
    }

    for (int i = 0; i < a.alphaKeys.Length; i++)
    {
      float k = a.alphaKeys[i].time;
      if (!keysTimes.Contains(k))
        keysTimes.Add(k);
    }

    for (int i = 0; i < b.alphaKeys.Length; i++)
    {
      float k = b.alphaKeys[i].time;
      if (!keysTimes.Contains(k))
        keysTimes.Add(k);
    }

    GradientColorKey[] clrs = new GradientColorKey[keysTimes.Count];
    GradientAlphaKey[] alphas = new GradientAlphaKey[keysTimes.Count];

    //Pick colors of both gradients at key times and lerp them
    for (int i = 0; i < keysTimes.Count; i++)
    {
      float key = keysTimes[i];
      var clr = Color.Lerp(a.Evaluate(key), b.Evaluate(key), t);
      clrs[i] = new GradientColorKey(clr, key);
      alphas[i] = new GradientAlphaKey(clr.a, key);
    }

    var g = new Gradient();
    g.SetKeys(clrs, alphas);

    return g;
  }
}