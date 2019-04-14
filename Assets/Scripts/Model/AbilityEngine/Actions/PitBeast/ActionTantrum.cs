using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionTantrum : Action {

    public int nEnemyDamage;
    public int nAllyDamage;
    public Damage dmgEnemy;
    public Damage dmgAlly;

    public ActionTantrum(Chr _chrOwner) : base(0, _chrOwner) {//number of target arguments

        //We don't need to target anything, since we always deal damage to everyone
        // arArgs[0] = new TargetArgTeam((own, tar) => true); 

        sName = "Tantrum";
        sDisplayName = "Tantrum";

        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 1, 0, 0, 1, 0 });

        nCd = 9;
        nFatigue = 5;
        nActionCost = 1;

        sDescription1 = "Deal 20 damage to all enemies and 5 damage to all other allies";

        nEnemyDamage = 20;
        nAllyDamage = 5;

        //Create a base Damage object that this action will apply
        dmgEnemy = new Damage(this.chrSource, null, nEnemyDamage);

        //Create a base Damage object that this action will apply
        dmgAlly = new Damage(this.chrSource, null, nAllyDamage);

        SetArgOwners();
    }

    override public void Execute(int[] lstTargettingIndices) {

        Player enemy = chrSource.plyrOwner.GetEnemyPlayer();

        stackClauses.Push(new Clause() {
            fExecute = () => {
                //Deal damage to all enemies
                for (int i = 0; i < enemy.arChr.Length; i++) {

                    //Don't target dead characters
                    if (enemy.arChr[i].bDead) continue;

                    //Make a copy of the damage object to give to the executable
                    Damage dmgToApply = new Damage(dmgEnemy);
                    //Give the damage object its target
                    dmgToApply.SetChrTarget(enemy.arChr[i]);

                    //TODO:: Organize this in the correct order
                    ContAbilityEngine.Get().AddExec(new ExecDealDamage() {
                        chrSource = this.chrSource,
                        chrTarget = enemy.arChr[i],
                        dmg = dmgToApply,

                        fDelay = ContTurns.fDelayNone,
                        sLabel = enemy.arChr[i].sName + " is caught in the tantrum"
                    });
                }

                //Deal damage to all other allies
                for (int i = 0; i < chrSource.plyrOwner.arChr.Length; i++) {
                    if (chrSource.plyrOwner.arChr[i] == chrSource) continue; //Don't hurt yourself

                    //Make a copy of the damage object to give to the executable
                    Damage dmgToApply = new Damage(dmgAlly);
                    //Give the damage object its target
                    dmgToApply.SetChrTarget(chrSource.plyrOwner.arChr[i]);

                    //TODO:: Organize this in the correct order
                    ContAbilityEngine.Get().AddExec(new ExecDealDamage() {
                        chrSource = this.chrSource,
                        chrTarget = chrSource.plyrOwner.arChr[i],
                        dmg = dmgToApply,

                        arSoundEffects = new SoundEffect[] { new SoundEffect("PitBeast/sndTantrum", 4.167f) },

                        fDelay = ContTurns.fDelayNone,
                        sLabel = chrSource.plyrOwner.arChr[i].sName + " is caught in the tantrum"
                    });
                }
            }
        });
    }

}