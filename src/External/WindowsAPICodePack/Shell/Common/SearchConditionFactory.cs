﻿//Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using MS.WindowsAPICodePack.Internal;

namespace Microsoft.WindowsAPICodePack.Shell
{
    /// <summary>
    /// Provides methods for creating or resolving a condition tree 
    /// that was obtained by parsing a query string.
    /// </summary>
    public static class SearchConditionFactory
    {
        /// <summary>
        /// Creates a leaf condition node that represents a comparison of property value and constant value. 
        /// </summary>
        /// <param name="propertyName">The name of a property to be compared, or null for an unspecified property. 
        /// The locale name of the leaf node is LOCALE_NAME_USER_DEFAULT.</param>
        /// <param name="value">The constant value against which the property value should be compared.</param>
        /// <param name="operation">Specific condition to be used when comparing the actual value and the expected value of the given property</param>
        /// <returns>SearchCondition based on the given parameters</returns>
        /// <remarks>
        /// The search will only work for files that are indexed, as well as the specific properties are indexed. To find 
        /// the properties that are indexed, look for the specific property's property description and 
        /// <see cref="P:Microsoft.WindowsAPICodePack.Shell.PropertySystem.ShellPropertyDescription.TypeFlags"/> property for IsQueryable flag.
        /// </remarks>
        public static SearchCondition CreateLeafCondition(string propertyName, string value, SearchConditionOperation operation)
        {
            PropVariant propVar = new PropVariant();
            propVar.SetString(value);

            return CreateLeafCondition(propertyName, propVar, null, operation);
        }

        /// <summary>
        /// Creates a leaf condition node that represents a comparison of property value and constant value. 
        /// Overload method takes a DateTime parameter for the comparison value.
        /// </summary>
        /// <param name="propertyName">The name of a property to be compared, or null for an unspecified property. 
        /// The locale name of the leaf node is LOCALE_NAME_USER_DEFAULT.</param>
        /// <param name="value">The DateTime value against which the property value should be compared.</param>
        /// <param name="operation">Specific condition to be used when comparing the actual value and the expected value of the given property</param>
        /// <returns>SearchCondition based on the given parameters</returns>
        /// <remarks>
        /// The search will only work for files that are indexed, as well as the specific properties are indexed. To find 
        /// the properties that are indexed, look for the specific property's property description and 
        /// <see cref="P:Microsoft.WindowsAPICodePack.Shell.PropertySystem.ShellPropertyDescription.TypeFlags"/> property for IsQueryable flag.
        /// </remarks>
        public static SearchCondition CreateLeafCondition(string propertyName, DateTime value, SearchConditionOperation operation)
        {
            PropVariant propVar = new PropVariant();
            propVar.SetDateTime(value);

            return CreateLeafCondition(propertyName, propVar, "System.StructuredQuery.CustomProperty.DateTime", operation);
        }

        /// <summary>
        /// Creates a leaf condition node that represents a comparison of property value and Integer value. 
        /// </summary>
        /// <param name="propertyName">The name of a property to be compared, or null for an unspecified property. 
        /// The locale name of the leaf node is LOCALE_NAME_USER_DEFAULT.</param>
        /// <param name="value">The Integer value against which the property value should be compared.</param>
        /// <param name="operation">Specific condition to be used when comparing the actual value and the expected value of the given property</param>
        /// <returns>SearchCondition based on the given parameters</returns>
        /// <remarks>
        /// The search will only work for files that are indexed, as well as the specific properties are indexed. To find 
        /// the properties that are indexed, look for the specific property's property description and 
        /// <see cref="P:Microsoft.WindowsAPICodePack.Shell.PropertySystem.ShellPropertyDescription.TypeFlags"/> property for IsQueryable flag.
        /// </remarks>
        public static SearchCondition CreateLeafCondition(string propertyName, int value, SearchConditionOperation operation)
        {
            PropVariant propVar = new PropVariant();
            propVar.SetInt(value);

            return CreateLeafCondition(propertyName, propVar, "System.StructuredQuery.CustomProperty.Integer", operation);
        }

        /// <summary>
        /// Creates a leaf condition node that represents a comparison of property value and Boolean value. 
        /// </summary>
        /// <param name="propertyName">The name of a property to be compared, or null for an unspecified property. 
        /// The locale name of the leaf node is LOCALE_NAME_USER_DEFAULT.</param>
        /// <param name="value">The Boolean value against which the property value should be compared.</param>
        /// <param name="operation">Specific condition to be used when comparing the actual value and the expected value of the given property</param>
        /// <returns>SearchCondition based on the given parameters</returns>
        /// <remarks>
        /// The search will only work for files that are indexed, as well as the specific properties are indexed. To find 
        /// the properties that are indexed, look for the specific property's property description and 
        /// <see cref="P:Microsoft.WindowsAPICodePack.Shell.PropertySystem.ShellPropertyDescription.TypeFlags"/> property for IsQueryable flag.
        /// </remarks>
        public static SearchCondition CreateLeafCondition(string propertyName, bool value, SearchConditionOperation operation)
        {
            PropVariant propVar = new PropVariant();
            propVar.SetBool(value);

            return CreateLeafCondition(propertyName, propVar, "System.StructuredQuery.CustomProperty.Boolean", operation);
        }

        /// <summary>
        /// Creates a leaf condition node that represents a comparison of property value and Floating Point value. 
        /// </summary>
        /// <param name="propertyName">The name of a property to be compared, or null for an unspecified property. 
        /// The locale name of the leaf node is LOCALE_NAME_USER_DEFAULT.</param>
        /// <param name="value">The Floating Point value against which the property value should be compared.</param>
        /// <param name="operation">Specific condition to be used when comparing the actual value and the expected value of the given property</param>
        /// <returns>SearchCondition based on the given parameters</returns>
        /// <remarks>
        /// The search will only work for files that are indexed, as well as the specific properties are indexed. To find 
        /// the properties that are indexed, look for the specific property's property description and 
        /// <see cref="P:Microsoft.WindowsAPICodePack.Shell.PropertySystem.ShellPropertyDescription.TypeFlags"/> property for IsQueryable flag.
        /// </remarks>
        public static SearchCondition CreateLeafCondition(string propertyName, double value, SearchConditionOperation operation)
        {
            PropVariant propVar = new PropVariant();
            propVar.SetDouble(value);

            return CreateLeafCondition(propertyName, propVar, "System.StructuredQuery.CustomProperty.FloatingPoint", operation);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1304:SpecifyCultureInfo", MessageId = "System.String.ToLower")]
        private static SearchCondition CreateLeafCondition(string propertyName, PropVariant propVar, string valueType, SearchConditionOperation operation)
        {
            IConditionFactory nativeConditionFactory = null;
            SearchCondition condition = null;

            try
            {
                // Same as the native "IConditionFactory:MakeLeaf" method
                nativeConditionFactory = (IConditionFactory)new ConditionFactoryCoClass();

                ICondition nativeCondition = null;

                if (string.IsNullOrEmpty(propertyName) || propertyName.ToLower() == "system.null")
                    propertyName = null;

                HRESULT hr = HRESULT.E_FAIL;

                hr = nativeConditionFactory.MakeLeaf(propertyName, operation, valueType,
                    ref propVar, null, null, null, false, out nativeCondition);

                if (!CoreErrorHelper.Succeeded((int)hr))
                    Marshal.ThrowExceptionForHR((int)hr);

                // Create our search condition and set the various properties.
                condition = new SearchCondition(nativeCondition);
            }
            finally
            {
                if (nativeConditionFactory != null)
                    Marshal.ReleaseComObject(nativeConditionFactory);
            }

            return condition;
        }

        /// <summary>
        /// Creates a leaf condition node that represents a comparison of property value and constant value. 
        /// </summary>
        /// <param name="propertyKey">The property to be compared.</param>
        /// <param name="value">The constant value against which the property value should be compared.</param>
        /// <param name="operation">Specific condition to be used when comparing the actual value and the expected value of the given property</param>
        /// <returns>SearchCondition based on the given parameters</returns>
        /// <remarks>
        /// The search will only work for files that are indexed, as well as the specific properties are indexed. To find 
        /// the properties that are indexed, look for the specific property's property description and 
        /// <see cref="P:Microsoft.WindowsAPICodePack.Shell.PropertySystem.ShellPropertyDescription.TypeFlags"/> property for IsQueryable flag.
        /// </remarks>
        public static SearchCondition CreateLeafCondition(PropertyKey propertyKey, string value, SearchConditionOperation operation)
        {
            string canonicalName;
            PropertySystemNativeMethods.PSGetNameFromPropertyKey(ref propertyKey, out canonicalName);

            if (!string.IsNullOrEmpty(canonicalName))
                return CreateLeafCondition(canonicalName, value, operation);
            else
                throw new ArgumentException("Given property key is invalid", "propertyKey");
        }

        /// <summary>
        /// Creates a leaf condition node that represents a comparison of property value and constant value. 
        /// Overload method takes a DateTime parameter for the comparison value.
        /// </summary>
        /// <param name="propertyKey">The property to be compared.</param>
        /// <param name="value">The DateTime value against which the property value should be compared.</param>
        /// <param name="operation">Specific condition to be used when comparing the actual value and the expected value of the given property</param>
        /// <returns>SearchCondition based on the given parameters</returns>
        /// <remarks>
        /// The search will only work for files that are indexed, as well as the specific properties are indexed. To find 
        /// the properties that are indexed, look for the specific property's property description and 
        /// <see cref="P:Microsoft.WindowsAPICodePack.Shell.PropertySystem.ShellPropertyDescription.TypeFlags"/> property for IsQueryable flag.
        /// </remarks>
        public static SearchCondition CreateLeafCondition(PropertyKey propertyKey, DateTime value, SearchConditionOperation operation)
        {
            string canonicalName;
            PropertySystemNativeMethods.PSGetNameFromPropertyKey(ref propertyKey, out canonicalName);

            if (!string.IsNullOrEmpty(canonicalName))
                return CreateLeafCondition(canonicalName, value, operation);
            else
                throw new ArgumentException("Given property key is invalid", "propertyKey");
        }

        /// <summary>
        /// Creates a leaf condition node that represents a comparison of property value and Boolean value. 
        /// Overload method takes a DateTime parameter for the comparison value.
        /// </summary>
        /// <param name="propertyKey">The property to be compared.</param>
        /// <param name="value">The boolean value against which the property value should be compared.</param>
        /// <param name="operation">Specific condition to be used when comparing the actual value and the expected value of the given property</param>
        /// <returns>SearchCondition based on the given parameters</returns>
        /// <remarks>
        /// The search will only work for files that are indexed, as well as the specific properties are indexed. To find 
        /// the properties that are indexed, look for the specific property's property description and 
        /// <see cref="P:Microsoft.WindowsAPICodePack.Shell.PropertySystem.ShellPropertyDescription.TypeFlags"/> property for IsQueryable flag.
        /// </remarks>
        public static SearchCondition CreateLeafCondition(PropertyKey propertyKey, bool value, SearchConditionOperation operation)
        {
            string canonicalName;
            PropertySystemNativeMethods.PSGetNameFromPropertyKey(ref propertyKey, out canonicalName);

            if (!string.IsNullOrEmpty(canonicalName))
                return CreateLeafCondition(canonicalName, value, operation);
            else
                throw new ArgumentException("Given property key is invalid", "propertyKey");
        }

        /// <summary>
        /// Creates a leaf condition node that represents a comparison of property value and Floating Point value. 
        /// Overload method takes a DateTime parameter for the comparison value.
        /// </summary>
        /// <param name="propertyKey">The property to be compared.</param>
        /// <param name="value">The Floating Point value against which the property value should be compared.</param>
        /// <param name="operation">Specific condition to be used when comparing the actual value and the expected value of the given property</param>
        /// <returns>SearchCondition based on the given parameters</returns>
        /// <remarks>
        /// The search will only work for files that are indexed, as well as the specific properties are indexed. To find 
        /// the properties that are indexed, look for the specific property's property description and 
        /// <see cref="P:Microsoft.WindowsAPICodePack.Shell.PropertySystem.ShellPropertyDescription.TypeFlags"/> property for IsQueryable flag.
        /// </remarks>
        public static SearchCondition CreateLeafCondition(PropertyKey propertyKey, double value, SearchConditionOperation operation)
        {
            string canonicalName;
            PropertySystemNativeMethods.PSGetNameFromPropertyKey(ref propertyKey, out canonicalName);

            if (!string.IsNullOrEmpty(canonicalName))
                return CreateLeafCondition(canonicalName, value, operation);
            else
                throw new ArgumentException("Given property key is invalid", "propertyKey");
        }

        /// <summary>
        /// Creates a leaf condition node that represents a comparison of property value and Integer value. 
        /// Overload method takes a DateTime parameter for the comparison value.
        /// </summary>
        /// <param name="propertyKey">The property to be compared.</param>
        /// <param name="value">The Integer value against which the property value should be compared.</param>
        /// <param name="operation">Specific condition to be used when comparing the actual value and the expected value of the given property</param>
        /// <returns>SearchCondition based on the given parameters</returns>
        /// <remarks>
        /// The search will only work for files that are indexed, as well as the specific properties are indexed. To find 
        /// the properties that are indexed, look for the specific property's property description and 
        /// <see cref="P:Microsoft.WindowsAPICodePack.Shell.PropertySystem.ShellPropertyDescription.TypeFlags"/> property for IsQueryable flag.
        /// </remarks>
        public static SearchCondition CreateLeafCondition(PropertyKey propertyKey, int value, SearchConditionOperation operation)
        {
            string canonicalName;
            PropertySystemNativeMethods.PSGetNameFromPropertyKey(ref propertyKey, out canonicalName);

            if (!string.IsNullOrEmpty(canonicalName))
                return CreateLeafCondition(canonicalName, value, operation);
            else
                throw new ArgumentException("Given property key is invalid", "propertyKey");
        }

        /// <summary>
        /// Creates a condition node that is a logical conjunction ("AND") or disjunction ("OR") 
        /// of a collection of subconditions.
        /// </summary>
        /// <param name="conditionType">The SearchConditionType of the condition node. 
        /// Must be either AndCondition or OrCondition.</param>
        /// <param name="simplyfy">TRUE to logically simplify the result, if possible; 
        /// then the result will not necessarily to be of the specified kind. FALSE if the result should 
        /// have exactly the prescribed structure. An application that plans to execute a query based on the 
        /// condition tree would typically benefit from setting this parameter to TRUE. </param>
        /// <param name="conditionNodes">Array of subconditions</param>
        /// <returns>New SearchCondition based on the operation</returns>
        public static SearchCondition CreateAndOrCondition(SearchConditionType conditionType, bool simplyfy, params SearchCondition[] conditionNodes)
        {
            // Same as the native "IConditionFactory:MakeAndOr" method
            IConditionFactory nativeConditionFactory = (IConditionFactory)new ConditionFactoryCoClass();
            ICondition result = null;

            try
            {
                // 
                List<ICondition> conditionList = new List<ICondition>();
                foreach (SearchCondition c in conditionNodes)
                    conditionList.Add(c.NativeSearchCondition);

                IEnumUnknown subConditions = new EnumUnknownClass(conditionList.ToArray());

                HRESULT hr = nativeConditionFactory.MakeAndOr(conditionType, subConditions, simplyfy, out result);

                if (!CoreErrorHelper.Succeeded((int)hr))
                    Marshal.ThrowExceptionForHR((int)hr);
            }
            finally
            {
                if (nativeConditionFactory != null)
                    Marshal.ReleaseComObject(nativeConditionFactory);
            }

            return new SearchCondition(result);
        }

        /// <summary>
        /// Creates a condition node that is a logical negation (NOT) of another condition 
        /// (a subnode of this node). 
        /// </summary>
        /// <param name="conditionToBeNegated">SearchCondition node to be negated.</param>
        /// <param name="simplyfy">True to logically simplify the result if possible; False otherwise. 
        /// In a query builder scenario, simplyfy should typically be set to false.</param>
        /// <returns>New SearchCondition</returns>
        public static SearchCondition CreateNotCondition(SearchCondition conditionToBeNegated, bool simplyfy)
        {
            // Same as the native "IConditionFactory:MakeNot" method
            IConditionFactory nativeConditionFactory = (IConditionFactory)new ConditionFactoryCoClass();
            ICondition result;

            try
            {
                HRESULT hr = nativeConditionFactory.MakeNot(conditionToBeNegated.NativeSearchCondition, simplyfy, out result);

                if (!CoreErrorHelper.Succeeded((int)hr))
                    Marshal.ThrowExceptionForHR((int)hr);
            }
            finally
            {
                if (nativeConditionFactory != null)
                    Marshal.ReleaseComObject(nativeConditionFactory);
            }

            return new SearchCondition(result);
        }

        /// <summary>
        /// Parses an input string that contains Structured Query keywords (using Advanced Query Syntax 
        /// or Natural Query Syntax) and produces a SearchCondition object.
        /// </summary>
        /// <param name="query">The query to be parsed</param>
        /// <returns>Search condition resulting from the query</returns>
        /// <remarks>For more information on structured query syntax, visit http://msdn.microsoft.com/en-us/library/bb233500.aspx and
        /// http://www.microsoft.com/windows/products/winfamily/desktopsearch/technicalresources/advquery.mspx</remarks>
        public static SearchCondition ParseStructuredQuery(string query)
        {
            return ParseStructuredQuery(query, null);
        }
        
        /// <summary>
        /// Parses an input string that contains Structured Query keywords (using Advanced Query Syntax 
        /// or Natural Query Syntax) and produces a SearchCondition object.
        /// </summary>
        /// <param name="query">The query to be parsed</param>
        /// <param name="cultureInfo">The culture used to select the localized language for keywords.</param>
        /// <returns>Search condition resulting from the query</returns>
        /// <remarks>For more information on structured query syntax, visit http://msdn.microsoft.com/en-us/library/bb233500.aspx and
        /// http://www.microsoft.com/windows/products/winfamily/desktopsearch/technicalresources/advquery.mspx</remarks>
        public static SearchCondition ParseStructuredQuery(string query, CultureInfo cultureInfo)
        {
            if (string.IsNullOrEmpty(query))
                throw new ArgumentNullException("query");
            else
            {
                IQueryParserManager nativeQueryParserManager = (IQueryParserManager)new QueryParserManagerCoClass();
                IQueryParser queryParser = null;
                IQuerySolution querySolution = null;
                ICondition result = null;

                IEntity mainType = null;

                try
                {
                    // First, try to create a new IQueryParser using IQueryParserManager
                    Guid guid = new Guid(ShellIIDGuid.IQueryParser);
                    HRESULT hr = nativeQueryParserManager.CreateLoadedParser(
                        "SystemIndex",
                        cultureInfo == null ? (ushort)0 : (ushort)cultureInfo.LCID,
                        ref guid,
                        out queryParser);

                    if (!CoreErrorHelper.Succeeded((int)hr))
                        throw Marshal.GetExceptionForHR((int)hr);

                    if (queryParser != null)
                    {
                        // If user specified natural query, set the option on the query parser
                        PropVariant optionValue = new PropVariant();
                        optionValue.SetBool(true);
                        hr = queryParser.SetOption(StructuredQuerySingleOption.NaturalSyntax, ref optionValue);

                        if (!CoreErrorHelper.Succeeded((int)hr))
                            throw Marshal.GetExceptionForHR((int)hr);

                        // Next, try to parse the query.
                        // Result would be IQuerySolution that we can use for getting the ICondition and other
                        // details about the parsed query.
                        hr = queryParser.Parse(query, null, out querySolution);

                        if (!CoreErrorHelper.Succeeded((int)hr))
                            throw Marshal.GetExceptionForHR((int)hr);

                        if (querySolution != null)
                        {
                            // Lastly, try to get the ICondition from this parsed query
                            hr = querySolution.GetQuery(out result, out mainType);

                            if (!CoreErrorHelper.Succeeded((int)hr))
                                throw Marshal.GetExceptionForHR((int)hr);
                        }
                    }

                    return new SearchCondition(result);
                }
                finally
                {
                    if (nativeQueryParserManager != null)
                        Marshal.ReleaseComObject(nativeQueryParserManager);

                    if (queryParser != null)
                        Marshal.ReleaseComObject(queryParser);

                    if (querySolution != null)
                        Marshal.ReleaseComObject(querySolution);

                    if (mainType != null)
                        Marshal.ReleaseComObject(mainType);
                }
            }
        }
    }
}
