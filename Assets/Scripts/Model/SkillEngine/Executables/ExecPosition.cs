using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ExecPosition : Executable {

    public Position posTarget;

    public override bool isLegal() {

        //If any legality checks for positions are needed, add them here


        return base.isLegal();
    }

    public ExecPosition(Chr _chrSource, Position _posTarget) : base(_chrSource) {
        posTarget = _posTarget;
    }

    public ExecPosition(ExecPosition other) : base(other) {
        posTarget = other.posTarget;
    }

}
