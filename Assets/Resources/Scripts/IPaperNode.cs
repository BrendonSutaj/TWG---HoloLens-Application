/**
 * @author [Brendon Sutaj]
 * @email [s9brendon.sutaj@gmail.com]
 * @create date 2019-04-01 12:00:00
 * @modify date 2019-08-27 18:17:55
 * @desc [description]
 */

#region USINGS
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using static Config;
#endregion

public class IPaperNode : MonoBehaviour
{
    // Fields.
    [SerializeField] public Paper Paper;
    [SerializeField] public GameObject InfoPanel, ImageHolder;

    // Private Variables.
    private bool TriggeredOnce = false;


    /**
    * Pass the Paper-Data to the InfoPanel and ImageHolder.
    * Hide the NewOrigin Button if there is no newOrigin-Data.
    */
    private void Start() {
        
        InfoPanel.GetComponent<IInfoPanel>().Paper = Paper;

        // If the sciGraph file does not exist, or is even null, hide the button.
        if (string.IsNullOrEmpty(Paper.SciGraph) || !File.Exists(GetFilePath(Paper.SciGraph.Trim()))) {
            transform.Find("Menu/SciGraph").gameObject.SetActive(false);
        } else {
            ImageHolder.GetComponent<IImageHolder>().SciGraph = Paper.SciGraph;
        }
        
        // CHANGED!!!
        // NEWORIGIN WAS REMOVED BECAUSE OF NEW XML SCHEME.
        transform.Find("Menu/NewOrigin").gameObject.SetActive(false);
        /*
        if (string.IsNullOrEmpty(Paper.NewOrigin) || !File.Exists(GetFilePath(Paper.SciGraph.Trim())))
        {
            transform.Find("Menu/NewOrigin").gameObject.SetActive(false);
        }
        */

        InfoPanel.GetComponent<IInfoPanel>().createContent();
        ImageHolder.GetComponent<IImageHolder>().createContent();
    }


    /**
    * Triggered by Hololens-User walking onto it.
    * 
    * Deactivates all InfoPanel and ImageHolder of other Paper-Nodes.
    */
    private void OnTriggerEnter(Collider other) {

        // Should trigger just once.
        if (TriggeredOnce || !other.CompareTag("MainCamera")) 
        {
            return;
        }
        TriggeredOnce = true;

        // Get all Objects currently on the scene.
        var objects = FindObjectsOfType<GameObject>();
        foreach (var obj in objects)
        {
            if (obj.ToString().Contains("PaperNode"))
            {
                var nodeController = obj.GetComponent<IPaperNode>();
                // Skip this Paper-Node.
                if (nodeController == null || nodeController.Paper == null || nodeController.Paper.PaperName == Paper.PaperName)
                {
                    continue;
                }

                // Set InfoPanel and ImageHolder inactive.
                obj.transform.Find("InfoPanel").gameObject.SetActive(false);
                obj.transform.Find("ImageHolder").gameObject.SetActive(false);
            }

        }
    }

    /**
    * Let PaperNodes be triggered multiple times by setting "triggeredOnce" to false onTriggerExit.
    */
    private void OnTriggerExit(Collider other) {
        TriggeredOnce = false;
    }

    /**
    * SciGraph Button Eventhandler.
    * Activates/Deactivates ImageHolder.
    */
    public void ImageHolderHandler()
    {
        ImageHolder.SetActive(!ImageHolder.activeSelf);
    }

    /**
    * PaperInfo Button Eventhandler.
    * Activates/Deactivates InfoPanel.
    */
    public void paperInfoHandler()
    {
        // Resets the InfoPanel to the first page, if it is currently being deactivated.
        if (InfoPanel.activeSelf) {
            InfoPanel.GetComponent<IInfoPanel>().reset();
        }
        InfoPanel.SetActive(!InfoPanel.activeSelf);
    }


    /**
    * NewOrigin Button Eventhandler.
    * NOT USED ANYMORE, BECAUSE NEWORIGIN WAS REMOVED FROM THE XML SCHEME.
    */
    public void newOriginHandler()
    {
        // CHANGED!!!
        // Paper.NewOrigin.Trim();
        string newOrigin = null;

        // Get all the objects with tag deactivate, to save the level.
        List<GameObject> level = new List<GameObject>();
        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            if (obj.CompareTag("Deactivate"))
            {
                level.Add(obj);
            }
        }
        
        // Add the list as newLevel.
        Camera.main.GetComponent<IMainCamera>().addNewLevel(level);

        // Instantiate a POV with the path newOrigin of the new xml file.
        var pov = Instantiate((GameObject) Resources.Load("Prefabs/PointOfView", typeof(GameObject)));
        pov.transform.position = new Vector3(0, transform.localPosition.y, 0);
        pov.GetComponent<IPointOfView>().Path = newOrigin.Trim();
    }

    /**
    * This function is used to get the filePath from the StreamingAssets Folder on the Device.
    */
    private string GetFilePath(string fileName)
    {
        return System.IO.Path.Combine(Application.streamingAssetsPath, fileName);
    }
}
