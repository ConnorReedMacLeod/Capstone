﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour{

	bool bStarted;

	public int id;
	public static int MAXCHRS = 3;
	public static int MAXPLAYERS = 2;
	public Chr[] arChr;
	public Chr.CHARTYPE[] arChrTypeSelection;
	public int nChrs;

	public GameObject pfManaPanel;

	public Mana mana;

	public void setChrs(){
		//placeholder until character selection is available
		arChrTypeSelection[0] = Chr.CHARTYPE.KATARA;
		arChrTypeSelection[1] = Chr.CHARTYPE.LANCER;
		arChrTypeSelection[2] = Chr.CHARTYPE.SKELCOWBOY;
		nChrs = 3;
	}

	public void RegisterID(int _id){
		id = _id;
	}

	// Use this for initialization
	public void Start () {

		if (bStarted == false) {
			bStarted = true;
			arChrTypeSelection = new Chr.CHARTYPE[MAXCHRS];

			GameObject manaPanel = Instantiate(pfManaPanel);
			mana = manaPanel.GetComponent<Mana>();

			//TODO: Change this, all this, too work with networking
			if (id == 0) {
				manaPanel.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
			} else {
				manaPanel.transform.position = new Vector3(0.0f, 10.0f, 0.0f);
			}
		}
	}

}
