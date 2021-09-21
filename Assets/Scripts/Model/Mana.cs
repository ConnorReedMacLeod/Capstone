//BEN HICKS lol

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mana {

    public enum MANATYPE {
        PHYSICAL, MENTAL, ENERGY, BLOOD, EFFORT
    };

    //Provide string names to each of the mana types
    public static string[] arsManaTypes = { "Physical", "Mental", "Energy", "Blood", "Effort" };

    public const int nManaTypes = 5; //Number of mana types (PHYSICAL, MENTAL, ENERGY, BLOOD, EFFORT)

    public int[] arMana;      //The amount of each type of mana


    public Mana(int nPhys, int nMental, int nEnergy, int nBlood, int nEffort = 0) {
        arMana = new int[] { nPhys, nMental, nEnergy, nBlood, nEffort };
    }

    public Mana(int[] arnMana) {
        Debug.Assert(arnMana.Length == 4 || arnMana.Length == 5);

        arMana = new int[5];
        System.Array.Copy(arnMana, arMana, arnMana.Length);
    }

    public Mana(Mana other) : this(other.arMana) {

    }

    public static Mana GetNegatedMana(Mana mana) {
        int[] arNegatedMana = new int[5];
        for(int i = 0; i < nManaTypes; i++) {
            arNegatedMana[i] = -mana.arMana[i];
        }
        return new Mana(arNegatedMana);
    }

    public override string ToString() {
        return string.Format("P:{0} M:{1}, E:{2}, B{3}, O:{4}", this[0], this[1], this[2], this[3], this[4]);
    }

    //Allows the use of index-like operators, ex: mana[MANA.MANATYPE.PHYSICAL] = 2
    public int this[MANATYPE type] {
        get => arMana[(int)type];
        set => arMana[(int)type] = value;
    }

    public int this[int type] {
        get => arMana[type];
        set => arMana[type] = value;
    }

    //Get the total amount of non-effort mana
    public int GetTotalColouredMana() {
        int nColouredMana = 0;
        for(int i = 0; i < (int)MANATYPE.EFFORT; i++) {
            nColouredMana += arMana[i];
        }
        return nColouredMana;
    }

    //Get the total amount of mana
    public int GetTotalMana() {
        return GetTotalColouredMana() + arMana[(int)MANATYPE.EFFORT];
    }



    public void ChangeMana(Mana manaDelta) {
        for(int i = 0; i < nManaTypes; i++) {
            if(this[i] + manaDelta[i] < 0) {
                Debug.LogError("This would yield a negative mana amount for type " + (MANATYPE)i);
            }
            this[i] += manaDelta[i];
        }
    }

    public static List<MANATYPE> ManaToListOfTypes(Mana mana) {
        List<MANATYPE> lstManaTypes = new List<MANATYPE>();

        for(int manaType = (int)MANATYPE.PHYSICAL; manaType <= (int)MANATYPE.EFFORT; manaType++) {
            for (int i = 0; i < mana[manaType]; i++) {
                lstManaTypes.Add((MANATYPE)manaType);
            }
        }

        return lstManaTypes;
    }

}