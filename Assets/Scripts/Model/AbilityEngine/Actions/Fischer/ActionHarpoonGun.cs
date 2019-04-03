using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionHarpoonGun : Action {

    public Damage dmg;
    public int nBaseDamage;

    public ActionHarpoonGun(Chr _chrOwner) : base(1, _chrOwner) {//number of target arguments

        //Since the base constructor initializes this array, we can start filling it
        arArgs[0] = new TargetArgChr(Action.IsEnemy); //Choose a target enemy

        sName = "HarpoonGun";
        sDisplayName = "Harpoon Gun";

        type = new TypeChannel(this, 2, null);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 0, 0, 2 });

        nCd = 5;
        nFatigue = 2;
        nActionCost = 1;

        nBaseDamage = 30;
        //Create a base Damage object that this action will apply
        dmg = new Damage(this.chrSource, null, nBaseDamage);

        sDescription1 = "After channeling, deal 30 damage to the chosen enemy.  That enemy becomes the blocker.";

        SetArgOwners();
    }

    override public void Execute(int[] lstTargettingIndices) {

        Chr tarChr = Chr.GetTargetByIndex(lstTargettingIndices[0]);

        stackClauses.Push(new Clause() {
            fExecute = () => {

                //Make a copy of the damage object to give to the executable
                Damage dmgToApply = new Damage(dmg);
                //Give the damage object its target
                dmgToApply.SetChrTarget(tarChr);

                ContAbilityEngine.Get().AddExec(new ExecDealDamage() {
                    chrSource = this.chrSource,
                    chrTarget = tarChr,

                    arSoundEffects = new SoundEffect[] { new SoundEffect("Fischer/sndHarpoonGun", 2.067f) },

                    dmg = dmgToApply,
                    fDelay = ContTurns.fDelayStandard,
                    sLabel = tarChr.sName + " is being Harpooned"
                });
            }
        });

        stackClauses.Push(new Clause() {
            fExecute = () => {
                ContAbilityEngine.Get().AddExec(new ExecBecomeBlocker() {
                    chrSource = this.chrSource,
                    chrTarget = tarChr,

                    fDelay = ContTurns.fDelayStandard,
                    sLabel = tarChr.sName + " has become the blocker"
                });
            }
        });

    }

}