using UnityEngine;
using Random = UnityEngine.Random;

public class AttractorMover : MonoBehaviour
{
  private Vector3 boundLeftUp;
  private Vector3 boundRightDown;

  private Vector3 center;

  private float rangeHorizontal;
  private float rangeVertical;

  [SerializeField] private float speed;
  private Vector3 curMovement;

  private bool bouncing = false;
  private bool transitioning = false;
  private bool idling = false;

  private float perlinX = 0f;
  private float perlinStepX = .001f;
  private float perlinY = 0f;
  private float perlinStepY = .002f;

  [SerializeField] private float multiplier = 2f;
  [SerializeField] private ScreenManager screenManager = null;

  private Camera mainCamera;

  private void Awake()
  {
    mainCamera = Camera.main;
  }

  // Start is called before the first frame update
  void Start()
  {
    float distance = Vector3.Distance(transform.position, mainCamera.transform.position);

    boundLeftUp = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, distance * 0.9f));
    boundRightDown = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, distance * 0.9f));

    boundLeftUp.x *= screenManager.GetXModifier();
    boundRightDown.x *= screenManager.GetXModifier();

    boundLeftUp.y *= screenManager.GetYModifier();
    boundRightDown.y *= screenManager.GetYModifier();

    rangeHorizontal = boundRightDown.x;
    rangeVertical = boundRightDown.y;

    center = transform.position;
    curMovement = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
  }

  public void SetSpeed(float newSpeed)
  {
    speed = newSpeed;
  }

  public void StartBouncing()
  {
    transitioning = false;
    bouncing = true;
    //curMovement = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);

    float mag = curMovement.magnitude;

    if (Vector3.Angle(curMovement, new Vector3(1, 1)) <= 45f)
    {
      curMovement = new Vector3(1, 1) * mag;
    }

    else if (Vector3.Angle(curMovement, new Vector3(-1, 1)) <= 45f)
    {
      curMovement = new Vector3(-1, 1) * mag;
    }

    else if (Vector3.Angle(curMovement, new Vector3(1, -1)) <= 45f)
    {
      curMovement = new Vector3(1, -1) * mag;
    }

    else if (Vector3.Angle(curMovement, new Vector3(-1, -1)) <= 45f)
    {
      curMovement = new Vector3(-1, -1) * mag;
    }
  }

  public void StopBouncing()
  {
    transitioning = true;
    bouncing = false;
  }

  // Update is called once per frame
  void Update()
  {
    if (idling)
    {
      if (transitioning)
        MoveToCenter();
      else
        PerlinMove();
    }

    else
    {
      if (transitioning)
      {
        MoveToCenter();
      }
      else
      {
        if (bouncing)
          Bounce();
        else
          PerlinMove();
      }
    }
  }

  public void StartIdle()
  {
    transitioning = true;
    idling = true;
  }

  public void StopIdle()
  {
    transitioning = true;
    idling = false;
  }

  private void MoveToCenter()
  {
    Vector3 goal;
      
    if (!idling)
    {
      float x = Mathf.PerlinNoise(perlinX, perlinY) * rangeHorizontal - (rangeHorizontal / 2);
      float y = Mathf.PerlinNoise(perlinY, perlinX) * rangeVertical - (rangeVertical / 2);

      goal = center + new Vector3(x * multiplier, y * multiplier, 0);

      curMovement = Vector3.MoveTowards(transform.localPosition, goal, speed * .01f);
    }
    else
    {
      goal = center;
      curMovement = Vector3.MoveTowards(transform.position, goal, speed * .75f);
    }

    transform.localPosition = curMovement;
    
    if (Vector3.Distance(curMovement, goal) < .1)
      transitioning = false;
  }

  private void Bounce()
  {
    if (transform.localPosition.x < boundLeftUp.x * 1.2f)
    {
      curMovement = Vector3.Reflect(curMovement, Vector3.right);
    }

    if (transform.localPosition.x > boundRightDown.x * 1.2f)
    {
      curMovement = Vector3.Reflect(curMovement, Vector3.left);
    }

    if (transform.localPosition.y < boundLeftUp.y * 1.2f)
    {
      curMovement = Vector3.Reflect(curMovement, Vector3.down);
    }
    if (transform.localPosition.y > boundRightDown.y * 1.2f)
    {
      curMovement = Vector3.Reflect(curMovement, Vector3.up);
    }

    curMovement.Normalize();
    curMovement *= speed * 0.4f;

    transform.Translate(curMovement);
  }

  private void PerlinMove()
  {
    float speedmult = speed;
    if (idling)
      speedmult *= 30f;
    perlinX += perlinStepX * speedmult;
    perlinY += perlinStepY * speedmult;

    float x = Mathf.PerlinNoise(perlinX, perlinY) * rangeHorizontal - (rangeHorizontal / 2);
    float y = Mathf.PerlinNoise(perlinY, perlinX) * rangeVertical - (rangeVertical / 2);

    Vector3 newPos;
    
    if (!idling)
      newPos = center + new Vector3(x * multiplier, y * multiplier, 0);
    else
      newPos = center + new Vector3(x * .2f, y * .2f, 0);

    curMovement = newPos - transform.localPosition;
    transform.localPosition = newPos;
  }
}
