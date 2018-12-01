using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionVenemousBite : Action {

    public ActionVenemousBite(Chr _chrOwner) : base(0, _chrOwner) {//number of target arguments

        //Since the base constructor initializes this array, we can start filling it
        //arArgs[0] = new TargetArgChr((own, tar) => own.plyrOwner != tar.plyrOwner); we don't have any targets

        sName = "VenemousBite";
        type = ActionType.ACTIVE;

        //Physical, Mental, Energy, Blood, Effort
        arCost = new int[] { 0, 0, 0, 1, 1 };

        nCd = 8;
        nFatigue = 3;
        nActionCost = 1;

        sDescription = "Deal 5 damage and apply [Envenomed](3) to the blocker.\n" +
                       "[Envenomed]: Lose 5 Life at the end of each turn.  +1 duration each time you take damage";

        SetArgOwners();
    }        

    override public void Execute() {

        Chr tar = chrSource.GetEnemyPlayer().GetBlocker();

        stackClauses.Push(new Clause() {
            fExecute = () => {
                Debug.Log("This VenemousBite Clause put an ExecDamage on the stack");
                Damage dmgToDeal = new Damage(chrSource, tar, 5);

                ContAbilityEngine.Get().AddExec(new ExecDealDamage() {
                    chrSource = this.chrSource,
                    chrTarget = tar,
                    dmg = dmgToDeal,
                    fDelay = 1.0f,
                    sLabel = tar.sName + " is being bitten"
                });
            }
        });

        stackClauses.Push(new Clause() {
            fExecute = () => {
                Debug.Log("Venemous Bite's second clause put an ExecApplySoul on the stack");
                ContAbilityEngine.Get().AddExec(new ExecApplySoul() {
                    chrSource = this.chrSource,
                    chrTarget = tar,

                    funcCreateSoul = (Chr _chrSource, Chr _chrTarget) => {
                        return new SoulEnvenomed(_chrSource, _chrTarget);
                    }
               
                });
            }
        });


        //NOTE:: Every Execute extension should begin with a typecast and end with a base.Execute call;

        base.Execute();
    }

}
