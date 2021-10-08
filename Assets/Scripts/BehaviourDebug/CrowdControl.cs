using System.Collections.Generic;
using UnityEngine;

public class CrowdControl : MonoBehaviour
{
  private List<Guy> guys = new List<Guy>();

  [SerializeField]
  private GameObject kinect = null;

  [SerializeField]
  private DataProcessor dataProcessor = null;

  private void Update()
  {
    string kinectDataString = "empty";

    // Loop through every Guy in the room
    if (guys.Count > 0)
    {
      kinectDataString = "";

      for (int i = 0; i < guys.Count; ++i)
      {
        float guyDistance = Vector3.Distance(kinect.transform.position, guys[i].transform.position) * 100f;

        kinectDataString += guyDistance.ToString() + "," + (guys[i].transform.position.x * 100f).ToString();

        if (i != guys.Count - 1)
        {
          kinectDataString += ",";
        }
      }
    }

    dataProcessor.ReceiveKinectData(kinectDataString);
  }

  private void OnCollisionEnter(Collision collision)
  {
    Guy guyComponent = collision.gameObject.GetComponent<Guy>();

    if (guyComponent)
    {
      guys.Add(guyComponent);
    }
  }

  private void OnCollisionExit(Collision collision)
  {
    Guy guyComponent = collision.gameObject.GetComponent<Guy>();

    if (guyComponent)
    {
      guys.Remove(guyComponent);
    }
  }
}
