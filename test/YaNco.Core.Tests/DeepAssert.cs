using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using KellermanSoftware.CompareNetObjects;

namespace YaNco.Core.Tests
{
    [ExcludeFromCodeCoverage]
    public static class DeepAssert {

        public static void Equal<T> (T expected, T actual, params string[] propertiesToIgnore) {
            var compareLogic = new CompareLogic {Config =
            {
                MembersToIgnore = new List<string>(),
                IgnoreObjectTypes = true,
                IgnoreCollectionOrder = true,
                SkipInvalidIndexers = true,
            }};

            foreach (var property in propertiesToIgnore) {
                compareLogic.Config.MembersToIgnore.Add (property);
            }

            var comparisonResult = compareLogic.Compare (expected, actual);

            if (!comparisonResult.AreEqual) {
                throw new ObjectEqualException (comparisonResult.DifferencesString);
            }
        }
    }
}