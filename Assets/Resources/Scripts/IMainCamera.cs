﻿/**
 * @author [Brendon Sutaj]
 * @email [s9brendon.sutaj@gmail.com]
 * @create date 2019-04-01 12:00:00
 * @modify date 2019-08-27 18:18:05
 * @desc [description]
 */

// THIS CLASS IS NOT USED ANYMORE, SINCE NEWORIGIN WAS DISREGARDED.
// THE ONLY USE IS TO TRACK WHAT OBJECTS ARE ACTIVE ATM.
// DELETING THIS SCRIPT NEEDS REPLACEMENT OF ACTIVEGROUP AND CHILDS. --> Config maybe?
using System.Collections.Generic;
using UnityEngine;

public class IMainCamera : MonoBehaviour
{
    [SerializeField] public GameObject Prev, Next;

    public static List<GameObject> activatedGroupChilds;
    public static GameObject activeGroup;

    // The saved Levels.
    private List<List<GameObject>> savedLevels = new List<List<GameObject>>();
    private int level = 0;



    /**
    * This function is used to add a new Level to the savedLevels list, deactivates the previous GameObjects.
    */
    public void addNewLevel(List<GameObject> newLevel)
    {

        savedLevels.Add(newLevel);
        
        foreach (GameObject obj in savedLevels[level])
        {
            obj.SetActive(false);
        }

        level++;
    }


    /**
    * This function is used to load the previous level.
    * Level 0 don't do anything.
    */
    public void loadPreviousLevel()
    {
        // If level == 0, there is no previousLevel.
        if (level == 0)
        {
            return;
        }

        // Deactive all current objects.
        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            if (obj.CompareTag("Deactivate"))
            {
                obj.SetActive(false);
            }
        }
        
        // Load the objects of the previous level.
        foreach (GameObject obj in savedLevels[level - 1])
        {
            obj.SetActive(true);
        }

        level--;
    }


    /**
    * This function is used to load the next level.
    */
    public void loadNextLevel()
    {
        // Deactivate the current objects.
        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            if (obj.CompareTag("Deactivate"))
            {
                obj.SetActive(false);
            }
        }

        // Load all objects of the nextLevel.
        foreach (GameObject obj in savedLevels[level + 1])
        {
            obj.SetActive(true);
        }

        level++;
    }


    /**
    * This function is used to save the current level.
    */
    public void saveLevel()
    {

        List<GameObject> levelObjects = new List<GameObject>();

        // Saves all current sceneObjects.
        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            if (obj.CompareTag("Deactivate"))
            {
                levelObjects.Add(obj);
            }
        }

        savedLevels.Add(levelObjects);
    }
}
