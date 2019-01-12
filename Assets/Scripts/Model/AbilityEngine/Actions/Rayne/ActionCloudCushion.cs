using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionCloudCushion : Action {

    public int nDefenseGain;
    public int nDuration;

    public ActionCloudCushion(Chr _chrOwner) : base(1, _chrOwner) {//number of target arguments

        //Since the base constructor initializes this array, we can start filling it
        arArgs[0] = new TargetArgAlly((own, tar) => true); //Choose any friendly character

        sName = "CloudCushion";
        type = ActionTypes.TYPE.ACTIVE;

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 1, 0, 0 });

        nCd = 7;
        nFatigue = 1;
        nActionCost = 1;

        nDefenseGain = 25;
        nDuration = 4;

        sDescription = "Target ally gains 25 [DEFENSE] for 4 turns";

        SetArgOwners();
    }

    override public void Execute() {

        Chr chrTarget = ((TargetArgChr)arArgs[0]).chrTar; //Cast our first target to a ChrTarget and get that Chr

        stackClauses.Push(new Clause() {
            fExecute = () => {
                ContAbilityEngine.Get().AddExec(new ExecApplySoul() {
                    chrSource = this.chrSource,
                    chrTarget = chrTarget,

                    funcCreateSoul = (Chr _chrSource, Chr _chrTarget) => {
                        return new SoulCloudCushion(_chrSource, _chrTarget);
                    }
                });
            }
        });


        //NOTE:: Every Execute extension should begin with a typecast and end with a base.Execute call;

        base.Execute();
    }
}