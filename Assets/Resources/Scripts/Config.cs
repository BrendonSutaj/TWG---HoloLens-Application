/**
 * @author [Brendon Sutaj]
 * @email [s9brendon.sutaj@gmail.com]
 * @create date 2019-04-01 12:00:00
 * @modify date 2019-09-12 22:49:25
 * @desc [description]
 */


#region USINGS
using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.SpatialMapping;
#endregion

public class Config : MonoBehaviour
{
    /*
        The purpose of this class is to define global values for the graph.
        It's like a control point for the whole graph.
        Values changed here change the appearance of the graph.  
    */

    // The graph_height variable stores the inital height of the spatialprocessing script.
    private float floorYInitialValue;
    public static bool floorProcessingIsReady = false;
    public GameObject SpatialMapping, SpatialProcessing;

    // Used to get the inital graph_height value.
    private void Start() {
        floorYInitialValue = SpatialProcessing.GetComponent<SurfaceMeshesToPlanes>().FloorYPosition;
    }

    /**
    * When spatialProcessing is over, get the computed height and store it.
    * Notify that the height has been computed, and destroy the spatialMapping and spatialProcessing.
    */ 
    private void Update() {
        // No update needed, if spatialProcessing is already destroyed.
        if (SpatialProcessing == null)
        {
            return;
        }

        // UNITY EDITOR
        if (Time.realtimeSinceStartup > 13)
        {
            floorProcessingIsReady = true;
            return;
        }
        
        // Store the graphHeight, notify being ready and destroy spatialMapping and spatialProcessing.
        var updatedYPosition = SpatialProcessing.GetComponent<SurfaceMeshesToPlanes>().FloorYPosition;
        if (updatedYPosition != floorYInitialValue)
        {
            GRAPH_HEIGHT = updatedYPosition;
            floorProcessingIsReady = true;
            
            // Deactivate SM and SP, to avoid nullpointer exceptions.
            SpatialMapping.GetComponent<SMDeactivator>().deactivate();
            SpatialProcessing.GetComponent<SPDeactivator>().deactivate();
            StopAllCoroutines();
        }
    }


    // Dummy Values, don't really get used (Only in the Unity Editor.)
    #region GLOBAL VARIABLES
    public static float GRAPH_HEIGHT         = -1.5f;
    public static float POV_GROUP_DISTANCE   = 1.2f;
    public static float GROUP_PAPER_DISTANCE = 1.2f;

    public static int MAX_DISPLAYED_CHARS    = 12;

    // SET URLUSED = TRUE IF YOU WANT TO ACTUALLY USE THE WEB INTERFACE.
    public static Boolean URLUSED = true;
    public static String URL = @"http://brendon-sutaj.de/hololens/";

    public static String XMLNAME = "";

    public static String REGNAME = "Register.txt";
        
    #endregion


    /* XML / JSON Structure

        Root               Elements
        ------------------------------------------
        WalkableGraph   -> PaperInfo && Groups_Ref
        Groups_Ref      -> Group (List)
        PaperInfo       -> Paper
        Group           -> Paper (List) && name (Attribute)
        Paper           -> DOI && Abstract && Authors && Keywords? && Title && Typology? 
                               && Year && SciGraph? && NewOrigin && name (Attribute)
        ------------------------------------------

        ? := optional Data

        Encoding used: UTF8
    */
    #region SERIALIZATION

    [Serializable()]
    [XmlRoot("WalkableGraph")]
    public class WalkableGraph
    {
        [XmlElement("PaperInfo")]
        public PaperInfo PaperInfo  {get; set; }

        [XmlElement("Groups_Ref")]
        public Groups Groups        {get; set; }
    }


    [Serializable()]
    [XmlRoot("Groups_Ref")]
    public class Groups
    {
        [XmlElement("Group")]
        public List<Group> Group    {get; set; }
    }


    [Serializable()]
    [XmlRoot("Group")]
    public class Group
    {
        [XmlElement("Paper")]
        public List<Paper> Paper    {get; set; }

        [XmlAttribute("name")]
        public string GroupName     {get; set; }
    }


    [Serializable()]
    [XmlRoot("PaperInfo")]
    public class PaperInfo
    {
        [XmlElement("Paper")]
        public Paper Paper          {get; set; }
    }


    [Serializable()]
    [XmlRoot("Paper")]
    public class Paper
    {
        [XmlElement("DOI")]
        public String DOI           {get; set; }

        [XmlElement("Abstract")]
        public String Abstract      {get; set; }

        [XmlElement("Authors")]
        public String Authors       {get; set; }

        [XmlElement("Keywords")]
        public String Keywords      {get; set; }

        [XmlElement("Title")]
        public String Title         {get; set; }

        [XmlElement("Typology")]
        public String Typology      {get; set; }

        [XmlElement("Year")]
        public String Year          {get; set; }

        [XmlElement("SciGraph")]
        public String SciGraph      {get; set; }

        [XmlAttribute("name")]
        public String PaperName     {get; set; }
    }
    #endregion
}
