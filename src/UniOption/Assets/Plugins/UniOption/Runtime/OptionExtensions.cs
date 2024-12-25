#nullable enable
    namespace UniOption {
    public static class OptionExtensions {
        /// <summary>
        /// Converts the specified reference type content to an Option.
        /// </summary>
        /// <returns>An Option with the specified content.</returns>
        public static Option<T> ToOption<T>(this T content) where T : class => Option<T>.Some(content);

        /// <summary>
        /// Converts the specified Serialized Option into an Option.
        /// </summary>
        /// <returns>An Option with the same content.</returns>
        public static Option<T> ToOption<T>(this SerializableOption<T> serializableOption) where T : class => serializableOption.Match(Option<T>.Some, () => Option<T>.None);

        /// <summary>
        /// Converts the specified value type content to a ValueOption.
        /// </summary>
        /// <returns>A ValueOption with the specified content.</returns>
        public static ValueOption<T> ToValueOption<T>(this T content) where T : struct => ValueOption<T>.Some(content);

        /// <summary>
        /// Converts the nullable value type content to a ValueOption.
        /// </summary>
        /// <returns>A ValueOption with the specified content if it has a value; otherwise, a ValueOption with a None value.</returns>
        public static ValueOption<T> ToValueOption<T>(this T? content) where T : struct => content.HasValue ? ValueOption<T>.Some(content.Value) : ValueOption<T>.None;

        /// <summary>
        /// Flattens a nested ValueOption of ValueOption into a single ValueOption.
        /// </summary>
        /// <returns>A ValueOption that is the flattened result of the ValueOption of ValueOption.</returns>
        public static ValueOption<T> Flatten<T>(this ValueOption<ValueOption<T>> option) where T : struct => option.Match(some => some, () => ValueOption<T>.None);
    }
}