/**
 * @author [Brendon Sutaj]
 * @email [s9brendon.sutaj@gmail.com]
 * @create date 2019-04-01 12:00:00
 * @modify date 2019-09-12 23:52:50
 * @desc [description]
 */

#region USINGS
using System.Collections.Generic;
using UnityEngine;
using System;
using static Config;
using TMPro;
#endregion

public class IGroupNode : MonoBehaviour
{

    // Fields.
    [SerializeField] public Group Group;
    [SerializeField] public GameObject pov;

    // Private Variables.
    public bool TriggeredOnce               = false;
    private List<GameObject> childObjects   = new List<GameObject>();
    private float DISTANCE;


    void Start()
    {
        // Load global value from Config.
        DISTANCE = Config.GROUP_PAPER_DISTANCE;
    }


    /**
    * Triggered by Hololens-User walking onto it.
    * 
    * Spawns Node Objects in a 315° equidistant manner around the Group and assigns them their "Paper" - Data.
    */
    private void OnTriggerEnter(Collider other) {

        // This function should only be run once, can only be triggered by camera.
        if (TriggeredOnce || !other.CompareTag("MainCamera")) 
        {
            return;
        }
        TriggeredOnce = true;

        // Play Sound, whenever the group is entered and Nodes are spawned.
        gameObject.GetComponent<AudioSource>().Play();

        // Reset POV, ImageHolder and InfoPanel in case they are opened atm.
        var povController = pov.GetComponent<IPointOfView>();
        if (povController.ImageHolder.activeSelf) {
            povController.ImageHolderHandler();
        }

        if (povController.InfoPanel.activeSelf) {
            povController.PaperInfoHandler();
        }


        // Deactivate other GrpChilds in case they are active and reset the trigger for the Group.
        // The MainCamera script has the reference to the active group childs and the active group.
        var childs  = IMainCamera.activatedGroupChilds;
        var grp     = IMainCamera.activeGroup;

        // grp != null, means that there is an active group, so the group and all its childobjects need to be destroyed.
        if (grp != null)
        {
            foreach (var child in childs)
            {
                Destroy(child);
            }

            grp.GetComponent<IGroupNode>().TriggeredOnce = false;
        }

        // Now set this group, as the new active group object.
        IMainCamera.activeGroup = this.gameObject;

        // groupPosition and grpRotation.
        var paperCount      = Group.Paper.Count;
        var grpRotation     = Convert.ToSingle(transform.eulerAngles.y * Math.PI / 180);
        var groupPosition   = transform.position;
        var Scale           = transform.localScale.x / 2;

        // HARDCODED FOR BETTER VISUALS.
        // To make it visually more pleasant, for paperCount < 5 positions are determined hardcoded.
        if (paperCount < 5)
        {
            hardcodedPositions();
            return;
        }

        for (int i = 0; i < paperCount; i++)
        {
            // // Calculations just like in POV before.
            var phi = (Math.PI * 13 / 8) + (7 * Math.PI / (4 * (paperCount - 1))) * i;

            /* Calculations, phi = Angle to the new Paper-Node.
             * Remember -- grpRotation is the World Rotation of the grp - 90° 
               (That the grp has from the beginning of its instantiation)
             * Such that phi represents the angle in which direction the Paper-Node has to be spawned.
            */
            phi += grpRotation;

            var cosinus     = Convert.ToSingle(Math.Cos(phi));
            var sinus       = Convert.ToSingle(Math.Sin(phi));

            /* New Paper position and values lineFrom and lineTo for the Line.
            *  Startingpoint is different to the Startingpoint of the POV. POV Startingpoint was (0, 0)
            *  Here we need the groupPosition as our Startingpoint.
            */
            var paperPosition    = new Vector3(cosinus * DISTANCE + groupPosition.x, transform.localPosition.y, sinus * DISTANCE + groupPosition.z);
            var lineFrom         = new Vector3(cosinus * Scale + groupPosition.x,transform.localPosition.y, sinus * Scale + groupPosition.z);
            var lineTo           = new Vector3(cosinus * (DISTANCE - Scale) + groupPosition.x, transform.localPosition.y, sinus * (DISTANCE - Scale) + groupPosition.z);

            // (360° - rotation) because we want positiv angles. Its like calculating modulo 360.
            // C# just like Java does -4 mod 3 = -1, so + 360° is the way to go.
            spawnNode(Group.Paper[i], paperPosition, 360 - (Convert.ToSingle(phi * 180 / Math.PI) - 90));
            spawnLine(Group.Paper[i].Title, lineFrom, lineTo);
        }

        // Set the new active group childs in the maincamera script.
        if (IMainCamera.activatedGroupChilds != null)
        {
            IMainCamera.activatedGroupChilds.Clear();
        }
        IMainCamera.activatedGroupChilds = this.childObjects;
    }

    /**
    * This function is used to determine the positions of the paperNodes; 
    * HARDCODED!!!
    */
    private void hardcodedPositions()
    {
        // groupPosition and grpRotation.
        var paperCount      = Group.Paper.Count;
        var grpRotation     = Convert.ToSingle(transform.eulerAngles.y * Math.PI / 180);
        var groupPosition   = transform.position;
        var Scale           = transform.localScale.x / 2;
        var eulerAngles     = new List<Double>();
        Vector3 paperPosition, lineFrom, lineTo;

        // Hardcoded positions computed.
        switch(paperCount)
        {
            case 1:
            eulerAngles.Add(Math.PI / 2 + grpRotation);
            break;

            case 2:
            eulerAngles.Add(Math.PI / 4 + grpRotation);
            eulerAngles.Add(Math.PI * 3 / 4 + grpRotation);
            break;

            case 3:
            eulerAngles.Add(grpRotation);
            eulerAngles.Add(Math.PI / 2 + grpRotation);
            eulerAngles.Add(Math.PI + grpRotation);
            break;

            case 4:
            eulerAngles.Add(Math.PI / 4 + grpRotation);
            eulerAngles.Add(Math.PI * 3 / 4 + grpRotation);
            eulerAngles.Add(Math.PI * 5 / 4 + grpRotation);
            eulerAngles.Add(Math.PI * 7 / 4 + grpRotation);
            break;

            default: break;
        }

        // Set the PaperNodes, Text and Line now.
        for (int i = 0; i < paperCount; i++)
        {   var height      = transform.localPosition.y;
            var cosinus     = Convert.ToSingle(Math.Cos(eulerAngles[i]));
            var sinus       = Convert.ToSingle(Math.Sin(eulerAngles[i]));
            paperPosition   = new Vector3(cosinus * DISTANCE + groupPosition.x, height, sinus * DISTANCE + groupPosition.z);
            lineFrom        = new Vector3(cosinus * Scale + groupPosition.x, height, sinus * Scale + groupPosition.z);
            lineTo          = new Vector3(cosinus * (DISTANCE - Scale) + groupPosition.x, height, sinus * (DISTANCE - Scale) + groupPosition.z);
            spawnNode(Group.Paper[i], paperPosition, 360 - (Convert.ToSingle(eulerAngles[i] * 180 / Math.PI) - 90));
            spawnLine(Group.Paper[i].Title, lineFrom, lineTo);
        }

        // Set the active group childs in the maincamera script.
        if (IMainCamera.activatedGroupChilds != null)
        {
            IMainCamera.activatedGroupChilds.Clear();
        }
        IMainCamera.activatedGroupChilds = this.childObjects;
    }


    /**
    * Spawns a Node Object at the given position and rotation.
    */
    private void spawnNode(Paper node, Vector3 position, float rotation)
    {
        // Spawn a new Paper-Node, set the position and the new angle.
        var nodeObj         = Instantiate((GameObject) Resources.Load("Prefabs/PaperNode", typeof(GameObject)));
        var nodeController  = nodeObj.GetComponentInChildren<IPaperNode>();

        nodeObj.transform.position      = position;
        nodeObj.transform.eulerAngles   = new Vector3(nodeObj.transform.eulerAngles.x, rotation, nodeObj.transform.eulerAngles.z);

        // Pass the Node data to the node.
        nodeController.Paper = node;

        // Save the instantiated object as a childObject.
        childObjects.Add(nodeObj);
    }


    /**
    * Spawns a Line Object, from - to.
    */
    private void spawnLine(String paperTitle, Vector3 from, Vector3 to)
    {
        // Spawn a new Line at the given from and to positions.
        var lineObj             = Instantiate((GameObject) Resources.Load("Prefabs/Line", typeof(GameObject)));
        var lineRenderer        = lineObj.GetComponent<LineRenderer>();

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
        spawnTextOnLine(rendererTop, paperTitle);

        // Save the instantiated object as a childObject.
        childObjects.Add(lineObj);
        childObjects.Add(lineTop);
    }


    

    /**
    * Spawns text on the position half of the given lineRenderer.
    */
    private void spawnTextOnLine(LineRenderer lineRenderer, string text)
    {
        var lineStart   = lineRenderer.GetPosition(0);
        var lineEnd     = lineRenderer.GetPosition(1);
        var lineTextobj = Instantiate((GameObject) Resources.Load("Prefabs/LineText", typeof(GameObject)));

        lineTextobj.transform.position = new Vector3((lineStart.x + lineEnd.x) / 2, transform.localPosition.y + 0.05f, (lineStart.z + lineEnd.z) / 2);
        lineTextobj.transform.Find("Text").GetComponent<TextMeshPro>().text = text;

        // Save the instantiated object as a childObject.
        childObjects.Add(lineTextobj);
    }
}
