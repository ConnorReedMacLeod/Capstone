using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionCacophony : Action {

    public ActionCacophony(Chr _chrOwner) : base(_chrOwner, 0) {

        sName = "Cacophony";
        sDisplayName = "Cacophony";

        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 1, 0, 1});

        nCd = 8;
        nFatigue = 4;

        lstClauses = new List<Clause>() {
            new Clause1(this)
        };
    }

    //Deal critical damage and stun if the targetted character is a blocker
    public static bool IsCritical(Chr tarChr) {
        return (tarChr != null && tarChr.bBlocker);
    }


    class Clause1 : ClauseChr {

        Damage dmg;

        public int nBaseDamage;
        public int nCriticalBaseDamage;

        public int nBaseStun;
        public int nCriticalStun;

        public Clause1(Action _act) : base(_act) {
            plstTags = new Property<List<ClauseTagChr>>(new List<ClauseTagChr>() {
                new ClauseTagChrRanged(this) //Base Tag always goes first
            });

            nBaseDamage = 20;
            nCriticalBaseDamage = 30;

            nBaseStun = 2;
            nCriticalStun = 3;

            dmg = new Damage(action.chrSource, null,  (_chrSource, _chrTarget) => IsCritical(_chrTarget) ? nCriticalBaseDamage : nBaseDamage);
        }

        public override string GetDescription() {

            //TODO - think about how to make this dynamically update nicely - currently doesn't reflect power

            return string.Format("Deal {0} damage and {1} fatigue to the chosen character.\n" +
                "If the chosen character is the Vanguard, deal {2} damage and {3} fatigue instead", 
                nBaseDamage, nBaseStun, nCriticalBaseDamage, nCriticalStun);
        }

        public override void ClauseEffect(Chr chrSelected) {

            //TODO - make this better dynamically react.  Should probably just have a Stun effect object that can
            //       be modified freely like with damage
            ContAbilityEngine.PushSingleExecutable(new ExecStun(action.chrSource, chrSelected, nBaseStun) {
                GetDuration = () => IsCritical(chrSelected) ? nCriticalStun : nBaseStun,
                sLabel = "Oh, god! My ears!"
            });

            ContAbilityEngine.PushSingleExecutable(new ExecDealDamage(action.chrSource, chrSelected, dmg) {
                arSoundEffects = new SoundEffect[] { new SoundEffect("Katarina/sndCacophony", 3.767f) },
                sLabel = "Anyway, here's Wonderwall"
            });

        }

    };
}
