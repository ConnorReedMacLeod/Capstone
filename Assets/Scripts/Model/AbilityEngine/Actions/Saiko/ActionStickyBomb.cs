using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionStickyBomb : Action {

    public Damage dmg;
    public int nBaseDamage;

    public ActionStickyBomb(Chr _chrOwner) : base(_chrOwner, 0) {//set the dominant clause

        sName = "StickyBomb";
        sDisplayName = "Sticky Bomb";

        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 1, 0, 0, 1 });

        nCd = 6;
        nFatigue = 3;

        lstClauses = new List<Clause>() {
            new Clause1(this),
        };
    }

    class Clause1 : ClauseChr {

        Damage dmg;
        public int nBaseDamage = 5;
        public SoulStickyBomb soulToCopy;

        public Clause1(Action _act) : base(_act) {
            plstTags = new Property<List<ClauseTagChr>>(new List<ClauseTagChr>() {
                new ClauseTagChrRanged(this), //Base Tag always goes first
            });

            dmg = new Damage(action.chrSource, null, nBaseDamage);
            soulToCopy = new SoulStickyBomb(action.chrSource, null, action);
        }

        public override string GetDescription() {

            return string.Format("Deal {0} damage to the chosen character, and another {1} at the end of turn", dmg.Get(), soulToCopy.nDetonationDamage);
        }

        public override void ClauseEffect(Chr chrSelected) {


            ContAbilityEngine.PushSingleExecutable(new ExecApplySoul(action.chrSource, chrSelected, new SoulStickyBomb(soulToCopy, chrSelected)) {
                sLabel = "A bomb stuck"
            });

            ContAbilityEngine.PushSingleExecutable(new ExecDealDamage(action.chrSource, chrSelected, dmg) {
                arSoundEffects = new SoundEffect[] { new SoundEffect("Saiko/sndStickyBombToss", 2.133f) },
                sLabel = "Threw a bomb"
            });

        }

    };

}