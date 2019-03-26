using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionCacaphony : Action {

    public Damage dmg;

    public int nBaseDamage;
    public int nCriticalBaseDamage;

    public int nBaseStun;
    public int nCriticalStun;

    public ActionCacaphony(Chr _chrOwner) : base(1, _chrOwner) {//number of target arguments

        //Since the base constructor initializes this array, we can start filling it
        arArgs[0] = new TargetArgChr(Action.IsAnyCharacter); //Any character target is fine

        sName = "Cacaphony";
        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 1, 0, 1});

        nCd = 8;
        nFatigue = 4;
        nActionCost = 1;

        nBaseDamage = 20;
        nCriticalBaseDamage = 30;
        //Create a base Damage object that this action will apply
        dmg = new Damage(this.chrSource, null, 
            () =>  IsCritical(dmg.chrTarget)? nCriticalBaseDamage : nBaseDamage);

        nBaseStun = 2;
        nCriticalStun = 3;

        sDescription = "Deal 20 damage and 2 fatigue to the chosen character.  If the chosen character is blocking, deal 30 damage and 3 fatigue instead";

        SetArgOwners();
    }

    //Deal critical damage and stun if the targetted character is a blocker
    public bool IsCritical(Chr tarChr) {
        return (tarChr != null && tarChr.bBlocker);
    }

    override public void Execute(int[] lstTargettingIndices) {

        Chr tarChr = Chr.GetTargetByIndex(lstTargettingIndices[0]);

        

        stackClauses.Push(new Clause() {
            fExecute = () => {

                //Make a copy of the damage object to give to the executable
                Damage dmgToApply = new Damage(dmg);
                //Give the damage object its target
                dmgToApply.SetChrTarget(tarChr);
                dmgToApply.SetBase(() => IsCritical(dmgToApply.chrTarget) ? nCriticalBaseDamage : nBaseDamage);

                ContAbilityEngine.Get().AddExec(new ExecStun() {
                    chrSource = this.chrSource,
                    chrTarget = tarChr,

                    GetDuration = () => IsCritical(tarChr) ? nCriticalStun : nBaseStun,

                    fDelay = ContTurns.fDelayStandard,
                    sLabel = "Stunning " + tarChr.sName
                });
                

                ContAbilityEngine.Get().AddExec(new ExecDealDamage() {
                    chrSource = this.chrSource,
                    chrTarget = tarChr,

                    dmg = dmgToApply,

                    fDelay = ContTurns.fDelayStandard,
                    sLabel = "Screeching at " + tarChr.sName
                });



            }
        });
    }
}
