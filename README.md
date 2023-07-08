# UniOption
Provides an implementation of the **Option** type for Unity. This is similar to the Option type from Rust, or the Maybe type from Haskell.
Contains types ``Option<T>`` for reference types, and ``ValueOption<T>`` for value types.

## Usage
``Option<T>`` provides the following methods:
- ``Some(T value)``: Creates a new Option of value ``T``.
- ``Option<TResult> Map<TResult>(Func<T, TResult> map)``: Maps the value of the Option to a new value of type ``TResult``.
- ``ValueOption<TResult> MapValue<TResult>(Func<T, TResult> map)``: Maps the value of the Option to a ValueOption of value type ``TResult``.
- ``T Reduce(T defaultValue)``: Returns the value of the Option, or ``defaultValue`` if the Option is ``None``.
- ``Option<T> Do(Action<T> ifSome, Action ifNone)``: Executes ``ifSome`` if the Option is ``Some``, or ``ifNone`` if the Option is ``None``.
- ``Option<T> Do(Action<T> ifSome)``: Executes ``ifSome`` if the Option is ``Some``.
- ``async UniTask<Option<T>> DoAsync(Func<T, UniTask> ifSome)``: Async version of ``Do`` using UniTask.
- ``Option<T> Where(Func<T, bool> predicate)``: Filters the Option using the input predicate, return ``None`` if the predicate returns ``false``.
- ``Option<TValue> OfType<TValue>()``: Filters the Option to only contain values of type ``TValue``.

``ValueOption<T>`` is the same as ``Option<T>``, but for value types. It provides the same methods, except for ``MapValue``.
Any type can be implicitly converted to an ``Option<T>`` or ``ValueOption<T>``. ``null`` is converted to ``None``, and any other value is converted to ``Some(value)``.
## Examples
```csharp
// Create a new Option
Option<string> some = "Hello World";
Option<string> none = null;
ValueOption<int> someValue = 42;

//Perform operations on the options
some.Map(s => s.ToUpper())
    .Reduce("Default");
    .Do(s => Debug.Log(s))// Prints "HELLO WORLD"
    
none.Map(s => s.ToUpper())
    .Reduce("Default");
    .Do(s => Debug.Log(s))// Prints "Default"
    
someValue.Map(i => i * 2)
         .Reduce(0);
         .Do(i => Debug.Log(i))// Prints "84"
         
 //Async operations
some.Map(s => s.ToUpper())
    .DoAsync(async s => 
        {
            await UniTask.Delay(1000);
            Debug.Log(s);
        }).Forget();
```
