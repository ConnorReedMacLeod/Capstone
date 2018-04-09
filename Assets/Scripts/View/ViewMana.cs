using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Mana))]
public class ViewMana : Observer {

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

	public GameObject[] arManaText;

	public Mana mod;                   //Character model

	// Use this for initialization
	public void Start() {
		if (bStarted == false) {
			bStarted = true;
			Init();
			InitModel();
		}
	}

	//Variable initialization
	public void Init() {
		arManaText = GameObject.FindGameObjectsWithTag("Mana");

		for (int i = 0; i < arManaText.Length; i++) {
			Text txtIndex = arManaText[i].GetComponent<Text>();
			if (txtIndex == null) {
				Debug.LogError("Mana tagged component w/o Text");
			} else {
				switch (arManaText[i].name) {
					case "txtPhysical":
						txtManaPhysical = txtIndex;
						break;

					case "txtMental":
						txtManaMental = txtIndex;
						break;

					case "txtEnergy":
						txtManaEnergy = txtIndex;
						break;

					case "txtBlood":
						txtManaBlood = txtIndex;
						break;

					case "txtEffort":
						txtManaEffort = txtIndex;
						break;

					case "txtEffortPhysical":
						txtManaEffortPhysical = txtIndex;
						break;

					case "txtEffortMental":
						txtManaEffortMental = txtIndex;
						break;

					case "txtEffortEnergy":
						txtManaEffortEnergy = txtIndex;
						break;

					case "txtEffortBlood":
						txtManaEffortBlood = txtIndex;
						break;
				}

			}
		}
	}

	public void InitModel() {
		mod = GetComponent<Mana>();
		mod.Subscribe(this);
	}

	public void DisplayMana(Text _txtMana, int _nMana) {
		_txtMana.text = _nMana.ToString();
	}


	//Updates mana view, detecting if changes are needed to the values
	override public void UpdateObs(string eventType, Object target, params object[] args) {

		switch (eventType) {
			case "ManaChange":
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
					/*case Mana.MANATYPE.EFFORT:
						DisplayMana(txtManaEffort, mod.arMana[(int)Mana.MANATYPE.EFFORT]);
						break;*/
				}
				break;

			case "ManaPoolChange":

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
				break;
		}
	}
}
