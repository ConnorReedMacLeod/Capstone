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

    public Vector2[,] arCharacterPositions =
        new Vector2[2, 7] {
             {  new Vector2(0.0f, 0.0f),
                new Vector2(0.0f, 0.0f),
                new Vector2(0.0f, 0.0f),
                new Vector2(0.0f, 0.0f),
                new Vector2(0.0f, 0.0f),
                new Vector2(0.0f, 0.0f),
                new Vector2(0.0f, 0.0f) },
             {  new Vector2(0.0f, 0.0f),
                new Vector2(0.0f, 0.0f),
                new Vector2(0.0f, 0.0f),
                new Vector2(0.0f, 0.0f),
                new Vector2(0.0f, 0.0f),
                new Vector2(0.0f, 0.0f),
                new Vector2(0.0f, 0.0f) } };


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

	public void PlaceUnit(Chr chr, float x, float y){

		if (x - chr.fRad < -(nWidth / 2)) {
			Debug.LogError ("ERROR!  Trying to place this Chr too far left!");
		}
		if (x + chr.fRad > (nWidth / 2)) {
			Debug.LogError ("ERROR!  Trying to place this Chr too far right!");
		}
		if (y - chr.fRad < -(nHeight / 2)) {
			Debug.LogError ("ERROR!  Trying to place this Chr too far up!");
		}
		if (y + chr.fRad > (nHeight / 2)) {
			Debug.LogError ("ERROR!  Trying to place this Chr too far down!");
		}

		chr.SetPosition (x, y);
	}

	public void InitPlaceUnit(Chr chr){
		if (chr.plyrOwner.id == 0) {
			PlaceUnit (chr, -fStartPosX * fArenaWidth, arfStartingPosY [chr.id] * fArenaHeight);
		} else {
			PlaceUnit (chr, fStartPosX * fArenaWidth, arfStartingPosY [chr.id] * fArenaHeight);
		}
	}

	public void InitArenaSize(){

		//Set the starting positions (as a percentage of the total arena size);
		fStartPosX = 0.3f;
		arfStartingPosY = new float[]{-0.25f, 0.0f, 0.25f};

		fArenaWidth = transform.localScale.x;
		fArenaHeight = transform.localScale.y;

		fUnit = Chr.arfSize [(int)Chr.SIZE.MEDIUM];

		float fRem = (fArenaWidth / fUnit) % 1.0f;
		if (Mathf.Abs (fRem * (1.0f - fRem)) > 0.1f) {
			Debug.LogError ("ALERT! Arena's width is not a multiple of the unit size!");
		}
		nWidth = (int)(fArenaWidth / fUnit);

		fRem = (fArenaHeight / fUnit) % 1.0f;
		if (Mathf.Abs (fRem * (1.0f - fRem)) > 0.1f) {
			Debug.LogError ("ALERT! Arena's height is not a multiple of the unit size!");
		}
		nHeight = (int)(fArenaHeight / fUnit);
	}

	public void Start(){

		if (bStarted == false) {
			bStarted = true;

			InitArenaSize ();

			view = GetComponent<ViewArena> ();
		}
	}


}
