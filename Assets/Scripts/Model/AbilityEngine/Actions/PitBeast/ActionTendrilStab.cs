using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionTendrilStab : Action {

    public ActionTendrilStab(Chr _chrOwner) : base(0, _chrOwner) {//number of target arguments

    //Since the base constructor initializes this array, we can start filling it
    //arArgs[0] = new TargetArgChr((own, tar) => own.plyrOwner != tar.plyrOwner); we don't have any targets

    sName = "TendrilStab";
    type = ActionType.ACTIVE;

    //Physical, Mental, Energy, Blood, Effort
    arCost = new int[] { 1, 0, 0, 0, 0 };

    nCd = 6;
    nFatigue = 3;
    nActionCost = 1;

    sDescription = "Deal 25 [PIERCING] damage to the enemy blocker";

    SetArgOwners();
}

override public void Execute() {

    Chr tar = chrSource.GetEnemyPlayer().GetBlocker();

    stackClauses.Push(new Clause() {
        fExecute = () => {
            Debug.Log("This Tendril Stab Clause put an ExecDamage on the stack");
            Damage dmgToDeal = new Damage(chrSource, tar, 25, false, true);

            ContAbilityEngine.Get().AddExec(new ExecDealDamage() {
                chrSource = this.chrSource,
                chrTarget = tar,
                dmg = dmgToDeal,

                fDelay = 1.0f,
                sLabel = tar.sName + " is being stabbed"
            });
        }
    });


    //NOTE:: Every Execute extension should begin with a typecast and end with a base.Execute call;

    base.Execute();
}

}
