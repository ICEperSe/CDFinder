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
            FlagsProvider.GetOptions("-i -s").Should().BeEquivalentTo(new[]{Option.Help});
        }

        [Test]
        [TestCase(new object[] {"-all", "-sss"})]
        [TestCase(new object[] {"-all", "-t", "-i"})]
        [TestCase(new object[] {"-y"})]
        public void ReturnUndefined_OnInvalidFlag(string[] args)
        {
            FlagsProvider.GetOptions(args).Should().BeEquivalentTo(new[]{Option.Undefined});
        }
    }
}
