using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionTranquilize : Action {

    public int nStunDuration;

    public ActionTranquilize(Chr _chrOwner) : base(0, _chrOwner) {//number of target arguments

        // arArgs[0] = new TargetArgTeam((own, tar) => true); 

        sName = "Tranquilize";
        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 1, 0, 0, 0 });

        nCd = 11;
        nFatigue = 3;
        nActionCost = 1;

        sDescription = "Deal 4 fatigue to the enemy blocker.";

        nStunDuration = 4;

        SetArgOwners();
    }

    override public void Execute() {

        Chr chrEnemyBlocker = chrSource.GetEnemyPlayer().GetBlocker();

        stackClauses.Push(new Clause() {
            fExecute = () => {

                //Stun the enemy blocker
                ContAbilityEngine.Get().AddExec(new ExecStun() {
                    chrSource = this.chrSource,
                    chrTarget = chrEnemyBlocker,
                    nAmount = nStunDuration,

                    fDelay = 1.0f,
                    sLabel = chrEnemyBlocker.sName + " is being stunned"
                });
            }
        });


        //NOTE:: Every Execute extension should begin with a typecast and end with a base.Execute call;
        base.Execute();
    }

}