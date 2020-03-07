using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulChannelAmbush : SoulChannel {

    public int nBaseDamage;
    public Damage dmg;

    public SoulChannelAmbush(Action _act) : base(_act) {

        nBaseDamage = 20;
        dmg = new Damage(chrSource, null, nBaseDamage);

        lstTriggers = new List<Soul.TriggerEffect>() {
            new Soul.TriggerEffect() {
                sub = Chr.subAllPostExecuteAbility,
                cb = (target, args) => {

                    Chr chrStoredSelection = ((SelectionSerializer.SelectionChr)act.type.GetSelectionInfo()).chrSelected;

                    //If the character who used an ability is the one we targetted, and they are using a proper action (not a rest)
                    // Then we can ambush them
                    if((Chr)target == chrStoredSelection && ((Action)args[0]).bProperActive == true){

                         ContAbilityEngine.PushSingleExecutable(new ExecDealDamage(chrSource, chrTarget, new Damage(dmg)){
                             arSoundEffects = new SoundEffect[] { new SoundEffect("Saiko/sndAmbush", 3.433f) },
                             sLabel = "Surprise!"
                         });
                    }

                }
            }
        };
    }
}
