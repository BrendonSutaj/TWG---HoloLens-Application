/**
 * @author [Brendon Sutaj]
 * @email [s9brendon.sutaj@gmail.com]
 * @create date 2019-04-01 12:00:00
 * @modify date 2019-08-21 08:23:42
 * @desc [description]
 */

#region USINGS
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
#endregion

public class IImageHolder : MonoBehaviour
{
    [SerializeField] public string SciGraph;
    [SerializeField] public GameObject Button, Image;
    private bool contentCreated = false;

    public static List<string> ImageExtensions = new List<string> { ".jpg", ".jpe", ".bmp", ".gif", ".png" };

    /**
    * Used to load the sciGraph into the Image-Object.
    */
    void Start()
    {
        if (!contentCreated) {
            createContent();
        }
    }

    public void createContent() {
        if (contentCreated) {
            return;
        }

        if (Config.URLUSED)
        {
            createContentFromURL();
            return;
        }

        // Load the sciGraph as a texture onto the Image-Object from the StreamingAssets folder of the device.
        var path            = @Path.Combine(Application.streamingAssetsPath, SciGraph.Trim());

        // If the path does not exist or the extension is unknown hide the SciGraph button and return.
        if (!ImageExtensions.Contains(Path.GetExtension(path))) {
            Button.SetActive(false);
            return;
        }

        var byteArray       = File.ReadAllBytes(path);
        var sciGraph        = new Texture2D(2, 2);
        // LoadImage resets the (2, 2) Texture size to the actual size.
        sciGraph.LoadImage(byteArray);

        // Remove transparancy.
        var pixels          = sciGraph.GetPixels();
        for (int i = 0; i < pixels.Length; i++) {
            pixels[i] = pixels[i].Equals(Color.clear) ? Color.white : pixels[i];
        }
        sciGraph.SetPixels(pixels);
        sciGraph.Apply();

        Image.GetComponent<RawImage>().texture = sciGraph; 
    
        contentCreated = true;
    }


    private IEnumerator createContentFromURL()
    {
        // Scigraph is the URL now.
        var www = new WWW(SciGraph.Trim());
        yield return www;
        var byteArray = www.bytes;

        // Rest is similar.

        var sciGraph        = new Texture2D(2, 2);
        // LoadImage resets the (2, 2) Texture size to the actual size.
        sciGraph.LoadImage(byteArray);

        // Remove transparancy.
        var pixels          = sciGraph.GetPixels();
        for (int i = 0; i < pixels.Length; i++) {
            pixels[i] = pixels[i].Equals(Color.clear) ? Color.white : pixels[i];
        }
        sciGraph.SetPixels(pixels);
        sciGraph.Apply();

        Image.GetComponent<RawImage>().texture = sciGraph; 
    
        contentCreated = true;


    }

}
