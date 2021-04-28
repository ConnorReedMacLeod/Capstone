using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulChannelHydrasRegen : SoulChannel {

    public int nBaseHealing;
    public Healing heal;

    public SoulChannelHydrasRegen(Action _act) : base(_act) {

        nBaseHealing = 10;
        heal = new Healing(chrSource, null, nBaseHealing);


        lstTriggers = new List<Soul.TriggerEffect>() {
            new Soul.TriggerEffect() {
                 sub = ExecTurnEndTurn.subAllPostTrigger,
                 cb = (target, args) => {

                     //At the end of the turn, restore health to ourselves
                     ContAbilityEngine.PushSingleExecutable(new ExecHeal(chrSource, chrTarget, new Healing(heal)){
                         arSoundEffects = new SoundEffect[] { new SoundEffect("Sophidia/sndHydrasRegen", 3f) },
                         sLabel = "Growing another head"
                      });

                  }
             }
        };
    }

}
