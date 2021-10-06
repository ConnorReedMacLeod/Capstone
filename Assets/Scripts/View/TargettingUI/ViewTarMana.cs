using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewTarMana : MonoBehaviour {


    public List<GameObject> lstgoManaIcons;

    public const int nManaSymbolSpacing = 20;

    public GameObject goRequiredManaPosition; //The game object that will contain the mana icons that are being requested

    




    public void SpawnStartingManaIcons(Mana manaToPay, Player plyrPaying) {
        //For each mana type, fill in as many of the mana pips as we can cover with our mana pool,
        //  and X out the rest (can just leave un-covered effort mana empty for now until covered by coloured mana)

        for (int i = 0; i <= (int)Mana.MANATYPE.EFFORT; i++) {

            int nManaToPay = manaToPay[i];
            int nManaCanPay = Mathf.Min(nManaToPay, plyrPaying.mana.manaUsableToPay[i]);
            int nManaUnpayable = nManaToPay - nManaCanPay;

            //For each mana pip we can afford, spawn a paid icon for it
            for (int j = 0; j < nManaCanPay; j++) {
                AddManaIcon((Mana.MANATYPE)i, true, (Mana.MANATYPE)i);
            }
            //For each mana pip we can't afford, spawn an unpaid icon for it
            for (int j = 0; j < nManaUnpayable; j++) {
                AddManaIcon((Mana.MANATYPE)i, false);
            }
        }

    }

    public void ReplaceManaIcon(int indexToReplace, Mana.MANATYPE manaType, bool bPaidFor, Mana.MANATYPE manaPaidWith) {
        LibView.AssignSpritePathToObject(GetManaIconSpritePath(manaType, bPaidFor, manaPaidWith), lstgoManaIcons[indexToReplace]);
    }

    public void AddManaIcon(Mana.MANATYPE manaType, bool bPaidFor, Mana.MANATYPE manaPaidWith = Mana.MANATYPE.EFFORT) {

        GameObject goManaIcon = Instantiate(new GameObject(string.Format("sprManaIcon{0}", lstgoManaIcons.Count)), goRequiredManaPosition.transform);

        //Assign the appropriate sprite
        LibView.AssignSpritePathToObject(GetManaIconSpritePath(manaType, bPaidFor, manaPaidWith), goManaIcon);

        //Place the icon at the appropriate spot
        goManaIcon.transform.localPosition = new Vector3(nManaSymbolSpacing * (0.5f + 1.5f * lstgoManaIcons.Count), 0f, 0f);

        lstgoManaIcons.Add(goManaIcon);
    }

    //Destroy the last-most mana icon
    public void DestroyManaIcon() {
        Debug.Assert(lstgoManaIcons.Count != 0);

        Destroy(lstgoManaIcons[lstgoManaIcons.Count - 1]);
        lstgoManaIcons[lstgoManaIcons.Count - 1] = null;
    }



    public string GetManaIconSpritePath(Mana.MANATYPE manatype, bool bPaidFor, Mana.MANATYPE manaPaidWith = Mana.MANATYPE.EFFORT) {
        if (bPaidFor == false) {
            return string.Format("Images/Mana/{0}Unpaid.png", Mana.arsManaTypes[(int)manatype]);
        } else if (manatype != Mana.MANATYPE.EFFORT){
            //For paid coloured mana
            return string.Format("Images/Mana/{0}Paid.png", Mana.arsManaTypes[(int)manatype]);
        } else {
            //For paid effort mana 
            return string.Format("Images/Mana/EffortPaidWith{0}.png", Mana.arsManaTypes[(int)manaPaidWith]);
        }
    }



    // Start is called before the first frame update
    void Start() {


    }

    // Update is called once per frame
    void Update() {

    }
}
