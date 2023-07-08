#nullable enable
using System;
using UniOption.Runtime;

public struct ValueOption<T> : IEquatable<ValueOption<T>> where T : struct {
    T? _content;

    public static ValueOption<T> Some(T obj) => new() { _content = obj };
    public static ValueOption<T> None        => new();

    public ValueOption<TResult> Map<TResult>(Func<T, TResult> map) where TResult : struct =>
        new() { _content = _content.HasValue ? map(_content.Value) : null };

    public Option<TResult> MapOptional<TResult>(Func<T, Option<TResult>> map) where TResult : class =>
        _content.HasValue ? map(_content.Value) : Option<TResult>.None;

    public T Reduce()               => _content ?? default(T);
    public T Reduce(T ifNone)       => _content ?? ifNone;
    public T Reduce(Func<T> ifNone) => _content ?? ifNone();

    public ValueOption<T> Where(Func<T, bool> predicate) =>
        _content.HasValue && predicate(_content.Value) ? this : None;

    public ValueOption<T> WhereNot(Func<T, bool> predicate) =>
        _content.HasValue && !predicate(_content.Value) ? this : None;

    public ValueOption<T> Do(Action<T?> ifSome) {
        if (_content.HasValue) ifSome(_content.Value);
        return this;
    }

    public ValueOption<T> Do(Action<T?> ifSome, Action ifNone) {
        if (_content.HasValue) ifSome(_content.Value);
        else ifNone();
        return this;
    }

    public static implicit operator ValueOption<T>(T content) => Some(content);

    public override int  GetHashCode()         => _content?.GetHashCode() ?? 0;
    public override bool Equals(object? other) => other is ValueOption<T> option && Equals(option);

    public bool Equals(ValueOption<T> other) => _content.HasValue
                                                    ? other._content.HasValue && _content.Value.Equals(other._content.Value)
                                                    : !other._content.HasValue;

    public static bool operator ==(ValueOption<T> a, ValueOption<T> b) => a.Equals(b);
    public static bool operator !=(ValueOption<T> a, ValueOption<T> b) => !(a.Equals(b));
}