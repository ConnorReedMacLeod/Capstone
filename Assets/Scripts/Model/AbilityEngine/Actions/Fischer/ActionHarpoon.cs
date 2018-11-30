﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionHarpoon : Action {

    public ActionHarpoon(Chr _chrOwner) : base(1, _chrOwner) {//number of target arguments

        //Since the base constructor initializes this array, we can start filling it
        arArgs[0] = new TargetArgChr((own, tar) => own.plyrOwner != tar.plyrOwner); //Choose a target enemy

        sName = "Harpoon";
        type = ActionType.ACTIVE;

        //Physical, Mental, Energy, Blood, Effort
        arCost = new int[] { 0, 0, 0, 0, 2 };

        nCd = 5;
        nFatigue = 2;
        nActionCost = 1;

        sDescription = "NOT COMPLETED YET - HAVEN'T IMPLEMENTED CHANNELS";

        SetArgOwners();
    }

    override public void Execute() {

        Chr tar = ((TargetArgChr)arArgs[0]).chrTar; //Cast our first target to a ChrTarget and get that Chr

        stackClauses.Push(new Clause() {
            fExecute = () => {
                Debug.Log("This Harpoon Clause put an ExecDamage on the stack");
                ContAbilityEngine.Get().AddExec(new ExecDealDamage() {
                    chrSource = this.chrSource,
                    chrTarget = tar,
                    nDamage = 30,
                    fDelay = 1.0f,
                    sLabel = tar.sName + " is being Harpooned"
                });
            }
        });

        stackClauses.Push(new Clause() {
            fExecute = () => {
                Debug.Log("Harpoon's second clause put an ExecBecomeBlocker on the stack");
                ContAbilityEngine.Get().AddExec(new ExecBecomeBlocker() {
                    chrSource = this.chrSource,
                    chrTarget = tar

                });
            }
        });


        //NOTE:: Every Execute extension should begin with a typecast and end with a base.Execute call;

        base.Execute();
    }

}