namespace Dragonfly.Umbraco7Helpers
{
    using System;
    using System.Collections.Generic;

    public static class Macros
    {
        private const string ThisClassName = "Dragonfly.Umbraco7Helpers.Macros";

        /// <summary>
        /// Return an Object for a Macro Parameter
        /// </summary>
        /// <param name="MacrosCollection">ex: 'Model.MacroParameters'</param>
        /// <param name="Key">Parameter alias</param>
        /// <param name="DefaultNullValue"></param>
        /// <returns>Default null value - if provided, otherwise, NULL</returns>
        public static object GetSafeParamObject(IDictionary<string, object> MacrosCollection, string Key, object DefaultNullValue = null)
        {
            return MacrosCollection[Key] != null ? MacrosCollection[Key] : DefaultNullValue;
        }

        /// <summary>
        /// Return a String for a Macro Parameter
        /// </summary>
        /// <param name="MacrosCollection">ex: 'Model.MacroParameters'</param>
        /// <param name="Key">Parameter alias</param>
        /// <param name="DefaultNullValue">Default null value - if provided, otherwise, an Empty String</param>
        /// <returns></returns>
        public static string GetSafeParamString(IDictionary<string, object> MacrosCollection, string Key, string DefaultNullValue = "")
        {
            return MacrosCollection[Key] != null ? MacrosCollection[Key].ToString() : DefaultNullValue;
        }

        /// <summary>
        /// Return an Int for a Macro Parameter
        /// </summary>
        /// <param name="MacrosCollection">ex: 'Model.MacroParameters'</param>
        /// <param name="Key">Parameter alias</param>
        /// <param name="DefaultNullValue"></param>
        /// <returns>Default null value - if provided, otherwise, 0</returns>
        public static int GetSafeParamInt(IDictionary<string, object> MacrosCollection, string Key, int DefaultNullValue = 0)
        {
            var value = MacrosCollection[Key];

            if (value != null && value.ToString() != "")
            {
                var returnInt = Convert.ToInt32(value);
                return returnInt;
            }
            else
            {
                return DefaultNullValue;
            }

        }

        /// <summary>
        /// Return a Boolean for a Macro Parameter
        /// </summary>
        /// <param name="MacrosCollection">ex: 'Model.MacroParameters'</param>
        /// <param name="Key">Parameter alias</param>
        /// <param name="DefaultNullValue"></param>
        /// <returns>Default null value - if provided, otherwise, false</returns>
        public static bool GetSafeParamBool(IDictionary<string, object> MacrosCollection, string Key, bool DefaultNullValue = false)
        {
            var value = MacrosCollection[Key];

            if (value != null && value.ToString() != "")
            {
                var returnBool = false;

                if (value.ToString() == "1")
                {
                    returnBool = true;
                }
                else if (value.ToString() == "true")
                {
                    returnBool = true;
                }

                return returnBool;
            }
            else
            {
                return DefaultNullValue;
            }
        }
    }
}
