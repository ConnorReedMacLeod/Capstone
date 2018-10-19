using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Mana))]
public class ViewMana : MonoBehaviour {

	bool bStarted;                          //Confirms the Start() method has executed

	public Text txtManaPhysical;
	public Text txtManaMental;
	public Text txtManaEnergy;
	public Text txtManaBlood;

	public Text txtManaEffort;

	public Text txtManaEffortPhysical;
	public Text txtManaEffortMental;
	public Text txtManaEffortEnergy;
	public Text txtManaEffortBlood;

	public Mana mod;                   //Character model

	// Use this for initialization
	public void Start() {
		if (bStarted == false) {
			bStarted = true;
			InitModel();

            mod.subManaChange.Subscribe(cbManaChange);
            mod.subManaPoolChange.Subscribe(cbManaPoolChange);
		}
	}

	public void InitModel() {
		mod = GetComponent<Mana>();
	}

    public void DisplayMana(Text _txtMana, int _nMana) {
        if (_nMana == 0) {
            _txtMana.text = "";
        } else { 
            _txtMana.text = _nMana.ToString();
        }
    }

    public void cbManaChange(Object target, params object[] args) {
        switch ((Mana.MANATYPE)args[0]) {
            case Mana.MANATYPE.PHYSICAL:
                DisplayMana(txtManaPhysical, mod.arMana[(int)Mana.MANATYPE.PHYSICAL]);
                break;
            case Mana.MANATYPE.MENTAL:
                DisplayMana(txtManaMental, mod.arMana[(int)Mana.MANATYPE.MENTAL]);
                break;
            case Mana.MANATYPE.ENERGY:
                DisplayMana(txtManaEnergy, mod.arMana[(int)Mana.MANATYPE.ENERGY]);
                break;
            case Mana.MANATYPE.BLOOD:
                DisplayMana(txtManaBlood, mod.arMana[(int)Mana.MANATYPE.BLOOD]);
                break;
        }
    }

    public void cbManaPoolChange(Object target, params object[] args) {
        DisplayMana(txtManaEffort, mod.nManaPool);

        switch ((Mana.MANATYPE)args[0]) {
            case Mana.MANATYPE.PHYSICAL:
                DisplayMana(txtManaEffortPhysical, mod.arManaPool[(int)Mana.MANATYPE.PHYSICAL]);
                break;
            case Mana.MANATYPE.MENTAL:
                DisplayMana(txtManaEffortMental, mod.arManaPool[(int)Mana.MANATYPE.MENTAL]);
                break;
            case Mana.MANATYPE.ENERGY:
                DisplayMana(txtManaEffortEnergy, mod.arManaPool[(int)Mana.MANATYPE.ENERGY]);
                break;
            case Mana.MANATYPE.BLOOD:
                DisplayMana(txtManaEffortBlood, mod.arManaPool[(int)Mana.MANATYPE.BLOOD]);
                break;
        }
    }
}
