using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Can hold information about arena conditions like terrain or effects
/// </summary>

[RequireComponent (typeof(ViewArena))]
public class Arena : Subject{

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

	public void InitPlaceUnit(Chr chr){
        chr.transform.position = arChrPositions[chr.plyrOwner.id, chr.id].getPos();
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
