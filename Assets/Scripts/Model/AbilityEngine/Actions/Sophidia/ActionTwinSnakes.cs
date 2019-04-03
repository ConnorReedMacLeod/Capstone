using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionTwinSnakes : Action {

    public Damage dmg;
    public int nBaseDamage;

    public int nLifeLoss;

    public ActionTwinSnakes(Chr _chrOwner) : base(1, _chrOwner) {//number of target arguments

        //Since the base constructor initializes this array, we can start filling it
        arArgs[0] = new TargetArgChr(Action.IsEnemy);

        sName = "TwinSnakes";
        sDisplayName = "Twin Snakes";

        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 0, 1, 1 });

        nCd = 8;
        nFatigue = 4;
        nActionCost = 1;

        nBaseDamage = 20;
        //Create a base Damage object that this action will apply
        dmg = new Damage(this.chrSource, null, nBaseDamage);

        nLifeLoss = 5;

        sDescription1 = "Deal 20 damage to the chosen character twice.  Lose 5 health twice";

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
                    dmg = dmgToApply,

                    arSoundEffects = new SoundEffect[] { new SoundEffect("Sophidia/sndTwinSnakes", 2f) },

                    fDelay = ContTurns.fDelayStandard,
                    sLabel = "Snake Biting " + tarChr.sName
                });
            }
        });

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
                    sLabel = "Snake Biting " + tarChr.sName
                });
            }
        });

        stackClauses.Push(new Clause() {
            fExecute = () => {
                ContAbilityEngine.Get().AddExec(new ExecLoseLife() {
                    chrSource = this.chrSource,
                    chrTarget = this.chrSource,
                    nLifeLoss = nLifeLoss,

                    fDelay = ContTurns.fDelayStandard,
                    sLabel = this.chrSource.sName + " has lost her Snakes"
                });
            }
        });

        stackClauses.Push(new Clause() {
            fExecute = () => {
                ContAbilityEngine.Get().AddExec(new ExecLoseLife() {
                    chrSource = this.chrSource,
                    chrTarget = this.chrSource,
                    nLifeLoss = nLifeLoss,

                    fDelay = ContTurns.fDelayStandard,
                    sLabel = this.chrSource.sName + " has lost her Snakes"
                });
            }
        });

    }

}