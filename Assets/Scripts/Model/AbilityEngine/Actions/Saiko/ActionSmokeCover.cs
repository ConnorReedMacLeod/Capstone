using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSmokeCover : Action {

    public ActionSmokeCover(Chr _chrOwner) : base(0, _chrOwner) {//number of target arguments

        //Note that we don't have any targets for this ability
        //arArgs[0] = new TargetArgChr((own, tar) => own.plyrOwner != tar.plyrOwner);

        sName = "SmokeCover";
        sDisplayName = "Smoke Cover";

        type = new TypeCantrip(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 1, 0, 0, 0 });

        nCd = 10;
        nFatigue = 2;
        nActionCost = 0;//0 since this is a cantrip

		sDescription1 = "Gain SHROUDED (4).";
		sDescription2 = "[SHROUDED]\n" + _chrOwner.sName + " is immune to damage.  If " + _chrOwner.sName + " is the Vanguard, dispel.";

        SetArgOwners();
    }


    override public void Execute(int[] lstTargettingIndices) {

        stackClauses.Push(new Clause() {
            fExecute = () => {
                
                ContAbilityEngine.Get().AddExec(new ExecApplySoul() {
                    chrSource = this.chrSource,
                    chrTarget = this.chrSource,

                    funcCreateSoul = (Chr _chrSource, Chr _chrTarget) => {

                        return new SoulSmokeCover(_chrSource, _chrTarget, this);

                    },

                    arSoundEffects = new SoundEffect[] { new SoundEffect("Saiko/sndSmokeCover", 4.3f) },

                    fDelay = ContTurns.fDelayStandard,
                    sLabel = "Applying SmokeCover"
                });
            }
        });

    }
}
