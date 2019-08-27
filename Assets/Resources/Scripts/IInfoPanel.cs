/**
 * @author [Brendon Sutaj]
 * @email [s9brendon.sutaj@gmail.com]
 * @create date 2019-04-01 12:00:00
 * @modify date 2019-08-27 18:14:20
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

    private string headerColor = "#2949B0";


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

        var keyAbsent = string.IsNullOrEmpty(keyWords);
        var typAbsent = string.IsNullOrEmpty(typology);

        // Set the Title.
        Title.GetComponent<TextMeshPro>().text = title.Trim();

        // Set the Description.
        var displayedText   = string.Format("{0}\n{1}\n", header("Authors:"), authors.Trim()); // Authors
            displayedText  += !keyAbsent ? string.Format("{0}\n{1}\n", header("Keywords:"), keyWords.Trim()) : ""; // Keywords
            displayedText  += "<pos=0%>" + header("Published:") + "</pos>"; // Year-Header
            displayedText  += !typAbsent ? "<pos=50%>" + header("Typology:") + "</pos>\n" : "\n"; // Typology-Header
            displayedText  += string.Format("<pos=0%>{0}</pos>", year.Trim());   // Year
            displayedText  += !typAbsent ? string.Format("<pos=50%>{0}</pos>\n", typology.Trim()) : "\n"; // Typology
            displayedText  += string.Format("{0}\n{1}", header("Abstract:"), abs.Trim()); // Abstract

        var meshPro = Description.GetComponent<TextMeshPro>();
        meshPro.text = displayedText;

        // This is required to update the textInfos, TextMeshPro doesn't 
        // do that immediately thats why i need to force the update like this.
        gameObject.SetActive(true);
        meshPro.ForceMeshUpdate();
        gameObject.SetActive(false);
    }

    /**
    * Helper function to make the string assignments more readable.
    */
    private string colorize(string text, string color)
    {
        return string.Format("<color={0}>{1}</color>", color, text);
    }

    /**
    * Helper function to make the string assignments more readable.
    */
    private string bold(string text)
    {
        return string.Format("<b>{0}</b>", text);
    }

    /**
    * Helper function to make the string assignments more readable.
    */
    private string underline(string text)
    {
        return string.Format("<u>{0}</u>", text);
    }

    /**
    * Helper function to make the string assignments more readable.
    */
    private string header(string text)
    {
        return underline(bold(colorize(text, headerColor)));
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
