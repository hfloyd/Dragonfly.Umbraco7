namespace Dragonfly.Umbraco7Helpers
{
    using Umbraco.Web;

    public static class Dictionary
    {
        private const string ThisClassName = "Dragonfly.Umbraco7Helpers.Dictionary";

        private static UmbracoHelper umbracoHelper = new Umbraco.Web.UmbracoHelper(Umbraco.Web.UmbracoContext.Current);

        /// <summary>
        /// The get dictionary value or a placeholder representing that the dictionary value needs to be entered.
        /// </summary>
        /// <param name="DictionaryKey">
        /// The dictionary key.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetDictionaryOrPlaceholder(string DictionaryKey)
        {
            var DictValue = umbracoHelper.GetDictionaryValue(DictionaryKey);
            if (DictValue == string.Empty)
            {
                return string.Format("[{0}]", DictionaryKey);
            }
            else
            {
                return DictValue;
            }

            //TODO: Add 'create' functionality options

        }
    }
}
