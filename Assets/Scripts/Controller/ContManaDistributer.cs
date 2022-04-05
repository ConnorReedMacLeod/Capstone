using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContManaDistributer : Singleton<ContManaDistributer> {

    public const int NRANDOMRESERVESPERMANA = 3;
    //Don't need to distribute effort mana - hence the -1
    public const int NRANDOMRESERVELENGTH = NRANDOMRESERVESPERMANA * (Mana.nManaTypes - 1);

    public Mana.MANATYPE[,] arManaRandomReserves = new Mana.MANATYPE[Match.NPLAYERS, NRANDOMRESERVELENGTH];

    public int[] ariRandomReserveProgression = new int[Match.NPLAYERS];

    public override void Init() {

    }

    public void InitializeRandomReserves() {

        for(int i = 0; i < ariRandomReserveProgression.Length; i++) {
            //Init RandomReserveProgression to the start of the reserves for each player
            ariRandomReserveProgression[i] = 0;

            //And initialize the contents of the reserves to be NRANDOMRESERVESPERMANA copies of each mana type
            for(int j = 0; j < NRANDOMRESERVELENGTH; j++) {
                //so the mana type increases after every NRANDOMRESERVESPERMANA iterations
                arManaRandomReserves[i, j] = (Mana.MANATYPE)(j / NRANDOMRESERVESPERMANA);
            }

            //Initially scramble that player's mana reserves
            RandomizePlayerReserves(i);
        }

    }

    public void RandomizePlayerReserves(int iPlayer) {

        Mana.MANATYPE swap;
        for(int i = 0; i < NRANDOMRESERVELENGTH; i++) {
            swap = arManaRandomReserves[iPlayer, i];
            //Note that we have to use a standardized randomization source so that every player will simulate with the same randomization
            int iRandomIndex = ContRandomization.Get().GetRandom(0, NRANDOMRESERVELENGTH);
            arManaRandomReserves[iPlayer, i] = arManaRandomReserves[iPlayer, iRandomIndex];
            arManaRandomReserves[iPlayer, iRandomIndex] = swap;
        }
    }

    public Mana.MANATYPE PeekNextMana(int iPlayer) {
        return arManaRandomReserves[iPlayer, ariRandomReserveProgression[iPlayer]];
    }

    public Mana.MANATYPE GetNextRandomManaForPlayer(int iPlayer) {
        Mana.MANATYPE manaReturn = PeekNextMana(iPlayer);
        ariRandomReserveProgression[iPlayer]++;

        //if we've advanced through all of our reserves
        if(ariRandomReserveProgression[iPlayer] == NRANDOMRESERVELENGTH) {

            //scramble their mana reserves
            RandomizePlayerReserves(iPlayer);

            //and reset the 'cursor' to the beginning
            ariRandomReserveProgression[iPlayer] = 0;
        }

        return manaReturn;
    }

    public Mana GetCurrentTurnStartManaForPlayer(Player plyr) {

        ManaDate manadateCur = plyr.manacalendar.GetCurrentManaDate();

        //Start with a base of any coloured mana we'll be scheduled to give the player
        Mana manaToGive = new Mana(manadateCur.pmanaScheduled.Get());

        //Add a random mana type (dictated by the random reserves) for each effort mana we're scheduled to recieve
        for(int i = manadateCur.pmanaScheduled.Get().arMana[(int)Mana.MANATYPE.EFFORT]; i > 0; i--) {
            manaToGive.ChangeMana(GetNextRandomManaForPlayer(plyr.id));
        }

        //Clear out the effort mana now that it's been distributed as randomized coloured mana
        manaToGive.ChangeMana(Mana.MANATYPE.EFFORT, -manaToGive.arMana[(int)Mana.MANATYPE.EFFORT]);

        return manaToGive;
    }
}
