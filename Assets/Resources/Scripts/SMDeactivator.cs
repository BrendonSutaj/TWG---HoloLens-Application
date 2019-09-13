/**
 * @author [Brendon Sutaj]
 * @email [s9brendon.sutaj@gmail.com]
 * @create date 2019-04-01 12:00:00
 * @modify date 2019-09-12 22:49:54
 * @desc [description]
 */

using HoloToolkit.Unity.SpatialMapping;
using UnityEngine;

public class SMDeactivator : MonoBehaviour
{
    // Deactivates the Scripts running on the SpatialMapping prefab, to avoid nullpointer.
    public void deactivate()
    {
        gameObject.GetComponent<SpatialMappingObserver>().enabled = false;
        Destroy(gameObject.GetComponent<SpatialMappingObserver>().gameObject);

        gameObject.GetComponent<SpatialMappingManager>().enabled = false;
        Destroy(gameObject.GetComponent<SpatialMappingManager>().gameObject);

        gameObject.GetComponent<ObjectSurfaceObserver>().enabled = false;
        Destroy(gameObject.GetComponent<ObjectSurfaceObserver>().gameObject);

        Destroy(gameObject);
    }
}
