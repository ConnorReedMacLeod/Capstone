﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSerenade : Action {

    public Healing heal;
    public int nBaseHealing;

    public ActionSerenade(Chr _chrOwner) : base(1, _chrOwner) {//number of target arguments

        //Since the base constructor initializes this array, we can start filling it
        arArgs[0] = new TargetArgAlly((own, tar) => true);

        sName = "Serenade";
        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 1, 0, 0 });

        nCd = 8;
        nFatigue = 4;
        nActionCost = 1;

        nBaseHealing = 25;
        //Create a base Healing object that this action will apply
        heal = new Healing(this.chrSource, null, nBaseHealing);

        sDescription = "Heal 25 health to the chosen ally";


        SetArgOwners();
    }

    override public void Execute() {
        //It's a bit awkward that you have to do this typecasting, 
        // but at least it's eliminated from the targetting lambda
        Chr tar = ((TargetArgAlly)arArgs[0]).chrTar;

        stackClauses.Push(new Clause() {
            fExecute = () => {

                //Make a copy of the heal object to give to the executable
                Healing healToApply = new Healing(heal);
                //Give the healing object its target
                healToApply.SetChrTarget(tar);

                ContAbilityEngine.Get().AddExec(new ExecHeal() {
                    chrSource = this.chrSource,
                    chrTarget = tar,

                    heal = healToApply,

                    fDelay = ContTurns.fDelayStandard,
                    sLabel = "Healing " + tar.sName
                });
            }
        });

        //NOTE:: Every Execute extension should begin with a typecast and end with a base.Execute call;

        base.Execute();
    }
}
