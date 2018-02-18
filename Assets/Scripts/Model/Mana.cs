using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mana : Subject {

	public enum MANATYPE {
		PHYSICAL, MENTAL, ENERGY, BLOOD, EFFORT
	};

	public static int nManaTypes = 5;

	public int[] arMana;
	public int[] arManaPool;
	//for keeping track of the qorder in which mana was added
	public LinkedList<MANATYPE> qManaPool;


	//For adding just a single type of mana
	public void AddMana(MANATYPE type, int nAmount){
		arMana [(int)type] += nAmount;
	}

	//If it's more convenient to add a bunch of mana
	public void AddMana(int [] _arMana){
		Debug.Assert (_arMana.Length == nManaTypes || _arMana.Length == nManaTypes - 1);

		for (int i = 0; i < _arMana.Length; i++) {
			arMana [i] += _arMana [i];
		}
	}

	public bool AddToPool(MANATYPE type){
		return AddToPool (type, 1);
	}

	public bool AddToPool(MANATYPE type, int nAmount){

		if (arMana [(int)type] < nAmount) {
			Debug.Log ("Not enough mana");
			return false;
		}

		//We know we have enough mana
		for (int i = 0; i < nAmount; i++) {
			arMana [(int)type]--;
			arManaPool [(int)type]++;
			qManaPool.AddLast (type);
		}

		return true;
	}

	public bool RemoveFromPool(MANATYPE type){
		return RemoveFromPool (type, 1);
	}

	public bool RemoveFromPool(MANATYPE type, int nAmount){

		if (arManaPool [(int)type] < nAmount) {
			Debug.Log ("Not enough mana in pool");
			return false;
		}

		//We know we have enough mana
		for (int i = 0; i < nAmount; i++) {
			arManaPool [(int)type]--;
			arMana [(int)type]++;
			qManaPool.Remove (type);
		}

		return true;
	}


	public bool SpendMana(MANATYPE type){
		return SpendMana (type, 1);
	}

	public bool SpendMana(MANATYPE type, int nAmount){
		int [] cost = new int[nManaTypes];
		cost [(int)type] = nAmount;
		return SpendMana (cost);
	}

	public bool SpendMana(int [] arCost){
		Debug.Assert (arCost.Length == nManaTypes || arCost.Length == nManaTypes - 1);

		int nTotalMana = 0;
		int nTotalCost = 0;


		//WARNING:: This assume that effort can be paid with any
		//          type of mana and that it is always in the last position in the array
		for (int i = 0; i < arCost.Length; i++) {
			nTotalMana += arManaPool [i];
			nTotalCost += arCost [i];
			if(arManaPool[i] < arCost[i]){
				if (i == (int)MANATYPE.EFFORT && nTotalMana >= nTotalCost) {
					Debug.Log ("Can pay effort with other types of mana");
					break;
				}
				Debug.Log ("Can't pay this mana cost");
				return false;
			}
		}

		//We know we have enough mana to pay for this
		for (int i = 0; i < arCost.Length; i++) {
			for (int j = 0; j < arCost [i]; j++) {
				if (arManaPool [i] > 0) {
					arManaPool [i]--;
					qManaPool.Remove ((MANATYPE)i);
				} else if (i == (int)MANATYPE.EFFORT) {
					//Then remove any type of mana (whatever was added longest ago)
					arManaPool [(int)(qManaPool.First.Value)]--;
					qManaPool.RemoveFirst ();
				} else {
					Debug.LogError ("RAN OUT OF MANA TO SPEND!");
					return false;
				}
			}
		}

		return true;
	}




	void Start () {
		arMana = new int[nManaTypes];
		arManaPool = new int[nManaTypes];
		qManaPool = new LinkedList<MANATYPE> ();
	}

}
