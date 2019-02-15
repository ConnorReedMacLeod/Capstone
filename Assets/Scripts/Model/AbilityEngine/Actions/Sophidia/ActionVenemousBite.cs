using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionVenemousBite : Action {

    public Damage dmg;
    public int nBaseDamage;

    public ActionVenemousBite(Chr _chrOwner) : base(0, _chrOwner) {//number of target arguments

        //Since the base constructor initializes this array, we can start filling it
        //arArgs[0] = new TargetArgChr((own, tar) => own.plyrOwner != tar.plyrOwner); we don't have any targets

        sName = "VenemousBite";
        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 0, 1, 0 });

        nCd = 8;
        nFatigue = 3;
        nActionCost = 1;

        nBaseDamage = 5;
        //Create a base Damage object that this action will apply
        dmg = new Damage(this.chrSource, null, nBaseDamage);

        sDescription = "Deal 5 damage and apply [Envenomed](3) to the blocker.\n" +
                       "[Envenomed]: Lose 5 Life at the end of each turn.  +1 duration each time you take damage";

        SetArgOwners();
    }        

    override public void Execute() {

        Chr tar = chrSource.GetEnemyPlayer().GetBlocker();

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
                    fDelay = ContTurns.fDelayStandard,
                    sLabel = tar.sName + " is being bitten"
                });
            }
        });

        stackClauses.Push(new Clause() {
            fExecute = () => {
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
