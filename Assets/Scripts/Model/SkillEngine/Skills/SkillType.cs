using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static SkillType.SKILLTYPE;
using static Discipline.DISCIPLINE;


//When making a new skill, add an entry for its enum, update the dictSkillTypeInfos to include an entry for which disciplines it uses,
//  and finally add an entry for how to construct its skill in the InstantiateNewSkill function
public static class SkillType {
    //TODO - eventually look at transferring this long list to a text file - possible?  worth it?
    public enum SKILLTYPE {
        //TESTING
        LEECH, TRANSFUSE, KNOCKBACK, ADVANCE, BUNKER, FIREBALL, EXPLOSION, HEAL, STRATEGIZE, MANABLOSSOM,

        //Fischer
        BUCKLERPARRY, HARPOONGUN, HUNTERSQUARRY, IMPALE,

        //Katarina
        CACOPHONY, FORTISSIMO, REVERBERATE, SERENADE,

        //Pit Beast
        FORCEDEVOLUTION, SADISM, TANTRUM, TENDRILSTAB,

        //Rayne
        CHEERLEADER, CLOUDCUSHION, SPIRITSLAP, THUNDERSTORM,

        //Saiko
        AMBUSH, SMOKECOVER, STICKYBOMB, TRANQUILIZE,

        //Sophidia
        HISS, HYDRASREGEN, TWINSNAKES, VENEMOUSBITE,

        //Utility
        REST

    };

    public struct SkillTypeInfo {
        public SKILLTYPE type;
        public string sName;
        public List<Discipline.DISCIPLINE> lstRequiredDisciplines;


        public SkillTypeInfo(SKILLTYPE _type, string _sName, List<Discipline.DISCIPLINE> _lstRequiredDisciplines) {
            type = _type;
            sName = _sName;
            lstRequiredDisciplines = _lstRequiredDisciplines;
        }
    }

    static Dictionary<SKILLTYPE, SkillTypeInfo> dictSkillTypeInfos = new Dictionary<SKILLTYPE, SkillTypeInfo>()
    {   //TESTING
        { LEECH, new SkillTypeInfo ( LEECH, "Leech", new List<Discipline.DISCIPLINE> { TESTING } ) },
        { TRANSFUSE, new SkillTypeInfo ( TRANSFUSE, "Transfuse", new List<Discipline.DISCIPLINE> { TESTING } ) },
        { KNOCKBACK, new SkillTypeInfo ( KNOCKBACK, "Knockback", new List<Discipline.DISCIPLINE> { TESTING } ) },
        { ADVANCE, new SkillTypeInfo ( ADVANCE, "Advance", new List<Discipline.DISCIPLINE> { TESTING } ) },
        { BUNKER, new SkillTypeInfo ( BUNKER, "Bunker", new List<Discipline.DISCIPLINE> { TESTING } ) },
        { FIREBALL, new SkillTypeInfo (FIREBALL, "Fireball", new List<Discipline.DISCIPLINE> { TESTING } ) },
        { EXPLOSION, new SkillTypeInfo (EXPLOSION, "Explosion", new List<Discipline.DISCIPLINE> { TESTING } ) },
        { HEAL, new SkillTypeInfo (HEAL, "Heal", new List<Discipline.DISCIPLINE> { TESTING } ) },
        { STRATEGIZE, new SkillTypeInfo (STRATEGIZE, "Strategize", new List<Discipline.DISCIPLINE> { TESTING } ) },
        { MANABLOSSOM, new SkillTypeInfo (MANABLOSSOM, "Mana Blossom", new List<Discipline.DISCIPLINE> { TESTING } ) },

        //FISHER
        { BUCKLERPARRY, new SkillTypeInfo ( BUCKLERPARRY, "Buckler Parry", new List<Discipline.DISCIPLINE> { FISCHER } ) },
        { HARPOONGUN, new SkillTypeInfo ( HARPOONGUN, "Harpoon Gun", new List<Discipline.DISCIPLINE> { FISCHER } ) },
        { HUNTERSQUARRY, new SkillTypeInfo ( HUNTERSQUARRY, "Hunter's Quarry", new List<Discipline.DISCIPLINE> { FISCHER } ) },
        { IMPALE, new SkillTypeInfo ( IMPALE, "Impale", new List<Discipline.DISCIPLINE> { FISCHER } ) },

        //KATARINA
        { CACOPHONY, new SkillTypeInfo ( CACOPHONY, "Cacophony", new List<Discipline.DISCIPLINE> { KATARINA } ) },
        { FORTISSIMO, new SkillTypeInfo ( FORTISSIMO, "Fortissimo", new List<Discipline.DISCIPLINE> { KATARINA } ) },
        { REVERBERATE, new SkillTypeInfo ( REVERBERATE, "Reverberate", new List<Discipline.DISCIPLINE> { KATARINA } ) },
        { SERENADE, new SkillTypeInfo ( SERENADE, "Serenade", new List<Discipline.DISCIPLINE> { KATARINA } ) },

        //PITBEAST
        { FORCEDEVOLUTION, new SkillTypeInfo ( FORCEDEVOLUTION, "Forced Evolution", new List<Discipline.DISCIPLINE> { PITBEAST } ) },
        { SADISM, new SkillTypeInfo ( SADISM, "Sadism", new List<Discipline.DISCIPLINE> { PITBEAST } ) },
        { TANTRUM, new SkillTypeInfo ( TANTRUM, "Tantrum", new List<Discipline.DISCIPLINE> { PITBEAST } ) },
        { TENDRILSTAB, new SkillTypeInfo ( TENDRILSTAB, "Tendril Stab", new List<Discipline.DISCIPLINE> { PITBEAST } ) },

        //RAYNE
        { CHEERLEADER, new SkillTypeInfo ( CHEERLEADER, "Cheerleader", new List<Discipline.DISCIPLINE> { RAYNE } ) },
        { CLOUDCUSHION, new SkillTypeInfo ( CLOUDCUSHION, "Cloud Cushion", new List<Discipline.DISCIPLINE> { RAYNE } ) },
        { SPIRITSLAP, new SkillTypeInfo ( SPIRITSLAP, "Spirit Slap", new List<Discipline.DISCIPLINE> { RAYNE } ) },
        { THUNDERSTORM, new SkillTypeInfo ( THUNDERSTORM, "Thunderstorm", new List<Discipline.DISCIPLINE> { RAYNE } ) },

        //SAIKO
        { AMBUSH, new SkillTypeInfo ( AMBUSH, "Ambush", new List<Discipline.DISCIPLINE> { SAIKO } ) },
        { SMOKECOVER, new SkillTypeInfo ( SMOKECOVER, "Smoke Cover", new List<Discipline.DISCIPLINE> { SAIKO } ) },
        { STICKYBOMB, new SkillTypeInfo ( STICKYBOMB, "Stickybomb", new List<Discipline.DISCIPLINE> { SAIKO } ) },
        { TRANQUILIZE, new SkillTypeInfo ( TRANQUILIZE, "Tranquilize", new List<Discipline.DISCIPLINE> { SAIKO } ) },

        //SOPHIDIA
        { HISS, new SkillTypeInfo ( HISS, "Hiss", new List<Discipline.DISCIPLINE> { SOPHIDIA } ) },
        { HYDRASREGEN, new SkillTypeInfo ( HYDRASREGEN, "Hydra's Regeneration", new List<Discipline.DISCIPLINE> { SOPHIDIA } ) },
        { TWINSNAKES, new SkillTypeInfo ( TWINSNAKES, "Twinsnakes", new List<Discipline.DISCIPLINE> { SOPHIDIA } ) },
        { VENEMOUSBITE, new SkillTypeInfo ( VENEMOUSBITE, "Venemous Bite", new List<Discipline.DISCIPLINE> { SOPHIDIA } ) },

    };


    public static SkillTypeInfo GetSkillTypeInfo(SKILLTYPE skilltype) {
        return dictSkillTypeInfos[skilltype];
    }

    public static List<SkillTypeInfo> GetSkillInfosUnderDisciplines(Chr chr) {
        return GetSkillInfosUnderDisciplines(chr.lstDisciplines);
    }

    public static List<SkillTypeInfo> GetSkillInfosUnderDisciplines(CharType.CHARTYPE chartype) {
        return GetSkillInfosUnderDisciplines(CharType.GetDisciplines(chartype));
    }

    public static List<SkillTypeInfo> GetSkillInfosUnderDisciplines(List<Discipline.DISCIPLINE> lstDisciplines) {

        //For each kvp, check if there are any required disciplines that aren't given in the passed lstDisciplines.  Ensure there are not any of these kvp in the
        //  kvps we keep, then only select the keys from those kvps.
        List<SkillTypeInfo> lstUsableSkills = dictSkillTypeInfos.Where(kvp => kvp.Value.lstRequiredDisciplines.Except(lstDisciplines).Any() == false).Select(kvp => kvp.Value).ToList();

        return lstUsableSkills;
    }


    //Whenever a new skill is added, have to include a skillType -> Constructor mapping for it
    //  so that it can be added in the loadout setup phase
    public static Skill InstantiateNewSkill(SKILLTYPE skillType, Chr chr) {

        Skill skillNew = null;

        switch(skillType) {

        //TESTING
        case SKILLTYPE.LEECH:
            skillNew = new SkillLeech(chr);
            break;
        case SKILLTYPE.TRANSFUSE:
            skillNew = new SkillTransfuse(chr);
            break;
        case SKILLTYPE.KNOCKBACK:
            skillNew = new SkillKnockback(chr);
            break;
        case SKILLTYPE.ADVANCE:
            skillNew = new SkillAdvance(chr);
            break;
        case SKILLTYPE.BUNKER:
            skillNew = new SkillBunker(chr);
            break;
        case SKILLTYPE.FIREBALL:
                skillNew = new SkillFireball(chr);
            break;
        case SKILLTYPE.EXPLOSION:
            skillNew = new SkillExplosion(chr);
            break;
        case SKILLTYPE.HEAL:
            skillNew = new SkillHeal(chr);
            break;
        case SKILLTYPE.STRATEGIZE:
            skillNew = new SkillStrategize(chr);
            break;
        case SKILLTYPE.MANABLOSSOM:
            skillNew = new SkillManaBlossom(chr);
            break;


            //Fischer
            case SKILLTYPE.BUCKLERPARRY:
            skillNew = new SkillBucklerParry(chr);
            break;
        case SKILLTYPE.HARPOONGUN:
            skillNew = new SkillHarpoonGun(chr);
            break;
        case SKILLTYPE.HUNTERSQUARRY:
            skillNew = new SkillHuntersQuarry(chr);
            break;
        case SKILLTYPE.IMPALE:
            skillNew = new SkillImpale(chr);
            break;

        //Katarina
        case SKILLTYPE.CACOPHONY:
            skillNew = new SkillCacophony(chr);
            break;
        case SKILLTYPE.FORTISSIMO:
            skillNew = new SkillFortissimo(chr);
            break;
        case SKILLTYPE.REVERBERATE:
            skillNew = new SkillReverberate(chr);
            break;
        case SKILLTYPE.SERENADE:
            skillNew = new SkillSerenade(chr);
            break;

        //Pit Beast
        case SKILLTYPE.FORCEDEVOLUTION:
            skillNew = new SkillForcedEvolution(chr);
            break;
        case SKILLTYPE.SADISM:
            skillNew = new SkillSadism(chr);
            break;
        case SKILLTYPE.TANTRUM:
            skillNew = new SkillTantrum(chr);
            break;
        case SKILLTYPE.TENDRILSTAB:
            skillNew = new SkillTendrilStab(chr);
            break;

        //Rayne
        case SKILLTYPE.CHEERLEADER:
            skillNew = new SkillCheerleader(chr);
            break;
        case SKILLTYPE.CLOUDCUSHION:
            skillNew = new SkillCloudCushion(chr);
            break;
        case SKILLTYPE.SPIRITSLAP:
            skillNew = new SkillSpiritSlap(chr);
            break;
        case SKILLTYPE.THUNDERSTORM:
            skillNew = new SkillThunderStorm(chr);
            break;

        //Saiko
        case SKILLTYPE.AMBUSH:
            skillNew = new SkillAmbush(chr);
            break;
        case SKILLTYPE.SMOKECOVER:
            skillNew = new SkillSmokeCover(chr);
            break;
        case SKILLTYPE.STICKYBOMB:
            skillNew = new SkillStickyBomb(chr);
            break;
        case SKILLTYPE.TRANQUILIZE:
            skillNew = new SkillTranquilize(chr);
            break;

        //Sophidia
        case SKILLTYPE.HISS:
            skillNew = new SkillHiss(chr);
            break;
        case SKILLTYPE.HYDRASREGEN:
            skillNew = new SkillHydrasRegen(chr);
            break;
        case SKILLTYPE.TWINSNAKES:
            skillNew = new SkillTwinSnakes(chr);
            break;
        case SKILLTYPE.VENEMOUSBITE:
            skillNew = new SkillVenomousBite(chr);
            break;

        case SKILLTYPE.REST:
            skillNew = new SkillRest(chr);
            break;




        default:
            Debug.LogError("ERROR! No constructor for " + skillType + " exists!");
            break;
        }

        return skillNew;
    }

}
