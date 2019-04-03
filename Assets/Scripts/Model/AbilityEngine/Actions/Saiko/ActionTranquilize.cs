using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionTranquilize : Action {

    public int nStunDuration;

    public ActionTranquilize(Chr _chrOwner) : base(0, _chrOwner) {//number of target arguments

        // arArgs[0] = new TargetArgTeam((own, tar) => true); 

        sName = "Tranquilize";
        sDisplayName = "Tranquilize";

        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 1, 0, 0, 0 });

        nCd = 11;
        nFatigue = 3;
        nActionCost = 1;

        sDescription1 = "Deal 4 fatigue to the enemy Vanguard.";

        nStunDuration = 4;

        SetArgOwners();
    }

    override public void Execute(int[] lstTargettingIndices) {

        Chr chrEnemyBlocker = chrSource.GetEnemyPlayer().GetBlocker();

        stackClauses.Push(new Clause() {
            fExecute = () => {

                //Stun the enemy blocker
                ContAbilityEngine.Get().AddExec(new ExecStun() {
                    chrSource = this.chrSource,
                    chrTarget = chrEnemyBlocker,

                    GetDuration = () => nStunDuration,

                    arSoundEffects = new SoundEffect[] { new SoundEffect("Saiko/sndTranquilize", 1.4f) },

                    fDelay = ContTurns.fDelayStandard,
                    sLabel = chrEnemyBlocker.sName + " is being stunned"
                });
            }
        });

    }

}