using System.Collections;
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
        return (byte)chr.id;
    }
    public static Chr DeserializeChr(byte b) {
        return ChrCollection.Get().GetChr(b);
    }
    public static Chr DeserializeChr(int n) {
        return DeserializeChr((byte)n);
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
        return Match.Get().arPlayers[b];
    }

    public static int SerializeByte(SkillSlot skillslot) {
        return Serializer.Serialize((byte)skillslot.chrOwner.id, (byte)skillslot.iSlot, 0, 0);
    }

    public static SkillSlot DeserializeSkillSlot(int nSerializedSkill) {
        Chr chrUsing = Serializer.DeserializeChr(Serializer.GetByte(0, nSerializedSkill));
        int iSkillSlot = Serializer.GetByte(1, nSerializedSkill);

        return chrUsing.arSkillSlots[iSkillSlot];
    }

    public static int SerializeByte(Position.Coords coords) {
        return ContPositions.CoordsToIndex(coords);
    }

    public static Position.Coords DeserializeCoords(int nSerializedCoords) {
        return ContPositions.IndexToCoords(nSerializedCoords);
    }

    //We'll just be implementing the serialization of positions through their coordinates
    public static int SerializeByte(Position pos) {
        return SerializeByte(pos.coords);
    }

    public static Position DeserializePosition(int nSerializedPosition) {
        return ContPositions.Get().GetPosition(DeserializeCoords(nSerializedPosition));
    }
}


