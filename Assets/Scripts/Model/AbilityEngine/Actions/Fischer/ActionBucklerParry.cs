using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBucklerParry : Action {

    public ActionBucklerParry(Chr _chrOwner) : base(0, _chrOwner) {//number of target arguments

        //Since the base constructor initializes this array, we can start filling it
        //arArgs[0] = new TargetArgChr((own, tar) => own.plyrOwner != tar.plyrOwner); //no targets needed

        sName = "BucklerParry";
        sDisplayName = "Buckler Parry";

        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 1, 0, 0, 0, 0 });

        nCd = 8;
        nFatigue = 2;
        nActionCost = 0;


		sDescription1 = "Gain 15 DEFENSE and PARRY (4).";
		sDescription2 = "[PARRY]\n" + "When an enemy would deal damage to " + chrSource.sName + ", deal 15 damage to them and dispel.";

        SetArgOwners();
    }

    override public void Execute(int[] lstTargettingIndices) {

        stackClauses.Push(new Clause() {
            fExecute = () => {
                ContAbilityEngine.Get().AddExec(new ExecApplySoul() {
                    chrSource = this.chrSource,
                    chrTarget = this.chrSource,

                    funcCreateSoul = (Chr _chrSource, Chr _chrTarget) => {
                        return new SoulParry(_chrSource, _chrTarget, this);
                    }

                });
            }
        });

    }

}