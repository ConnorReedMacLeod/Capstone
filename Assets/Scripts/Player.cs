using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player {

	static int MAXCHRS = 6;
	public Character[] arChr;
	public Arena.CHARTYPE[] arChrTypeSelection;
	public int nChrs;

	// Use this for initialization
	public Player () {
		arChrTypeSelection = new Arena.CHARTYPE[MAXCHRS];
		arChrTypeSelection[0] = Arena.CHARTYPE.SKELCOWBOY;
		arChrTypeSelection[1] = Arena.CHARTYPE.SKELCOWBOY;
		arChrTypeSelection[2] = Arena.CHARTYPE.SKELCOWBOY;
		nChrs = 3;
	}

}
