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
        RECON, FLUSHOUT, SNAPTRAP, PLANTSUNFLOWER, SURVEYTHELAND, MULCH, LEECH, TRANSFUSE,
        KNOCKBACK, ADVANCE
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
    {   { RECON, new SkillTypeInfo ( RECON, "Recon", new List<Discipline.DISCIPLINE> { SCOUT } ) },
        { FLUSHOUT, new SkillTypeInfo ( FLUSHOUT, "Flush Out", new List<Discipline.DISCIPLINE> { TRAPPER } ) },
        { SNAPTRAP, new SkillTypeInfo ( SNAPTRAP, "Snap Trap", new List<Discipline.DISCIPLINE> { TRAPPER } ) },
        { PLANTSUNFLOWER, new SkillTypeInfo ( PLANTSUNFLOWER, "Plant Sunflowers", new List<Discipline.DISCIPLINE> { GARDENER } ) },
        { SURVEYTHELAND, new SkillTypeInfo ( SURVEYTHELAND, "Survey the Land", new List<Discipline.DISCIPLINE> { GARDENER } ) },
        { MULCH, new SkillTypeInfo ( MULCH, "Mulch", new List<Discipline.DISCIPLINE> { GARDENER, GIANT } ) },
        { LEECH, new SkillTypeInfo ( LEECH, "Leech", new List<Discipline.DISCIPLINE> { GARDENER, GIANT } ) },
        { TRANSFUSE, new SkillTypeInfo ( TRANSFUSE, "Transfuse", new List<Discipline.DISCIPLINE> { GARDENER, GIANT } ) },
        { KNOCKBACK, new SkillTypeInfo ( KNOCKBACK, "Knockback", new List<Discipline.DISCIPLINE> { TRAPPER} ) },
        { ADVANCE, new SkillTypeInfo ( ADVANCE, "Advance", new List<Discipline.DISCIPLINE> { TRAPPER} ) }
    };


    public static SkillTypeInfo GetSkillTypeInfo(SKILLTYPE skilltype) {
        return dictSkillTypeInfos[skilltype];
    }

    public static List<SkillTypeInfo> GetsSkillsUnderDisciplines(List<Discipline.DISCIPLINE> lstDisciplines) {

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
        case SKILLTYPE.FLUSHOUT:
            skillNew = new SkillAmbush(chr);
            break;
        case SKILLTYPE.MULCH:
            skillNew = new SkillHydrasRegen(chr);
            break;
        case SKILLTYPE.PLANTSUNFLOWER:
            skillNew = new SkillCheerleader(chr);
            break;
        case SKILLTYPE.RECON:
            skillNew = new SkillImpale(chr);
            break;
        case SKILLTYPE.SNAPTRAP:
            skillNew = new SkillLeech(chr);
            break;
        case SKILLTYPE.SURVEYTHELAND:
            skillNew = new SkillHiss(chr);
            break;
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

        default:
            Debug.LogError("ERROR! No constructor for " + skillType + " exists!");
            break;
        }

        return skillNew;
    }

}
