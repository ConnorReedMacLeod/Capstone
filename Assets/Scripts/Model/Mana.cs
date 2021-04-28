//BEN HICKS

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ViewMana))]
public class Mana : MonoBehaviour {

    public enum MANATYPE {
        PHYSICAL, MENTAL, ENERGY, BLOOD, EFFORT
    };

    //Provide string names to each of the mana types
    public static string[] arsManaTypes = { "Physical", "Mental", "Energy", "Blood", "Effort" };

    public const int nManaTypes = 5; //Number of mana types (PHYSICAL, MENTAL, ENERGY, BLOOD, EFFORT)

    public int[] arMana;        //Player's mana totals for each type
    public int[] arManaPool;    //Player's mana of each type in the pool
    public int nManaPool;       //Total amount of mana in mana pool

    public Player plyr;         //Reference to the Player who owns this

    bool bStarted = false;

    public ViewMana view;

    //Tracks the order in which mana was added to the mana pool
    public LinkedList<MANATYPE> qManaPool;

    public Subject subManaChange = new Subject();
    public static Subject subAllManaChange = new Subject(Subject.SubType.ALL);

    public Subject subManaPoolChange = new Subject();
    public static Subject subAllManaPoolChange = new Subject(Subject.SubType.ALL);


    public static int[] ConvertToCost(int[] arCost) {

        Debug.Assert(arCost.Length == nManaTypes);

        int[] arNegCost = new int[nManaTypes];

        for(int i = 0; i < nManaTypes; i++) {
            arNegCost[i] = -arCost[i];
            //Don't allow negative (switched to positive) costs
            if(arNegCost[i] > 0) arCost[i] = 0;
        }

        return arNegCost;

    }


    //For adding one mana of one type to player's total mana
    public void ChangeMana(MANATYPE type) {
        ChangeMana(type, 1);
    }

    //For adding any number of mana of one type to player's total mana
    public void ChangeMana(MANATYPE type, int nAmount) {
        //If the change is positive, just add it directly
        if(nAmount >= 0) {
            arMana[(int)type] += nAmount;
        } else {
            //Otherwise we're spending mana
            SpendMana(type, -nAmount);

        }

        subManaChange.NotifyObs(null, type);
        subAllManaChange.NotifyObs(null, type);
    }

    //For adding any number of mana of any number of types to player's total mana, using an array of MANATYPEs
    public void ChangeMana(int[] _arMana) {
        Debug.Assert(_arMana.Length == nManaTypes || _arMana.Length == nManaTypes - 1);

        for(int i = 0; i < _arMana.Length; i++) {
            ChangeMana((MANATYPE)i, _arMana[i]);
        }
    }

    //For adding one mana of one type to player's mana pool
    public bool AddToPool(MANATYPE type) {
        return AddToPool(type, 1);
    }

    public bool AddToPool(int indexManaType) {
        return AddToPool((MANATYPE)indexManaType);
    }

    //For adding any number of mana of one type to player's mana pool
    public bool AddToPool(MANATYPE type, int nAmount) {

        //Checks if the player has enough mana outside the mana pool
        if(arMana[(int)type] < nAmount) {
            Debug.Log("Not enough " + (MANATYPE)type + " to add to the pool");
            return false;
        }

        //Removes mana from outside the mana pool, placing it in the mana pool
        for(int i = 0; i < nAmount; i++) {
            arMana[(int)type]--;
            arManaPool[(int)type]++;
            qManaPool.AddLast(type);
            nManaPool++;
        }

        subManaChange.NotifyObs(null, type);
        subAllManaChange.NotifyObs(null, type);

        subManaPoolChange.NotifyObs(null, type);
        subAllManaPoolChange.NotifyObs(null, type);

        return true;
    }

    //For removing one mana of one type from the player's mana pool
    public bool RemoveFromPool(MANATYPE type) {
        return RemoveFromPool(type, 1);
    }

    //For removing any number of mana of one type from the player's mana pool
    public bool RemoveFromPool(MANATYPE type, int nAmount) {

        //Checks if the player has enough mana in the mana pool
        if(arManaPool[(int)type] < nAmount) {
            Debug.Log("Not enough mana in pool");
            return false;
        }

        //Removes mana from the mana pool, placing it outside the mana pool
        for(int i = 0; i < nAmount; i++) {
            arManaPool[(int)type]--;
            arMana[(int)type]++;
            qManaPool.Remove(type);
            nManaPool--;
        }

        subManaChange.NotifyObs(null, type);
        subAllManaChange.NotifyObs(null, type);
        subManaPoolChange.NotifyObs(null, type);
        subAllManaPoolChange.NotifyObs(null, type);

        return true;
    }

    public bool HasMana(int nManaType) {

        return HasMana(nManaType, 1);

    }

    public bool HasMana(int nManaType, int nAmount) {
        return arMana[nManaType] >= nAmount;
    }

    //Checks to see if the player has enough mana total and in their mana pool to pay for a given cost
    public bool HasMana(int[] arCost) {
        //Checks that the given cost contains only up to 5 mana types
        Debug.Assert(arCost.Length == nManaTypes || arCost.Length == nManaTypes - 1);

        //WARNING:: This assumes that effort can be paid with any
        //          type of mana and that it is always in the last position in the array
        for(int i = 0; i < arCost.Length; i++) {

            //For mana type [i], checks if the player has enough to cover the cost
            //Effort mana, or mana type [4], will often fail this check, as it is always 0
            if(arMana[i] < arCost[i]) {
                //After all other costs are paid, leftover mana pool mana is used to pay for effort
                if(i == (int)MANATYPE.EFFORT) {

                    if(nManaPool >= arCost[i]) {
                        //Enough mana is in the pool to pay for this

                    } else {
                        Debug.Log("Not enough mana in the mana pool");
                        return false;
                    }
                } else {
                    Debug.Log("Not enough " + (MANATYPE)i + " to pay this cost");
                    return false;
                }
            }
        }

        return true;
    }

    //For spending one mana of one type
    public bool SpendMana(MANATYPE type) {
        return SpendMana(type, 1);
    }

    //For spending any number of mana of one type
    public bool SpendMana(MANATYPE type, int nAmount) {

        for(int j = 0; j < nAmount; j++) {

            //Pays for coloured mana (which you will actually have mana for)
            if(arMana[(int)type] > 0) {
                arMana[(int)type]--;
                subManaChange.NotifyObs(null, type);
                subAllManaChange.NotifyObs(null, type);

                //Pays for effort mana
            } else if(type == MANATYPE.EFFORT && nManaPool > 0) {

                //Uses mana in order of most recently added to mana pool
                arManaPool[(int)(qManaPool.First.Value)]--;
                nManaPool--;
                subManaPoolChange.NotifyObs(null, qManaPool.First.Value);
                subAllManaPoolChange.NotifyObs(null, qManaPool.First.Value);

                qManaPool.RemoveFirst();

                //Catches non-existant mana types
            } else {
                Debug.LogError("RAN OUT OF MANA TO SPEND!");
                return false;
            }
        }

        return true;
    }

    //For spending any number of mana of any type
    public bool SpendMana(int[] arManaChange) {
        Debug.Assert(arManaChange.Length == nManaTypes || arManaChange.Length == nManaTypes - 1);

        //Cycle through each mana type in the change
        for(int i = 0; i < arManaChange.Length; i++) {

            if(SpendMana((MANATYPE)i, arManaChange.Length) == false) {
                return false;
            }

            //After changing this cost, ensure it didn't go negative
            if(arMana[i] < 0) {
                if(i == (int)MANATYPE.EFFORT) {
                    if(nManaPool < 0) {
                        Debug.LogError("Mana Effort is " + nManaPool);
                    }
                } else {
                    Debug.LogError("Mana " + (MANATYPE)i + " is " + arMana[i]);
                }
            }
        }

        return true;
    }


    public void SetPlayer(Player _plyr) {
        plyr = _plyr;
    }

    public void Start() {
        if(bStarted == false) {
            bStarted = true;

            view = GetComponent<ViewMana>();
            view.Start();

            arMana = new int[nManaTypes];
            arManaPool = new int[nManaTypes];
            qManaPool = new LinkedList<MANATYPE>();

            ChangeMana(new int[] { 4, 4, 4, 4, 0 });
        }
    }
}
