using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SelectionSerializer {

    public static byte GetByte(int nPos, int nSerialized) {
        //Positions are (0, 1, 2, 3)
        return (byte)(nSerialized & (255 << (8 * (3 - nPos))) >> (8 * (3 - nPos)));
    }

    public static int Serialize(byte b1, byte b2, byte b3, byte b4) {
        return (b1 << 24) + (b2 << 16) + (b3 << 8) + b4;
    }

    public static Action PeekAction(Chr chrOwner, int nSerialized) {
        return DeserializeAction(chrOwner, GetByte(0, nSerialized));
    }

    //TODONOW - ensure that these ids are set up properly
    public static byte SerializeByte(Chr chr) {
        return (byte)chr.id;
    }
    public static Chr DeserializeChr(byte b) {
        return Chr.lstAllChrs[b];
    }

    public static byte SerializeByte(Action act) {
        return (byte)act.id;
    }
    public static Action DeserializeAction(Chr chrOwner, byte b) {
        return chrOwner.arActions[b];
    }
    //TODO:: add Soul and WorldSoul serializers

    // ***************** BASE SELECTIONS CLASS ******************

    public class SelectionInfo {
        public Chr chrOwner;
        public Action actUsed;

        public SelectionInfo(Chr _chrOwner, int nSerialized) {
            chrOwner = _chrOwner;
            actUsed = DeserializeAction(chrOwner, GetByte(0, nSerialized));
        }
    };

    // *****************   CHR SELECTIONS   *****************
    public static int SerializeChrSelection(Action act, Chr chrSelect, byte bExtra1=0, byte bExtra2=0) {
        return Serialize(SerializeByte(act), SerializeByte(chrSelect), bExtra1, bExtra2);
    }

    public class SelectionChr : SelectionInfo {
        public Chr chrSelected;
        public byte bExtra1;
        public byte bExtra2;

        public SelectionChr(Chr _chrOwner, int nSerialized) : base(_chrOwner, nSerialized) {
            chrSelected = DeserializeChr(GetByte(1, nSerialized));
            bExtra1 = GetByte(2, nSerialized);
            bExtra2 = GetByte(3, nSerialized);
        }
    }

    public static SelectionChr DeserializeChrSelection(Chr chrOwner, int nSerialized) {
        return new SelectionChr(chrOwner, nSerialized);
    }


    // *****************   ACTION SELECTIONS   *****************
    public static int SerializeActionSelection(Action actUsed, Chr chrSelected, Action actSelected, byte bExtra1=0) {
        return Serialize(SerializeByte(actUsed), SerializeByte(chrSelected), SerializeByte(actSelected), bExtra1);
    }

    public class SelectionAction : SelectionInfo {
        public Chr chrSelected;
        public Action actSelected;
        public byte bExtra1;

        public SelectionAction(Chr _chrOwner, int nSerialized) : base(_chrOwner, nSerialized) {
            chrSelected = DeserializeChr(GetByte(1, nSerialized));
            actSelected = DeserializeAction(chrSelected, GetByte(2, nSerialized));
            bExtra1 = GetByte(3, nSerialized);
        }
    }

    public static SelectionAction DeserializeActionSelection(Chr chrOwner, int nSerialized) {
        return new SelectionAction(chrOwner, nSerialized);
    }

    // *****************   ACTION SELECTIONS   *****************
    public static int SerializeSpecialSelection(Action actUsed, byte bExtra1=0, byte bExtra2=0, byte bExtra3=0) {
        return Serialize(SerializeByte(actUsed), bExtra1, bExtra2, bExtra3);
    }

    public class SelectionSpecial : SelectionInfo {
        public byte bExtra1;
        public byte bExtra2;
        public byte bExtra3;

        public SelectionSpecial(Chr _chrOwner, int nSerialized) : base(_chrOwner, nSerialized) {
            bExtra1 = GetByte(1, nSerialized);
            bExtra2 = GetByte(2, nSerialized);
            bExtra3 = GetByte(3, nSerialized);
        }
    }

    public static SelectionSpecial DeserializeSpecialSelection(Chr chrOwner, int nSerialized) {
        return new SelectionSpecial(chrOwner, nSerialized);
    }


    // *****************   TARGETLESS SELECTIONS   *****************
    public static int SerializeTargetlessSelection(Action actUsed) {
        //Just use a 'custom' selection serialization where we don't need to use any of the parameters
        return SerializeSpecialSelection(actUsed);
    }

}
