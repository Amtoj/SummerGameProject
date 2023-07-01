using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using System.Linq;
using System.ComponentModel;

public class Portal : Interactable
{
    public string SaveDataPath
    {
        // will return the file directory of the protal save data text file
        get { return Application.dataPath + "/Save Data/Portal Data.txt"; }
    }

    public string PortalID
    {
        // will return the portal ID
        get { return worldType.ToString() + "_" + SceneManager.GetActiveScene().name + "_" + gameObject.name; }
    }
    
    public WorldType worldType;

    public void InitPortal()
    {
        UnlockPortal();
        // open Portal UI
    }

    public void UnlockPortal()
    {
        // will create a list from the portal data text file
        List<string> portals = File.ReadAllLines(SaveDataPath).ToList();

        if (!portals.Contains(PortalID))
        {
            // if the portal ID is not in the list, add it to the list and write the new list to the portal data text file
            portals.Add(PortalID);
            File.WriteAllLines(SaveDataPath, portals.ToArray());
            Debug.Log("Portal has been unlocked!");
        }
        else
        {
            Debug.Log("Portal is already unlocked!");
        }
    }

    public void LockPortal()
    {
        // will create a list from the portal data text file
        List<string> portals = File.ReadAllLines(SaveDataPath).ToList();

        if (portals.Contains(PortalID))
        {
            // if the portal ID is in the list, remove it from the list and write the new list to the portal data text file
            portals.Remove(PortalID);
            File.WriteAllLines(SaveDataPath, portals.ToArray());
            Debug.Log("Portal has been locked!");
        }
        else
        {
            Debug.Log("Portal is already locked!");
        }
    }

    public void ClearPortalData()
    {
        // will clear the portal data text file
        File.WriteAllLines(SaveDataPath, new string[0]);
        Debug.Log("Portal has been Cleared!");
    }

    public void ActivatePortal(string portalID)
    {
        // will read the selected portal ID adn identify the scene name and portal name
        string sceneName = portalID.Split('_')[1];
        string portalName = portalID.Split('_')[2];

        // will inform upon scene loading to spawn the player at a portal
        PlayerPrefs.SetInt("isPortalUsed", 1);
        PlayerPrefs.SetString("currentPortal", portalName);

        SceneManager.LoadScene(sceneName);
    }
}

public enum WorldType
{
    Earth,
    Water,
    Fire,
    Air,
    Debug
}