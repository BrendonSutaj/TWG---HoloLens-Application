/**
 * @author [Brendon Sutaj]
 * @email [s9brendon.sutaj@gmail.com]
 * @create date 2019-04-01 12:00:00
 * @modify date 2019-07-31 10:17:42
 * @desc [description]
 */

using HoloToolkit.Unity.SpatialMapping;
using HoloToolkit.Unity.SpatialMapping.Tests;
using UnityEngine;

public class SPDeactivator : MonoBehaviour
{
    // Deactivates the Scripts running on the SpatialProcessing prefab, to avoid nullpointer.
    public void deactivate()
    {
        gameObject.GetComponent<RemoveSurfaceVertices>().enabled = false;
        Destroy(gameObject.GetComponent<RemoveSurfaceVertices>().gameObject);

        gameObject.GetComponent<SurfaceMeshesToPlanes>().enabled = false;
        Destroy(gameObject.GetComponent<SurfaceMeshesToPlanes>().gameObject);

        gameObject.GetComponent<SpatialProcessingTest>().enabled = false;
        Destroy(gameObject.GetComponent<SpatialProcessingTest>().gameObject);

        Destroy(gameObject);
    }
}
