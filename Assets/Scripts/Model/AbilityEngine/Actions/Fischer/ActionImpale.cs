using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionImpale : Action {

    public Damage dmg;
    public int nBaseDamage;

    public ActionImpale(Chr _chrOwner) : base(0, _chrOwner) {//number of target arguments

        //Since the base constructor initializes this array, we can start filling it
        //arArgs[0] = new TargetArgChr((own, tar) => own.plyrOwner != tar.plyrOwner); we don't have any targets

        sName = "Impale";
        sDisplayName = "Impale";

        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 1, 0, 0, 0, 0 });

        nCd = 6;
        nFatigue = 2;
        nActionCost = 1;

        nBaseDamage = 20;
        //Create a base Damage object that this action will apply
        dmg = new Damage(this.chrSource, null, nBaseDamage);

        sDescription1 = "Deal 20 damage to the enemy Vanguard and reduce their max health by 10.";

        SetArgOwners();
    }

    override public void Execute(int[] lstTargettingIndices) {

        Chr tarChr = chrSource.GetEnemyPlayer().GetBlocker();

        stackClauses.Push(new Clause() {
            fExecute = () => {

                //Make a copy of the damage object to give to the executable
                Damage dmgToApply = new Damage(dmg);
                //Give the damage object its target
                dmgToApply.SetChrTarget(tarChr);

                ContAbilityEngine.Get().AddExec(new ExecDealDamage() {
                    chrSource = this.chrSource,
                    chrTarget = tarChr,

                    dmg = dmgToApply,

                    fDelay = ContTurns.fDelayStandard,
                    sLabel = tarChr.sName + " is being impaled"
                });
            }
        });

        stackClauses.Push(new Clause() {
            fExecute = () => {
                ContAbilityEngine.Get().AddExec(new ExecApplySoul() {
                    chrSource = this.chrSource,
                    chrTarget = tarChr,

                    funcCreateSoul = (Chr _chrSource, Chr _chrTarget) => {
                        return new SoulImpaled(_chrSource, _chrTarget, this);
                    }

                });
            }
        });

    }

}
