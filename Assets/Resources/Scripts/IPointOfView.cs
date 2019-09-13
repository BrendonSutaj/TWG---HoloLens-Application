/**
 * @author [Brendon Sutaj]
 * @email [s9brendon.sutaj@gmail.com]
 * @create date 2019-04-01 12:00:00
 * @modify date 2019-09-12 23:52:28
 * @desc [description]
 */

#region USINGS
using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using static Config;
using System.Collections;
using UnityEngine.Networking;
using System.Xml;
#endregion

public class IPointOfView : MonoBehaviour
{
    // Childobjects and Fields, for convinience.
    [SerializeField] public string Path;
    [SerializeField] public bool TriggeredOnce;
    [SerializeField] public GameObject ImageHolder, InfoPanel;

    // Private Variables.
    private WalkableGraph Graph;

    private float Distance;
    private bool deserializationIsDone = false;

    /**
    * Deserializes the .xml file ("Path") and stores the WalkableGraph in "Graph".
    */
    void Start()
    {
        
        DeserializeXml();

        if (Config.URLUSED) {
            return;
        }

        // Compute the right Distances from pov to groupd and from group to their respective nodes.
        ComputeDistances();

        // Load global value from Config.
        Distance = Config.POV_GROUP_DISTANCE;

        // Pass the PaperInfo/SciGraph data to the InfoPanel/ImageHolder.
        InfoPanel.GetComponent<IInfoPanel>().Paper          = Graph.PaperInfo.Paper;

        // If the sciGraph file does not exist, or is even null, hide the button.
        if (string.IsNullOrEmpty(Graph.PaperInfo.Paper.SciGraph) || !File.Exists(GetFilePath(Graph.PaperInfo.Paper.SciGraph.Trim()))) {
            transform.Find("Menu/SciGraph").gameObject.SetActive(false);
        } else {
            ImageHolder.SetActive(true);
            ImageHolder.GetComponent<IImageHolder>().SciGraph = Graph.PaperInfo.Paper.SciGraph;
            ImageHolder.GetComponent<IImageHolder>().createContent();
            ImageHolder.SetActive(false);
        }

        InfoPanel.SetActive(true);
        InfoPanel.GetComponent<IInfoPanel>().createContent();
        InfoPanel.SetActive(false);

        deserializationIsDone = true;
    }

    void Update() {
        if (TriggeredOnce) {
            return;
        }

        if (deserializationIsDone && Graph != null) {
            TriggeredOnce = true;
            GenerateGraph();
        }
    }

    /**
    * This function is used to compute the distances from pov to group and from group to their respective nodes.
    */
    private void ComputeDistances()
    {
        var maxNodes = 0;
        foreach (var group in Graph.Groups.Group)
        {
            maxNodes = Math.Max(maxNodes, group.Paper.Count);
        }

        if (maxNodes == 1) {
            Config.GROUP_PAPER_DISTANCE = 1;
        } else {
            Config.GROUP_PAPER_DISTANCE = Convert.ToSingle(
                Math.Max(1, Math.Round(0.6 / Math.Sqrt(2 - 2 * Math.Cos(7 * Math.PI / (4 * (maxNodes - 1)))), 2) + 0.01)
            );
        }

        int m = Graph.Groups.Group.Count;

        if (m == 1 || m == 2) {
            Config.POV_GROUP_DISTANCE = 1;
        }

        if (m == 3) {
            Config.POV_GROUP_DISTANCE = Convert.ToSingle(
                (Config.GROUP_PAPER_DISTANCE + 0.6) * Math.Sin(37.5 * Math.PI / 180) / Math.Sin(2 * Math.PI / 3)
            );
        }

        if (m == 4) {
            Config.POV_GROUP_DISTANCE = Convert.ToSingle(
                (Config.GROUP_PAPER_DISTANCE + 0.6) * Math.Cos(Math.PI / 8)
            );
        }

        if (m > 4) {
            Config.POV_GROUP_DISTANCE = Convert.ToSingle(
                Math.Max(1, Math.Round((0.6 + Config.GROUP_PAPER_DISTANCE) / Math.Sin(2 * Math.PI / m), 2) + 0.01)
            );
        }
    }

    /**
    * Deserializes the xml file ("Path") into the WalkableGraph Object "Graph".
    */
    private void DeserializeXml()
    {
        if (Config.URLUSED)
        {
            StartCoroutine(DeserializeXmlFromURL());
            return;
        }

        // Load the file from StreamingAssets as a String.
        var fileAsStr   = GetFileData(Path);

        // XML deserialize into the WalkableGraph object "graph".
        var memStream   = new MemoryStream(Encoding.UTF8.GetBytes(fileAsStr));
        var serializer  = new XmlSerializer(typeof(WalkableGraph));
        Graph = (WalkableGraph)serializer.Deserialize(memStream);
    }

    /**
    * Deserializes the xml file from the URL given in the Config into the WalkableGraph Object "Graph" 
    */
    private IEnumerator DeserializeXmlFromURL()
    {
        using(UnityWebRequest www = UnityWebRequest.Get(Config.URL + Config.XMLNAME)) {
            yield return www.SendWebRequest();

            // XML deserialize into the WalkableGraph object "graph".
            var memStream   = new MemoryStream(www.downloadHandler.data);
            var serializer  = new XmlSerializer(typeof(WalkableGraph));
            Graph = (WalkableGraph)serializer.Deserialize(memStream);


            // Compute the right Distances from pov to groupd and from group to their respective nodes.
            ComputeDistances();

            // Load global value from Config.
            Distance = Config.POV_GROUP_DISTANCE;

            // Pass the PaperInfo/SciGraph data to the InfoPanel/ImageHolder.
            InfoPanel.GetComponent<IInfoPanel>().Paper          = Graph.PaperInfo.Paper;


            InfoPanel.SetActive(true);
            InfoPanel.GetComponent<IInfoPanel>().createContent();
            InfoPanel.SetActive(false);

            // If the sciGraph file does not exist, or is even null, hide the button.
            if (string.IsNullOrEmpty(Graph.PaperInfo.Paper.SciGraph)) {
                transform.Find("Menu/SciGraph").gameObject.SetActive(false);
            } else {
                ImageHolder.SetActive(true);
                ImageHolder.GetComponent<IImageHolder>().SciGraph = Graph.PaperInfo.Paper.SciGraph;
                StartCoroutine(ImageHolder.GetComponent<IImageHolder>().createContentFromURL());
            }

            deserializationIsDone = true;
        }
    }

    /**
    * Triggered by Hololens-User walking onto the POV.
    * 
    * Spawns Group Objects in a 360° equidistant manner around the POV and assigns them their "Group" - Data.
    * (Euler Function)
    */
    private void GenerateGraph() 
    {


        var groupCount  = Graph.Groups.Group.Count;
        float Scale     = transform.localScale.x / 2;
        for (int i = 0; i < groupCount; i++)
        {

            // Calculations, phi = Angle to the new Group. -- exp(i*phi) = cos(phi) + i*sin(phi)
            // The Addition "+ Math.PI / 2" makes sure that a Group always spawns in front of the User.
            var phi     = (Math.PI / 2) + (2 * Math.PI / groupCount) * i;
            var sinus   = Convert.ToSingle(Math.Sin(phi));
            var cosinus = Convert.ToSingle(Math.Cos(phi));

            // New Group position and value to rotate the Group Object itself.
            var grpDestination = new Vector3(cosinus * Distance, transform.localPosition.y, sinus * Distance);

            // Due to the fact that all instantiated Objects face to the North (90°), we only need to rotate them (phi - 90°) further.
            // Note - This will not result in a negative angle because phi is alway bigger then 90°.
            var grpRotation = (int)(phi * 180 / Math.PI - 90);

            /* Line Positions lineFrom and lineTo.
             * The POV always starts at (x,z) = (0,0) and has a radius of Scale. 
             * In order to let the line start at the edge of the POV it has go from (x,z) = (cos * Scale, sin * Scale).
             * to the distant point following that direction.
            */
            var lineFrom    = new Vector3(cosinus * Scale, transform.localPosition.y, sinus * Scale);
            var lineTo      = new Vector3(cosinus * (Distance - Scale), transform.localPosition.y, sinus * (Distance - Scale));

            // Spawn the Group and Line.
            SpawnGroup(Graph.Groups.Group[i], grpDestination, grpRotation);
            SpawnLine(Graph.Groups.Group[i].GroupName, lineFrom, lineTo); 
        }
    }


    /**
    * Spawns Group Objects at the given position and rotation.
    */
    private void SpawnGroup(Group group, Vector3 position, float rotation)
    {
        // Spawn a new Group Node, set the position and the new angle.
        var groupObj        = Instantiate((GameObject) Resources.Load("Prefabs/GroupNode", typeof(GameObject)));
        var groupController = groupObj.GetComponent<IGroupNode>();

        groupObj.transform.position      = position;
        groupObj.transform.eulerAngles   = new Vector3(0.0f, rotation, 0.0f);

        // Pass the Group data to the group.
        groupController.Group = group;

        // Pass the POV object to the group too.
        groupController.pov = this.gameObject;
    }


    /**
    * Spawns a Line Object, from - to.
    */
    private void SpawnLine(String groupName, Vector3 from, Vector3 to)
    {
        // Spawn a new Line at the given from and to positions.
        var lineObj      = Instantiate((GameObject) Resources.Load("Prefabs/Line", typeof(GameObject)));
        var lineRenderer = lineObj.GetComponent<LineRenderer>();

        lineRenderer.SetPosition(0, from);
        lineRenderer.SetPosition(1, to);

        // Little Addition to make it look even cooler!
        var lineTop     = Instantiate((GameObject) Resources.Load("Prefabs/LineTop", typeof(GameObject)));
        var rendererTop = lineTop.GetComponent<LineRenderer>();

        var newFrom = new Vector3(from.x, from.y + 0.005f, from.z);
        var newTo   = new Vector3(to.x, to.y + 0.005f, to.z);

        rendererTop.SetPosition(0, newFrom);
        rendererTop.SetPosition(1, newTo);


        // Spawn the Text onto the line at the half distance of the line.
        SpawnTextOnLine(rendererTop, groupName);
    }


    /**
    * Spawns text on the position half of the given lineRenderer.
    */
    private void SpawnTextOnLine(LineRenderer lineRenderer, string text)
    {
        var lineTextobj = Instantiate((GameObject) Resources.Load("Prefabs/LineText", typeof(GameObject)));
        var lineStart   = lineRenderer.GetPosition(0);
        var lineEnd     = lineRenderer.GetPosition(1);

        lineTextobj.transform.position = new Vector3((lineStart.x + lineEnd.x) / 2, transform.localPosition.y + 0.05f, (lineStart.z + lineEnd.z) / 2);
        lineTextobj.transform.Find("Text").GetComponent<TextMeshPro>().text = text;
    }

    /**
    * This function is used to get the FileData returned as a string.
    */
    private string GetFileData(string fileName)
    {
        var filePath = GetFilePath(fileName);

        // Open the file in readonly to avoid access exceptions.
        var fileStream  = File.OpenRead(filePath);
        var bytes       = ReadStream(fileStream);
        
        return Encoding.UTF8.GetString(bytes);
    }

    /**
    * This function is used to get the filePath from the StreamingAssets Folder on the Device.
    */
    private string GetFilePath(string fileName)
    {
        return System.IO.Path.Combine(Application.streamingAssetsPath, fileName);
    }

    /**
    * This function is used to get all bytes from the given stream.
    */
    private byte[] ReadStream(Stream fileStream)
    {
        using (MemoryStream memStream = new MemoryStream())
        {
            fileStream.CopyTo(memStream);
            var result = memStream.ToArray();
            return result;
        }
    }

    /**
    * PaperInfo Button EventHandler.
    * Activates/Deactivates InfoPanel.
    */
    public void PaperInfoHandler()
    {   // Resets the InfoPanel to page 1, if it is being currently deactivated.
        if (InfoPanel.activeSelf) {
            InfoPanel.GetComponent<IInfoPanel>().reset();
        }
        InfoPanel.SetActive(!InfoPanel.activeSelf);
    }

    /**
    * SciGraph Button EventHandler.
    * Activates/Deactivates ImageHolder.
    */
    public void ImageHolderHandler()
    {
        ImageHolder.SetActive(!ImageHolder.activeSelf);
    }
}