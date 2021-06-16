using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulChannelAmbush : SoulChannel {

    public int nBaseDamage;
    public Damage dmg;

    public SoulChannelAmbush(Skill _skill) : base(_skill) {

        nBaseDamage = 20;
        dmg = new Damage(chrSource, null, nBaseDamage);

        InitTriggers();
    }

    public override void InitTriggers() {

        lstTriggers = new List<Soul.TriggerEffect>() {
            new Soul.TriggerEffect() {
                sub = Chr.subAllPostExecuteSkill,
                cb = (target, args) => {

                    Chr chrStoredSelection = ((SelectionSerializer.SelectionChr)skillSource.type.GetSelectionInfo()).chrSelected;

                    //If the character who used a skill is the one we targetted, and they are using a proper skill (not a rest with no proper skillslot)
                    // Then we can ambush them
                    if((Chr)target == chrStoredSelection && ((Skill)args[0]).skillslot != null){

                         ContSkillEngine.PushSingleExecutable(new ExecDealDamage(chrSource, chrTarget, new Damage(dmg)){
                             arSoundEffects = new SoundEffect[] { new SoundEffect("Saiko/sndAmbush", 3.433f) },
                             sLabel = "Surprise!"
                         });
                    }

                }
            }
        };
    }
}
