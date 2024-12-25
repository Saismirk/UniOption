    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using Cysharp.Threading.Tasks;
    using System.Diagnostics.Contracts;

    namespace UniOption {
        public readonly struct Option<T> : IOption where T : class {
            readonly T _content;

            /// <summary>
            /// Creates a new Option with a Some value.
            /// </summary>
            /// <param name="content">The value to wrap.</param>
            /// <returns>An Option with the specified value.</returns>
            public static Option<T> Some(T content) => new(content);

            /// <summary>
            /// Gets an Option with a None value.
            /// </summary>
            public static readonly Option<T> None = new();

            Option(T content = null) => _content = content;

            /// <summary>
            /// True if this Option has a Some value; otherwise, false.
            /// </summary>
            [Pure]
            public bool IsSome => NullCheck.IsNotNull(_content);

            /// <summary>
            /// True if this Option has a None value; otherwise, false.
            /// </summary>
            [Pure]
            public bool IsNone => NullCheck.IsNull(_content);

            /// <summary>
            /// Applies a mapping function to the value of this Option and returns a new Option with the result.
            /// </summary>
            /// <typeparam name="TResult">The type of the result value.</typeparam>
            /// <param name="map">The mapping function.</param>
            /// <returns>An Option with the mapped value if this Option has a Some value; otherwise, an Option with a None value.</returns>
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Option<TResult> Map<TResult>(Func<T, TResult> map) where TResult : class =>
                IsSome
                    ? Option<TResult>.Some(map(_content!))
                    : Option<TResult>.None;

            /// <summary>
            /// Applies a mapping function to the value of this Option and returns a new ValueOption with the result.
            /// </summary>
            /// <typeparam name="TResult">The type of the result value.</typeparam>
            /// <param name="map">The mapping function.</param>
            /// <returns>A ValueOption with the mapped value if this Option has a Some value; otherwise, a ValueOption with a None value.</returns>
            [Pure]
            public ValueOption<TResult> MapValue<TResult>(Func<T, TResult> map) where TResult : struct =>
                IsSome
                    ? ValueOption<TResult>.Some(map(_content!))
                    : ValueOption<TResult>.None;

            /// <summary>
            /// Checks if this Option has a Some value and satisfies the given predicate.
            /// </summary>
            /// <param name="predicate">The predicate to check.</param>
            /// <returns>True if this Option has a Some value and satisfies the predicate; otherwise, false.</returns>
            public bool IsSomeAnd(Func<T, bool> predicate) => IsSome && predicate(_content!);

            /// <summary>
            /// Returns the value of this Option if it has a Some value; otherwise, returns the specified default value.
            /// </summary>
            /// <param name="defaultValue">The default value to return.</param>
            /// <returns>The value of this Option if it has a Some value; otherwise, the specified default value.</returns>
            [Pure]
            public T Reduce(T defaultValue) => IsSome
                                                   ? _content
                                                   : NullCheck.NullReturn(defaultValue);

            /// <summary>
            /// Returns the value of this Option if it has a Some value; otherwise, returns the value returned by the specified function.
            /// This method is unsafe as it may throw an exception if the specified function throws an exception.
            /// It is recommended to use <see cref="Match{TResult}(Func{T, TResult}, Func{TResult})"/> instead.
            /// </summary>
            /// <param name="defaultValue">The function that returns the default value to return.</param>
            /// <returns>The value of this Option if it has a Some value; otherwise, the value returned by the specified function.</returns>
            public T Reduce(Func<T> defaultValue) => _content ?? defaultValue();

            /// <summary>
            /// Filters the value of this Option based on the given predicate.
            /// This method is unsafe as it may throw an exception if the specified function throws an exception.
            /// It is recommended to use <see cref="Match{TResult}(Func{T, TResult}, Func{TResult})"/> instead.
            /// </summary>
            /// <param name="predicate">The predicate to filter the value.</param>
            /// <returns>This Option if it has a None value or the value satisfies the predicate; otherwise, an Option with a None value.</returns>
            public Option<T> Where(Func<T, bool> predicate) => IsNone || predicate(_content!) ? this : None;

            /// <summary>
            /// Filters the value of this Option based on the given predicate.
            /// </summary>
            /// <param name="predicate">The predicate to filter the value.</param>
            /// <returns>This Option if it has a None value or the value does not satisfy the predicate; otherwise, an Option with a None value.</returns>
            public Option<T> WhereNot(Func<T, bool> predicate) => IsNone || !predicate(_content!) ? this : None;

            /// <summary>
            /// Filters the value of this Option to the specified type.
            /// </summary>
            /// <typeparam name="TValue">The type to filter the value.</typeparam>
            /// <returns>An Option with the value cast to the specified type if it has a Some value and satisfies the type constraint; otherwise, an Option with a None value.</returns>
            [Pure]
            public Option<TValue> OfType<TValue>() where TValue : class => _content is TValue content ? Option<TValue>.Some(content) : Option<TValue>.None;

            /// <summary>
            /// Returns this Option if it has a Some value; otherwise, returns the specified alternative option.
            /// </summary>
            /// <param name="orOption">The alternative option to return if this Option has a None value.</param>
            /// <returns>This Option if it has a Some value; otherwise, the specified alternative option.</returns>
            [Pure]
            public Option<T> Or(T orOption) => IsNone ? orOption : this;

            /// <summary>
            /// Matches the value of this Option and applies the specified functions accordingly.
            /// </summary>
            /// <typeparam name="TResult">The type of the result.</typeparam>
            /// <param name="some">The function to apply if this Option has a Some value.</param>
            /// <param name="none">The function to apply if this Option has a None value.</param>
            /// <returns>The result of applying the specified function to the value of this Option.</returns>
            public TResult Match<TResult>(Func<T, TResult> some, Func<TResult> none) => IsSome ? some(_content!) : none();

            /// <summary>
            /// Converts this Option to an enumerable containing the value if it has a Some value; otherwise, returns an empty enumerable.
            /// </summary>
            /// <returns>An enumerable containing the value of this Option if it has a Some value; otherwise, an empty enumerable.</returns>
            public IEnumerable<T> ToEnumerable() => (IsSome ? new[] { _content } : Array.Empty<T>())!;

            /// <summary>
            /// Combines the value of this Option with the value of the specified Option into a ValueOption.
            /// </summary>
            /// <typeparam name="T2">The type of the value in the other Option.</typeparam>
            /// <param name="other">The other Option to zip.</param>
            /// <returns>A ValueOption with a tuple of the values if both Options have Some values; otherwise, a ValueOption with a None value.</returns>
            [Pure]
            public ValueOption<(T, T2)> Zip<T2>(Option<T2> other) where T2 : class =>
                IsSome && other.IsSome
                    ? ValueOption<(T, T2)>.Some((_content!, other.Reduce(default(T2)!)!))
                    : ValueOption<(T, T2)>.None;

            /// <summary>
            /// Combines the value of this Option with the specified value into a ValueOption.
            /// </summary>
            /// <typeparam name="T2">The type of the specified value.</typeparam>
            /// <param name="other">The specified value to zip.</param>
            /// <returns>A ValueOption with a tuple of the values if this Option has a Some value; otherwise, a ValueOption with a None value.</returns>
            [Pure]
            public ValueOption<(T, T2)> Zip<T2>(T2 other) where T2 : struct =>
                IsSome ? ValueOption<(T, T2)>.Some((_content!, other)) : ValueOption<(T, T2)>.None;

            /// <summary>
            /// Performs the specified action on the value of this Option if it has a Some value.
            /// </summary>
            /// <param name="ifSome">The action to perform if this Option has a Some value.</param>
            /// <returns>This Option.</returns>
            public Option<T> Do(Action<T> ifSome) {
                if (IsSome) ifSome(_content!);
                return this;
            }

            /// <summary>
            /// Performs the specified actions on the value of this Option based on whether it has a Some value or a None value.
            /// </summary>
            /// <param name="ifSome">The action to perform if this Option has a Some value.</param>
            /// <param name="ifNone">The action to perform if this Option has a None value.</param>
            /// <returns>This Option.</returns>
            public Option<T> Do(Action<T> ifSome, Action ifNone) {
                if (IsSome) ifSome(_content!);
                else ifNone();
                return this;
            }

            /// <summary>
            /// Performs the specified action, with a mutable reference to a context, on the value of this Option based on whether it has a Some value or a None value.
            /// </summary>
            /// <param name="ifSome">The action to perform if this Option has a Some value.</param>
            /// <param name="context">The context to pass to the action.</param>
            /// <param name="ifNone">The action to perform if this Option has a None value.</param>
            /// <returns>This Option.</returns>
            public Option<T> Do<TContext>(ActionRef<T, TContext> ifSome, ref TContext context, ActionRef<TContext> ifNone) {
                if (IsSome) ifSome?.Invoke(_content!, ref context);
                else ifNone?.Invoke(ref context);
                return this;
            }

            /// <summary>
            /// Performs the specified action, with a mutable reference to a context, on the value of this Option based on whether it has a Some value or a None value.
            /// </summary>
            /// <param name="ifSome">The action to perform if this Option has a Some value.</param>
            /// <param name="context">The context to pass to the action.</param>
            /// <returns>This Option.</returns>
            public Option<T> Do<TContext>(ActionRef<T, TContext> ifSome, ref TContext context) {
                if (IsSome) ifSome?.Invoke(_content!, ref context);
                return this;
            }

            /// <summary>
            /// Performs the specified action, with a value reference to a context, on the value of this Option based on whether it has a Some value or a None value.
            /// </summary>
            /// <param name="ifSome">The action to perform if this Option has a Some value.</param>
            /// <param name="context">The context to pass to the action.</param>
            /// <param name="ifNone">The action to perform if this Option has a None value.</param>
            /// <returns>This Option.</returns>
            public Option<T> Do<TContext>(Action<T, TContext> ifSome, TContext context, Action<TContext> ifNone) {
                if (IsSome) ifSome(_content!, context);
                else ifNone(context);
                return this;
            }

            /// <summary>
            /// Performs the specified action, with a value reference to a context, on the value of this Option based on whether it has a Some value or a None value.
            /// </summary>
            /// <param name="ifSome">The action to perform if this Option has a Some value.</param>
            /// <param name="context">The context to pass to the action.</param>
            /// <param name="ifNone">The action to perform if this Option has a None value.</param>
            /// <returns>This Option.</returns>
            public Option<T> Do<TContext>(Action<T, TContext> ifSome, TContext context, Action ifNone) {
                if (IsSome) ifSome(_content!, context);
                else ifNone();
                return this;
            }

            /// <summary>
            /// Performs the specified action, with a value reference to a context, on the value of this Option based on whether it has a Some value or a None value.
            /// </summary>
            /// <param name="ifSome">The action to perform if this Option has a Some value.</param>
            /// <param name="context">The context to pass to the action.</param>
            /// <returns>This Option.</returns>
            public Option<T> Do<TContext>(Action<T, TContext> ifSome, TContext context) {
                if (IsSome) ifSome(_content!, context);
                return this;
            }

            /// <summary>
            /// Asynchronously performs the specified async action on the value of this Option if it has a Some value.
            /// </summary>
            /// <param name="ifSome">The async action to perform if this Option has a Some value.</param>
            /// <returns>A UniTask representing the asynchronous operation.</returns>
            public async UniTask<Option<T>> DoAsync(Func<T, UniTask> ifSome) {
                if (IsSome) await ifSome(_content!);
                return this;
            }

            /// <summary>
            /// Asynchronously performs the specified async actions on the value of this Option based on whether it has a Some value or a None value.
            /// </summary>
            /// <param name="ifSome">The async action to perform if this Option has a Some value.</param>
            /// <param name="ifNone">The async action to perform if this Option has a None value.</param>
            /// <returns>A UniTask representing the asynchronous operation.</returns>
            public async UniTask<Option<T>> DoAsync(Func<T, UniTask> ifSome, Func<UniTask> ifNone) {
                if (IsSome) await ifSome(_content!);
                else await ifNone();
                return this;
            }

            [Pure]
            public static implicit operator Option<T>(T content) => content != null ? Some(content) : None;

            [Pure]
            public static implicit operator Option<T>(SerializableOption<T> serializableOption) => serializableOption.ToOption();

            public static bool operator ==(Option<T> a, Option<T> b) => a.Equals(b);
            public static bool operator !=(Option<T> a, Option<T> b) => !(a == b);

            public override int GetHashCode() => _content?.GetHashCode() ?? 0;
        #nullable enable
            public override bool Equals(object? obj) => obj is Option<T> other && (IsSome ? _content.Equals(other._content) : other.IsNone);
        #nullable disable
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override string ToString() => IsSome ? _content!.ToString() : "None";

            public bool Equals(Option<T> other) => IsSome ? other.IsSome && _content!.Equals(other._content) : other.IsNone;
        }
    }