using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// README ON USAGE FOR THIS CLASS
// For conventional "setting" effects like dealing damage, you can just directly change
// the underlying value with SetBase.   Really, decorations (with AddModifier) should only
// be implemented for overarching changes of 'how' the value should behave - not it's base value


public class Property<T> {

    public delegate T Default();
    public delegate T Modifier(T tBelow);

    public Default baseValue;
    public LinkedList<Modifier> lstModifiers;

    public Subject subChanged = new Subject();

    //Allowed the creation of a Property with just a literal value to be
    // the base value (we will just convert this to a function)
    public Property(T tBase) {
        SetBase(() => tBase);
        lstModifiers = new LinkedList<Modifier>();

    }

    //Require that a base accessor be passed in
    public Property(Default _baseValue) {

        SetBase(_baseValue);
        lstModifiers = new LinkedList<Modifier>();

    }

    public Property(Property<T> pToCopy) {
        baseValue = pToCopy.baseValue;
        lstModifiers = new LinkedList<Modifier>(pToCopy.lstModifiers);

        //Rather than directly copying the existing subChanged (which could lead to stale method references to the original's instances),
        //  we'll rely on the caller of this function to correctly manage the subChanged list on creation 
        subChanged = new Subject();
    }

    public T Get(LinkedListNode<Modifier> nodeCur) {
        //If there's nothing in the linked list, take the base value
        if(nodeCur == null) {
            return baseValue();
        }

        //Fetch the result stored below this node
        T tBelow = Get(nodeCur.Next);

        //Return our modification of that value
        return nodeCur.Value(tBelow);
    }

    public T Get() {
        //Go through the entire list of modifiers to get the desired value

        return Get(lstModifiers.First);
    }

    //Adds a modifier on top of the current modifiers
    // and return a reference to the newly added linked list node
    public LinkedListNode<Modifier> AddModifier(Modifier modifier) {
        LinkedListNode<Modifier> newNode = new LinkedListNode<Modifier>(modifier);

        lstModifiers.AddFirst(newNode);

        //Let observers know that this value has maybe changed
        subChanged.NotifyObs(null);
        return newNode;
    }

    //Removes a given modifier
    public void RemoveModifier(LinkedListNode<Modifier> nodeToRemove) {
        if(lstModifiers.Contains(nodeToRemove.Value) == false) {
            //Just return if that node isn't actually in the linked list already
            return;
        }

        lstModifiers.Remove(nodeToRemove);

        //Let observers know that this value has maybe changed
        subChanged.NotifyObs(null);
    }


    //We can directly modify the base value, if we don't want to 
    //stack a huge of amount of small modifications
    public void SetBase(Default _baseValue) {
        baseValue = _baseValue;

        //Let observers know that this value has maybe changed
        subChanged.NotifyObs(null);
    }

    //A convenient way to redefine the base value
    public void SetBase(T tBase) {
        SetBase(() => tBase);

        //Let observers know that this value has maybe changed
        subChanged.NotifyObs(null);
    }

    //Gets the base value
    public Default GetBase() {

        return baseValue;
    }
}
