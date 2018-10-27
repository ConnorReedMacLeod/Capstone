using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used for placing characters in the proper slots
/// </summary>

[RequireComponent (typeof(ViewArena))]
public class Arena : MonoBehaviour{

	bool bStarted;

	public float fArenaWidth;
	public float fArenaHeight;

	public int nWidth;
	public int nHeight;
	public float fUnit;

    public ViewChrSlot[,] arChrPositions = new ViewChrSlot[2, 7];


    public ViewArena view;

	public static Arena instance;

	public static Arena Get (){
		if (instance == null) {
			GameObject go = GameObject.FindGameObjectWithTag ("Arena");
			if (go == null) {
				Debug.LogError ("ERROR! NO OBJECT HAS A ARENA TAG!");
			}
			instance = go.GetComponent<Arena> ();
			if (instance == null) {
				Debug.LogError ("ERROR! ARENA TAGGED OBJECT DOES NOT HAVE A ARENA COMPONENT!");
			}
		}
		return instance;
	}

	public void InitPlaceUnit(bool bLocal, ViewChr chr){
        Vector3 v3DesiredPos;

        if(bLocal) {
            //Then this is a character owned by the local player, so position him on the bottom
            v3DesiredPos = arChrPositions[0, chr.mod.id].getPos();
        } else {
            //Then this is an opponent's character, so put them on the top
            v3DesiredPos = arChrPositions[1, chr.mod.id].getPos();
        }

        chr.transform.position = v3DesiredPos;
	}

    // Find and store each starting location stored in the prefab
	public void InitChrSlots(){

        foreach (ViewChrSlot slot in GetComponentsInChildren<ViewChrSlot>()){
    
            arChrPositions[slot.nTeam, slot.nSlot] = slot;

        }

	}

	public void Start(){

		if (bStarted == false) {
			bStarted = true;

            InitChrSlots();

			view = GetComponent<ViewArena> ();
		}
	}


}
