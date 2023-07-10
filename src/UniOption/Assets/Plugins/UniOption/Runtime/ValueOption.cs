#nullable enable
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace UniOption {
    public readonly struct ValueOption<T> : IOption, IEquatable<ValueOption<T>> where T : struct {
        readonly T? _content;

        public static ValueOption<T> Some(T obj) => new(obj);
        public static ValueOption<T> None        => new();
        ValueOption(T? content = null) => _content = content;
        public bool IsSome => _content.HasValue;
        public bool IsNone => !_content.HasValue;

        public ValueOption<TResult> Map<TResult>(Func<T, TResult> map) where TResult : struct      => new(IsSome ? map(_content!.Value) : null);
        public Option<TResult>      MapObject<TResult>(Func<T, TResult> map) where TResult : class => IsSome ? map(_content!.Value) : Option<TResult>.None;

        public bool           IsSomeAnd(Func<T, bool> predicate)                        => IsSome && predicate(_content!.Value);
        public T              Reduce()                                                  => _content ?? default(T);
        public T              Reduce(T ifNone)                                          => _content ?? ifNone;
        public T              Reduce(Func<T> ifNone)                                    => _content ?? ifNone();
        public ValueOption<T> Where(Func<T, bool> predicate)                            => IsSome && predicate(_content!.Value) ? this : None;
        public ValueOption<T> WhereNot(Func<T, bool> predicate)                         => IsSome && !predicate(_content!.Value) ? this : None;
        public ValueOption<T> Or(T orOption)                                            => IsNone ? orOption : this;
        public TResult        Match<TResult>(Func<T, TResult> some, Func<TResult> none) => IsSome ? some(_content!.Value) : none();
        public IEnumerable<T> ToEnumerable()                                            => IsSome ? new[] { _content!.Value } : Array.Empty<T>();

        public ValueOption<(T, T2)> Zip<T2>(T2 other) where T2 : struct =>
            IsSome ? ValueOption<(T, T2)>.Some((_content!.Value, other)) : ValueOption<(T, T2)>.None;

        public ValueOption<(T, T2)> Zip<T2>(Option<T2> other) where T2 : class =>
            IsSome && other.IsSome ? ValueOption<(T, T2)>.Some((_content!.Value, other.Reduce()!)) : ValueOption<(T, T2)>.None;

        public ValueOption<T> Do(Action<T> ifSome) {
            if (_content.HasValue) ifSome(_content.Value);
            return this;
        }

        public ValueOption<T> Do(Action<T> ifSome, Action ifNone) {
            if (_content.HasValue) ifSome(_content.Value);
            else ifNone();
            return this;
        }

        public async UniTask<ValueOption<T>> DoAsync(Func<T, UniTask> ifSome) {
            if (IsSome) await ifSome(_content!.Value);
            return this;
        }

        public async UniTask<ValueOption<T>> DoAsync(Func<T, UniTask> ifSome, Func<UniTask> ifNone) {
            if (IsSome) await ifSome(_content!.Value);
            else await ifNone();
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
}