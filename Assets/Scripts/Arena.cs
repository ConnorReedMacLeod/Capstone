using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arena : MonoBehaviour {

	public float fChrScaleX;
	public float fChrScaleY;
	public float fChrScaleZ;


	public GameObject baseChr;

	public enum CHARTYPE {
		LANCER, KATARA, SKELCOWBOY
	};

	// Not sure if I want array of gameobjects or character scripts
	List<GameObject> lstChrAll;

	List<GameObject> lstChrPlayer1;
	List<GameObject> lstChrPlayer2;

	GameObject CreateChr(CHARTYPE type, Vector3 pos){
		Debug.Log (baseChr);
		GameObject newObj = Instantiate (baseChr, pos, Quaternion.identity, GetComponent<Transform>());
		newObj.GetComponent<Transform> ().localScale = new Vector3 (fChrScaleX, fChrScaleY, fChrScaleZ);


		switch(type){
		case CHARTYPE.KATARA:
			newObj.AddComponent<ChrKatara> ();
			break;
		case CHARTYPE.LANCER:
			newObj.AddComponent<ChrLancer> ();
			break;
		case CHARTYPE.SKELCOWBOY:
			newObj.AddComponent<ChrSkelCowboy> ();
			break;
		default:
			Debug.Log ("CHARACTER TYPE NOT FOUND!");
			Debug.Assert (true == false);
			break;
		}

		lstChrAll.Add (newObj);
		return newObj;
	}

	public void SetPlayers(Player player1, Player player2){
		//Make this better
		for (int i = 0; i < player1.nChrs; i++) {
			//Debug.Log (i);
			lstChrPlayer1.Add(CreateChr (player1.arChrTypeSelection[i], new Vector3(-3.0f, -3.0f + 3.0f*i, 0)));
		}

		for (int i = 0; i < player2.nChrs; i++) {
			//Debug.Log (i);
			lstChrPlayer2.Add(CreateChr (player2.arChrTypeSelection[i], new Vector3(3.0f, -3.0f + 3.0f*i, 0)));
		}
	}

	public Arena(){
		lstChrAll = new List<GameObject> ();
		lstChrPlayer1 = new List<GameObject> ();
		lstChrPlayer2 = new List<GameObject> ();
	}

	// Use this for initialization
	void Start () {



	}

	void OnMouseDown(){
		/*
		Vector3 mousePos = Input.mousePosition;
		mousePos.z = 10.0f;
		mousePos = Camera.main.ScreenToWorldPoint (mousePos);
		CreateChr (CHARTYPE.SKELCOWBOY, mousePos);*/
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
