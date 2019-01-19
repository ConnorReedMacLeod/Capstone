using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionHarpoon : Action {

    public ActionHarpoon(Chr _chrOwner) : base(1, _chrOwner) {//number of target arguments

        //Since the base constructor initializes this array, we can start filling it
        arArgs[0] = new TargetArgChr((own, tar) => own.plyrOwner != tar.plyrOwner); //Choose a target enemy

        sName = "Harpoon";
        type = new TypeChannel(this, 2, null);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 0, 0, 2 });

        nCd = 5;
        nFatigue = 2;
        nActionCost = 1;

        sDescription = "After channeling, deal 30 damage to the chosen enemy.  That enemy becomes the blocker";

        SetArgOwners();
    }

    override public void Execute() {

        Chr tar = ((TargetArgChr)arArgs[0]).chrTar; //Cast our first target to a ChrTarget and get that Chr

        stackClauses.Push(new Clause() {
            fExecute = () => {
                Damage dmgToDeal = new Damage(chrSource, tar, 10);

                ContAbilityEngine.Get().AddExec(new ExecDealDamage() {
                    chrSource = this.chrSource,
                    chrTarget = tar,
                    dmg = dmgToDeal,
                    fDelay = 1.0f,
                    sLabel = tar.sName + " is being Harpooned"
                });
            }
        });

        stackClauses.Push(new Clause() {
            fExecute = () => {
                ContAbilityEngine.Get().AddExec(new ExecBecomeBlocker() {
                    chrSource = this.chrSource,
                    chrTarget = tar,

                    fDelay = 1.0f,
                    sLabel = tar.sName + " has become the blocker"
                });
            }
        });


        //NOTE:: Every Execute extension should begin with a typecast and end with a base.Execute call;

        base.Execute();
    }

}