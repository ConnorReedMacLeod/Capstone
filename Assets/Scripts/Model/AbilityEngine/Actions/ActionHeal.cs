using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionHeal : Action {



    public ActionHeal(Chr _chrOwner) : base(1, _chrOwner) {//number of target arguments

        //Since the base constructor initializes this array, we can start filling it
        arArgs[0] = new TargetArgAlly((own, tar) => true);

        sName = "Heal (Cantrip)";
        type = ActionTypes.TYPE.ACTIVE;

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 0, 1, 0 });

        nCd = 3;
        nFatigue = 3;
        nActionCost = 0;

        sDescription = "Restore 5 health to target ally";


        SetArgOwners();
    }

    override public void Execute() {
        //It's a bit awkward that you have to do this typecasting, 
        // but at least it's eliminated from the targetting lambda
        Chr tar = ((TargetArgAlly)arArgs[0]).chrTar;

        stackClauses.Push(new Clause() {
            fExecute = () => {
                Debug.Log("This Heal Clause put an ExecHeal on the stack");
                ContAbilityEngine.Get().AddExec(new ExecHeal() {
                    chrSource = this.chrSource,
                    chrTarget = tar,
                    nAmount = 10,
                    fDelay = 1.0f,
                    sLabel = "Healing"
                });
            }
        });

        //NOTE:: Every Execute extension should begin with a typecast and end with a base.Execute call;

        base.Execute();
    }

}
