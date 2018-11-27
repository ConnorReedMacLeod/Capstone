using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionVenemousBite : Action {

    public ActionVenemousBite(Chr _chrOwner) : base(0, _chrOwner) {//number of target arguments

        //Since the base constructor initializes this array, we can start filling it
        //arArgs[0] = new TargetArgChr((own, tar) => own.plyrOwner != tar.plyrOwner); we don't have any targets

        sName = "VenemousBite";
        type = ActionType.ACTIVE;

        //Physical, Mental, Energy, Blood, Effort
        arCost = new int[] { 0, 0, 0, 1, 1 };

        nCd = 8;
        nFatigue = 3;
        nActionCost = 1;

        sDescription = "Deal 5 damage and apply [Envenomed](3) to the blocker.\n" +
                       "[Envenomed]: Lose 5 Life at the end of each turn.  +1 duration each time you take damage";

        SetArgOwners();
    }

    //TODONOW:: Should make a curry method in a library that can bake in the current value when we create the function.
    //          This gives the option to either grab the value on function creation, or on function evaluation
    //          

    override public void Execute() {

        Chr tar = chrOwner.GetEnemyPlayer().GetBlocker();

        stackClauses.Push(new Clause() {
            fExecute = () => {
                Debug.Log("This VenemousBite Clause put an ExecDamage on the stack");
                ContAbilityEngine.Get().AddExec(new ExecDealDamage() {
                    chrOwner = this.chrOwner,
                    chrTarget = tar,
                    nDamage = 5,
                    fDelay = 1.0f,
                    sLabel = tar.sName + " is being bitten"
                });
            }
        });

        stackClauses.Push(new Clause() {
            fExecute = () => {
                Debug.Log("Venemous Bite's second clause put an ExecApplySoul on the stack");
                ContAbilityEngine.Get().AddExec(new ExecApplySoul() {
                    chrOwner = this.chrOwner,
                    soulContainerTarget = tar.soulContainer,

                    funcCreateSoul = () => {
                        return new Soul() {
                            sName = "Envenomed",
                            bVisible = true,
                            bDuration = true,

                            nMaxDuration = 3,

                            funcOnApplication = () => {
                                Debug.Log("Envenomed's OnApplication function has been called");
                                
                            },

                            lstTriggers = new List<Soul.TriggerEffect>() {
                                //Add an effect that will cause a poison life loss at the end of each turn
                                new Soul.TriggerEffect{
                                    sub = ExecTurnEndTurn.subAllPostTrigger,
                                    cb = (target, args) => {
                                        Debug.Log("We have been triggered at the end of turn to add a poison life loss exec");
                                        ContAbilityEngine.Get().AddExec(new ExecLoseLife() {
                                            chrOwner = this.chrOwner,
                                            chrTarget = tar,
                                            nAmount = 5,
                                            fDelay = 1.0f,
                                            sLabel = tar.sName + " is losing life from poison"
                                        });

                                    }
                                },
                                new Soul.TriggerEffect{
                                    sub = ExecDealDamage.subAllPostTrigger,
                                    cb = (target, args) => {
                                        Chr chrDamageTaker = ((ExecDealDamage)args[0]).chrTarget;


                                        Debug.Log("Damage has been dealt to " + chrDamageTaker + ".  Should " + chrOwner.sName + "'s Envenomed duration increase?");
                                        if(chrOwner == chrDamageTaker) {
                                            Debug.Log("No, it shouldn't increase");
                                            return;
                                        }

                                        Debug.Log("Yes, it should increase");
                                        
                                        
                                    }
                                }

                            },

                            funcOnRemoval = () => {
                                Debug.Log("Envenomed's OnRemoval function has been called");
                            }
                        };
                    },

                    fDelay = 1.0f,
                    sLabel = "Applying Burn Effect"
                });
            }
        });


        //NOTE:: Every Execute extension should begin with a typecast and end with a base.Execute call;

        base.Execute();
    }

}
