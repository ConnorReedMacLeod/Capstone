using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterManaDistributer : MonoBehaviour {
    
    public const int NRESERVESPERMANA = 3;
    public const int NRESERVELENGTH = NRESERVESPERMANA * (Mana.nManaTypes - 1);

    //Don't need to distribute effort mana - hence the -1
    public int[,] arnManaReserves = new int[Player.MAXPLAYERS, NRESERVELENGTH];

    public int[] ariReserveProgression = new int[Player.MAXPLAYERS];

    // Start is called before the first frame update
    void Start() {
        
        for(int i=0; i<Player.MAXPLAYERS; i++) {
            //Init ReserveProgression to the start of the reserves for each player
            ariReserveProgression[i] = 0;

            //And initialize the contents of the reserves to be NRESERVESPERMANA copies of each mana type
            for(int j=0; j<NRESERVELENGTH; j++) {
                //so the mana type increases after every NRESERVESPERMANA iterations
                arnManaReserves[i, j] = j / NRESERVESPERMANA;
            }

            //Initially scramble that player's mana reserves
            RandomizePlayerReserves(i);
        }

    }

    public void RandomizePlayerReserves(int iPlayer) {

        int nSwap;
        for (int i = 0; i < NRESERVELENGTH; i++) {
            nSwap = arnManaReserves[iPlayer, i];
            int iRandomIndex = Random.Range(0, NRESERVELENGTH);
            arnManaReserves[iPlayer, i] = arnManaReserves[iPlayer, iRandomIndex];
            arnManaReserves[iPlayer, iRandomIndex] = nSwap;
        }
    }

    public int PeekNextMana(int iPlayer) {
        return arnManaReserves[iPlayer, ariReserveProgression[iPlayer]];
    }

    public int[] TakeNextMana() {
        int[] arnReturnMana = new int[Player.MAXPLAYERS];

        for(int i=0; i<Player.MAXPLAYERS; i++) {
            arnReturnMana[i] = TakeNextManaFromPlayer(i);
        }

        return arnReturnMana;
    }

    public int TakeNextManaFromPlayer(int iPlayer) {
        int nReturnMana = PeekNextMana(iPlayer);
        ariReserveProgression[iPlayer]++;

        //if we've advanced through all of our reserves
        if(ariReserveProgression[iPlayer] == NRESERVELENGTH) {

            //scramble their mana reserves
            RandomizePlayerReserves(iPlayer);

            //and reset the 'cursor' to the beginning
            ariReserveProgression[iPlayer] = 0;
        }

        return nReturnMana;
    }
}
