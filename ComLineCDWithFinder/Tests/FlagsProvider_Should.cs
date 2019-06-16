using System;
using System.Linq;
using ComLineCDWithFinder.Infrastructure;
using FluentAssertions;
using NUnit.Framework;

namespace ComLineCDWithFinder.Tests
{
    [TestFixture]
    public class FlagsProvider_Should
    {
        [Test]
        [TestCase(new object[] {"-all", "-s"})]
        [TestCase(new object[] {"-count=10", "-s"})]
        [TestCase(new object[] {"-i", "-c"})]
        [TestCase(new object[] {"-i", "-h"})]
        public void ReturnUndefined_OnIncompatibleFlags(object[] args)
        {
            FlagsProvider.GetOptions(args.Select(o=>o.ToString()).ToArray()).Should().BeEquivalentTo(new[]{Option.Undefined});
        }

        [Test]
        public void ReturnArray_OnValidFlags()
        {
            FlagsProvider.GetOptions("-i", "-s").Should().BeEquivalentTo(new[]{Option.IgnoreCase, Option.OutputSingle});
        }

        [Test]
        [TestCase(new object[] {"-all", "-sss"})]
        [TestCase(new object[] {"-all", "-t", "-i"})]
        [TestCase(new object[] {"-count=4r"})]
        [TestCase(new object[] {"-r"})]
        public void ReturnUndefined_OnInvalidFlag(object[] args)
        {
            FlagsProvider.GetOptions(args.Select(o=>o.ToString()).ToArray()).Should().BeEquivalentTo(new[]{Option.Undefined});
        }

        [Test]
        public void Throw_OnGetCount_IfNoCountFlag()
        {
            Assert.Throws<ArgumentException>(
                ()=>FlagsProvider.GetCountForCountFlag("-s","-i")
                );
        }

        [Test]
        public void Throw_OnGetCount_IfCountFlagInvalid()
        {
            Assert.Throws<ArgumentException>(
                ()=>FlagsProvider.GetCountForCountFlag("-count=3e3","-i")
            );
        }

        [Test]
        [TestCase(new object[] {"-count=50"}, 50)]
        [TestCase(new object[] {"-i","-count=10"}, 10)]
        [TestCase(new object[] {"-count=1330", "-c"}, 1330)]
        public void ReturnCount_OnGetCount_IfCountFlagValid(object[] args, int count)
        {
            FlagsProvider.GetCountForCountFlag(
                args.Select(o=>o.ToString()).ToArray()
                ).Should()
                .Be(count);
        }
    }
}
