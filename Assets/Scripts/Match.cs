using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match : MonoBehaviour {

	Arena arena;

	//TODO:: make these an array of arbitrary number
	Player player1;
	Player player2;

	// Use this for initialization
	void Start () {

		arena = GetComponentInChildren<Arena> ();
		Debug.Assert (arena != null);

		player1 = new Player ();
		player2 = new Player ();

		arena.SetPlayers (player1, player2);

	}


	
	// Update is called once per frame
	void Update () {
		
	}
}
