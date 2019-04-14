using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionVenomousBite : Action {

    public Damage dmg;
    public int nBaseDamage;

    public ActionVenomousBite(Chr _chrOwner) : base(0, _chrOwner) {//number of target arguments

        //Since the base constructor initializes this array, we can start filling it
        //arArgs[0] = new TargetArgChr((own, tar) => own.plyrOwner != tar.plyrOwner); we don't have any targets

        sName = "VenomousBite";
        sDisplayName = "Venomous Bite";

        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 0, 1, 0 });

        nCd = 8;
        nFatigue = 3;
        nActionCost = 1;

        nBaseDamage = 5;
        //Create a base Damage object that this action will apply
        dmg = new Damage(this.chrSource, null, nBaseDamage);

		sDescription1 = "Deal 5 damage and apply ENVENOMED (3) to the enemy Vanguard.\n";
		sDescription2 = "[ENVENOMED]\n" + "At the end of turn, lose 5 health.  Whenever this character takes damage, +1 duration.";


		SetArgOwners();
    }        

    override public void Execute(int[] lstTargettingIndices) {

        Chr tarBlocker = chrSource.plyrOwner.GetEnemyPlayer().GetBlocker();

        stackClauses.Push(new Clause() {
            fExecute = () => {

                //Make a copy of the damage object to give to the executable
                Damage dmgToApply = new Damage(dmg);
                //Give the damage object its target
                dmgToApply.SetChrTarget(tarBlocker);

                ContAbilityEngine.Get().AddExec(new ExecDealDamage() {
                    chrSource = this.chrSource,
                    chrTarget = tarBlocker,
                    dmg = dmgToApply,

                    arSoundEffects = new SoundEffect[] { new SoundEffect("Sophidia/sndVenomousBite1", 2f),
                                                         new SoundEffect("Sophidia/sndVenomousBite2", 2f)},

                    fDelay = ContTurns.fDelayStandard,
                    sLabel = tarBlocker.sName + " is being bitten"
                });
            }
        });

        stackClauses.Push(new Clause() {
            fExecute = () => {
                ContAbilityEngine.Get().AddExec(new ExecApplySoul() {
                    chrSource = this.chrSource,
                    chrTarget = tarBlocker,

                    funcCreateSoul = (Chr _chrSource, Chr _chrTarget) => {
                        return new SoulEnvenomed(_chrSource, _chrTarget, this);
                    }
               
                });
            }
        });

    }

}
