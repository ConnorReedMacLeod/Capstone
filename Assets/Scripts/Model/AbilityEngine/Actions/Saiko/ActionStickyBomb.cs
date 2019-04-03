using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionStickyBomb : Action {

    public Damage dmg;
    public int nBaseDamage;

    public ActionStickyBomb(Chr _chrOwner) : base(1, _chrOwner) {//number of target arguments

        //We don't need to target anything, since we always deal damage to everyone
        arArgs[0] = new TargetArgChr(Action.IsEnemy); 

        sName = "StickyBomb";
        sDisplayName = "Sticky Bomb";

        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 1, 0, 0, 1 });

        nCd = 6;
        nFatigue = 3;
        nActionCost = 1;

        nBaseDamage = 5;
        //Create a base Damage object that this action will apply
        dmg = new Damage(this.chrSource, null, nBaseDamage);

        sDescription1 = "Deal 5 damage to the chosen character and apply STICKIED";
		sDescription2 = "[STICKIED]\n" + "At the end of turn, deal 30 damage to this character and dispel.";

        

        SetArgOwners();
    }

    override public void Execute(int[] lstTargettingIndices) {
        //Cast the first target to be a character
        Chr tar = Chr.GetTargetByIndex(lstTargettingIndices[0]);

        stackClauses.Push(new Clause() {
            fExecute = () => {

                //Make a copy of the damage object to give to the executable
                Damage dmgToApply = new Damage(dmg);
                //Give the damage object its target
                dmgToApply.SetChrTarget(tar);

                ContAbilityEngine.Get().AddExec(new ExecDealDamage() {
                    chrSource = this.chrSource,
                    chrTarget = tar,
                    dmg = dmgToApply,

                    arSoundEffects = new SoundEffect[] { new SoundEffect("Saiko/sndBombToss", 2.133f) },

                    fDelay = ContTurns.fDelayStandard,
                    sLabel = tar.sName + " got a bomb thrown at them"
                });

                //Apply the stickybomb Soul effect to the target
                ContAbilityEngine.Get().AddExec(new ExecApplySoul() {
                    chrSource = this.chrSource,
                    chrTarget = tar,

                    funcCreateSoul = (Chr _chrSource, Chr _chrTarget) => {
                        return new SoulStickyBomb(_chrSource, _chrTarget, this);
                    },

                    fDelay = ContTurns.fDelayStandard,
                    sLabel = tar.sName + " is stuck with the bomb"

                });
            }
        });

    }

}