using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulParry : Soul {

    public int nDamage;
    public int nArmour;

    public LinkedListNode<Property<int>.Modifier> nodeArmourModifier;

    public void OnDamaged(Chr chrDamager) {

        //First remove this soul effect
        soulContainer.RemoveSoul(this);

        //Then retaliate with damage
        ContAbilityEngine.Get().AddClause(new Clause() {
            fExecute = () => {
                Damage dmgToDeal = new Damage(chrSource, chrDamager, this.nDamage);

                ContAbilityEngine.Get().AddExec(new ExecDealDamage() {
                    chrSource = this.chrTarget,
                    chrTarget = chrDamager,

                    dmg = dmgToDeal
                });
            }
        });
       
    }

    public SoulParry(Chr _chrSource, Chr _chrTarget) : base(_chrSource, _chrTarget) {

        sName = "Parry";

        bVisible = true;
        bDuration = true;
        pnMaxDuration = new Property<int>(4);

        nDamage = 15;
        nArmour = 15;


        lstTriggers = new List<TriggerEffect>() {

            new TriggerEffect() {
                sub = ExecDealDamage.subAllPostTrigger,
                cb = (target, args) => {
                    //Check which character is about to be dealing damage
                    Chr chrSource = ((ExecDealDamage)args[0]).chrSource;

                    //Check which character is about to be taking damage
                    Chr chrTarget = ((ExecDealDamage)args[0]).chrTarget;

                    // If the source of the damage is owned by a different player
                    // AND if the target of the damage is the target of this soul
                    if(chrSource.plyrOwner.id != this.chrSource.plyrOwner.id && chrTarget == this.chrTarget) {
                        //Then perform a parry action
                        OnDamaged(chrSource);
                    }

                }
            }
        };
    }

    //Called when the character's armour has reached 0
    public void cbOnArmourClear(Object target, params object[] args) {
        //Decide if we want to remove the buff or not when the armour is completely
        // destroyed - for this particular buff, since we do more than just provide
        // armour, then we shouldn't remove the buff.  If this only gave armour,
        // then it would be reasonable to remove this buff when the armour is broken

    }

    public override void funcOnApplication() {

        //Add a modifier onto armour
        nodeArmourModifier = chrTarget.pnArmour.AddModifier((nArmour) => nArmour + this.nArmour);

        chrTarget.subArmourCleared.Subscribe(cbOnArmourClear);

    }

    public override void funcOnRemoval() {

        //Remove the modifier we put onto armour
        chrTarget.pnArmour.RemoveModifier(nodeArmourModifier);
        chrTarget.subArmourCleared.UnSubscribe(cbOnArmourClear);

    }

}
