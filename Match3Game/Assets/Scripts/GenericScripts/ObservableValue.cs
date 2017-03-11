using System;

public interface IReadOnlyObservableValue<T>
{
    T Value { get; }
    event Action Changed;
    event Action<T> ValueChanged;
}

public interface IReadOnlyObservableValue<T, TSender> : IReadOnlyObservableValue<T>
{
	event Action<TSender> SenderChanged;
}

public abstract class ObservableValue
{
    public event Action Changed;

    public abstract object RawValue { get; }
    
    public virtual void Reset()
    {
        Changed = null;
    }

    protected virtual void RaiseChanged()
    {
        if (Changed != null)
            Changed();
    }
}

public abstract class ReadonlyObservableValue<T> : ObservableValue, IReadOnlyObservableValue<T>
{
    public T Value { get; protected set; }
    public event Action<T> ValueChanged;

    public sealed override object RawValue { get { return Value; } }
    
    public static implicit operator T(ReadonlyObservableValue<T> property)
    {
        return property.Value;
    }

    public sealed override void Reset()
    {
        base.Reset();

        ValueChanged = null;
        Value = default(T);
    }

    protected override void RaiseChanged()
    {
        base.RaiseChanged();

        if (ValueChanged != null)
            ValueChanged(Value);
    }
}

public class ObservableValue<T> : ReadonlyObservableValue<T>
{
    private bool _changing;

    private static readonly bool IsValueType = typeof(T).IsValueType;

    public ObservableValue()
    { }

    public ObservableValue(T initValue)
    {
        Value = initValue;
    }

    public bool Set(T value, bool forced = false)
	{
        if (_changing)
            return false;

        _changing = true;
        bool changed = forced || !IsEquals(value);

        if (changed)
        {
            Value = value;
            RaiseChanged();
        }
        
        _changing = false;
        return changed;
	}

    protected virtual bool IsEquals(T another)
    {
        return Equals(Value, another);
    }

    public override string ToString()
    {
        var valueStr = IsValueType
            ? Value.ToString()
            : (ReferenceEquals(Value, null) ? "<null>" : Value.ToString());

        return '[' + valueStr + ']';
    }
}

public class ObservableValue<T, TSender> : ObservableValue<T>, IReadOnlyObservableValue<T, TSender>
{
    public event Action<TSender> SenderChanged;

    private readonly TSender _sender;

    public ObservableValue(TSender sender)
        : base()
    {
        _sender = sender;
    }

    public ObservableValue(TSender sender, T initialValue)
        : base(initialValue)
    {
        _sender = sender;
    }

    protected override void RaiseChanged()
    {
        base.RaiseChanged();

        if (SenderChanged != null)
			SenderChanged(_sender);
    }
}


