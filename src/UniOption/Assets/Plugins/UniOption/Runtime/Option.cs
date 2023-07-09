#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace UniOption {
    public class Option<T> where T : class {
        readonly T? _content;

        public static Option<T> Some(T? content) => new(content);
        public static Option<T> None             => new();

        Option(T? content = null) => _content = content;
        public bool IsSome => _content is not null;
        public bool IsNone => _content is null;

        public Option<TResult> Map<TResult>(Func<T, TResult> map) where TResult : class => IsNone ? Option<TResult>.None : Option<TResult>.Some(map(_content!));

        public ValueOption<TResult> MapValue<TResult>(Func<T, TResult> map) where TResult : struct =>
            IsSome ? ValueOption<TResult>.Some(map(_content!)) : ValueOption<TResult>.None;

        public bool           IsSomeAnd(Func<T, bool> predicate) => IsSome && predicate(_content!);
        public T?             Reduce() => _content ?? default;
        public T              Reduce(T defaultValue) => _content ?? defaultValue;
        public T              Reduce(Func<T> defaultValue) => _content ?? defaultValue();
        public Option<T>      Where(Func<T, bool> predicate) => IsNone || predicate(_content!) ? this : None;
        public Option<T>      WhereNot(Func<T, bool> predicate) => IsNone || !predicate(_content!) ? this : None;
        public Option<TValue> OfType<TValue>() where TValue : class => _content is TValue content ? Option<TValue>.Some(content) : Option<TValue>.None;
        public Option<T>      Or(T orOption) => IsNone ? orOption : this;
        public TResult        Match<TResult>(Func<T, TResult> some, Func<TResult> none) => IsSome ? some(_content!) : none();
        public IEnumerable<T> ToEnumerable() => (IsSome ? new[] { _content } : Array.Empty<T>())!;

        public Option<Tuple<T, T2>> ZipTuple<T2>(T2 other) where T2 : class =>
            IsSome ? Option<Tuple<T, T2>>.Some(new Tuple<T, T2>(_content!, other)) : Option<Tuple<T, T2>>.None;

        public Option<T> Do(Action<T> ifSome) {
            if (IsSome) ifSome(_content!);
            return this;
        }

        public Option<T> Do(Action<T> ifSome, Action ifNone) {
            if (IsSome) ifSome(_content!);
            else ifNone();
            return this;
        }

        public async UniTask<Option<T>> DoAsync(Func<T, UniTask> ifSome) {
            if (IsSome) await ifSome(_content!);
            return this;
        }

        public async UniTask<Option<T>> DoAsync(Func<T, UniTask> ifSome, Func<UniTask> ifNone) {
            if (IsSome) await ifSome(_content!);
            else await ifNone();
            return this;
        }

        public static implicit operator Option<T>(T? content) => content is not null ? Some(content) : None;

        public static bool operator ==(Option<T>? a, Option<T>? b) => a?.Equals(b) ?? b is null;
        public static bool operator !=(Option<T>? a, Option<T>? b) => !(a == b);

        public override int  GetHashCode()       => _content?.GetHashCode() ?? 0;
        public override bool Equals(object? obj) => this.Equals(obj as Option<T>);

        public bool Equals(Option<T>? obj) => obj is not null && (_content?.Equals(obj._content) ?? false);
    }
}