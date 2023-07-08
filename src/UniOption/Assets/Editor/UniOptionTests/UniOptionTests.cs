using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UniOption;

public class UniOptionTests {
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
    public void OptionNone() {
        var testString = "TestString";
        testString = null;
        Option<string> stringOption = testString;
        Assert.IsTrue(stringOption != null);
        Assert.IsFalse(stringOption.IsSome);
        Assert.IsTrue(stringOption.IsNone);
        Assert.IsTrue(stringOption.Reduce(string.Empty) == string.Empty);
    }

    [Test]
    public void ValueOptionNone() {
        int? nullableInt = 5;
        nullableInt = null;
        var intOption = nullableInt.ToValueOption();
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
                            .MapObject(i => "5");
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
        var upperOption  = string.Empty;
        stringOption.Do(s => upperOption = s.ToUpper());
        Assert.IsTrue(upperOption == "TESTSTRING");
    }

    [Test]
    public void ValueOptionDo() {
        var intOption   = 5.ToValueOption();
        var upperOption = string.Empty;
        intOption.Do(s => upperOption = s.ToString());
        Assert.IsTrue(upperOption == "5");
    }

    [Test]
    public void OptionDoNone() {
        var stringOption = Option<string>.None;
        var upperOption  = string.Empty;
        stringOption.Do(s => upperOption = s.ToUpper());
        Assert.IsTrue(upperOption == string.Empty);
    }

    [Test]
    public void ValueOptionDoNone() {
        var intOption   = ValueOption<int>.None;
        var upperOption = string.Empty;
        intOption.Do(s => upperOption = s.ToString());
        Assert.IsTrue(upperOption == string.Empty);
    }

    [Test]
    public void OptionDoOrElse() {
        var stringOption = "TestString".ToOption();
        var upperOption  = string.Empty;
        stringOption.Do(s => upperOption = s.ToUpper(),
                        () => upperOption = "None");
        Assert.IsTrue(upperOption == "TESTSTRING");
    }

    [Test]
    public void OptionDoOrElseNone() {
        var stringOption = Option<string>.None;
        var upperOption  = string.Empty;
        stringOption.Do(s => upperOption = s.ToUpper(),
                        () => upperOption = "None");
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
        int? nullableInt = null;
        var intOption = nullableInt.ToValueOption()
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

    [UnityTest]
    public IEnumerator OptionDoAsync() => UniTask.ToCoroutine(async () => {
        var upperOption = string.Empty;
        await "TestString".ToOption()
                          .DoAsync(async s => {
                               await UniTask.Delay(100);
                               upperOption = s.ToUpper();
                           });
        Assert.IsTrue(upperOption == "TESTSTRING");
    });

    [UnityTest]
    public IEnumerator ValueOptionDoAsync() => UniTask.ToCoroutine(async () => {
        var upperOption = string.Empty;
        await 5.ToValueOption()
               .DoAsync(async s => {
                    await UniTask.Delay(100);
                    upperOption = s.ToString();
                });
        Assert.IsTrue(upperOption == "5");
    });
}