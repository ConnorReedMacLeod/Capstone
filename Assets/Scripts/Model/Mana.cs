//BEN HICKS

//TODO::
//Right now, the code only allows mana in the mana pool to be spent.
//Instead, the mana pool shuold be changed so that it indicates what mana is available to be used for effort.
//Coloured mana can be taken from anywhere, prioritizing mana outside the mana pool.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mana : Subject {

	public enum MANATYPE {
		PHYSICAL, MENTAL, ENERGY, BLOOD, EFFORT
	};

	//Provide string names to each of the mana types
	public static string[] arsManaTypes = { "Physical", "Mental", "Energy", "Blood", "Effort" };

	public static int nManaTypes = 5; //Number of mana types (PHYSICAL, MENTAL, ENERGY, BLOOD, EFFORT)

	public int[] arMana;        //Player's mana, total
	public int[] arManaPool;    //Player's mana in mana pool

	//Tracks the order in which mana was added to the mana pool
	public LinkedList<MANATYPE> qManaPool;

	//For adding one mana of one type to player's total mana
	public void AddMana(MANATYPE type){
		AddMana (type, 1);
	}

	//For adding any number of mana of one type to player's total mana
	public void AddMana(MANATYPE type, int nAmount){
		arMana [(int)type] += nAmount;
	}

	//For adding any number of mana of any number of types to player's total mana, using an array of MANATYPEs
	public void AddMana(int [] _arMana){
		Debug.Assert (_arMana.Length == nManaTypes || _arMana.Length == nManaTypes - 1);

		for (int i = 0; i < _arMana.Length; i++) {
			arMana [i] += _arMana [i];
		}
	}

    //For adding one mana of one type to player's mana pool
	public bool AddToPool(MANATYPE type){
		return AddToPool (type, 1);
	}

    //For adding any number of mana of one type to player's mana pool
    public bool AddToPool(MANATYPE type, int nAmount){

        //Checks if the player has enough mana outside the mana pool
        if (arMana [(int)type] < nAmount) {
			Debug.Log ("Not enough mana");
			return false;
		}

        //Removes mana from outside the mana pool, placing it in the mana pool
        for (int i = 0; i < nAmount; i++) {
			arMana [(int)type]--;
			arManaPool [(int)type]++;
			qManaPool.AddLast (type);
		}

		return true;
	}

    //For removing one mana of one type from the player's mana pool
	public bool RemoveFromPool(MANATYPE type){
		return RemoveFromPool (type, 1);
	}

    //For removing any number of mana of one type from the player's mana pool
    public bool RemoveFromPool(MANATYPE type, int nAmount){

        //Checks if the player has enough mana in the mana pool
        if (arManaPool [(int)type] < nAmount) {
			Debug.Log ("Not enough mana in pool");
			return false;
		}

        //Removes mana from the mana pool, placing it outside the mana pool
        for (int i = 0; i < nAmount; i++) {
			arManaPool [(int)type]--;
			arMana [(int)type]++;
			qManaPool.Remove (type);
		}

		return true;
	}

    //Checks to see if the player has enough mana total and in their mana pool to pay for a given cost
    public bool HasMana(int[] arCost)
    {
        //Checks that the given cost contains only up to 5 mana types
        Debug.Assert(arCost.Length == nManaTypes || arCost.Length == nManaTypes - 1);

        int nTotalMana = 0;
        int nTotalCost = 0;

        //WARNING:: This assumes that effort can be paid with any
        //          type of mana and that it is always in the last position in the array
        for (int i = 0; i < arCost.Length; i++)
        {
            nTotalMana += arManaPool[i];
            nTotalCost += arCost[i];

            //For mana type [i], checks if the player has enough to cover the cost
            //Effort mana, or mana type [5], will often fail this check, as it is always 0
            if (arManaPool[i] < arCost[i])
            {
                //After all other costs are paid, leftover mana pool mana is used to pay for effort
                if (i == (int)MANATYPE.EFFORT && nTotalMana >= nTotalCost)
                {
                    Debug.Log("Can pay effort with other types of mana");
                    break;
                }

                Debug.Log("Can't pay this mana cost");
                return false;
            }
        }

        return true;
    }

    //For spending one mana of one type
    public bool SpendMana(MANATYPE type){
		return SpendMana (type, 1);
	}

    //For spending any number of mana of one type
	public bool SpendMana(MANATYPE type, int nAmount){
		int [] arCost = new int[nManaTypes];
		arCost [(int)type] = nAmount;
		return SpendMana (arCost);
	}

    //For spending any number of mana of any type
	public bool SpendMana(int [] arCost){
		Debug.Assert (arCost.Length == nManaTypes || arCost.Length == nManaTypes - 1);

        //Uses HasMana function to check if player has enough mana to pay the given mana cost
        if (!HasMana(arCost)) {
            return false;
        }

        //Removes mana to pay for cost
		for (int i = 0; i < arCost.Length; i++) {
			for (int j = 0; j < arCost [i]; j++) {
                
                //Pays for coloured mana
                if (arManaPool [i] > 0) {
					arManaPool [i]--;
					qManaPool.Remove ((MANATYPE)i);

                //Pays for effort mana
				} else if (i == (int)MANATYPE.EFFORT) {

					//Uses mana in order of most recently added to mana pool
					arManaPool [(int)(qManaPool.First.Value)]--;
					qManaPool.RemoveFirst ();
                
                //Catches non-existant mana types
                } else {
					Debug.LogError ("RAN OUT OF MANA TO SPEND!");
					return false;
				}
			}
		}

		return true;
	}


	public Mana () {
		arMana = new int[nManaTypes];
		arManaPool = new int[nManaTypes];
		qManaPool = new LinkedList<MANATYPE> ();
	}

}
