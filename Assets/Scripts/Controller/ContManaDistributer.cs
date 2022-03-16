using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContManaDistributer : Singleton<ContManaDistributer> {
    
    public const int NRESERVESPERMANA = 3;
    //Don't need to distribute effort mana - hence the -1
    public const int NRESERVELENGTH = NRESERVESPERMANA * (Mana.nManaTypes - 1);

    public Mana.MANATYPE[,] arManaReserves = new Mana.MANATYPE[Player.MAXPLAYERS, NRESERVELENGTH];

    public int[] ariReserveProgression = new int[Player.MAXPLAYERS];

    public override void Init() {
        
    }

    // Start is called before the first frame update
    public void InitializeReserves() {
        
        for(int i=0; i<Player.MAXPLAYERS; i++) {
            //Init ReserveProgression to the start of the reserves for each player
            ariReserveProgression[i] = 0;

            //And initialize the contents of the reserves to be NRESERVESPERMANA copies of each mana type
            for(int j=0; j<NRESERVELENGTH; j++) {
                //so the mana type increases after every NRESERVESPERMANA iterations
                arManaReserves[i, j] = (Mana.MANATYPE)(j / NRESERVESPERMANA);
            }

            //Initially scramble that player's mana reserves
            RandomizePlayerReserves(i);
        }

    }

    public void RandomizePlayerReserves(int iPlayer) {

        Mana.MANATYPE swap;
        for (int i = 0; i < NRESERVELENGTH; i++) {
            swap = arManaReserves[iPlayer, i];
            //Note that we have to use a standardized randomization source so that every player will simulate with the same randomization
            int iRandomIndex = ContRandomization.Get().GetRandom(0, NRESERVELENGTH);
            arManaReserves[iPlayer, i] = arManaReserves[iPlayer, iRandomIndex];
            arManaReserves[iPlayer, iRandomIndex] = swap;
        }
    }

    public Mana.MANATYPE PeekNextMana(int iPlayer) {
        return arManaReserves[iPlayer, ariReserveProgression[iPlayer]];
    }

    public Mana.MANATYPE[] TakeNextMana() {
        Mana.MANATYPE[] arReturnMana = new Mana.MANATYPE[Player.MAXPLAYERS];

        for(int i=0; i<Player.MAXPLAYERS; i++) {
            arReturnMana[i] = TakeNextManaFromPlayer(i);
        }

        return arReturnMana;
    }

    public Mana.MANATYPE TakeNextManaFromPlayer(int iPlayer) {
        Mana.MANATYPE manaReturn = PeekNextMana(iPlayer);
        ariReserveProgression[iPlayer]++;

        //if we've advanced through all of our reserves
        if(ariReserveProgression[iPlayer] == NRESERVELENGTH) {

            //scramble their mana reserves
            RandomizePlayerReserves(iPlayer);

            //and reset the 'cursor' to the beginning
            ariReserveProgression[iPlayer] = 0;
        }

        return manaReturn;
    }
}
