using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SelectionSerializer {

    public static byte GetByte(int nPos, int nSerialized) {
        //Positions are (0, 1, 2, 3)

        return (byte)((nSerialized & (255 << (8 * (3 - nPos)))) >> (8 * (3 - nPos)));
    }

    public static int Serialize(byte b1, byte b2, byte b3, byte b4) {
        return (b1 << 24) + (b2 << 16) + (b3 << 8) + b4;
    }

    public static Skill PeekSkill(Chr chrOwner, int nSerialized) {
        return DeserializeSkill(chrOwner, GetByte(0, nSerialized));
    }

    public static byte SerializeByte(Chr chr) {
        return (byte)chr.globalid;
    }
    public static Chr DeserializeChr(byte b) {
        return Chr.lstAllChrs[b];
    }

    public static byte SerializeByte(Skill skill) {
        return (byte)skill.skillslot.iSlot;
    }
    public static Skill DeserializeSkill(Chr chrOwner, byte b) {
        return chrOwner.arSkillSlots[b].skill;
    }

    public static byte SerializeByte(Player plyr) {
        return (byte)plyr.id;
    }

    public static Player DeserializePlayer(byte b) {
        return Player.lstAllPlayers[b];
    }

    //TODO:: add Soul and WorldSoul serializers

    // ***************** BASE SELECTIONS CLASS ******************

    public abstract class SelectionInfo {
        public Chr chrOwner;
        public Skill skillUsed;

        public SelectionInfo(Chr _chrOwner, Skill _skillUsed) {
            chrOwner = _chrOwner;
            skillUsed = _skillUsed;
        }

        public SelectionInfo(Chr _chrOwner, int nSerialized) {
            chrOwner = _chrOwner;
            skillUsed = DeserializeSkill(chrOwner, GetByte(0, nSerialized));
        }

        //Give a constructor for a single byte if making custom selectionInfos outside of the standard process
        public SelectionInfo(Chr _chrOwner, byte bSkill) {
            chrOwner = _chrOwner;
            skillUsed = DeserializeSkill(chrOwner, bSkill);
        }

        public SelectionInfo(SelectionInfo other) {
            chrOwner = other.chrOwner;
            skillUsed = other.skillUsed;
        }

        public abstract int Serialize();

        public abstract SelectionInfo GetCopy();

        public override string ToString() {
            return chrOwner.sName + " using " + skillUsed.sName;
        }

        public bool CanSelect() {
            return skillUsed.CanSelect(this);
        }
    };

    // *****************   CHR SELECTIONS   *****************
    public static int SerializeChrSelection(Skill skill, Chr chrSelect, byte bExtra1 = 0, byte bExtra2 = 0) {
        return Serialize(SerializeByte(skill), SerializeByte(chrSelect), bExtra1, bExtra2);
    }

    public class SelectionChr : SelectionInfo {
        public Chr chrSelected;
        public byte bExtra1;
        public byte bExtra2;

        public SelectionChr(Chr _chrOwner, Skill _skillUsed, Chr _chrSelected, byte _bExtra1 = 0, byte _bExtra2 = 0) : base(_chrOwner, _skillUsed) {
            chrSelected = _chrSelected;
            bExtra1 = _bExtra1;
            bExtra2 = _bExtra2;
        }

        public SelectionChr(Chr _chrOwner, int nSerialized) : base(_chrOwner, nSerialized) {
            chrSelected = DeserializeChr(GetByte(1, nSerialized));
            bExtra1 = GetByte(2, nSerialized);
            bExtra2 = GetByte(3, nSerialized);
        }

        public SelectionChr(SelectionChr other) : base(other) {
            chrSelected = other.chrSelected;
            bExtra1 = other.bExtra1;
            bExtra2 = other.bExtra2;
        }

        public override SelectionInfo GetCopy() {
            return new SelectionChr(this);
        }

        public override int Serialize() {
            return SerializeChrSelection(skillUsed, chrSelected, bExtra1, bExtra2);
        }

        public override string ToString() {
            return base.ToString() + " on " + chrSelected.sName + "(" + bExtra1 + ", " + bExtra2 + ")";
        }
    }

    public static SelectionChr DeserializeChrSelection(Chr chrOwner, int nSerialized) {
        return new SelectionChr(chrOwner, nSerialized);
    }


    // *****************   SKILL SELECTIONS   *****************
    public static int SerializeSkillSelection(Skill skillUsed, Chr chrSelected, Skill skillSelected, byte bExtra1 = 0) {
        return Serialize(SerializeByte(skillUsed), SerializeByte(chrSelected), SerializeByte(skillSelected), bExtra1);
    }

    public class SelectionSkill : SelectionInfo {
        public Chr chrSelected;
        public Skill skillSelected;
        public byte bExtra1;

        public SelectionSkill(Chr _chrOwner, Skill _skillUsed, Chr _chrSelected, Skill _skillSelected, byte _bExtra1 = 0) : base(_chrOwner, _skillUsed) {
            chrSelected = _chrSelected;
            skillSelected = _skillSelected;
            bExtra1 = _bExtra1;
        }

        public SelectionSkill(Chr _chrOwner, int nSerialized) : base(_chrOwner, nSerialized) {
            chrSelected = DeserializeChr(GetByte(1, nSerialized));
            skillSelected = DeserializeSkill(chrSelected, GetByte(2, nSerialized));
            bExtra1 = GetByte(3, nSerialized);
        }

        public SelectionSkill(SelectionSkill other) : base(other) {
            chrSelected = other.chrSelected;
            skillSelected = other.skillSelected;
            bExtra1 = other.bExtra1;
        }

        public override SelectionInfo GetCopy() {
            return new SelectionSkill(this);
        }

        public override int Serialize() {
            return SerializeSkillSelection(skillUsed, chrSelected, skillSelected, bExtra1);
        }

        public override string ToString() {
            return base.ToString() + " on " + chrSelected.sName + "'s " + skillSelected.sName + "(" + bExtra1 + ")"; ;
        }
    }

    public static SelectionSkill DeserializeSkillSelection(Chr chrOwner, int nSerialized) {
        return new SelectionSkill(chrOwner, nSerialized);
    }

    // *****************   SPECIAL SELECTIONS   *****************
    public static int SerializeSpecialSelection(Skill skillUsed, byte bExtra1 = 0, byte bExtra2 = 0, byte bExtra3 = 0) {
        return Serialize(SerializeByte(skillUsed), bExtra1, bExtra2, bExtra3);
    }

    public class SelectionSpecial : SelectionInfo {
        public byte bExtra1;
        public byte bExtra2;
        public byte bExtra3;

        public SelectionSpecial(Chr _chrOwner, Skill _skillUsed, byte _bExtra1 = 0, byte _bExtra2 = 0, byte _bExtra3 = 0) : base(_chrOwner, _skillUsed) {
            bExtra1 = _bExtra1;
            bExtra2 = _bExtra2;
            bExtra3 = _bExtra3;
        }

        public SelectionSpecial(Chr _chrOwner, int nSerialized) : base(_chrOwner, nSerialized) {
            bExtra1 = GetByte(1, nSerialized);
            bExtra2 = GetByte(2, nSerialized);
            bExtra3 = GetByte(3, nSerialized);
        }

        public SelectionSpecial(SelectionSpecial other) : base(other) {
            bExtra1 = other.bExtra1;
            bExtra2 = other.bExtra2;
            bExtra3 = other.bExtra3;
        }

        public override SelectionInfo GetCopy() {
            return new SelectionSpecial(this);
        }

        public override int Serialize() {
            return SerializeSpecialSelection(skillUsed, bExtra1, bExtra2, bExtra3);
        }

        public override string ToString() {
            return base.ToString() + " with " + "(" + bExtra1 + ", " + bExtra2 + ", " + bExtra3 + ")";
        }
    }

    public static SelectionSpecial DeserializeSpecialSelection(Chr chrOwner, int nSerialized) {
        return new SelectionSpecial(chrOwner, nSerialized);
    }

    // *****************   PLAYER SELECTIONS   *****************
    public static int SerializePlayerSelection(Skill skillUsed, Player plyrSelect, byte bExtra1 = 0, byte bExtra2 = 0) {
        return Serialize(SerializeByte(skillUsed), SerializeByte(plyrSelect), bExtra1, bExtra2);
    }

    public class SelectionPlayer : SelectionInfo {
        public Player plyrSelected;
        public byte bExtra1;
        public byte bExtra2;

        public SelectionPlayer(Chr _chrOwner, Skill _skillUsed, Player _plyrSelected, byte _bExtra1 = 0, byte _bExtra2 = 0) : base(_chrOwner, _skillUsed) {
            plyrSelected = _plyrSelected;
            bExtra1 = _bExtra1;
            bExtra2 = _bExtra2;
        }

        public SelectionPlayer(Chr _chrOwner, int nSerialized) : base(_chrOwner, nSerialized) {
            plyrSelected = DeserializePlayer(GetByte(1, nSerialized));
            bExtra1 = GetByte(2, nSerialized);
            bExtra2 = GetByte(3, nSerialized);
        }

        public SelectionPlayer(SelectionPlayer other) : base(other) {
            plyrSelected = other.plyrSelected;
            bExtra1 = other.bExtra1;
            bExtra2 = other.bExtra2;
        }

        public override SelectionInfo GetCopy() {
            return new SelectionPlayer(this);
        }

        public override int Serialize() {
            return SerializePlayerSelection(skillUsed, plyrSelected, bExtra1, bExtra2);
        }

        public override string ToString() {
            return base.ToString() + " on Player " + plyrSelected.id + " with " + "(" + bExtra1 + ", " + bExtra2 + ")";
        }
    }

    public static SelectionSpecial DeserializePlayerSelection(Chr chrOwner, int nSerialized) {
        return new SelectionSpecial(chrOwner, nSerialized);
    }


    // *****************   TARGETLESS SELECTIONS   *****************
    public static int SerializeTargetlessSelection(Skill skillUsed) {
        //Just use a 'custom' selection serialization where we don't need to use any of the parameters
        return SerializeSpecialSelection(skillUsed);
    }

    public static SelectionInfo MakeRestSelection(Chr chrOwner) {
        return new SelectionSpecial(chrOwner, (byte)Chr.nRestSlotId);
    }

    public static int SerializeRest() {
        return Serialize(Chr.nRestSlotId, 0, 0, 0);
    }

    //Remember to cast the result to the type that you expect
    public static SelectionInfo Deserialize(Chr chrOwner, int nSerialized) {

        Skill skillUsed = PeekSkill(chrOwner, nSerialized);

        //Switch on the type of selection that this skill will be using
        switch(skillUsed.GetDominantClause().targetType) {
        case Clause.TargetType.CHR:
            return new SelectionChr(chrOwner, nSerialized);

        case Clause.TargetType.SKILL:
            return new SelectionSkill(chrOwner, nSerialized);

        case Clause.TargetType.PLAYER:
            return new SelectionPlayer(chrOwner, nSerialized);

        case Clause.TargetType.SPECIAL:
            return new SelectionSpecial(chrOwner, nSerialized);

        }

        Debug.LogError("Unrecognized targetting type for " + skillUsed);

        return null;

    }
}
