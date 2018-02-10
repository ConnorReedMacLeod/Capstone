using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player {

	public int id;
	public static int MAXCHRS = 3;
	public static int MAXPLAYERS = 2;
	public Character[] arChr;
	public Character.CHARTYPE[] arChrTypeSelection;
	public int nChrs;

	public void setChrs(){
		//placeholder until character selection is available
		arChrTypeSelection[0] = Character.CHARTYPE.KATARA;
		arChrTypeSelection[1] = Character.CHARTYPE.LANCER;
		arChrTypeSelection[2] = Character.CHARTYPE.SKELCOWBOY;
		nChrs = 3;
	}

	// Use this for initialization
	public Player (int _id) {
		id = _id;
		arChrTypeSelection = new Character.CHARTYPE[MAXCHRS];


	}

}
