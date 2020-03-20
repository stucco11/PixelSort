using System;

// Code for this Window Switcher found here - https://www.technical-recipes.com/2018/navigating-between-views-in-wpf-mvvm/
public class EventArgs<T> : EventArgs
{
    public EventArgs(T value)
    {
        Value = value;
    }

    public T Value { get; private set; }
}