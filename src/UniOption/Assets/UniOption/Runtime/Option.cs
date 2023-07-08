#nullable enable
using System;
using Cysharp.Threading.Tasks;

namespace UniOption.Runtime {
    public class Option<T> where T : class {
        readonly T? _content;
        Option(T? content = null) => _content = content;

        public static Option<T> Some(T? content) => new(content);
        public static Option<T> None             => new();

        public Option<TResult> Map<TResult>(Func<T, TResult> map) where TResult : class => _content is null
                                                                                               ? Option<TResult>.None
                                                                                               : Option<TResult>.Some(map(_content));

        public ValueOption<TResult> MapValue<TResult>(Func<T, TResult> map) where TResult : struct =>
            _content is not null ? ValueOption<TResult>.Some(map(_content)) : ValueOption<TResult>.None;

        public T Reduce(T defaultValue)       => _content ?? defaultValue;
        public T Reduce(Func<T> defaultValue) => _content ?? defaultValue();

        public Option<T> Do(Action<T> ifSome) {
            if (_content is not null) ifSome(_content);
            return this;
        }

        public Option<T> Do(Action<T> ifSome, Action ifNone) {
            if (_content is not null) ifSome(_content);
            else ifNone();
            return this;
        }

        public async UniTask<Option<T>> DoAsync(Func<T, UniTask> ifSome) {
            if (_content is not null) await ifSome(_content);
            return this;
        }

        public Option<T>      Where(Func<T, bool> predicate)        => _content is null || predicate(_content) ? this : None;
        public Option<TValue> OfType<TValue>() where TValue : class => _content is TValue content ? Option<TValue>.Some(content) : Option<TValue>.None;

        public static implicit operator Option<T>(T? content) => content is not null ? Some(content) : None;

        public static bool operator ==(Option<T>? a, Option<T>? b) => a?.Equals(b) ?? b is null;
        public static bool operator !=(Option<T>? a, Option<T>? b) => !(a == b);

        public override int  GetHashCode()       => _content?.GetHashCode() ?? 0;
        public override bool Equals(object? obj) => this.Equals(obj as Option<T>);

        public bool Equals(Option<T>? obj) => obj is not null && (_content?.Equals(obj._content) ?? false);
    }
}