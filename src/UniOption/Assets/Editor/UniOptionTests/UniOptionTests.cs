using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UniOption;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;
// ReSharper disable HeapView.CanAvoidClosure

namespace Editor.UniOptionTests {
    public class UniOptionTests {
        int    _validator;
        string _validatorString;
        [SetUp]
        public void Setup() { }

        [Test]
        public void OptionCreateImplicit() {
            Option<string> stringOption = "TestString";
            Assert.IsTrue(stringOption != null);
            Assert.IsTrue(stringOption.IsSome);
            Assert.IsFalse(stringOption.IsNone);
            Assert.IsTrue(stringOption.Reduce(string.Empty) == "TestString");
        }

        [Test]
        public void OptionCreateSome() {
            var stringOption = Option<string>.Some("TestString");
            Assert.IsTrue(stringOption != null);
            Assert.IsTrue(stringOption.IsSome);
            Assert.IsFalse(stringOption.IsNone);
            Assert.IsTrue(stringOption.Reduce(string.Empty) == "TestString");
        }

        [Test]
        public void OptionObjectToOption() {
            var stringOption = "TestString".ToOption();
            Assert.IsTrue(stringOption != null);
            Assert.IsTrue(stringOption.IsSome);
            Assert.IsFalse(stringOption.IsNone);
            Assert.IsTrue(stringOption.Reduce(string.Empty) == "TestString");
        }

        [Test]
        public void ValueToValueOption() {
            var intOption = 5.ToValueOption();
            Assert.IsTrue(intOption.IsSome);
            Assert.IsFalse(intOption.IsNone);
            Assert.AreEqual(5, intOption.Reduce(0));
        }

        [Test]
        public void NullableValueToValueOption() {
            int? nullableInt = 5;
            var  intOption   = nullableInt.ToValueOption();
            Assert.IsTrue(intOption.IsSome);
            Assert.IsFalse(intOption.IsNone);
            Assert.IsTrue(intOption.Reduce(0) == 5);
        }

        [Test]
        public void OptionFromNullObject() {
            var    objectOption = ((Object)null)!.ToOption();
            Assert.IsTrue(objectOption.IsNone);
            Assert.IsFalse(objectOption.IsSome);
        }

        [Test]
        public void OptionNone() {
            Option<string> stringOption = default;
            Assert.IsFalse(stringOption.IsSome, "Option<string>.IsSome is true");
            Assert.IsTrue(stringOption.IsNone, "Option<string>.IsNone is false");
            Assert.IsTrue(stringOption.Reduce(string.Empty) == string.Empty, "Option<string>.Reduce(string.Empty) != string.Empty");
        }

        [Test]
        public void OptionIsSomeAnd() {
            var stringOption = Option<string>.Some("TestString");
            Assert.IsTrue(stringOption.IsSomeAnd(s => s.Length > 5));
            Assert.IsFalse(stringOption.IsSomeAnd(s => s.Length < 5));
            var valueOption = 5.ToValueOption();
            Assert.IsTrue(valueOption.IsSomeAnd(i => i > 0));
            Assert.IsFalse(valueOption.IsSomeAnd(i => i < 0));
        }

        [Test]
        public void OptionGetHashCode() {
            var stringOption = Option<string>.Some("TestString");
            Assert.IsTrue(stringOption.GetHashCode() == "TestString".GetHashCode());
            var valueOption = 5.ToValueOption();
            Assert.IsTrue(valueOption.GetHashCode() == 5.GetHashCode());
        }

        [Test]
        [SuppressMessage("ReSharper", "ExpressionIsAlwaysNull")]
        public void ValueOptionNone() {
            int? nullableInt = null;
            var  intOption   = nullableInt.ToValueOption();
            Assert.IsFalse(intOption.IsSome);
            Assert.IsTrue(intOption.IsNone);
            Assert.IsTrue(intOption.Reduce(0) == 0);
        }

        [Test]
        public void OptionMapValue() {
            var stringOption = Option<string>.Some("TestString");
            var intOption    = stringOption.MapValue(s => s.Length);
            Assert.IsTrue(intOption.IsSome);
            Assert.IsFalse(intOption.IsNone);
            Assert.IsTrue(intOption.Reduce(0) == 10);
        }

        [Test]
        public void OptionMap() {
            var stringOption = "TestString".ToOption();
            var upperOption  = stringOption.Map(s => s.ToUpper());
            Assert.IsTrue(upperOption.IsSome);
            Assert.IsFalse(upperOption.IsNone);
            Assert.IsTrue(upperOption.Reduce(string.Empty) == "TESTSTRING");
        }

        [Test]
        public void ValueOptionMap() {
            var intOption = 5.ToValueOption()
                             .Map(i => i * 10);
            Assert.IsTrue(intOption.IsSome);
            Assert.IsFalse(intOption.IsNone);
            Assert.IsTrue(intOption.Reduce(0) == 50);
        }

        [Test]
        public void ValueOptionMapObject() {
            var mappedOption = 5.ToValueOption()
                                .MapObject(i => i.ToString());
            Assert.IsTrue(mappedOption.IsSome);
            Assert.IsFalse(mappedOption.IsNone);
            Assert.IsTrue(mappedOption.Reduce(string.Empty) == "5");
        }

        [Test]
        public void OptionMapNone() {
            var stringOption = Option<string>.None;
            var upperOption  = stringOption.Map(s => s.ToUpper());
            Assert.IsFalse(upperOption.IsSome);
            Assert.IsTrue(upperOption.IsNone);
            Assert.IsTrue(upperOption.Reduce(string.Empty) == string.Empty);
        }

        [Test]
        public void OptionMapValueNone() {
            var stringOption = Option<string>.None;
            var intOption    = stringOption.MapValue(s => s.Length);
            Assert.IsFalse(intOption.IsSome);
            Assert.IsTrue(intOption.IsNone);
            Assert.IsTrue(intOption.Reduce(0) == 0);
        }

        [Test]
        public void OptionDo() {
            var stringOption = "TestString".ToOption();
            _validatorString = string.Empty;
            stringOption.Do(s => _validatorString = s.ToUpper());
            Assert.IsTrue(_validatorString == "TESTSTRING");
        }

        [Test]
        public void OptionDoWithContext() {
            var stringOption = "TestString".ToOption();
            var upperOption  = string.Empty;
            stringOption.Do((string s, ref string upper) => upper = s.ToUpper(), ref upperOption);
            Assert.IsTrue(upperOption == "TESTSTRING");
        }

        [Test]
        public void OptionDoWithContextNoRef() {
            var stringOption = "TestString".ToOption();
            var upperOption  = "_TEST";
            _validatorString = string.Empty;
            stringOption.Do((s, upper) => _validatorString = s.ToUpper() + upper, upperOption);
            Assert.IsTrue(_validatorString == "TESTSTRING_TEST");
        }

        [Test]
        public void ValueOptionDo() {
            var intOption   = 5.ToValueOption();
            var upperOption = intOption.Match(s => s.ToString(), () => string.Empty);
            Assert.IsTrue(upperOption == "5");
        }

        [Test]
        public void ValueOptionDoWithRef() {
            var intOption   = 5.ToValueOption();
            var upperOption = string.Empty;
            intOption.Do((int s, ref string upper) => upper = s.ToString(), ref upperOption);
            Assert.IsTrue(upperOption == "5");
        }

        [Test]
        public void OptionDoNone() {
            var stringOption = Option<string>.None;
            _validatorString = string.Empty;
            stringOption.Do(s => _validatorString = s.ToUpper(), () => _validatorString += "FAILED");
            Assert.IsTrue(_validatorString == "FAILED");
        }

        [Test]
        public void OptionDoNoneWithContext() {
            var stringOption = Option<string>.None;
            var upperOption  = string.Empty;
            stringOption.Do((string s, ref string upper) => upper = s.ToUpper(), ref upperOption);
            Assert.IsTrue(upperOption == string.Empty);
        }

        [Test]
        public void OptionDoNoneWithContextWithNoneRef() {
            var stringOption = Option<string>.None;
            var upperOption  = string.Empty;
            stringOption.Do((string s, ref string upper) => upper = s.ToUpper(), ref upperOption, (ref string upper) => upper += "FAILED");
            Assert.IsTrue(upperOption == "FAILED");
        }

        [Test]
        public void OptionDoNoneWithContextNoRef() {
            var stringOption = Option<string>.None;
            var upperOption  = "_TEST";
            _validatorString = string.Empty;
            stringOption.Do((s, _) => _validatorString = s.ToUpper(), upperOption, upper => _validatorString += upper);
            Assert.IsTrue(_validatorString == "_TEST");
        }

        [Test]
        public void OptionDoNoneWithContextNoRefWithNoneAction() {
            var stringOption = Option<string>.None;
            var upperOption  = "_TEST";
            _validatorString = string.Empty;
            stringOption.Do((s, _) => _validatorString = s.ToUpper(), upperOption, () => _validatorString += "_TEST");
            Assert.IsTrue(_validatorString == "_TEST");
        }

        [Test]
        public void ValueOptionDoNone() {
            var intOption   = ValueOption<int>.None;
            var upperOption = intOption.Match(s => s.ToString().ToUpper(), () => string.Empty);
            Assert.IsTrue(upperOption == string.Empty);
        }

        [Test]
        public void ValueOptionDoNoneWithContext() {
            var intOption   = ValueOption<int>.None;
            var upperOption = string.Empty;
            intOption.Do((int s, ref string upper) => upper = s.ToString().ToUpper(), ref upperOption);
            Assert.IsTrue(upperOption == string.Empty);
        }

        [Test]
        public void ValueOptionDoNoneWithContextNoRef() {
            var intOption   = ValueOption<int>.None;
            var upperOption = string.Empty;
            _validator = 0;
            intOption.Do((_, _) => _validator++, upperOption, _ => _validator += 2);
            Assert.IsTrue(upperOption == string.Empty);
            Assert.IsTrue(_validator == 2);
        }

        [Test]
        public void OptionDoOrElse() {
            var stringOption = "TestString".ToOption();
            var upperOption = stringOption.Match(s => s.ToUpper(),
                                                 () => "None");
            Assert.IsTrue(upperOption == "TESTSTRING");
            stringOption = Option<string>.None;
            upperOption = stringOption.Match(s => s.ToUpper(),
                                             () => "None");
            Assert.IsTrue(upperOption == "None");
        }

        [Test]
        public void ValueOptionDoOrElse() {
            var intOption   = 5.ToValueOption();
            var upperOption = intOption.Match(s => s.ToString(), () => "None");
            Assert.IsTrue(upperOption == "5");
            intOption = ValueOption<int>.None;
            upperOption = intOption.Match(s => s.ToString(),
                                          () => "None");
            Assert.IsTrue(upperOption == "None");
        }

        [Test]
        public void OptionReduce() {
            var stringOption = "TestString".ToOption();
            Assert.IsTrue(stringOption.Reduce(string.Empty) == "TestString");
            Assert.IsTrue(stringOption.Reduce(() => string.Empty) == "TestString");
            stringOption = Option<string>.None;
            Assert.IsTrue(stringOption.Reduce(string.Empty) == string.Empty);
            Assert.IsTrue(stringOption.Reduce(() => string.Empty) == string.Empty);
        }

        [Test]
        public void ValueOptionReduce() {
            var intOption = 5.ToValueOption();
            Assert.IsTrue(intOption.Reduce(0) == 5);
            Assert.IsTrue(intOption.Reduce(() => 0) == 5);
            intOption = ValueOption<int>.None;
            Assert.IsTrue(intOption.Reduce(0) == 0);
            Assert.IsTrue(intOption.Reduce(() => 0) == 0);
        }

        [Test]
        public void OptionDoOrElseNone() {
            var stringOption = Option<string>.None;
            var upperOption = stringOption.Match(s => s.ToUpper(),
                                                 () => "None");
            Assert.IsTrue(upperOption == "None");
        }

        [Test]
        public void OptionWhere() {
            var stringOption = "TestString".ToOption()
                                           .Where(s => s.Length > 5);
            Assert.IsTrue(stringOption.IsSome);
            Assert.IsFalse(stringOption.IsNone);
            Assert.IsTrue(stringOption.Reduce(string.Empty) == "TestString");
        }

        [Test]
        public void ValueOptionWhere() {
            var intOption = 5.ToValueOption()
                             .Where(i => i >= 5);
            Assert.IsTrue(intOption.IsSome);
            Assert.IsFalse(intOption.IsNone);
            Assert.IsTrue(intOption.Reduce(0) == 5);
        }

        [Test]
        public void OptionWhereNone() {
            var stringOption = "TestString".ToOption()
                                           .Where(s => s.Length > 15);
            Assert.IsFalse(stringOption.IsSome);
            Assert.IsTrue(stringOption.IsNone);
            Assert.IsTrue(stringOption.Reduce(string.Empty) == string.Empty);
        }

        [Test]
        public void ValueOptionWhereNone() {
            var intOption = 5.ToValueOption()
                             .Where(i => i < 5);
            Assert.IsFalse(intOption.IsSome);
            Assert.IsTrue(intOption.IsNone);
            Assert.IsTrue(intOption.Reduce(0) == 0);
        }

        [Test]
        public void OptionWhereNot() {
            var stringOption = "TestString".ToOption()
                                           .WhereNot(s => s.Length > 15);
            Assert.IsTrue(stringOption.IsSome);
            Assert.IsFalse(stringOption.IsNone);
            Assert.IsTrue(stringOption.Reduce(string.Empty) == "TestString");
        }

        [Test]
        public void ValueOptionWhereNot() {
            var intOption = 5.ToValueOption()
                             .WhereNot(i => i < 5);
            Assert.IsTrue(intOption.IsSome);
            Assert.IsFalse(intOption.IsNone);
            Assert.IsTrue(intOption.Reduce(0) == 5);
        }

        [Test]
        public void OptionWhereNotNone() {
            var stringOption = "TestString".ToOption()
                                           .WhereNot(s => s.Length > 5);
            Assert.IsFalse(stringOption.IsSome);
            Assert.IsTrue(stringOption.IsNone);
            Assert.AreEqual(string.Empty, stringOption.Reduce(string.Empty));
        }

        [Test]
        public void OptionOr() {
            var stringOption = "TestString".ToOption()
                                           .Or("AnotherString");
            Assert.AreEqual("TestString", stringOption.Reduce(string.Empty));
        }

        [Test]
        public void ValueOptionOr() {
            var intOption = 5.ToValueOption()
                             .Or(10);
            Assert.AreEqual(5, intOption.Reduce(0));
        }

        [Test]
        public void OptionOrNone() {
            var stringOption = Option<string>.None;
            var orOption     = stringOption.Or("AnotherString");
            Assert.IsTrue(orOption.Reduce(string.Empty) == "AnotherString");
        }

        [Test]
        public void ValueOptionOrNone() {
            var intOption = ((int?)null).ToValueOption()
                                        .Or(10);
            Assert.AreEqual(10, intOption.Reduce(0));
        }

        [Test]
        public void OptionOfType() {
            var stringOption = "TestString".ToOption();
            var typeOption   = stringOption.OfType<string>();
            Assert.IsTrue(typeOption.IsSome);
            Assert.IsFalse(typeOption.IsNone);
            Assert.IsTrue(typeOption.Reduce(string.Empty) == "TestString");
        }

        [Test]
        public void OptionOfTypeNone() {
            var stringOption = "TestString".ToOption();
            var typeOption   = stringOption.OfType<Object>();
            Assert.IsFalse(typeOption.IsSome);
            Assert.IsTrue(typeOption.IsNone);
        }

        [Test]
        public void OptionToEnumerable() {
            var stringOption = "TestString".ToOption();
            foreach (var s in stringOption.ToEnumerable()) {
                Assert.IsTrue(s == "TestString");
            }
        }

        [Test]
        public void ValueOptionToEnumerable() {
            var intOption = 5.ToValueOption();
            foreach (var s in intOption.ToEnumerable()) {
                Assert.IsTrue(s == 5);
            }
        }

        [Test]
        public void OptionChaining() {
            var stringOption = "TestString".ToOption()
                                           .Map(s => s.ToUpper())
                                           .OfType<string>()
                                           .Where(s => s.Length > 5)
                                           .Or("AnotherString")
                                           .Reduce("None");
            Assert.IsTrue(stringOption == "TESTSTRING");
        }

        [Test]
        public void ValueOptionChaining() {
            var intOption = 5.ToValueOption()
                             .Map(s => s * 10)
                             .Where(s => s > 5)
                             .Or(10)
                             .Reduce(0);
            Assert.IsTrue(intOption == 50);
        }

        [Test]
        public void OptionChainingNone() {
            var stringOption = "TestString".ToOption()
                                           .Map(s => s.ToUpper())
                                           .OfType<string>()
                                           .Where(s => s.Length > 15)
                                           .Or("AnotherString")
                                           .Reduce("None");
            Assert.IsTrue(stringOption == "AnotherString");
        }

        [Test]
        public void OptionMatch() {
            var option = "Hello World".ToOption();
            var result = option.Match(some: s => s.ToUpper(),
                                      none: () => "Default");
            Assert.IsTrue(result == "HELLO WORLD");
        }

        [Test]
        public void OptionMatchWithContext() {
            var option  = "Hello ".ToOption();
            var context = "World";
            var result = option.Match(some: (s, c) => (s + c).ToUpper(),
                                      context: context,
                                      none: () => "Default");
            Assert.IsTrue(result == "HELLO WORLD");
        }

        [Test]
        public void ValueOptionMatch() {
            var option = 5.ToValueOption();
            var result = option.Match(some: s => s.ToString(),
                                      none: () => "Default");
            Assert.IsTrue(result == "5");
        }

        [Test]
        public void ValueOptionMatchWithContext() {
            var option = 5.ToValueOption();
            var  context = 10;
            var result = option.Match(some: (s, c) => (s + c).ToString(),
                                      context: context,
                                      none: () => "Default");
            Assert.IsTrue(result == "15");
        }

        [Test]
        public void OptionMatchNone() {
            var option = Option<string>.None;
            var result = option.Match(some: s => s.ToUpper(),
                                      none: () => "Default");
            Assert.IsTrue(result == "Default");
        }

        [Test]
        public void OptionMatchNoneWithContext() {
            var          option  = Option<string>.None;
            var context = "World";
            var result = option.Match(some: (s, c) => (s + c).ToUpper(),
                                      context: context,
                                      none: () => "Default");
            Assert.IsTrue(result == "Default");
        }

        [Test]
        public void OptionMatchNoneWithContextOnNone() {
            var option  = Option<string>.None;
            var context = "World";
            var result = option.Match(some: (s, c) => (s + c).ToUpper(),
                                      context: context,
                                      none: c => "Default " + c);
            Assert.IsTrue(result == "Default World");
        }

        [Test]
        public void ValueOptionMatchNone() {
            var option = ValueOption<int>.None;
            var result = option.Match(some: s => s.ToString(),
                                      none: () => "Default");
            Assert.IsTrue(result == "Default");
        }

        [Test]
        public void ValueOptionMatchNoneWithContext() {
            var option  = ValueOption<int>.None;
            var context = 10;
            var result = option.Match(some: (s, c) => (s + c).ToString(),
                                      context: context,
                                      none: () => "Default");
            Assert.IsTrue(result == "Default");
        }

        [Test]
        public void ValueOptionMatchNoneWithContextOnNone() {
            var option  = ValueOption<int>.None;
            var context = 10;
            var result = option.Match(some: (s, c) => (s + c).ToString(),
                                      context: context,
                                      none: c => "Default " + c);
            Assert.IsTrue(result == "Default 10");
        }

        [Test]
        public void OptionZip() {
            var option1 = "Hello".ToOption();
            var result  = option1.Zip("World".ToOption());
            Assert.IsTrue(result.IsSome);
            Assert.IsFalse(result.IsNone);
            Assert.AreEqual(("Hello", "World"), result.Reduce());
        }

        [Test]
        public void OptionZipValue() {
            var option1 = "Hello".ToOption();
            var result  = option1.Zip(5);
            Assert.IsTrue(result.IsSome);
            Assert.IsFalse(result.IsNone);
            Assert.AreEqual(("Hello", 5), result.Reduce());
        }

        [Test]
        public void OptionZipNone() {
            var option1 = Option<string>.None;
            var result  = option1.Zip("World".ToOption());
            Assert.IsTrue(result.IsNone);
            Assert.IsFalse(result.IsSome);
        }

        [Test]
        public void ValueOptionZip() {
            var option1 = 5.ToValueOption();
            var result  = option1.Zip(10);
            Assert.IsTrue(result.IsSome);
            Assert.IsFalse(result.IsNone);
            Assert.AreEqual((5, 10), result.Reduce((0, 0)));
        }

        [Test]
        public void OptionEquals() {
            var option1 = "Hello".ToOption();
            var option2 = "Hello".ToOption();
            Assert.IsTrue(option1.Equals(option2));
        }

        [Test]
        public void OptionEqualsNone() {
            var option1 = Option<string>.None;
            var option2 = Option<string>.None;
            Assert.IsTrue(option1.Equals(option2));
        }

        [Test]
        public void OptionEqualsValueOption() {
            var option1 = "Hello".ToOption();
            var option2 = 5.ToValueOption();
            // ReSharper disable once SuspiciousTypeConversion.Global
            Assert.IsFalse(option1.Equals(option2));
        }

        [Test]
        public void OptionToString() {
            var option1 = "Hello".ToOption();
            Assert.IsTrue(option1.ToString() == "Hello");
        }

        [Test]
        public void OptionToStringNone() {
            var option1 = Option<string>.None;
            Assert.IsTrue(option1.ToString() == "None");
        }

        string _test = "TestString";

        [UnityTest]
        public IEnumerator OptionDoAsync() => UniTask.ToCoroutine(async () => {
            _test = string.Empty;
            await "TestString".ToOption()
                              .DoAsync(async s => {
                                   await UniTask.Delay(100);
                                   _test = s.ToUpper();
                               });
            Assert.IsTrue(_test == "TESTSTRING");
        });

        [UnityTest]
        public IEnumerator OptionDoAsyncIfNone() => UniTask.ToCoroutine(async () => {
            _test = string.Empty;
            await Option<string>.None
                                .DoAsync(async s => {
                                             await UniTask.Delay(100);
                                             _test = s.ToUpper();
                                         },
                                         async () => {
                                             await UniTask.Delay(100);
                                             _test = "None";
                                         });
            Assert.IsTrue(_test == "None");
            await "TestString".ToOption()
                              .DoAsync(async s => {
                                           await UniTask.Delay(100);
                                           _test = s.ToUpper();
                                       },
                                       async () => {
                                           await UniTask.Delay(100);
                                           _test = "None";
                                       });
            Assert.IsTrue(_test == "TESTSTRING");
        });

        [UnityTest]
        public IEnumerator ValueOptionDoAsync() => UniTask.ToCoroutine(async () => {
            _test = string.Empty;
            await 5.ToValueOption()
                   .DoAsync(async s => {
                        await UniTask.Delay(100);
                        _test = s.ToString();
                    });
            Assert.IsTrue(_test == "5");
        });

        [Test]
        public void ValueOptionFlattenTest() {
            var option = 5.ToValueOption();
            var nestedOption = option.ToValueOption();
            var flattenedOption = nestedOption.Flatten();
            Assert.That(flattenedOption.IsSome);
        #pragma warning disable CS0183 // 'is' expression's given expression is always of the provided type
            Assert.That(flattenedOption is ValueOption<int>);
        #pragma warning restore CS0183 // 'is' expression's given expression is always of the provided type
            Assert.IsTrue(flattenedOption == 5);
        }

        SerializableOption<GameObject> _gameObject;
        [Test]
        public void SerializableOptionToOptionNone() {
            Assert.IsTrue(_gameObject.ToOption().IsNone);
        }
    }
}