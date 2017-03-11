using System;

public class BoolObservableValue : ObservableValue<bool>
{
    public BoolObservableValue()
    { }

    public BoolObservableValue(bool value)
        : base(value)
    { }

    protected override bool IsEquals(bool another)
    {
        return Value == another;
    }
}

public class BoolObservableValue<TSender> : ObservableValue<bool, TSender>
{
    public BoolObservableValue(TSender owner)
        : base(owner)
    { }

    public BoolObservableValue(TSender owner, bool value)
        : base(owner, value)
    { }

    protected override bool IsEquals(bool another)
    {
        return Value == another;
    }
}

public class IntObservableValue : ObservableValue<int>
{
    public IntObservableValue()
    { }

    public IntObservableValue(int value)
        : base(value)
    { }

    protected override bool IsEquals(int another)
    {
        return Value == another;
    }

    // Set Helpers
    public bool Add(int delta)
    {
        return Set(Value + delta);
    }

    public bool Subtract(int delta)
    {
        return Set(Value - delta);
    }

    public bool Clamp(int min, int max)
    {
        var res = Value;
        if (res < min)
            res = min;

        if (res > max)
            res = max;

        return Set(res);
    }
}

public class IntObservableValue<TSender> : ObservableValue<int, TSender>
{
    public IntObservableValue(TSender sender)
        : base(sender)
    { }

    public IntObservableValue(TSender sender, int value)
        : base(sender, value)
    { }

    protected override bool IsEquals(int another)
    {
        return Value == another;
    }

    // Set Helpers
    public bool Add(int delta)
    {
        return Set(Value + delta);
    }

    public bool Subtract(int delta)
    {
        return Set(Value - delta);
    }

    public bool Clamp(int min, int max)
    {
        var res = Value;
        if (res < min)
            res = min;

        if (res > max)
            res = max;

        return Set(res);
    }
}

public class FloatObservableValue : ObservableValue<float>
{
    public FloatObservableValue()
    { }

    public FloatObservableValue(float value)
        : base(value)
    { }

    protected override bool IsEquals(float another)
    {
        return Math.Abs(Value - another) < 0.000000001f;
    }

    // Set Helpers
    public bool Add(float delta)
    {
        return Set(Value + delta);
    }

    public bool Subtract(float delta)
    {
        return Set(Value - delta);
    }
}

public class FloatObservableValue<TSender> : ObservableValue<float, TSender>
{
    public FloatObservableValue(TSender sender)
        : base(sender)
    { }

    public FloatObservableValue(TSender sender, float value)
        : base(sender, value)
    { }

    protected override bool IsEquals(float another)
    {
        return Math.Abs(Value - another) < 0.000000001f;
    }
}

public class DoubleObservableValue : ObservableValue<double>
{
    protected override bool IsEquals(double another)
    {
        return Math.Abs(Value - another) < double.Epsilon;
    }
}

public class StringObservableValue : ObservableValue<string>
{
    protected override bool IsEquals(string another)
    {
        return Value == another;
    }
}

public class DateTimeObservableValue : ObservableValue<DateTime>
{
    protected override bool IsEquals(DateTime another)
    {
        return Value == another;
    }
}

public class TimeSpanObservableValue : ObservableValue<TimeSpan>
{
    protected override bool IsEquals(TimeSpan another)
    {
        return Value == another;
    }
}

public class Vector3ObservableValue : ObservableValue<UnityEngine.Vector3>
{
    protected override bool IsEquals(UnityEngine.Vector3 another)
    {
        return Value == another;
    }
}

public static class ObservableValueLegacyExtensions
{
    public static void Bind<T>(this IReadOnlyObservableValue<T> property, Action callback)
    {
        if (callback != null)
        {
            property.Changed += callback;
            callback();
        }
    }

    public static void Bind<T>(this IReadOnlyObservableValue<T> property, Action<T> callback)
    {
        if (callback != null)
        {
            property.ValueChanged += callback;
            callback(property.Value);
        }
    }

    public static void Detach<T>(this IReadOnlyObservableValue<T> property, Action callback)
    {
        property.Changed -= callback;
    }

    public static void Detach<T>(this IReadOnlyObservableValue<T> property, Action<T> callback)
    {
        property.ValueChanged -= callback;
    }
}
