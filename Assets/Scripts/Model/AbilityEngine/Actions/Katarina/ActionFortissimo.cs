using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionFortissimo : Action {

    public ActionFortissimo(Chr _chrOwner) : base(_chrOwner, 0) {//Set the dominant clause

        sName = "Fortissimo";
        sDisplayName = "Fortissimo";

        type = new TypeCantrip(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 0, 0, 0 });

        nCd = 8;
        nFatigue = 0;
        nActionCost = 0;//0 since this is a cantrip

        lstClauses = new List<Clause>() {
            new Clause1(this)
        };
    }


    class Clause1 : ClauseChr {

        public SoulFortissimo soulToCopy;

        public Clause1(Action _act) : base(_act) {
            plstTags = new Property<List<ClauseTagChr>>(new List<ClauseTagChr>() {
                new ClauseTagChrRanged(this), //Base Tag always goes first
                new ClauseTagChrSelf(this)
            });

            soulToCopy = new SoulFortissimo(action.chrSource, null, action);
        }

        public override string GetDescription() {

            return string.Format("Gain {0} POWER and {1} DEFENSE for {2} turns.", soulToCopy.nPowerBuff, soulToCopy.nDefenseBuff, soulToCopy.pnMaxDuration.Get());
        }

        public override void ClauseEffect(Chr chrSelected) {

            ContAbilityEngine.PushSingleExecutable(new ExecApplySoul(action.chrSource, chrSelected, new SoulFortissimo(soulToCopy, chrSelected)) {
                arSoundEffects = new SoundEffect[] { new SoundEffect("Katarina/sndFortissimo", 6.2f) },
                sLabel = "Let's do it louder this time"
            });

        }

    };
}
