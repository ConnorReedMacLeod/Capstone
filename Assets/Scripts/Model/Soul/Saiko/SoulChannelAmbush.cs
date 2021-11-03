using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulChannelAmbush : SoulChannel {

    public int nBaseDamage;
    public Damage dmg;

    public SoulChannelAmbush(Skill _skill) : base(_skill) {

        nBaseDamage = 20;
        dmg = new Damage(chrSource, null, nBaseDamage);

    }

    public override void InitTriggers() {

        lstTriggers = new List<SoulChr.TriggerEffect>() {
            new SoulChr.TriggerEffect() {
                sub = Chr.subAllPostExecuteSkill,
                cb = (target, args) => {

                    Chr chrStoredSelection = (Chr)(skillSource.typeUsage.GetUsedSelections()).lstSelections[1];

                    //If the character who used a skill is the one we targetted, and they are using a proper skill (not a rest with no proper skillslot)
                    // Then we can ambush them
                    if((Chr)target == chrStoredSelection && ((Skill)args[0]).skillslot != null){

                         ContSkillEngine.PushSingleExecutable(new ExecDealDamage(chrSource, chrStoredSelection, new Damage(dmg)){
                             arSoundEffects = new SoundEffect[] { new SoundEffect("Saiko/sndAmbush", 3.433f) },
                             sLabel = "Surprise!"
                         });
                    }

                }
            }
        };

    }


    public SoulChannelAmbush(SoulChannelAmbush other, Skill _skill) : base(other, _skill) {
        nBaseDamage = other.nBaseDamage;
        dmg = new Damage(other.dmg);
    }

    public override SoulChannel GetCopy(Skill _skill) {
        return new SoulChannelAmbush(this, _skill);
    }
}
