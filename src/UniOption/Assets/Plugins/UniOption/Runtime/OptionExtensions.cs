#nullable enable
namespace UniOption {
    public static partial class OptionExtensions {
        public static Option<T>      ToOption<T>(this T content) where T : class       => Option<T>.Some(content);
        public static ValueOption<T> ToValueOption<T>(this T content) where T : struct => ValueOption<T>.Some(content);

        public static ValueOption<T> ToValueOption<T>(this T? content) where T : struct =>
            content.HasValue ? ValueOption<T>.Some(content.Value) : ValueOption<T>.None;
    }
}