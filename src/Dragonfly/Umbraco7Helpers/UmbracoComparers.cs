namespace Dragonfly.Umbraco7Helpers
{
    using System.Collections.Generic;
    using Umbraco.Core.Models;

    public class PublishedContentComparer : IEqualityComparer<IPublishedContent>
        {
            public bool Equals(IPublishedContent x, IPublishedContent y)
            {
                if (ReferenceEquals(x, y)) return true;

                if (ReferenceEquals(x, null) || ReferenceEquals(y, null)) return false;

                return x.Id == y.Id;
            }

        public int GetHashCode(IPublishedContent obj)
            {
                if (ReferenceEquals(obj, null)) return 0;

                return obj.Id.GetHashCode();
            }

    }

}
