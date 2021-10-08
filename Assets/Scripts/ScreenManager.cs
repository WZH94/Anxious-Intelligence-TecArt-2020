using UnityEngine;
using UnityEngine.UI;

public class ScreenManager : MonoBehaviour
{
  [SerializeField]
  private Image leftBlocker = null, rightBlocker = null, topBlocker = null, bottomBlocker = null;
  [SerializeField]
  private Vector2 targetResolution, defaultResolution;

  public Vector2 CurrentResolution { get; private set; }
  private Vector2 centerPoint;
  public Vector2 ScreenSize { get; private set; }

  private Vector2 referenceResolution;
  private Vector2 screenResolution;
  private Vector2 resolutionModifier;

  [SerializeField]
  private Text debugText;

  private void Awake()
  {
    FindResolution();

    centerPoint = new Vector2((topBlocker.transform.position.x - bottomBlocker.transform.position.x) / 2f, (leftBlocker.transform.position.y - rightBlocker.transform.position.y) / 2f);

    referenceResolution = GetComponent<CanvasScaler>().referenceResolution;
    SetTargetResolution();
  }

  private void Update()
  {
    FindResolution();
    Debug.Log(Screen.width + " " + Screen.height);
    Debug.Log(CurrentResolution);

    debugText.text = CurrentResolution.ToString();
  }

  private void SetTargetResolution()
  {
    float targetResolutionUnit = Screen.height / targetResolution.x;

    ScreenSize = new Vector2(Screen.height, targetResolutionUnit * targetResolution.y);

    Vector3 topBlockerPos = topBlocker.transform.position;
    topBlockerPos.x = centerPoint.x + (targetResolutionUnit * targetResolution.y) / 2f;
    topBlocker.transform.position = topBlockerPos;

    Vector3 bottomBlockerPos = bottomBlocker.transform.position;
    bottomBlockerPos.x = centerPoint.x - (targetResolutionUnit * targetResolution.y) / 2f;
    bottomBlocker.transform.position = bottomBlockerPos;
  }

  private void FindResolution()
  {
    screenResolution = new Vector2(Screen.width, Screen.height);
    resolutionModifier = new Vector2(screenResolution.x / referenceResolution.x, screenResolution.y / referenceResolution.y);

    //Debug.Log(referenceResolution + " " + screenResolution + " " + resolutionModifier.x.ToString("F4") + "," + resolutionModifier.y.ToString("F4"));

    //Debug.Log(Screen.width + " " + Screen.height);
    //Debug.Log(horizontalOffset + " " + verticalOffset);
    Debug.Log(leftBlocker.transform.position + " " + rightBlocker.transform.position);
    Debug.Log(topBlocker.transform.position + " " + bottomBlocker.transform.position);

    CurrentResolution = new Vector2(
      (leftBlocker.rectTransform.position.y - rightBlocker.rectTransform.position.y),
      (topBlocker.rectTransform.position.x - bottomBlocker.rectTransform.position.x)
    );
  }

  public float GetXModifier()
  {
    return ScreenSize.x / Screen.height;
  }

  public float GetYModifier()
  {
    Debug.Log(ScreenSize.y / Screen.width);
    return ScreenSize.y / Screen.width;
  }
}
