using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSnakeLaunch : Action {

    public ActionSnakeLaunch(Chr _chrOwner) : base(1, _chrOwner) {//number of target arguments

        //Since the base constructor initializes this array, we can start filling it
        arArgs[0] = new TargetArgChr((own, tar) => own.plyrOwner != tar.plyrOwner);

        sName = "SnakeLaunch";
        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 0, 1, 1 });

        nCd = 8;
        nFatigue = 4;
        nActionCost = 1;

        sDescription = "Deal 20 damage twice.  Lose 5 life twice";

        SetArgOwners();
    }

    override public void Execute() {
        //We have to typecast our targetting parameter to the type we expect
        Chr tar = ((TargetArgChr)arArgs[0]).chrTar;

        stackClauses.Push(new Clause() {
            fExecute = () => {
                Damage dmgToDeal = new Damage(chrSource, tar, 20);

                ContAbilityEngine.Get().AddExec(new ExecDealDamage() {
                    chrSource = this.chrSource,
                    chrTarget = tar,
                    dmg  = dmgToDeal,

                    fDelay = 1.0f,
                    sLabel = "Snake Biting " + tar.sName
                });
            }
        });

        stackClauses.Push(new Clause() {
            fExecute = () => {
                Damage dmgToDeal = new Damage(chrSource, tar, 20);

                ContAbilityEngine.Get().AddExec(new ExecDealDamage() {
                    chrSource = this.chrSource,
                    chrTarget = tar,
                    dmg = dmgToDeal,

                    fDelay = 1.0f,
                    sLabel = "Snake Biting " + tar.sName
                });
            }
        });

        stackClauses.Push(new Clause() {
            fExecute = () => {
                ContAbilityEngine.Get().AddExec(new ExecLoseLife() {
                    chrSource = this.chrSource,
                    chrTarget = this.chrSource,
                    nLifeLoss = 5,

                    fDelay = 1.0f,
                    sLabel = this.chrSource.sName + " has lost her Snakes"
                });
            }
        });

        stackClauses.Push(new Clause() {
            fExecute = () => {
                ContAbilityEngine.Get().AddExec(new ExecLoseLife() {
                    chrSource = this.chrSource,
                    chrTarget = this.chrSource,
                    nLifeLoss = 5,

                    fDelay = 1.0f,
                    sLabel = this.chrSource.sName + " has lost her Snakes"
                });
            }
        });



        //NOTE:: Every Execute extension should begin with a typecast and end with a base.Execute call;

        base.Execute();
    }

}