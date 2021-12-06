﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Serializer {

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
        return Player.arAllPlayers[b];
    }

    public static int SerializeSkill(Skill skill) {
        return Serializer.Serialize((byte)skill.chrOwner.globalid, (byte)skill.skillslot.iSlot, 0, 0);
    }

    public static Skill DeserializeSkill(int nSerializedSkill) {
        Chr chrUsing = Serializer.DeserializeChr(Serializer.GetByte(0, nSerializedSkill));
        int iSkillSlot = Serializer.GetByte(1, nSerializedSkill);

        return chrUsing.arSkillSlots[iSkillSlot].skill;
    }
}


