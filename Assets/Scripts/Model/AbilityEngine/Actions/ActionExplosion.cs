using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionExplosion : Action {

    public Damage dmg;
    public int nBaseDamage;

    public ActionExplosion(Chr _chrOwner) : base(1, _chrOwner) {//number of target arguments

        //Since the base constructor initializes this array, we can start filling it
        arArgs[0] = new TargetArgTeam((own, tar) => true); // any team selection is fine

        sName = "Explosion";
        sDisplayName = "Explosion";

        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 2, 0, 0 });

        nCd = 10;
        nFatigue = 6;
        nActionCost = 1;

        nBaseDamage = 5;
        //Create a base Damage object that this action will apply
        dmg = new Damage(this.chrSource, null, nBaseDamage);

        sDescription1 = "Deal 5 damage to all characters on the chosen team.";

        SetArgOwners();
    }

    override public void Execute(int[] lstTargettingIndices) {

        Player tarPlyr = Player.GetTargetByIndex(lstTargettingIndices[0]);

        stackClauses.Push(new Clause() {
            fExecute = () => {
                for (int i = 0; i < tarPlyr.arChr.Length; i++) {
                    Debug.Log("This Explosion Clause put an ExecDamage on the stack");

                    //Make a copy of the damage object to give to the executable
                    Damage dmgToApply = new Damage(dmg);
                    //Give the damage object its target
                    dmgToApply.SetChrTarget(tarPlyr.arChr[i]);

                    //TODO:: Organize this in the correct order
                    ContAbilityEngine.Get().AddExec(new ExecDealDamage() {
                        chrSource = this.chrSource,
                        chrTarget = tarPlyr.arChr[i],
                        dmg = dmgToApply,
                        fDelay = ContTurns.fDelayStandard,
                        sLabel = "Exploding"
                    });
                }
            }
        });

    }

}
