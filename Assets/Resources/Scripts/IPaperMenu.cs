/**
 * @author [Brendon Sutaj]
 * @email [s9brendon.sutaj@gmail.com]
 * @create date 2019-04-01 12:00:00
 * @modify date 2019-07-10 16:13:51
 * @desc [description]
 */

#region USINGS
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
#endregion

public class IPaperMenu : MonoBehaviour
{
    // Childobjects of the PaperMenu.
    [SerializeField] public GameObject Dropdown, ComputeButton, Title;

    private bool dropDownDataReady = false;
    private int maxCharsToDisplay = 16;

    Dictionary<string, string> xmlNames = new Dictionary<string, string>();

    // After height computation, prepare the dropdown list of XML files and change the title text.
    void Update() {
        if (Config.floorProcessingIsReady)
        {
            Prep();
            transform.Find("Title").GetComponent<TextMeshPro>().text = "Choose the Paper\nto be displayed";
            enabled = false;
        }
    }
    

    /* 
    * This function is used to get all .xml files from the StreamingAssets folder and adding them as
    * options to the TMP_Dropdown menu.
    */
    void Prep()
    {
        // Get all .xml files from the streamingAssets directory.
        var infoAssetsDir   = new DirectoryInfo(Application.streamingAssetsPath);
        var xmlFiles        = infoAssetsDir.GetFiles("*.xml", SearchOption.AllDirectories);
        var optionData      = new List<TMP_Dropdown.OptionData>();

        // If there are no xml files, display it to the user.
        if (xmlFiles.Length == 0)
        {
            ComputeButton.SetActive(false);
            Dropdown.SetActive(false);
            Title.GetComponent<TextMeshPro>().text = "There are no\nPapers in the\nStreamingAssets Folder.";
            return;
        }

        // Add all those files as TMP_Dropdown Options.
        foreach (var xmlFile in xmlFiles)
        {   // Remove the ".xml" part of the string and get min(15, string.length) of characters for the dropdown name.
            var name = xmlFile.Name.Trim().Replace(".xml", "");
            name = name.Length > 15 ? name.Substring(0, maxCharsToDisplay) : name;

            // Add the shortened name and the actual file name, to be able to load the right file later on.
            xmlNames.Add(name, xmlFile.Name.Trim());
            optionData.Add(new TMP_Dropdown.OptionData(name));
        }

        // Assign the options to the TMP_Dropdown.
        Dropdown.GetComponent<TMP_Dropdown>().options = optionData;

        dropDownDataReady = true;
    }


    /**
    * This function is used to compute the WalkableGraph, by instatiating the PointOfView-Object and passing 
    * the Data selected in the DropdownMenu.
    */
    public void ComputeButtonHandler()
    {
        // Wait until the SpatialProcessing is ready.
        if (!dropDownDataReady)
        {
            return;
        }

        // Instantiate the POV Object.
        var pov = Instantiate((GameObject) Resources.Load("Prefabs/PointOfView", typeof(GameObject)));
        pov.transform.position = new Vector3(0, Config.GRAPH_HEIGHT, 0);

        // Pass the right Path to the POV Object and deactivate the PaperMenu object.
        pov.GetComponent<IPointOfView>().Path = xmlNames[Dropdown.GetComponent<TMP_Dropdown>().captionText.text];
        gameObject.SetActive(false);
    }
}
