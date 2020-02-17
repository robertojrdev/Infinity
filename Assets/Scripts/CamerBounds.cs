 using UnityEngine;
 
 public class CameraBounds
 {
     public Bounds targetBounds;
 
     public static void Bound(Bounds targetBounds, Camera camera)
     {
 
         float screenRatio = (float)Screen.width / (float)Screen.height;
         float targetRatio = targetBounds.size.x / targetBounds.size.y;
 
         if (screenRatio >= targetRatio)
         {
             Camera.main.orthographicSize = targetBounds.size.y / 2;
         }
         else
         {
             float differenceInSize = targetRatio / screenRatio;
             Camera.main.orthographicSize = targetBounds.size.y / 2 * differenceInSize;
         }
 
         camera.transform.position = new Vector3(targetBounds.center.x, targetBounds.center.y, -1f);

     }
 
 }