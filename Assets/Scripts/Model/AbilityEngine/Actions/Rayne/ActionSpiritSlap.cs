using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSpiritSlap : Action {

    public ActionSpiritSlap(Chr _chrOwner) : base(0, _chrOwner) {//number of target arguments

        //Since the base constructor initializes this array, we can start filling it
        //arArgs[0] = new TargetArgChr((own, tar) => own.plyrOwner != tar.plyrOwner); we don't have any targets

        sName = "SpiritSlap";
        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 1, 0, 0, 0 });

        nCd = 0;
        nFatigue = 2;
        nActionCost = 1;

        sDescription = "Deal 5 damage to the enemy blocker and apply [Dispirited](4).\n" +
                       "[Dispirited]: This character's cantrips cost [O] more.";

        SetArgOwners();
    }

    override public void Execute() {

        Chr tar = chrSource.GetEnemyPlayer().GetBlocker();

        stackClauses.Push(new Clause() {
            fExecute = () => {
                Damage dmgToDeal = new Damage(chrSource, tar, 5);

                ContAbilityEngine.Get().AddExec(new ExecDealDamage() {
                    chrSource = this.chrSource,
                    chrTarget = tar,
                    dmg = dmgToDeal,
                    fDelay = 1.0f,
                    sLabel = tar.sName + " is being slapped"
                });
            }
        });

        stackClauses.Push(new Clause() {
            fExecute = () => {
                ContAbilityEngine.Get().AddExec(new ExecApplySoul() {
                    chrSource = this.chrSource,
                    chrTarget = tar,

                    funcCreateSoul = (Chr _chrSource, Chr _chrTarget) => {
                        return new SoulDispirited(_chrSource, _chrTarget);
                    },

                    fDelay = 1.0f,
                    sLabel = tar.sName + "'s soul is drained"

                });
            }
        });


        //NOTE:: Every Execute extension should begin with a typecast and end with a base.Execute call;

        base.Execute();
    }

}
