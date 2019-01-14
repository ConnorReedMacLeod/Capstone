using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionHuntersQuarry : Action {

    public ActionHuntersQuarry(Chr _chrOwner) : base(1, _chrOwner) {//number of target arguments

        //Since the base constructor initializes this array, we can start filling it
        arArgs[0] = new TargetArgChr((own, tar) => own.plyrOwner != tar.plyrOwner); //Choose an enemy character

        sName = "HuntersQuarry";
        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 0, 0, 0 });

        nCd = 8;
        nFatigue = 3;
        nActionCost = 1;

        sDescription = "Before " + _chrOwner.sName + " deals damage to this character, they lose 5 Defense until end of turn";

        SetArgOwners();
    }

    override public void Execute() {

        Chr tar = ((TargetArgChr)arArgs[0]).chrTar; //Cast our first target to a ChrTarget and get that Chr

        stackClauses.Push(new Clause() {
            fExecute = () => {
                ContAbilityEngine.Get().AddExec(new ExecApplySoul() {
                    chrSource = this.chrSource,
                    chrTarget = tar,

                    funcCreateSoul = (Chr _chrSource, Chr _chrTarget) => {
                        return new SoulHunted(_chrSource, _chrTarget);
                    }

                });
            }
        });


        //NOTE:: Every Execute extension should begin with a typecast and end with a base.Execute call;

        base.Execute();
    }

}
