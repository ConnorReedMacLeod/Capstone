using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionFortissimo : Action {

    public ActionFortissimo(Chr _chrOwner) : base(0, _chrOwner) {//number of target arguments

        //Note that we don't have any targets for this ability
        //arArgs[0] = new TargetArgChr((own, tar) => own.plyrOwner != tar.plyrOwner);

        sName = "Fortissimo";
        sDisplayName = "Fortissimo";

        type = new TypeCantrip(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 0, 0, 0 });

        nCd = 8;
        nFatigue = 0;
        nActionCost = 0;//0 since this is a cantrip

        sDescription1 = "Gain 10 POWER and 10 DEFENSE for 4 turns.";

        SetArgOwners();
    }


    override public void Execute(int[] lstTargettingIndices) {

        stackClauses.Push(new Clause() {
            fExecute = () => {
                ContAbilityEngine.Get().AddExec(new ExecApplySoul() {
                    chrSource = this.chrSource,
                    chrTarget = this.chrSource,

                    funcCreateSoul = (Chr _chrSource, Chr _chrTarget) => {

                        return new SoulFortissimo(_chrSource, _chrTarget, this);

                    },

                    arSoundEffects = new SoundEffect[] { new SoundEffect("Katarina/sndFortissimo", 6.2f) },

                    fDelay = ContTurns.fDelayStandard,
                    sLabel = "Applying Fortissimo"
                });
            }
        });
    }
}
