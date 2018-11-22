﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionExplosion : Action {



    public ActionExplosion(Chr _chrOwner) : base(1, _chrOwner) {//number of target arguments

        //Since the base constructor initializes this array, we can start filling it
        arArgs[0] = new TargetArgTeam((own, tar) => true); // any team selection is fine

        sName = "Explosion";
        type = ActionType.ACTIVE;

        //Physical, Mental, Energy, Blood, Effort
        arCost = new int[] { 0, 0, 2, 0, 0 };

        nCd = 10;
        nFatigue = 6;
        nActionCost = 1;

        sDescription = "Deal 5 damage to all characters on target team";

        SetArgOwners();
    }

    override public void Execute() {
        //It's a bit awkward that you have to do this typecasting, 
        // but at least it's eliminated from the targetting lambda
        Player tar = ((TargetArgTeam)arArgs[0]).plyrTar;

        queueClauses.Enqueue(new Clause() {
            fExecute = () => {
                for (int i = 0; i < tar.arChr.Length; i++) {
                    Debug.Log("This Explosion Clause put an ExecDamage on the stack");
                    ContAbilityEngine.Get().AddExec(new ExecDealDamage() {
                        chrOwner = this.chrOwner,
                        chrTarget = tar.arChr[i],
                        nDamage = 5,
                        fDelay = 1.0f,
                        sLabel = "Exploding"
                    });
                }
            }
        });

        //NOTE:: Every Execute extension should begin with a typecast and end with a base.Execute call;


        base.Execute();
    }

}