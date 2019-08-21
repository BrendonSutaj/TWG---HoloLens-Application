/**
 * @author [Brendon Sutaj]
 * @email [s9brendon.sutaj@gmail.com]
 * @create date 2019-04-01 12:00:00
 * @modify date 2019-07-31 10:13:08
 * @desc [description]
 */

#region USINGS
using TMPro;
using UnityEngine;
using static Config;
#endregion

public class IInfoPanel : MonoBehaviour
{
    [SerializeField] public Paper Paper;
    [SerializeField] public GameObject Title, Description, PreviousPage, NextPage;


    /**
    * Used to hide/show the previous/next buttons.
    */
    void Update()
    {
        var meshPro = Description.GetComponent<TextMeshPro>();

        // If we are at Page < PageCount, we can go a step further so display the NextPage-Button.
        NextPage.SetActive(meshPro.pageToDisplay < meshPro.textInfo.pageCount);

        // If we are at Page > 1, we can go a step back so display the PreviousPage-Button.
        PreviousPage.SetActive(meshPro.pageToDisplay > 1);
    }

    /**
    * Creates the Content of the "Paper"-Data and puts it onto the InfoPanel.
    */
    public void createContent()
    {
        // Get all the Infos we need to display.
        var title       = Paper.Title;
        var abs         = Paper.Abstract;
        var authors     = Paper.Authors;
        var year        = Paper.Year;
        var typology    = Paper.Typology;
        var keyWords    = Paper.Keywords;

        // If any of these essential infos is missing, deactivate the PaperInfo Button.
        if (title == null || abs == null || authors == null || year == null)
        {
            transform.parent.transform.Find("Menu/PaperInfo").gameObject.SetActive(false);
            return;
        }

        // Set the Title.
        Title.GetComponent<TextMeshPro>().text = title.Trim();

        // Set the Description.
        var textToDisplay = "";
        textToDisplay += string.Format("<u><b><color=#2949B0>Authors:</color></b></u>\n{0}\n", authors.Trim());     // Authors
        textToDisplay += (!string.IsNullOrEmpty(keyWords))  ? string.Format("<u><b><color=#2949B0>Keywords:</color></b></u>\n{0}\n", keyWords.Trim())   // Keywords?
                                                            : "";
        textToDisplay += "<pos=0%><u><b><color=#2949B0>Published:</color></b></u></pos> ";
        textToDisplay += (!string.IsNullOrEmpty(typology))  ? "<pos=50%><u><b><color=#2949B0>Typology:</color></b></u></pos>\n" // Typology?
                                                            : "\n";
        textToDisplay += string.Format("<pos=0%>{0}</pos>", year.Trim());   // Year
        textToDisplay += (!string.IsNullOrEmpty(typology))  ? string.Format("<pos=50%>{0}</pos>\n", typology.Trim())
                                                            : "\n";
        textToDisplay += string.Format("<u><b><color=#2949B0>Abstract:</color></b></u>\n{0}", abs.Trim());  // Abstract


        var meshPro = Description.GetComponent<TextMeshPro>();
        meshPro.text = textToDisplay;

        // This is required to update the textInfos, TextMeshPro doesn't 
        // do that immediately thats why i need to force the update like this.
        gameObject.SetActive(true);
        meshPro.ForceMeshUpdate();
        gameObject.SetActive(false);
    }

    /**
    * Used as NextPage-Button Eventhandler.
    */
    public void nextPageHandler()
    {
        var meshPro = Description.GetComponent<TextMeshPro>();
        meshPro.pageToDisplay++;
        meshPro.ForceMeshUpdate();
    }

    /**
    * Used as PreviousPage-Button Eventhandler.
    */
    public void previousPageHandler()
    {
        var meshPro = Description.GetComponent<TextMeshPro>();
        meshPro.pageToDisplay--;
        meshPro.ForceMeshUpdate();
    }

    /**
    * Used to reset the InfoPanel to the first page again.
    */
    public void reset() {
        var meshPro = Description.GetComponent<TextMeshPro>();
        meshPro.pageToDisplay = 1;
    }
}
