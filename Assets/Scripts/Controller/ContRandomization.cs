using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Maintains the randomization generator to use for *simulated* randomization events
public class ContRandomization : Singleton<ContRandomization> {

    System.Random randGenerator;

    public void InitGenerator(int nSeed) {
        Debug.LogFormat("Initializing the randomization generator with seed {0}", nSeed);
        randGenerator = new System.Random(nSeed);
    }
    
    public int GetRandom(int nMin, int nMax) {
        return randGenerator.Next(nMin, nMax);
    }

    public int GetRandom() {
        return randGenerator.Next();
    }

    public override void Init() {
    }
}
