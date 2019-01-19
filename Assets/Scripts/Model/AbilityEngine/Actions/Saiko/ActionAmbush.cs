﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionAmbush : Action {

    public int nDamage;

    public ActionAmbush(Chr _chrOwner) : base(1, _chrOwner) {//number of target arguments

        //Since the base constructor initializes this array, we can start filling it
        arArgs[0] = new TargetArgChr((own, tar) => true);

        sName = "Ambush";

        SoulChannel soulChannelEffect = new SoulChannel(this) {

            lstTriggers = new List<Soul.TriggerEffect>() {
                new Soul.TriggerEffect() {
                    sub = Chr.subAllPostExecuteAbility,
                    cb = (target, args) => {

                        //If the character who used an ability is the one we targetted,
                        // Then we can ambush them
                        if((Chr)target == ((TargetArgChr)arArgs[0]).chrTar){
                            onAbilityUse();
                        }

                    }
                }
            }
        };

        type = new TypeChannel(this, 4, soulChannelEffect);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 0, 0, 1 });

        nCd = 3;
        nFatigue = 1;
        nActionCost = 1;

        nDamage = 20;

        sDescription = "Channel 4: While channeling, after the chosen character uses an ability\n" +
                       "or blocks, deal 20 damage to them";

        SetArgOwners();
    }

    public void onAbilityUse() {

        Chr tar = ((TargetArgChr)arArgs[0]).chrTar; //Cast our first target to a ChrTarget and get that Chr

        ContAbilityEngine.Get().AddClause(new Clause() {
            fExecute = () => {

                ContAbilityEngine.Get().AddExec(new ExecDealDamage() {
                    chrSource = this.chrSource,
                    chrTarget = tar,

                    dmg = new Damage(chrSource, tar, nDamage),
                    fDelay = 1.0f,
                    sLabel = chrSource.sName + " ambushed " + tar.sName + "!"
                });
            }
        });

    }


    override public void Execute() {



    }

}