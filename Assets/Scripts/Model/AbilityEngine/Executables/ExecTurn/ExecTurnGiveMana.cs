using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecTurnGiveMana : Executable {

    public void GiveMana() {
        //TODO::Make this only semi-random
        //TODO:: Consider breaking this up into smaller executables
        Mana.MANATYPE manaGen = (Mana.MANATYPE)Random.Range(0, Mana.nManaTypes - 1);

        //Give the mana to each player
        for (int i = 0; i < Match.Get().nPlayers; i++) {
            Match.Get().arPlayers[i].mana.AddMana(manaGen);
        }
    }

    public override void Execute() {

        GiveMana();

        ContTurns.Get().SetTurnState(ContTurns.STATETURN.REDUCECOOLDOWNS);

        sLabel = "Giving Mana";
        fDelay = 1.0f;

        base.Execute();
    }
}
