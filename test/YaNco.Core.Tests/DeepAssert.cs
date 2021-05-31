using System.Collections.Generic;
using KellermanSoftware.CompareNetObjects;

namespace YaNco.Core.Tests
{
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
                throw new ObjectEqualException (expected, actual, comparisonResult.DifferencesString);
            }
        }
    }
}