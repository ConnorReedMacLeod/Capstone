using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionReverberate : Action {

    public Damage dmg;
    public int nBaseDamage;

    public ActionReverberate(Chr _chrOwner) : base(0, _chrOwner) {//number of target arguments

        //We don't need to target anything, since we always deal damage to the enemy team
        // arArgs[0] = new TargetArgTeam((own, tar) => true); 

        sName = "Reverberate";
        sDisplayName = "Reverberate";

        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 0, 0, 0 });

        nCd = 8;
        nFatigue = 4;
        nActionCost = 1;

        nBaseDamage = 5;
        //Create a base Damage object that this action will apply
        dmg = new Damage(this.chrSource, null, nBaseDamage);

        sDescription1 = "Deal 5 damage to all characters on the chosen team.";

        SetArgOwners();
    }

    override public void Execute(int[] lstTargettingIndices) {

        Player tar = chrSource.GetEnemyPlayer();

        stackClauses.Push(new Clause() {
            fExecute = () => {
                for (int i = 0; i < tar.arChr.Length; i++) {

                    //Don't target dead characters
                    if (tar.arChr[i].bDead) continue;

                    //Make a copy of the damage object to give to the executable
                    Damage dmgToApply = new Damage(dmg);
                    //Give the damage object its target
                    dmgToApply.SetChrTarget(tar.arChr[i]);

                    //TODO:: Organize this in the correct order
                    ContAbilityEngine.Get().AddExec(new ExecDealDamage() {
                        chrSource = this.chrSource,
                        chrTarget = tar.arChr[i],

                        dmg = dmgToApply,
                        
                        arSoundEffects = new SoundEffect[] { new SoundEffect("Katarina/sndReverberate", 1.633f) },

                        fDelay = ContTurns.fDelayNone,
                        sLabel = "Reverberating " + tar.arChr[i].sName
                    });
                }
            }
        });
    }

}