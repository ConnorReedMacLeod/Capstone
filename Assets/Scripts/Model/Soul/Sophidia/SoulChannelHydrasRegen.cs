using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulChannelHydrasRegen : SoulChannel {

    public int nBaseHealing;
    public Healing heal;

    public SoulChannelHydrasRegen(Skill _skill) : base(_skill) {

        nBaseHealing = 10;
        heal = new Healing(chrSource, null, nBaseHealing);

    }


    public override void InitTriggers() {

        lstTriggers = new List<SoulChr.TriggerEffect>() {
            new SoulChr.TriggerEffect() {
                 sub = ExecTurnEndTurn.subAllPostTrigger,
                 cb = (target, args) => {

                     //At the end of the turn, restore health to ourselves
                     ContSkillEngine.PushSingleExecutable(new ExecHeal(chrSource, chrTarget, new Healing(heal)){
                         arSoundEffects = new SoundEffect[] { new SoundEffect("Sophidia/sndHydrasRegen", 3f) },
                         sLabel = "Growing another head"
                      });

                  }
             }
        };
    }

    public SoulChannelHydrasRegen(SoulChannelHydrasRegen other, Skill _skill) : base(other, _skill) {
        nBaseHealing = other.nBaseHealing;
        heal = new Healing(other.heal);
    }

    public override SoulChannel GetCopy(Skill _skill) {
        return new SoulChannelHydrasRegen(this, _skill);
    }

}
