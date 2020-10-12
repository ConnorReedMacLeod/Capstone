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
        public Action actUsed;

        public SelectionInfo(Chr _chrOwner, Action _actUsed) {
            chrOwner = _chrOwner;
            actUsed = _actUsed;
        }

        public SelectionInfo(Chr _chrOwner, int nSerialized) {
            chrOwner = _chrOwner;
            actUsed = DeserializeAction(chrOwner, GetByte(0, nSerialized));
        }

        //Give a constructor for a single byte if making custom selectionInfos outside of the standard process
        public SelectionInfo(Chr _chrOwner, byte bAction) {
            chrOwner = _chrOwner;
            actUsed = DeserializeAction(chrOwner, bAction);
        }

        public SelectionInfo(SelectionInfo other) {
            chrOwner = other.chrOwner;
            actUsed = other.actUsed;
        }

        public abstract int Serialize();

        public abstract SelectionInfo GetCopy();

        public override string ToString() {
            return chrOwner.sName + " using " + actUsed.sName;
        }

        public bool CanActivate() {
            return actUsed.CanActivate(this);
        }
    };

    // *****************   CHR SELECTIONS   *****************
    public static int SerializeChrSelection(Action act, Chr chrSelect, byte bExtra1 = 0, byte bExtra2 = 0) {
        return Serialize(SerializeByte(act), SerializeByte(chrSelect), bExtra1, bExtra2);
    }

    public class SelectionChr : SelectionInfo {
        public Chr chrSelected;
        public byte bExtra1;
        public byte bExtra2;

        public SelectionChr(Chr _chrOwner, Action _actUsed, Chr _chrSelected, byte _bExtra1 = 0, byte _bExtra2 = 0) : base(_chrOwner, _actUsed) {
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
            return SerializeChrSelection(actUsed, chrSelected, bExtra1, bExtra2);
        }

        public override string ToString() {
            return base.ToString() + " on " + chrSelected.sName + "(" + bExtra1 + ", " + bExtra2 + ")";
        }
    }

    public static SelectionChr DeserializeChrSelection(Chr chrOwner, int nSerialized) {
        return new SelectionChr(chrOwner, nSerialized);
    }


    // *****************   ACTION SELECTIONS   *****************
    public static int SerializeActionSelection(Action actUsed, Chr chrSelected, Action actSelected, byte bExtra1 = 0) {
        return Serialize(SerializeByte(actUsed), SerializeByte(chrSelected), SerializeByte(actSelected), bExtra1);
    }

    public class SelectionAction : SelectionInfo {
        public Chr chrSelected;
        public Action actSelected;
        public byte bExtra1;

        public SelectionAction(Chr _chrOwner, Action _actUsed, Chr _chrSelected, Action _actSelected, byte _bExtra1 = 0) : base(_chrOwner, _actUsed) {
            chrSelected = _chrSelected;
            actSelected = _actSelected;
            bExtra1 = _bExtra1;
        }

        public SelectionAction(Chr _chrOwner, int nSerialized) : base(_chrOwner, nSerialized) {
            chrSelected = DeserializeChr(GetByte(1, nSerialized));
            actSelected = DeserializeAction(chrSelected, GetByte(2, nSerialized));
            bExtra1 = GetByte(3, nSerialized);
        }

        public SelectionAction(SelectionAction other) : base(other) {
            chrSelected = other.chrSelected;
            actSelected = other.actSelected;
            bExtra1 = other.bExtra1;
        }

        public override SelectionInfo GetCopy() {
            return new SelectionAction(this);
        }

        public override int Serialize() {
            return SerializeActionSelection(actUsed, chrSelected, actSelected, bExtra1);
        }

        public override string ToString() {
            return base.ToString() + " on " + chrSelected.sName + "'s " + actSelected.sName + "(" + bExtra1 + ")"; ;
        }
    }

    public static SelectionAction DeserializeActionSelection(Chr chrOwner, int nSerialized) {
        return new SelectionAction(chrOwner, nSerialized);
    }

    // *****************   SPECIAL SELECTIONS   *****************
    public static int SerializeSpecialSelection(Action actUsed, byte bExtra1 = 0, byte bExtra2 = 0, byte bExtra3 = 0) {
        return Serialize(SerializeByte(actUsed), bExtra1, bExtra2, bExtra3);
    }

    public class SelectionSpecial : SelectionInfo {
        public byte bExtra1;
        public byte bExtra2;
        public byte bExtra3;

        public SelectionSpecial(Chr _chrOwner, Action _actUsed, byte _bExtra1 = 0, byte _bExtra2 = 0, byte _bExtra3 = 0) : base(_chrOwner, _actUsed) {
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
            return SerializeSpecialSelection(actUsed, bExtra1, bExtra2, bExtra3);
        }

        public override string ToString() {
            return base.ToString() + " with " + "(" + bExtra1 + ", " + bExtra2 + ", " + bExtra3 + ")";
        }
    }

    public static SelectionSpecial DeserializeSpecialSelection(Chr chrOwner, int nSerialized) {
        return new SelectionSpecial(chrOwner, nSerialized);
    }

    // *****************   PLAYER SELECTIONS   *****************
    public static int SerializePlayerSelection(Action actUsed, Player plyrSelect, byte bExtra1 = 0, byte bExtra2 = 0) {
        return Serialize(SerializeByte(actUsed), SerializeByte(plyrSelect), bExtra1, bExtra2);
    }

    public class SelectionPlayer : SelectionInfo {
        public Player plyrSelected;
        public byte bExtra1;
        public byte bExtra2;

        public SelectionPlayer(Chr _chrOwner, Action _actUsed, Player _plrSelected, byte _bExtra1 = 0, byte _bExtra2 = 0) : base(_chrOwner, _actUsed) {
            plyrSelected = _plrSelected;
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
            return SerializePlayerSelection(actUsed, plyrSelected, bExtra1, bExtra2);
        }

        public override string ToString() {
            return base.ToString() + " on Player " + plyrSelected.id + " with " + "(" + bExtra1 + ", " + bExtra2 + ")";
        }
    }

    public static SelectionSpecial DeserializePlayerSelection(Chr chrOwner, int nSerialized) {
        return new SelectionSpecial(chrOwner, nSerialized);
    }


    // *****************   TARGETLESS SELECTIONS   *****************
    public static int SerializeTargetlessSelection(Action actUsed) {
        //Just use a 'custom' selection serialization where we don't need to use any of the parameters
        return SerializeSpecialSelection(actUsed);
    }

    public static SelectionInfo MakeRestSelection(Chr chrOwner) {
        return new SelectionSpecial(chrOwner, (byte)Chr.idResting);
    }

    public static int SerializeRest() {
        return Serialize(Chr.idResting, 0, 0, 0);
    }

    //Remember to cast the result to the type that you expect
    public static SelectionInfo Deserialize(Chr chrOwner, int nSerialized) {

        Action actUsed = PeekAction(chrOwner, nSerialized);

        //Switch on the type of selection that this action will be using
        switch(actUsed.GetDominantClause().targetType) {
        case Clause.TargetType.CHR:
            return new SelectionChr(chrOwner, nSerialized);

        case Clause.TargetType.ACTION:
            return new SelectionAction(chrOwner, nSerialized);

        case Clause.TargetType.PLAYER:
            return new SelectionPlayer(chrOwner, nSerialized);

        case Clause.TargetType.SPECIAL:
            return new SelectionSpecial(chrOwner, nSerialized);

        }

        Debug.LogError("Unrecognized targetting type for " + actUsed);

        return null;

    }
}
