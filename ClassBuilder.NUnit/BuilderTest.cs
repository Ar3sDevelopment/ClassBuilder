using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ClassBuilder.Classes;
using ClassBuilder.NUnit.Common.Classes;
using NUnit.Framework;

namespace ClassBuilder.NUnit
{
    [TestFixture]
    public class BuilderTest
    {
        [Test]
        public void TestNull()
        {
            var stopwatch = Stopwatch.StartNew();
            var a = default(TestA);
            var b = Builder.Build(a).To<TestB>();
            Assert.IsNull(b);
            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
        }

        [Test]
        public void TestNoBuilder()
        {
            var stopwatch = Stopwatch.StartNew();
            var a = new TestA { A = "test", C = "test" };
            var b = new TestB { A = "test", B = a.C + " no mapper" };
            var str = "A: " + b.A + " B: " + b.B;
            Assert.AreEqual(str, "A: test B: test no mapper");
            Console.WriteLine(str);
            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
        }

        [Test]
        public void TestBuilder()
        {
            var stopwatch = Stopwatch.StartNew();
            var a = new TestA { A = "test", C = "test" };
            var b = Builder.Build(a).To<TestB>();
            var str = "A: " + b.A + " B: " + b.B;
            Assert.AreEqual(str, "A: test B: test mapper");
            Console.WriteLine(str);
            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
        }

        [Test]
        public void TestBuilderEdit()
        {
            var stopwatch = Stopwatch.StartNew();
            var a = new TestA { A = "test", C = "test" };
            var b = new TestB { A = "pippo", B = "pluto" };
            Builder.Build(a).To(b);
            var str = "A: " + b.A + " B: " + b.B;
            Assert.AreEqual(str, "A: test B: test mapper");
            Console.WriteLine(str);
            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
        }

        [Test]
        public void TestDefaultMapper()
        {
            var stopwatch = Stopwatch.StartNew();
            var b = new TestB { A = "test", B = "test2" };
            var a = Builder.Build(b).To<TestA>();
            var str = "A: " + a.A + " C: " + a.C;
            Assert.AreEqual(str, "A: test C: test2");
            Console.WriteLine(str);
            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
        }

        [Test]
        public void TestList()
        {
            var stopwatch = Stopwatch.StartNew();
            var b = new List<TestB>();

            for (var i = 0; i < 10; i++)
                b.Add(new TestB { A = "test", B = "test2" });

            var a = Builder.BuildList(b).ToList<TestA>();

            foreach (var i in a)
            {
                var str = "A: " + i.A + " C: " + i.C;
                Assert.AreEqual(str, "A: test C: test2");
                Console.WriteLine(str);
            }

            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
        }

        [Test]
        public void TestDefaultMapperAttributes()
        {
            var stopwatch = Stopwatch.StartNew();
            var d = new TestD { A = "test", B = "test2" };
            var c = Builder.Build(d).To<TestC>();
            var str = "A: " + c.A + " C: " + c.C;
            Assert.AreEqual(str, "A: test C: test2");
            Console.WriteLine(str);
            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
        }

        [Test]
        public void TestDefaultMapperEqualsAttribute()
        {
            var stopwatch = Stopwatch.StartNew();
            var e = new TestE { A = "test", B = "test2", F = "test3" };
            var f = Builder.Build(e).To<TestF>();
            var str = "C: " + f.C + " D: " + f.D;
            Assert.AreEqual(str, "C: test D: test2");
            Console.WriteLine(str);
            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
        }

        [Test]
        public void TestIntFloat()
        {
            var stopwatch = Stopwatch.StartNew();
            var source = 10;
            var destination = Builder.Build(source).To<float>();
            Console.WriteLine(destination);
            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
        }
    }
}