using UnityEngine;

public class Guy : MonoBehaviour
{
  public bool move = false;
  public float speed = 0;

  // Update is called once per frame
  void Update()
  {
    if (move)
    {
      if (speed > 30f)
      {
        speed = 30f;
      }

      if (speed < -30f)
      {
        speed = -30f;
      }

      Vector3 newPos = transform.position;

      newPos.z += speed / 100f;

      if (newPos.z > 0)
      {
        newPos.z = 0;
      }

      if (newPos.z < - 50f)
      {
        newPos.z = -50f;
      }

      transform.position = newPos;
    }
  }
}
