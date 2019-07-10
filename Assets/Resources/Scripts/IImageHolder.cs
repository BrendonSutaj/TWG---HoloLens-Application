/**
 * @author [Brendon Sutaj]
 * @email [s9brendon.sutaj@gmail.com]
 * @create date 2019-04-01 12:00:00
 * @modify date 2019-07-10 16:12:58
 * @desc [description]
 */

#region USINGS
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Collections.Generic;
#endregion

public class IImageHolder : MonoBehaviour
{
    [SerializeField] public string SciGraph;

    public static List<string> ImageExtensions = new List<string> { ".jpg", ".jpe", ".bmp", ".gif", ".png" };

    /**
    * Used to load the sciGraph into the Image-Object.
    */
    void Start()
    {   
        // If there is no SciGraph - Data, or the Path is corrupt hide the SciGraph-Button.
        // Should not happen, since already checked (better save than sorry)
        if (SciGraph == null || SciGraph.Trim() == "")
        {
            transform.parent.transform.Find("Menu/SciGraph").gameObject.SetActive(false);
            return;
        }
        else
        {
            // Load the sciGraph as a texture onto the Image-Object from the StreamingAssets folder of the device.
            var path            = @Path.Combine(Application.streamingAssetsPath, SciGraph.Trim());

            // If the path does not exist or the extension is unknown hide the SciGraph button and return.
            if (!File.Exists(path) || !ImageExtensions.Contains(Path.GetExtension(path))) {
                transform.parent.transform.Find("Menu/SciGraph").gameObject.SetActive(false);
                return;
            }
            var byteArray       = File.ReadAllBytes(path);
            var sciGraph        = new Texture2D(1000, 1000);
            sciGraph.LoadImage(byteArray);

            // Remove transparancy.
            var pixels          = sciGraph.GetPixels();
            for (int i = 0; i < pixels.Length; i++) {
                pixels[i] = pixels[i].Equals(Color.clear) ? Color.white : pixels[i];
            }
            sciGraph.SetPixels(pixels);
            sciGraph.Apply();

            var image           = GameObject.Find("Canvas/Image");
            image.GetComponent<RawImage>().texture = sciGraph; 
        }
    }
}
