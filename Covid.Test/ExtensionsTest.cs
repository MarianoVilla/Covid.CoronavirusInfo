using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Covid.Lib;

namespace Covid.Test
{
    [TestFixture]
    public class ExtensionsTest
    {
        [Test]
        public void StringTryToKMBLong_ShouldWork()
        {
            var ValidLong = "1000";
            Assert.AreEqual(ValidLong.TryToLongKMB(), "1K");
        }
        [Test]
        public void DictionaryTryGetValueMulti_ShouldWork()
        {
            var Dict = new Dictionary<string, object>() { { "SomeKey", "SomeValue" } };

            Assert.True(Dict.TryGetValueMultiKey(out object TheValue, "SomeKey"));
            Assert.True(Dict.TryGetValueMultiKey(out TheValue, "somekey"));
            Assert.True(Dict.TryGetValueMultiKey(out TheValue, "SOMEKEY"));
            Assert.True(Dict.TryGetValueMultiKey(out TheValue, "SomeKey", "SOMEKEY", "AnotherOne"));
        }
        [Test]
        public void DictionaryTryGetValueMulti_ShouldFail()
        {
            var Dict = new Dictionary<string, object>() { { "SomeKey", "SomeValue" } };

            Assert.False(Dict.TryGetValueMultiKey(out object TheValue, ""));
            Assert.False(Dict.TryGetValueMultiKey(out TheValue, null));
            Assert.False(Dict.TryGetValueMultiKey(out TheValue, "AKey", "AnotherKey", null));
        }
        [Test]
        public void ObjectToJson_ShouldWork()
        {
            Assert.AreEqual("{}", new { }.ToJson());

            var OnePropActual = new { StringProp = "SomeValue" }.ToJson();
            Assert.AreEqual(@"{""StringProp"":""SomeValue""}", OnePropActual);

            var TwoPropsActual = new { StringProp = "SomeValue", IntProp = 1  }.ToJson();
            Assert.AreEqual(@"{""StringProp"":""SomeValue"",""IntProp"":1}", TwoPropsActual);

            var NestedObjectActual = new { StringProp = "SomeValue", IntProp = 1, NestedObject = new { NestedString = "NestedValue" } }.ToJson();
            Assert.AreEqual(@"{""StringProp"":""SomeValue"",""IntProp"":1,""NestedObject"":{""NestedString"":""NestedValue""}}", NestedObjectActual);

            object Null = null;
            Assert.AreEqual("null", Null.ToJson());

        }
        [Test]
        public void StringFromJson_ShouldWork()
        {
            var EmptyObject = new { };
            Assert.AreEqual("{}".FromJson<dynamic>(), EmptyObject);
        }

    }
}
