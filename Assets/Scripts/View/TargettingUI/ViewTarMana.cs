using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewTarMana : Singleton<ViewTarMana> {

    public TarMana modTarMana;
    public Mana manaToPay; //Save a reference to the mana that needs to be paid
    public Player plyrPaying; //Save a reference to the player paying the cost

    public List<GameObject> lstgoManaIcons;

    public const float fManaSymbolSize = 0.2f;
    public const float fManaSymbolSpacing = 0.5f;

    public GameObject goRequiredManaPosition; //The game object that will contain the mana icons that are being requested

    public Mana manaToSpend;//The amount of mana we have allocated for paying the manacost
    public Mana manaToSpendOnEffort; //Specifically the amount of mana allocated to cover the effort portion of the manacost

    public bool bCanPayCost; //Remember if we can or cannot pay the full cost we're being asked to pay

    public Vector3 v3OnScreen = new Vector3(0, 0, -2.5f);
    public Vector3 v3OffScreen = new Vector3(-100, -100, -2.5f);

    public void InitializeManaIcons() {

        //For each mana type, fill in as many of the mana pips as we can cover with our mana pool,
        //  and X out the rest (can just leave un-covered effort mana empty for now until covered by coloured mana)

        for(int i = 0; i <= (int)Mana.MANATYPE.EFFORT; i++) {

            int nManaToPay = manaToPay[i];
            int nManaCanPay = Mathf.Min(nManaToPay, plyrPaying.manapool.manaUsableToPay[i]);
            int nManaUnpayable = nManaToPay - nManaCanPay;

            //For each mana pip we can afford, spawn a paid icon for it
            for(int j = 0; j < nManaCanPay; j++) {
                AddManaIcon((Mana.MANATYPE)i, true, (Mana.MANATYPE)i);
                manaToSpend[j]++;
                if (i == (int)Mana.MANATYPE.EFFORT) manaToSpendOnEffort[j]++;
            }
            //For each mana pip we can't afford, spawn an unpaid icon for it
            for(int j = 0; j < nManaUnpayable; j++) {

                AddManaIcon((Mana.MANATYPE)i, false);
            }


        }
    }

    public void ReplaceManaIcon(int indexToReplace, Mana.MANATYPE manaType, bool bPaidFor, Mana.MANATYPE manaPaidWith = Mana.MANATYPE.EFFORT) {
        LibView.AssignSpritePathToObject(GetManaIconSpritePath(manaType, bPaidFor, manaPaidWith), lstgoManaIcons[indexToReplace]);
    }

    public void UpdateEffortManaIcons() {
        List<Mana.MANATYPE> lstManaAllocated = Mana.ManaToListOfTypes(manaToSpendOnEffort);

        //Go through each Effort Mana Icon and update it's payment icon to be paid with the appropriate amount of (coloured) allocated mana
        for(int i = manaToPay.GetTotalColouredMana(), j = 0; i < manaToPay.GetTotalMana(); i++, j++) {
            //Note:  Will have to eventually update the upper loop bounds to deal with X mana costs

            //If we've gone through all the allocated mana we've prepared so far, then the remaining mana icons can all be set as unpaid
            if(j >= manaToPay.GetTotalMana()) {
                ReplaceManaIcon(i, Mana.MANATYPE.EFFORT, false);
            } else {
                //If we've paid this mana, then fill in the icon with the colour of mana at this index in the list of allocated mana
                ReplaceManaIcon(i, Mana.MANATYPE.EFFORT, true, lstManaAllocated[j]);
            }

        }
    }

    public void AddManaIcon(Mana.MANATYPE manaType, bool bPaidFor, Mana.MANATYPE manaPaidWith = Mana.MANATYPE.EFFORT) {

        GameObject goManaIcon = new GameObject(string.Format("sprManaIcon{0}", lstgoManaIcons.Count));

        goManaIcon.transform.parent = goRequiredManaPosition.transform;

        SpriteRenderer sprRen = goManaIcon.AddComponent<SpriteRenderer>();

        //Assign the appropriate sprite
        LibView.AssignSpritePathToObject(GetManaIconSpritePath(manaType, bPaidFor, manaPaidWith), goManaIcon);

        //Sacle the icon apropriately
        goManaIcon.transform.localScale = new Vector3(fManaSymbolSize, fManaSymbolSize, 1);

        //Place the icon at the appropriate spot
        goManaIcon.transform.localPosition = new Vector3(fManaSymbolSpacing * (0.5f + 1.5f * lstgoManaIcons.Count), 0f, 0f);

        //Ensure the symbol appears ahead of the mana panel
        sprRen.sortingOrder = 1;

        lstgoManaIcons.Add(goManaIcon);
    }

    //Destroy the last-most mana icon
    public void DestroyManaIcon() {
        Debug.Assert(lstgoManaIcons.Count != 0);

        Destroy(lstgoManaIcons[lstgoManaIcons.Count - 1]);

        lstgoManaIcons.RemoveAt(lstgoManaIcons.Count - 1);
    }



    public string GetManaIconSpritePath(Mana.MANATYPE manatype, bool bPaidFor, Mana.MANATYPE manaPaidWith = Mana.MANATYPE.EFFORT) {
        if(bPaidFor == false) {
            return string.Format("Images/Mana/CostUI/img{0}Unpaid", Mana.arsManaTypes[(int)manatype]);
        } else if(manatype != Mana.MANATYPE.EFFORT) {
            //For paid coloured mana
            return string.Format("Images/Mana/CostUI/img{0}Paid", Mana.arsManaTypes[(int)manatype]);
        } else {
            //For paid effort mana 
            return string.Format("Images/Mana/CostUI/imgEffortPaidWith{0}", Mana.arsManaTypes[(int)manaPaidWith]);
        }
    }



    //Set the TarMana model that we're going to be facilitating payment for
    public void StartPayment(TarMana _modTarMana) {

        //Move the panel onscreen
        MoveOnScreen();

        modTarMana = _modTarMana;
        manaToPay = modTarMana.manaCostRequired.pManaCost.Get();
        plyrPaying = modTarMana.skill.chrOwner.plyrOwner;

        //Check (and save the result) if the player can even possibly afford the cost 
        bCanPayCost = plyrPaying.manapool.CanPayManaCost(modTarMana.manaCostRequired);

        manaToSpend = new Mana(0, 0, 0, 0, 0);

        //Initialize the mana icons we're displaying the mana cost with (and determine what amounts of mana we're
        //  paying for non-effort costs
        InitializeManaIcons();

        if(bCanPayCost) {
            //Initialize the currently allocated mana to auto-spend all effort mana (that we can)
            manaToSpendOnEffort = new Mana(0, 0, 0, 0, manaToSpend[Mana.MANATYPE.EFFORT]);

            //Have the paying player reserve the starting amount of mana
            plyrPaying.manapool.ReserveMana(manaToSpend);
        }

    }

    //CLear out anything from the current payment process
    public void CleanUp() {


        //Clear out the model we were paying for
        modTarMana = null;
        manaToPay = null;
        plyrPaying = null;
        bCanPayCost = false;

        manaToSpend = null;
        manaToSpendOnEffort = null;

        //Destroy all the mana cost icons we had
        for(int i = 0; i < lstgoManaIcons.Count; i++) {
            DestroyManaIcon();
        }

        //Hide the panel offscreen until it's needed again
        MoveOffscreen();
    }

    public void MoveOnScreen() {
        transform.position = v3OnScreen;
    }

    public void MoveOffscreen() {
        transform.position = v3OffScreen;
    }

    public override void Init() {

        //Set up all the keybindings we need for allocating/deallocating mana
        KeyBindings.SetBinding(AddPhysical, KeyCode.Q);
        KeyBindings.SetBinding(AddMental, KeyCode.W);
        KeyBindings.SetBinding(AddEnergy, KeyCode.E);
        KeyBindings.SetBinding(AddBlood, KeyCode.R);

        KeyBindings.SetBinding(RemovePhysical, KeyCode.A);
        KeyBindings.SetBinding(RemoveMental, KeyCode.S);
        KeyBindings.SetBinding(RemoveEnergy, KeyCode.D);
        KeyBindings.SetBinding(RemoveBlood, KeyCode.F);

        KeyBindings.SetBinding(SubmitAllocatedMana, KeyCode.T);
        
    }


    public void SubmitAllocatedMana(Object target, params object[] args) {

        if(plyrPaying == null) {
            Debug.Log("Cannot submit a mana payment since no player is paying a mana cost");
            return;
        }

        if(bCanPayCost == false) {
            Debug.Log("Cannot submit this mana payment since this cost cannot be paid with the player's mana resources");
            return;
        }

        if(manaToSpendOnEffort.GetTotalMana() < manaToPay[Mana.MANATYPE.EFFORT]) {
            Debug.Log("Cannot submit this mana payment since not enough mana has been allocated to pay for the effort portion");
            return;
        }

        if (modTarMana.manaCostRequired.CanBePaidWith(manaToSpend) == false) {
            Debug.Log("Cannot submit this mana payment since the apportioned mana is somehow insufficient for paying the cost");
            return;
        }

        //At this point, they should be able to pay, and should have allocated some amount of their mana to pay for the effort portion
        //  We can pass along the total mana amount to the TarMana model to submit as its payment
        modTarMana.AttemptSelection(manaToSpend);
    }

    public void AddMana(Mana.MANATYPE manaType) {
        if(plyrPaying == null) {
            Debug.Log("Cannot allocate mana since no player is paying a mana cost");
            return;
        }
        if(bCanPayCost == false) {
            Debug.Log("Cannot allocate mana since this cost cannot be paid with the player's mana resources");
            return;
        }

        if(manaToSpendOnEffort.GetTotalMana() == manaToPay[Mana.MANATYPE.EFFORT]) {
            Debug.Log("Cannot allocate mana since we've already allocated enough for the full effort cost");
            return;
        }

        if(plyrPaying.manapool.manaUsableToPay[manaType] == 0) {
            Debug.Log("Cannot allocate mana since we have already promised all our mana for this mana type");
            return;
        }

        //Increment the requested type of mana
        manaToSpend[manaType]++;
        manaToSpendOnEffort[manaType]++;

        //Reserve one mana from our mana pool to be ready to pay for this payment
        plyrPaying.manapool.ReserveMana(manaType);

        //Re-display the promised mana
        UpdateEffortManaIcons();
    }

    public void RemoveMana(Mana.MANATYPE manaType) {
        if(plyrPaying == null) {
            Debug.Log("Cannot deallocate mana since no player is paying a mana cost");
            return;
        }
        if(bCanPayCost == false) {
            Debug.Log("Cannot deallocate mana since this cost cannot be played with the player's mana resources");
            return;
        }
        if(manaToSpendOnEffort[manaType] == 0) {
            Debug.Log("Cannot deallocate mana since we haven't allocated any mana of this colour for effort payments");
        }

        //Decrement the requested type of mana
        manaToSpend[manaType]--;
        manaToSpendOnEffort[manaType]--;

        //Unreserve one mana that we had set aside in our mana pool to now be usable again
        plyrPaying.manapool.UnreserveMana(manaType);

        //Re-display the promised mana
        UpdateEffortManaIcons();

    }

    public void AddPhysical(Object target, params object[] args) { AddMana(Mana.MANATYPE.PHYSICAL); }
    public void AddMental(Object target, params object[] args) { AddMana(Mana.MANATYPE.MENTAL); }
    public void AddEnergy(Object target, params object[] args) { AddMana(Mana.MANATYPE.ENERGY); }
    public void AddBlood(Object target, params object[] args) { AddMana(Mana.MANATYPE.BLOOD); }

    public void RemovePhysical(Object target, params object[] args) { RemoveMana(Mana.MANATYPE.PHYSICAL); }
    public void RemoveMental(Object target, params object[] args) { RemoveMana(Mana.MANATYPE.MENTAL); }
    public void RemoveEnergy(Object target, params object[] args) { RemoveMana(Mana.MANATYPE.ENERGY); }
    public void RemoveBlood(Object target, params object[] args) { RemoveMana(Mana.MANATYPE.BLOOD); }

}
