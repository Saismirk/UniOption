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
- ``Option<T> WhereNot(Func<T, bool> predicate)``: Filters the Option using the input predicate, return ``None`` if the predicate returns ``true``.
- ``Option<TValue> OfType<TValue>()``: Filters the Option to only contain values of type ``TValue``.
- ``Option<T> Or(T value)``: Returns the Option if it is ``Some``, or ``Some(value)`` if the Option is ``None``.
- ``T Match<T>(Func<T, T> some, Func<T> none)``: Returns the result of ``some`` if the Option is ``Some``, or the result of ``none`` if the Option is ``None``.
- ``IEnumerable<T> ToEnumerable()``: Returns an enumerable containing the value of the Option, or an empty enumerable if the Option is ``None``.

``ValueOption<T>`` is the same as ``Option<T>``, but for value types. It provides the same methods, but instead of ``MapValue`` it is ``MapObject``.
Any type can be implicitly converted to an ``Option<T>`` or ``ValueOption<T>``. ``null`` is converted to ``None``, and any other value is converted to ``Some(value)``.
Alternatively, Any reference or value type can be converted to an ``Option<T>`` or ``ValueOption<T>`` using the ``ToOption()`` and ``ToValueOption()`` extension methods.
## Examples
### Basics
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
### Where/WhereNot
```csharp
var stringOption = "TestString".ToOption()
                               .Where(s => s.Length > 5)
                               .WhereNot(s => s.Length > 15)
                               .Do(s => Debug.Log(s))// Prints "TestString"
                               
var intOption = 5.ToValueOption()
                 .Where(i => i >= 5)
                 .Do(i => Debug.Log(i))// Prints "5"               
```

### OfType
```csharp
var stringOption = "TestString".ToOption()
                               .OfType<string>()
                               .Do(s => Debug.Log(s))// Prints "TestString"
```

### Or 
```csharp
string nullString = null;
var stringOption = nullString.ToOption()
                             .Or("Default")
                             .Do(s => Debug.Log(s))// Prints "Default"
```

### Match
```csharp
var option = "Hello World".ToOption();
var result = option.Match(some: s => s.ToUpper(),
                          none: () => "Default");
```

### ToEnumerable
```csharp
var stringOption = "TestString".ToOption();
        foreach (var s in stringOption.ToEnumerable()) {
            Debug.Log(s);// Prints "TestString"
        }
```

### Zip
Returns a ``ValueOption<(T1,T2)>`` containing the result of the input function if both options are ``Some``, or ``None`` if either option is ``None``.
```csharp
var stringOption = "TestString".ToOption()
                               .Zip(5);//Contains ValueTuple ("TestString", 5)
```