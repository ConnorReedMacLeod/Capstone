using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionAmbush : Action {

    public Damage dmg;
    public int nBaseDamage;

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
                        if((Chr)target == Chr.GetTargetByIndex(((StateChanneling)(this.chrSource.curStateReadiness)).lstStoredTargettingIndices[0])){
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

        nBaseDamage = 20;
        //Create a base Damage object that this action will apply
        dmg = new Damage(this.chrSource, null, nBaseDamage);

        sDescription = "Channel 4: While channeling, after the chosen character uses an ability\n" +
                       "or blocks, deal 20 damage to them";

        SetArgOwners();
    }

    public void onAbilityUse() {

        Chr tar = Chr.GetTargetByIndex(((StateChanneling)(this.chrSource.curStateReadiness)).lstStoredTargettingIndices[0]);


        ContAbilityEngine.Get().AddClause(new Clause() {
            fExecute = () => {

                //Make a copy of the damage object to give to the executable
                Damage dmgToApply = new Damage(dmg);
                //Give the damage object its target
                dmgToApply.SetChrTarget(tar);

                ContAbilityEngine.Get().AddExec(new ExecDealDamage() {
                    chrSource = this.chrSource,
                    chrTarget = tar,

                    dmg = dmgToApply,
                    fDelay = ContTurns.fDelayStandard,
                    sLabel = chrSource.sName + " ambushed " + tar.sName + "!"
                });
            }
        });

    }


    override public void Execute(int[] lstTargettingIndices) {

        //Don't need to do anything on completion

    }

}