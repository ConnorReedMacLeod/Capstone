using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBucklerParry : Action {

    public ActionBucklerParry(Chr _chrOwner) : base(0, _chrOwner) {//number of target arguments

        //Since the base constructor initializes this array, we can start filling it
        //arArgs[0] = new TargetArgChr((own, tar) => own.plyrOwner != tar.plyrOwner); //no targets needed

        sName = "BucklerParry";
        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 1, 0, 0, 0, 0 });

        nCd = 8;
        nFatigue = 2;
        nActionCost = 0;

        sDescription = "[CANTRIP] Gain [PARRY](4)\n" +
                       "[PARRY]: " + sName + " has +15[ARMOUR].  After the next time an enemy deals damage to Fischer, he deals 15 damage to them";

        SetArgOwners();
    }

    override public void Execute() {

        stackClauses.Push(new Clause() {
            fExecute = () => {
                ContAbilityEngine.Get().AddExec(new ExecApplySoul() {
                    chrSource = this.chrSource,
                    chrTarget = this.chrSource,

                    funcCreateSoul = (Chr _chrSource, Chr _chrTarget) => {
                        return new SoulParry(_chrSource, _chrTarget);
                    }

                });
            }
        });


        //NOTE:: Every Execute extension should begin with a typecast and end with a base.Execute call;

        base.Execute();
    }

}