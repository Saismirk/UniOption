#nullable enable
namespace UniOption {
    public static partial class OptionExtensions {
        public static Option<T>      ToOption<T>(this T content) where T : class       => Option<T>.Some(content);
        public static ValueOption<T> ToValueOption<T>(this T content) where T : struct => ValueOption<T>.Some(content);

        public static ValueOption<T> ToValueOption<T>(this T? content) where T : struct =>
            content.HasValue ? ValueOption<T>.Some(content.Value) : ValueOption<T>.None;
        public static Option<T> Flatten<T>(this Option<Option<T>> option) where T : class => option.Match(some => some,
                                                                                                              () => Option<T>.None);
        public static ValueOption<T> Flatten<T>(this ValueOption<ValueOption<T>> option) where T : struct =>
            option.Match(some => some,
                         () => ValueOption<T>.None);
    }
}