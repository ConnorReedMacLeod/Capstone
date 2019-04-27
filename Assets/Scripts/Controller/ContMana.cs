using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContMana : Singleton<ContMana>{

    public KeyCode keyEnemyModifier; //For testing, holding this will make you change the enemies mana pool

    public int[] arnManaReserves;
    public const int nMaxReserves = 3;

    public override void Init() {

        //TODO:: These bindings should probably be set somewhere else - they shouldn't really be re-initialized here every time

        InitBindings();
        keyEnemyModifier = KeyCode.Tab;

        arnManaReserves = new int[Mana.nManaTypes - 1]; //sub 1 since we don't give out effort
        ResetManaReserves();
    }


    public void ResetManaReserves() {
        for(int i=0; i<arnManaReserves.Length; i++) {
            arnManaReserves[i] = nMaxReserves;
        }
    }

    public Mana.MANATYPE GetTurnStartMana() {

        int iManaToGive = Random.Range(0, Mana.nManaTypes - 1);
        int nTypesTried = 0;
        while(nTypesTried < Mana.nManaTypes) { 
            if (arnManaReserves[iManaToGive%(Mana.nManaTypes-1)] > 0) {
                arnManaReserves[iManaToGive%(Mana.nManaTypes - 1)]--;
            return (Mana.MANATYPE)(iManaToGive % (Mana.nManaTypes - 1));
            }
            iManaToGive++;
            nTypesTried++;
        }
        //If we reach here, then there's no mana left in any of the reserves
        ResetManaReserves();
        return GetTurnStartMana();
    }

	public void InitBindings(){
		KeyBindings.SetBinding (AddPhysical, KeyCode.Q);
		KeyBindings.SetBinding (AddAllPhysical, KeyCode.Q, KeyCode.LeftShift);
		KeyBindings.SetBinding (RemovePhysical, KeyCode.A);
		KeyBindings.SetBinding (RemoveAllPhysical, KeyCode.A, KeyCode.LeftShift);

		KeyBindings.SetBinding (AddMental, KeyCode.W);
		KeyBindings.SetBinding (AddAllMental, KeyCode.W, KeyCode.LeftShift);
		KeyBindings.SetBinding (RemoveMental, KeyCode.S);
		KeyBindings.SetBinding (RemoveAllMental, KeyCode.S, KeyCode.LeftShift);

		KeyBindings.SetBinding (AddEnergy, KeyCode.E);
		KeyBindings.SetBinding (AddAllEnergy, KeyCode.E, KeyCode.LeftShift);
		KeyBindings.SetBinding (RemoveEnergy, KeyCode.D);
		KeyBindings.SetBinding (RemoveAllEnergy, KeyCode.D, KeyCode.LeftShift);

		KeyBindings.SetBinding (AddBlood, KeyCode.R);
		KeyBindings.SetBinding (AddAllBlood, KeyCode.R, KeyCode.LeftShift);
		KeyBindings.SetBinding (RemoveBlood, KeyCode.F);
		KeyBindings.SetBinding (RemoveAllBlood, KeyCode.F, KeyCode.LeftShift);

		KeyBindings.SetBinding (AddAll, KeyCode.T);
		KeyBindings.SetBinding (RemoveAll, KeyCode.G);
	}

    //CONSIDER:: Maybe abstract these to be created by a factory
    public void AddPhysical(Object target, params object[] args) {
        if (Input.GetKey(keyEnemyModifier)) {
            Match.Get().GetEnemyPlayer().mana.AddToPool(Mana.MANATYPE.PHYSICAL);
        } else {
            Match.Get().GetLocalPlayer().mana.AddToPool(Mana.MANATYPE.PHYSICAL);
        }
    }
	public void AddAllPhysical(Object target, params object[] args) {
        if (Input.GetKey(keyEnemyModifier)) {
            int totalMana = Match.Get().GetEnemyPlayer().mana.arMana[(int)Mana.MANATYPE.PHYSICAL];
            Match.Get().GetEnemyPlayer().mana.AddToPool(Mana.MANATYPE.PHYSICAL, totalMana);
        } else {
            int totalMana = Match.Get().GetLocalPlayer().mana.arMana[(int)Mana.MANATYPE.PHYSICAL];
            Match.Get().GetLocalPlayer().mana.AddToPool(Mana.MANATYPE.PHYSICAL, totalMana);
        }
	}
	public void RemovePhysical(Object target, params object[] args) {
        if (Input.GetKey(keyEnemyModifier)) {
            Match.Get().GetEnemyPlayer().mana.RemoveFromPool(Mana.MANATYPE.PHYSICAL);
        } else {
            Match.Get().GetLocalPlayer().mana.RemoveFromPool(Mana.MANATYPE.PHYSICAL);
        }
	}
	public void RemoveAllPhysical(Object target, params object[] args) {
        if (Input.GetKey(keyEnemyModifier)) {
            int totalPool = Match.Get().GetEnemyPlayer().mana.arManaPool[(int)Mana.MANATYPE.PHYSICAL];
            Match.Get().GetEnemyPlayer().mana.RemoveFromPool(Mana.MANATYPE.PHYSICAL, totalPool);
        } else {
            int totalPool = Match.Get().GetLocalPlayer().mana.arManaPool[(int)Mana.MANATYPE.PHYSICAL];
            Match.Get().GetLocalPlayer().mana.RemoveFromPool(Mana.MANATYPE.PHYSICAL, totalPool);
        }
	}


	public void AddMental(Object target, params object[] args) {
        if (Input.GetKey(keyEnemyModifier)) {
            Match.Get().GetEnemyPlayer().mana.AddToPool(Mana.MANATYPE.MENTAL);
        } else {
            Match.Get().GetLocalPlayer().mana.AddToPool(Mana.MANATYPE.MENTAL);
        }
	}
	public void AddAllMental(Object target, params object[] args) {
        if (Input.GetKey(keyEnemyModifier)) {
            int totalMana = Match.Get().GetEnemyPlayer().mana.arMana[(int)Mana.MANATYPE.MENTAL];
            Match.Get().GetEnemyPlayer().mana.AddToPool(Mana.MANATYPE.MENTAL, totalMana);
        } else {
            int totalMana = Match.Get().GetLocalPlayer().mana.arMana[(int)Mana.MANATYPE.MENTAL];
            Match.Get().GetLocalPlayer().mana.AddToPool(Mana.MANATYPE.MENTAL, totalMana);
        }
	}
	public void RemoveMental(Object target, params object[] args) {
        if (Input.GetKey(keyEnemyModifier)) {
            Match.Get().GetEnemyPlayer().mana.RemoveFromPool(Mana.MANATYPE.MENTAL);
        } else {
            Match.Get().GetLocalPlayer().mana.RemoveFromPool(Mana.MANATYPE.MENTAL);
        }
	}
	public void RemoveAllMental(Object target, params object[] args) {
        if (Input.GetKey(keyEnemyModifier)) {
            int totalPool = Match.Get().GetEnemyPlayer().mana.arManaPool[(int)Mana.MANATYPE.MENTAL];
            Match.Get().GetEnemyPlayer().mana.RemoveFromPool(Mana.MANATYPE.MENTAL, totalPool);
        } else {
            int totalPool = Match.Get().GetLocalPlayer().mana.arManaPool[(int)Mana.MANATYPE.MENTAL];
            Match.Get().GetLocalPlayer().mana.RemoveFromPool(Mana.MANATYPE.MENTAL, totalPool);
        }
	}


	public void AddEnergy(Object target, params object[] args) {
        if (Input.GetKey(keyEnemyModifier)) {
            Match.Get().GetEnemyPlayer().mana.AddToPool(Mana.MANATYPE.ENERGY);
        } else {
            Match.Get().GetLocalPlayer().mana.AddToPool(Mana.MANATYPE.ENERGY);
        }
	}
	public void AddAllEnergy(Object target, params object[] args) {
        if (Input.GetKey(keyEnemyModifier)) {
            int totalMana = Match.Get().GetEnemyPlayer().mana.arMana[(int)Mana.MANATYPE.ENERGY];
            Match.Get().GetEnemyPlayer().mana.AddToPool(Mana.MANATYPE.ENERGY, totalMana);
        } else {
            int totalMana = Match.Get().GetLocalPlayer().mana.arMana[(int)Mana.MANATYPE.ENERGY];
            Match.Get().GetLocalPlayer().mana.AddToPool(Mana.MANATYPE.ENERGY, totalMana);
        }
	}
	public void RemoveEnergy(Object target, params object[] args) {
        if (Input.GetKey(keyEnemyModifier)) {
            Match.Get().GetEnemyPlayer().mana.RemoveFromPool(Mana.MANATYPE.ENERGY);
        } else {
            Match.Get().GetLocalPlayer().mana.RemoveFromPool(Mana.MANATYPE.ENERGY);
        }
	}
	public void RemoveAllEnergy(Object target, params object[] args) {
        if (Input.GetKey(keyEnemyModifier)) {
            int totalPool = Match.Get().GetEnemyPlayer().mana.arManaPool[(int)Mana.MANATYPE.ENERGY];
            Match.Get().GetEnemyPlayer().mana.RemoveFromPool(Mana.MANATYPE.ENERGY, totalPool);
        } else {
            int totalPool = Match.Get().GetLocalPlayer().mana.arManaPool[(int)Mana.MANATYPE.ENERGY];
            Match.Get().GetLocalPlayer().mana.RemoveFromPool(Mana.MANATYPE.ENERGY, totalPool);
        }
	}


	public void AddBlood(Object target, params object[] args) {
        if (Input.GetKey(keyEnemyModifier)) {
            Match.Get().GetEnemyPlayer().mana.AddToPool(Mana.MANATYPE.BLOOD);
        } else {
            Match.Get().GetLocalPlayer().mana.AddToPool(Mana.MANATYPE.BLOOD);
        }
	}
	public void AddAllBlood(Object target, params object[] args) {
        if (Input.GetKey(keyEnemyModifier)) {
            int totalMana = Match.Get().GetEnemyPlayer().mana.arMana[(int)Mana.MANATYPE.BLOOD];
            Match.Get().GetEnemyPlayer().mana.AddToPool(Mana.MANATYPE.BLOOD, totalMana);
        } else {
            int totalMana = Match.Get().GetLocalPlayer().mana.arMana[(int)Mana.MANATYPE.BLOOD];
            Match.Get().GetLocalPlayer().mana.AddToPool(Mana.MANATYPE.BLOOD, totalMana);
        }
	}
	public void RemoveBlood(Object target, params object[] args) {
        if (Input.GetKey(keyEnemyModifier)) {
            Match.Get().GetEnemyPlayer().mana.RemoveFromPool(Mana.MANATYPE.BLOOD);
        } else {
            Match.Get().GetLocalPlayer().mana.RemoveFromPool(Mana.MANATYPE.BLOOD);
        }
	}
	public void RemoveAllBlood(Object target, params object[] args) {
        if (Input.GetKey(keyEnemyModifier)) {
            int totalPool = Match.Get().GetEnemyPlayer().mana.arManaPool[(int)Mana.MANATYPE.BLOOD];
            Match.Get().GetEnemyPlayer().mana.RemoveFromPool(Mana.MANATYPE.BLOOD, totalPool);
        } else {
            int totalPool = Match.Get().GetLocalPlayer().mana.arManaPool[(int)Mana.MANATYPE.BLOOD];
            Match.Get().GetLocalPlayer().mana.RemoveFromPool(Mana.MANATYPE.BLOOD, totalPool);
        }
	}

	public void AddAll(Object target, params object[] args) {
		AddAllPhysical (target);
		AddAllMental (target);
		AddAllEnergy (target);
		AddAllBlood (target);
	}

	public void RemoveAll(Object target, params object[] args) {
		RemoveAllPhysical (target);
		RemoveAllMental (target);
		RemoveAllEnergy (target);
		RemoveAllBlood (target);
	}
}