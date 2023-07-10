#nullable enable
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace UniOption {
    public readonly struct ValueOption<T> : IOption, IEquatable<ValueOption<T>> where T : struct {
        readonly T? _content;

        /// <summary>
        /// Creates a new ValueOption with a Some value.
        /// </summary>
        /// <param name="obj">The value to wrap.</param>
        /// <returns>A ValueOption with the specified value.</returns>
        public static ValueOption<T> Some(T obj) => new(obj);

        /// <summary>
        /// Gets a ValueOption with a None value.
        /// </summary>
        public static ValueOption<T> None => new();

        ValueOption(T? content = null) => _content = content;

        /// <summary>
        /// Gets a value indicating whether this ValueOption has a Some value.
        /// </summary>
        public bool IsSome => _content.HasValue;

        /// <summary>
        /// Gets a value indicating whether this ValueOption has a None value.
        /// </summary>
        public bool IsNone => !_content.HasValue;

        /// <summary>
        /// Applies a mapping function to the value of this ValueOption and returns a new ValueOption with the result.
        /// </summary>
        /// <typeparam name="TResult">The type of the result value.</typeparam>
        /// <param name="map">The mapping function.</param>
        /// <returns>A ValueOption with the mapped value if this ValueOption has a Some value; otherwise, a ValueOption with a None value.</returns>
        public ValueOption<TResult> Map<TResult>(Func<T, TResult> map) where TResult : struct => new(IsSome ? map(_content!.Value) : null);

        /// <summary>
        /// Applies a mapping function to the value of this ValueOption and returns a new Option with the result.
        /// </summary>
        /// <typeparam name="TResult">The type of the result value.</typeparam>
        /// <param name="map">The mapping function.</param>
        /// <returns>An Option with the mapped value if this ValueOption has a Some value; otherwise, an Option with a None value.</returns>
        public Option<TResult> MapObject<TResult>(Func<T, TResult> map) where TResult : class => IsSome ? map(_content!.Value) : Option<TResult>.None;

        /// <summary>
        /// Checks if this ValueOption has a Some value and satisfies the given predicate.
        /// </summary>
        /// <param name="predicate">The predicate to check.</param>
        /// <returns>True if this ValueOption has a Some value and satisfies the predicate; otherwise, false.</returns>
        public bool IsSomeAnd(Func<T, bool> predicate) => IsSome && predicate(_content!.Value);

        /// <summary>
        /// Returns the value of this ValueOption if it has a Some value; otherwise, returns the default value of type T.
        /// </summary>
        /// <returns>The value of this ValueOption if it has a Some value; otherwise, the default value of type T.</returns>
        public T Reduce() => _content ?? default(T);

        /// <summary>
        /// Returns the value of this ValueOption if it has a Some value; otherwise, returns the specified value.
        /// </summary>
        /// <param name="ifNone">The value to return if this ValueOption has a None value.</param>
        /// <returns>The value of this ValueOption if it has a Some value; otherwise, the specified value.</returns>
        public T Reduce(T ifNone) => _content ?? ifNone;

        /// <summary>
        /// Returns the value of this ValueOption if it has a Some value; otherwise, returns the value returned by the specified function.
        /// </summary>
        /// <param name="ifNone">The function that returns the value to return if this ValueOption has a None value.</param>
        /// <returns>The value of this ValueOption if it has a Some value; otherwise, the value returned by the specified function.</returns>
        public T Reduce(Func<T> ifNone) => _content ?? ifNone();

        /// <summary>
        /// Filters the value of this ValueOption based on the given predicate.
        /// </summary>
        /// <param name="predicate">The predicate to filter the value.</param>
        /// <returns>This ValueOption if it has a None value or the value satisfies the predicate; otherwise, a ValueOption with a None value.</returns>
        public ValueOption<T> Where(Func<T, bool> predicate) => IsSome && predicate(_content!.Value) ? this : None;

        /// <summary>
        /// Filters the value of this ValueOption based on the given predicate.
        /// </summary>
        /// <param name="predicate">The predicate to filter the value.</param>
        /// <returns>This ValueOption if it has a None value or the value does not satisfy the predicate; otherwise, a ValueOption with a None value.</returns>
        public ValueOption<T> WhereNot(Func<T, bool> predicate) => IsSome && !predicate(_content!.Value) ? this : None;

        /// <summary>
        /// Returns this ValueOption if it has a Some value; otherwise, returns the specified alternative option.
        /// </summary>
        /// <param name="orOption">The alternative option to return if this ValueOption has a None value.</param>
        /// <returns>This ValueOption if it has a Some value; otherwise, the specified alternative option.</returns>
        public ValueOption<T> Or(T orOption) => IsNone ? orOption : this;

        /// <summary>
        /// Matches the value of this ValueOption and applies the specified functions accordingly.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="some">The function to apply if this ValueOption has a Some value.</param>
        /// <param name="none">The function to apply if this ValueOption has a None value.</param>
        /// <returns>The result of applying the specified function to the value of this ValueOption.</returns>
        public TResult Match<TResult>(Func<T, TResult> some, Func<TResult> none) => IsSome ? some(_content!.Value) : none();

        /// <summary>
        /// Converts this ValueOption to an enumerable containing the value if it has a Some value; otherwise, returns an empty enumerable.
        /// </summary>
        /// <returns>An enumerable containing the value of this ValueOption if it has a Some value; otherwise, an empty enumerable.</returns>
        public IEnumerable<T> ToEnumerable() => IsSome ? new[] { _content!.Value } : Array.Empty<T>();

        /// <summary>
        /// Combines the value of this ValueOption with the specified value into a ValueOption.
        /// </summary>
        /// <typeparam name="T2">The type of the specified value.</typeparam>
        /// <param name="other">The specified value to zip.</param>
        /// <returns>A ValueOption with a tuple of the values if this ValueOption has a Some value; otherwise, a ValueOption with a None value.</returns>
        public ValueOption<(T, T2)> Zip<T2>(T2 other) where T2 : struct =>
            IsSome ? ValueOption<(T, T2)>.Some((_content!.Value, other)) : ValueOption<(T, T2)>.None;

        /// <summary>
        /// Combines the value of this ValueOption with the value of the specified Option into a ValueOption.
        /// </summary>
        /// <typeparam name="T2">The type of the value in the other Option.</typeparam>
        /// <param name="other">The other Option to zip.</param>
        /// <returns>A ValueOption with a tuple of the values if both ValueOptions have Some values; otherwise, a ValueOption with a None value.</returns>
        public ValueOption<(T, T2)> Zip<T2>(Option<T2> other) where T2 : class =>
            IsSome && other.IsSome ? ValueOption<(T, T2)>.Some((_content!.Value, other.Reduce()!)) : ValueOption<(T, T2)>.None;

        /// <summary>
        /// Performs the specified action on the value of this ValueOption if it has a Some value.
        /// </summary>
        /// <param name="ifSome">The action to perform if this ValueOption has a Some value.</param>
        /// <returns>This ValueOption.</returns>
        public ValueOption<T> Do(Action<T> ifSome) {
            if (_content.HasValue) ifSome(_content.Value);
            return this;
        }

        /// <summary>
        /// Performs the specified actions on the value of this ValueOption based on whether it has a Some value or a None value.
        /// </summary>
        /// <param name="ifSome">The action to perform if this ValueOption has a Some value.</param>
        /// <param name="ifNone">The action to perform if this ValueOption has a None value.</param>
        /// <returns>This ValueOption.</returns>
        public ValueOption<T> Do(Action<T> ifSome, Action ifNone) {
            if (_content.HasValue) ifSome(_content.Value);
            else ifNone();
            return this;
        }

        /// <summary>
        /// Asynchronously performs the specified async action on the value of this ValueOption if it has a Some value.
        /// </summary>
        /// <param name="ifSome">The async action to perform if this ValueOption has a Some value.</param>
        /// <returns>A UniTask representing the asynchronous operation.</returns>
        public async UniTask<ValueOption<T>> DoAsync(Func<T, UniTask> ifSome) {
            if (IsSome) await ifSome(_content!.Value);
            return this;
        }

        /// <summary>
        /// Asynchronously performs the specified async actions on the value of this ValueOption based on whether it has a Some value or a None value.
        /// </summary>
        /// <param name="ifSome">The async action to perform if this ValueOption has a Some value.</param>
        /// <param name="ifNone">The async action to perform if this ValueOption has a None value.</param>
        /// <returns>A UniTask representing the asynchronous operation.</returns>
        public async UniTask<ValueOption<T>> DoAsync(Func<T, UniTask> ifSome, Func<UniTask> ifNone) {
            if (IsSome) await ifSome(_content!.Value);
            else await ifNone();
            return this;
        }

        public static implicit operator ValueOption<T>(T content) => Some(content);

        public override int    GetHashCode()         => _content?.GetHashCode() ?? 0;
        public override bool   Equals(object? other) => other is ValueOption<T> option && Equals(option);
        public override string ToString()            => IsSome ? _content!.ToString() : "None";

        public bool Equals(ValueOption<T> other) => _content.HasValue
                                                        ? other._content.HasValue && _content.Value.Equals(other._content.Value)
                                                        : !other._content.HasValue;

        public static bool operator ==(ValueOption<T> a, ValueOption<T> b) => a.Equals(b);
        public static bool operator !=(ValueOption<T> a, ValueOption<T> b) => !(a.Equals(b));
    }
}