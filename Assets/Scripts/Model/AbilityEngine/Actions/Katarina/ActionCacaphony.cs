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
        arArgs[0] = new TargetArgChr((own, tar) => true); //Any character target is fine

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
            () =>  IsCritical()? nCriticalBaseDamage : nBaseDamage);

        nBaseStun = 2;
        nCriticalStun = 3;

        sDescription = "Deal 20 damage and 2 fatigue to the chosen character.  If the chosen character is blocking, deal 30 damage and 3 fatigue instead";

        SetArgOwners();
    }

    //Deal critical damage and stun if the targetted character is a blocker
    public bool IsCritical() {
        return ((TargetArgChr)arArgs[0]).chrTar != null && ((TargetArgChr)arArgs[0]).chrTar.bBlocker;
    }

    override public void Execute() {
        //It's a bit awkward that you have to do this typecasting, 
        // but at least it's eliminated from the targetting lambda
        Chr tar = ((TargetArgChr)arArgs[0]).chrTar;

        

        stackClauses.Push(new Clause() {
            fExecute = () => {

                //Make a copy of the damage object to give to the executable
                Damage dmgToApply = new Damage(dmg);
                //Give the damage object its target
                dmgToApply.SetChrTarget(tar);

                ContAbilityEngine.Get().AddExec(new ExecStun() {
                    chrSource = this.chrSource,
                    chrTarget = tar,

                    GetDuration = () => IsCritical() ? nCriticalStun : nBaseStun,

                    fDelay = ContTurns.fDelayStandard,
                    sLabel = "Stunning " + tar.sName
                });
                

                ContAbilityEngine.Get().AddExec(new ExecDealDamage() {
                    chrSource = this.chrSource,
                    chrTarget = tar,

                    dmg = dmgToApply,

                    fDelay = ContTurns.fDelayStandard,
                    sLabel = "Screeching at " + tar.sName
                });



            }
        });


        //NOTE:: Every Execute extension should begin with a typecast and end with a base.Execute call;

        base.Execute();
    }
}
