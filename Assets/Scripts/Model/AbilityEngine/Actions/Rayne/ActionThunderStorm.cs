using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionThunderStorm : Action {

    public int nBaseDamage;
    public int nStunDuration;
    public Damage dmg;

    public ActionThunderStorm(Chr _chrOwner) : base(0, _chrOwner) {//number of target arguments

        //We don't need to target anything, since we always deal damage to everyone
        // arArgs[0] = new TargetArgTeam((own, tar) => true); 

        sName = "ThunderStorm";
        sDisplayName = "Thunder Storm";

        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 1, 1, 0, 0 });

        nCd = 10;
        nFatigue = 5;
        nActionCost = 1;

        sDescription1 = "Deal 15 damage and 2 fatigue to all enemies.";

        nStunDuration = 2;
        nBaseDamage = 15;
        //Create a base Damage object that this action will apply
        dmg = new Damage(this.chrSource, null, nBaseDamage);

        SetArgOwners();
    }

    override public void Execute(int[] lstTargettingIndices) {

        Player enemy = chrSource.GetEnemyPlayer();

        stackClauses.Push(new Clause() {
            fExecute = () => {
                //Deal damage to all enemies
                for (int i = 0; i < enemy.arChr.Length; i++) {

                    //Make a copy of the damage object to give to the executable
                    Damage dmgToApply = new Damage(dmg);
                    //Give the damage object its target
                    dmgToApply.SetChrTarget(enemy.arChr[i]);

                    //TODO:: Organize this in the correct order
                    ContAbilityEngine.Get().AddExec(new ExecDealDamage() {
                        chrSource = this.chrSource,
                        chrTarget = enemy.arChr[i],
                        dmg = dmgToApply,

                        arSoundEffects = new SoundEffect[] { new SoundEffect("Rayne/sndStorm", 5.1f) },

                        fDelay = ContTurns.fDelayNone,
                        sLabel = enemy.arChr[i].sName + " is caught in the storm"
                    });
                }

                //Stun all enemies
                for (int i = 0; i < enemy.arChr.Length; i++) {

                    //TODO:: Organize this in the correct order
                    ContAbilityEngine.Get().AddExec(new ExecStun() {
                        chrSource = this.chrSource,
                        chrTarget = enemy.arChr[i],

                        GetDuration = () => nStunDuration,

                        fDelay = ContTurns.fDelayNone,
                        sLabel = enemy.arChr[i].sName + " is being stunned"
                    });
                }
            }
        });

    }

}