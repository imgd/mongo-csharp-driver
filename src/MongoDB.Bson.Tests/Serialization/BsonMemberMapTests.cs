﻿/* Copyright 2010-2016 MongoDB Inc.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;
using NUnit.Framework;

namespace MongoDB.Bson.Tests.Serialization
{
    [TestFixture]
    public class BsonMemberMapTests
    {
        private class TestClass
        {
            public readonly int ReadOnlyField;

            public int Field;

            public int Property { get; set; }

            public int PrivateSettableProperty { get; private set; }

            public int ReadOnlyProperty
            {
                get { return Property + 1; }
            }

            public TestClass()
            {
                ReadOnlyField = 13;
                PrivateSettableProperty = 10;
            }
        }

        [Test]
        public void TestGettingAField()
        {
            var instance = new TestClass { Field = 42 };
            var classMap = new BsonClassMap<TestClass>(cm => cm.AutoMap());
            var memberMap = classMap.GetMemberMap("Field");

            int value = (int)memberMap.Getter(instance);

            Assert.AreEqual(42, value);
        }

        [Test]
        public void TestIsReadOnlyPropertyOfAField()
        {
            var classMap = new BsonClassMap<TestClass>(cm => cm.AutoMap());
            var memberMap = classMap.GetMemberMap("Field");

            Assert.IsFalse(memberMap.IsReadOnly);
        }

        [Test]
        public void TestSetElementNameThrowsWhenElementNameContainsNulls()
        {
            var classMap = new BsonClassMap<TestClass>(cm => cm.AutoMap());
            var memberMap = classMap.GetMemberMap("Property");
            Assert.Throws<ArgumentException>(() => { memberMap.SetElementName("a\0b"); });
        }

        [Test]
        public void TestSetElementNameThrowsWhenElementNameIsNull()
        {
            var classMap = new BsonClassMap<TestClass>(cm => cm.AutoMap());
            var memberMap = classMap.GetMemberMap("Property");
            Assert.Throws<ArgumentNullException>(() => { memberMap.SetElementName(null); });
        }

        [Test]
        public void TestSettingAField()
        {
            var instance = new TestClass();
            var classMap = new BsonClassMap<TestClass>(cm => cm.AutoMap());
            var memberMap = classMap.GetMemberMap("Field");

            memberMap.Setter(instance, 42);

            Assert.AreEqual(42, instance.Field);
        }

        [Test]
        public void TestGettingAReadOnlyField()
        {
            var instance = new TestClass();
            var classMap = new BsonClassMap<TestClass>(cm =>
            {
                cm.AutoMap();
                cm.MapMember(c => c.ReadOnlyField);
            });
            var memberMap = classMap.GetMemberMap("ReadOnlyField");

            int value = (int)memberMap.Getter(instance);

            Assert.AreEqual(13, value);
        }

        [Test]
        public void TestIsReadOnlyPropertyOfAReadOnlyField()
        {
            var classMap = new BsonClassMap<TestClass>(cm =>
            {
                cm.AutoMap();
                cm.MapMember(c => c.ReadOnlyField);
            });
            var memberMap = classMap.GetMemberMap("ReadOnlyField");

            Assert.IsTrue(memberMap.IsReadOnly);
        }

        [Test]
        public void TestSettingAReadOnlyField()
        {
            var instance = new TestClass();
            var classMap = new BsonClassMap<TestClass>(cm =>
            {
                cm.AutoMap();
                cm.MapMember(c => c.ReadOnlyField);
            });
            var memberMap = classMap.GetMemberMap("ReadOnlyField");

            TestDelegate action = () => memberMap.Setter(instance, 12);

            var expectedMessage = "The field 'System.Int32 ReadOnlyField' of class 'MongoDB.Bson.Tests.Serialization.BsonMemberMapTests+TestClass' is readonly. To avoid this exception, call IsReadOnly to ensure that setting a value is allowed.";
            Assert.That(action, Throws.Exception.TypeOf<BsonSerializationException>().With.Message.EqualTo(expectedMessage));
        }

        [Test]
        public void TestGettingAProperty()
        {
            var instance = new TestClass { Property = 42 };
            var classMap = new BsonClassMap<TestClass>(cm => cm.AutoMap());
            var memberMap = classMap.GetMemberMap("Property");

            int value = (int)memberMap.Getter(instance);

            Assert.AreEqual(42, value);
        }

        [Test]
        public void TestIsReadOnlyPropertyOfAProperty()
        {
            var classMap = new BsonClassMap<TestClass>(cm => cm.AutoMap());
            var memberMap = classMap.GetMemberMap("Property");

            Assert.IsFalse(memberMap.IsReadOnly);
        }

        [Test]
        public void TestSettingAProperty()
        {
            var instance = new TestClass();
            var classMap = new BsonClassMap<TestClass>(cm => cm.AutoMap());
            var memberMap = classMap.GetMemberMap("Property");

            memberMap.Setter(instance, 42);

            Assert.AreEqual(42, instance.Property);
        }

        [Test]
        public void TestGettingAPrivateSettableProperty()
        {
            var instance = new TestClass();
            var classMap = new BsonClassMap<TestClass>(cm => cm.AutoMap());
            var memberMap = classMap.GetMemberMap("PrivateSettableProperty");

            int value = (int)memberMap.Getter(instance);

            Assert.AreEqual(10, value);
        }

        [Test]
        public void TestIsReadOnlyPropertyOfAPrivateSettableProperty()
        {
            var classMap = new BsonClassMap<TestClass>(cm => cm.AutoMap());
            var memberMap = classMap.GetMemberMap("PrivateSettableProperty");

            Assert.IsFalse(memberMap.IsReadOnly);
        }

        [Test]
        public void TestSettingAPrivateSettableProperty()
        {
            var instance = new TestClass();
            var classMap = new BsonClassMap<TestClass>(cm => cm.AutoMap());
            var memberMap = classMap.GetMemberMap("PrivateSettableProperty");

            memberMap.Setter(instance, 42);

            Assert.AreEqual(42, instance.PrivateSettableProperty);
        }

        [Test]
        public void TestGettingAReadOnlyProperty()
        {
            var instance = new TestClass { Property = 10 };
            var classMap = new BsonClassMap<TestClass>(cm =>
            {
                cm.AutoMap();
                cm.MapMember(c => c.ReadOnlyProperty);
            });

            var memberMap = classMap.GetMemberMap("ReadOnlyProperty");

            int value = (int)memberMap.Getter(instance);

            Assert.AreEqual(11, value);
        }

        [Test]
        public void TestIsReadOnlyPropertyOfAReadOnlyProperty()
        {
            var classMap = new BsonClassMap<TestClass>(cm =>
            {
                cm.AutoMap();
                cm.MapMember(c => c.ReadOnlyProperty);
            });
            var memberMap = classMap.GetMemberMap("ReadOnlyProperty");

            Assert.IsTrue(memberMap.IsReadOnly);
        }

        [Test]
        public void TestSettingAReadOnlyProperty()
        {
            var instance = new TestClass { Property = 10 };
            var classMap = new BsonClassMap<TestClass>(cm =>
            {
                cm.AutoMap();
                cm.MapMember(c => c.ReadOnlyProperty);
            });
            var memberMap = classMap.GetMemberMap("ReadOnlyProperty");

            TestDelegate action = () => memberMap.Setter(instance, 12);

            var expectedMessage = "The property 'System.Int32 ReadOnlyProperty' of class 'MongoDB.Bson.Tests.Serialization.BsonMemberMapTests+TestClass' has no 'set' accessor. To avoid this exception, call IsReadOnly to ensure that setting a value is allowed.";
            Assert.That(action, Throws.Exception.TypeOf<BsonSerializationException>().With.Message.EqualTo(expectedMessage));
        }

        [Test]
        public void TestReset()
        {
            var classMap = new BsonClassMap<TestClass>(cm =>
            {
                cm.MapMember(c => c.Property);
            });

            var originalSerializer = new Int32Serializer();

            var memberMap = classMap.GetMemberMap(x => x.Property);
            memberMap.SetDefaultValue(42);
            memberMap.SetElementName("oops");
            memberMap.SetIdGenerator(new GuidGenerator());
            memberMap.SetIgnoreIfDefault(true);
            memberMap.SetIsRequired(true);
            memberMap.SetOrder(21);
            memberMap.SetSerializer(originalSerializer);
            memberMap.SetShouldSerializeMethod(o => false);

            memberMap.Reset();

            Assert.AreEqual(0, (int)memberMap.DefaultValue);
            Assert.AreEqual("Property", memberMap.ElementName);
            Assert.IsNull(memberMap.IdGenerator);
            Assert.IsFalse(memberMap.IgnoreIfDefault);
            Assert.IsFalse(memberMap.IgnoreIfNull);
            Assert.IsFalse(memberMap.IsRequired);
            Assert.AreEqual(int.MaxValue, memberMap.Order);
            Assert.AreNotSame(originalSerializer, memberMap.GetSerializer());
            Assert.IsNull(memberMap.ShouldSerializeMethod);
        }
    }
}
