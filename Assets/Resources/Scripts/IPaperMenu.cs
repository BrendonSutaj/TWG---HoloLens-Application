/**
 * @author [Brendon Sutaj]
 * @email [s9brendon.sutaj@gmail.com]
 * @create date 2019-04-01 12:00:00
 * @modify date 2019-09-12 23:48:15
 * @desc [description]
 */

#region USINGS
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
#endregion


public class IPaperMenu : MonoBehaviour
{
    // Childobjects of the PaperMenu.
    [SerializeField] public GameObject Dropdown, ComputeButton, Title;

    private bool dropDownDataReady = false;
    Dictionary<string, string> xmlNameMappingOffline    = new Dictionary<string, string>();
    Dictionary<string, string> xmlNameMappingOnline     = new Dictionary<string, string>();

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
    * This function is used to get all .xml files from the StreamingAssets folder and from the URL. 
    * Further adding them as options to the TMP_Dropdown menu.
    */
    private void Prep()
    {
        // Get all .xml files from the streamingAssets directory. (Offline)
        var infoAssetsDir   = new DirectoryInfo(UnityEngine.Application.streamingAssetsPath);
        var fileInfos       = infoAssetsDir.GetFiles("*.xml", SearchOption.AllDirectories);

        var optionData      = new List<TMP_Dropdown.OptionData>();
        var maxChars        = Config.MAX_DISPLAYED_CHARS;

        
        foreach (var fileInfo in fileInfos) {
            var name = fileInfo.Name.Trim().Replace(".xml", "");
            name = name.Length > maxChars - 1 ? name.Substring(0, maxChars) : name;
            xmlNameMappingOffline.Add(name + "_off", fileInfo.Name.Trim());
            optionData.Add(new TMP_Dropdown.OptionData(name + "_off"));
        }

        // Now everything online.
        StartCoroutine(GetRegisterContent(optionData));
    }

    private IEnumerator GetRegisterContent(List<TMP_Dropdown.OptionData> optionData) {
        using(UnityWebRequest www = UnityWebRequest.Get(Config.URL + Config.REGNAME)) {
            yield return www.SendWebRequest();
            
            var paperNames      = www.downloadHandler.text.Trim().Split(',');

            foreach (var paper in paperNames) 
            {
                if (string.IsNullOrEmpty(paper)) {
                    continue;
                }
                var name = paper.Trim().Replace(".xml", "");
                name = name.Length > 15 ? name.Substring(0, Config.MAX_DISPLAYED_CHARS) : name;
                xmlNameMappingOnline.Add(name, paper.Trim());
                optionData.Add(new TMP_Dropdown.OptionData(name));
            }
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
        var selection = Dropdown.GetComponent<TMP_Dropdown>().captionText.text;
        if (xmlNameMappingOffline.ContainsKey(selection)) {
            Config.URLUSED = false;
            pov.GetComponent<IPointOfView>().Path = xmlNameMappingOffline[selection];
        } else {
            Config.URLUSED = true;
            Config.XMLNAME = xmlNameMappingOnline[selection];
            pov.GetComponent<IPointOfView>().Path = xmlNameMappingOnline[selection];
        }

        gameObject.SetActive(false);
    }
}