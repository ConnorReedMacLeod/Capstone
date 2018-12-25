using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionCacaphony : Action {

    public ActionCacaphony(Chr _chrOwner) : base(1, _chrOwner) {//number of target arguments

        //Since the base constructor initializes this array, we can start filling it
        arArgs[0] = new TargetArgChr((own, tar) => true); //Any character target is fine

        sName = "Cacaphony";
        type = ActionType.ACTIVE;

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 1, 0, 1});

        nCd = 8;
        nFatigue = 4;
        nActionCost = 1;

        sDescription = "Deal 20 damage and 2 fatigue to the chosen character.  If the chosen character is blocking, deal 30 damage and 3 fatigue instead";

        SetArgOwners();
    }

    override public void Execute() {
        //It's a bit awkward that you have to do this typecasting, 
        // but at least it's eliminated from the targetting lambda
        Chr tar = ((TargetArgChr)arArgs[0]).chrTar;

        Debug.Log("Cacaphony has its Execute() called with target " + tar.sName);

        stackClauses.Push(new Clause() {
            fExecute = () => {

                //TODO:: Should calculate this bonus damage/stun at execution time
                //plan - once the decorator pattern is implemented, the GetBaseDamage() function
                // can have a base conditional function (to check if target is currently the blocker)
                // that can be decorated with potentially extra effects
                int nToDamage = 20;
                int nToFatigue = 2;

                if(tar.bBlocker == true) {
                    Debug.Log("The target is the blocker");
                    nToDamage = 30;
                    nToFatigue = 3;
                }

                Debug.Log("This Cacaphony Clause put an ExecStun on the stack");

                ContAbilityEngine.Get().AddExec(new ExecStun() {
                    chrSource = this.chrSource,
                    chrTarget = tar,
                    nAmount = nToFatigue,
                    fDelay = 1.0f,
                    sLabel = "Stunning " + tar.sName + " for " + nToFatigue
                });

                Debug.Log("This Cacaphony Clause put an ExecDamage on the stack");
                Damage dmgToDeal = new Damage(chrSource, tar, nToDamage);

                ContAbilityEngine.Get().AddExec(new ExecDealDamage() {
                    chrSource = this.chrSource,
                    chrTarget = tar,
                    dmg = dmgToDeal,

                    fDelay = 1.0f,
                    sLabel = "Dealing " + nToDamage + " to " + tar.sName
                });



            }
        });


        //NOTE:: Every Execute extension should begin with a typecast and end with a base.Execute call;

        base.Execute();
    }
}
