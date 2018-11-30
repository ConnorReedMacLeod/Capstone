using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBucklerParry : Action {

    public ActionBucklerParry(Chr _chrOwner) : base(0, _chrOwner) {//number of target arguments

        //Since the base constructor initializes this array, we can start filling it
        //arArgs[0] = new TargetArgChr((own, tar) => own.plyrOwner != tar.plyrOwner); //no targets needed

        sName = "BucklerParry";
        type = ActionType.ACTIVE;

        //Physical, Mental, Energy, Blood, Effort
        arCost = new int[] { 1, 0, 0, 0, 0 };

        nCd = 8;
        nFatigue = 2;
        nActionCost = 0;

        sDescription = "[CANTRIP] Gain 15 [ARMOUR] and [PARRY](4)\n" +
                       "[PARRY]: After the next time an enemy deals damage to Fischer, he deals 15 damage to them";

        SetArgOwners();
    }

    override public void Execute() {

        stackClauses.Push(new Clause() {
            fExecute = () => {
                Debug.Log("This Buckler Parry Clause put an ExecGainArmour on the stack");
                ContAbilityEngine.Get().AddExec(new ExecGainArmour() {
                    chrSource = this.chrSource,
                    chrTarget = this.chrSource,
                    nArmour = 15,
                    fDelay = 1.0f,
                    sLabel = this.chrSource.sName + " is gaining armour"
                });
            }
        });

        stackClauses.Push(new Clause() {
            fExecute = () => {
                Debug.Log("Buckler Parry's second clause put an ExecApplySoul on the stack");
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