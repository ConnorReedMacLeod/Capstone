using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulParry : SoulChr {

    public int nDamage;
    public int nDefense;
    public Damage dmgCounterAttack;

    /* Removed while switching this to give defense rather than armour
    public LinkedListNode<Property<int>.Modifier> nodeArmourModifier;
    */
    public SoulChangeDefense soulChangeDefense;

    public void OnDamaged(Chr chrDamager) {

        //First remove this soul effect
        soulContainer.RemoveSoul(this);

        //Then retaliate with damage
        ContSkillEngine.PushSingleExecutable(new ExecDealDamage(chrSource, chrDamager, dmgCounterAttack) {
            arSoundEffects = new SoundEffect[] { new SoundEffect("Fischer/sndBucklerParry", 1.5f) },

            sLabel = "Counterattacking"
        });

    }

    public SoulParry(Chr _chrSource, Chr _chrTarget, Skill _skillSource) : base(_chrSource, _chrTarget, _skillSource) {

        sName = "Parry";

        bVisible = true;
        bDuration = true;
        bRecoilWhenApplied = false;

        pnMaxDuration = new Property<int>(4);

        nDamage = 15;
        nDefense = 15;
        //Create a base Damage object that this skill will apply
        dmgCounterAttack = new Damage(this.chrSource, null, nDamage);

    }

    public override void InitTriggers() {
        lstTriggers = new List<TriggerEffect>() {

            new TriggerEffect() {
                sub = ExecDealDamage.subAllPostTrigger,
                cb = cbOnDealDamage
            }
        };
    }

    public void cbOnDealDamage(Object target, object[] args) {
        //Check which character is about to be dealing damage
        Chr chrSource = ((ExecDealDamage)args[0]).chrSource;

        //Check which character is about to be taking damage
        Chr chrTarget = ((ExecDealDamage)args[0]).chrTarget;

        // If the source of the damage is owned by a different player
        // AND if the target of the damage is the target of this soul
        if(chrSource.plyrOwner.id != this.chrSource.plyrOwner.id && chrTarget == this.chrTarget) {
            //Then perform a parry effect
            OnDamaged(chrSource);
        }
    }

    public override void ApplicationEffect() {
        base.ApplicationEffect();

        //Make a Permanent SoulChangeDefense, and save a reference to it, so it can be removed later
        soulChangeDefense = new SoulChangeDefense(chrSource, chrTarget, skillSource, nDefense);
        chrTarget.soulContainer.ApplySoul(soulChangeDefense);

        /*We were having this apply armour, but this has been changed to defense (temporarily?)
         * 
         * //Add a modifier onto armour
        nodeArmourModifier = chrTarget.pnArmour.AddModifier((nArmour) => nArmour + this.nArmour);

        chrTarget.subArmourCleared.Subscribe(cbOnArmourClear);*/
    }

    public override void RemoveEffect() {
        base.RemoveEffect();

        chrTarget.soulContainer.RemoveSoul(soulChangeDefense);

        /* removed while making this give defense rather than armour
        //Remove the modifier we put onto armour
        chrTarget.pnArmour.RemoveModifier(nodeArmourModifier);
        chrTarget.subArmourCleared.UnSubscribe(cbOnArmourClear);
        */
    }

    public SoulParry(SoulParry other, Chr _chrTarget = null) : base(other, _chrTarget) {

        nDamage = other.nDamage;
        nDefense = other.nDefense;
        dmgCounterAttack = new Damage(other.dmgCounterAttack);

    }


    //**** CURRENTLY NOT NEEDED FOR THIS VERSION OF THE SKILL *****

    //Called when the character's armour has reached 0
    public void cbOnArmourClear(Object target, params object[] args) {
        //Decide if we want to remove the buff or not when the armour is completely
        // destroyed - for this particular buff, since we do more than just provide
        // armour, then we shouldn't remove the buff.  If this only gave armour,
        // then it would be reasonable to remove this buff when the armour is broken

    }
}
