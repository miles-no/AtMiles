﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace no.miles.at.Backend.Domain.Test
{
    //VERSION 1.4.2.0
    //http://comparenetobjects.codeplex.com/

    //This software is provided free of charge from Kellerman Software.
    //It may be used in any project, including commercial for sale projects.
    //
    #region License
    //Microsoft Public License (Ms-PL)

    //This license governs use of the accompanying software. If you use the software, you accept this license. If you do not accept the license, do not use the software.

    //1. Definitions

    //The terms "reproduce," "reproduction," "derivative works," and "distribution" have the same meaning here as under U.S. copyright law.

    //A "contribution" is the original software, or any additions or changes to the software.

    //A "contributor" is any person that distributes its contribution under this license.

    //"Licensed patents" are a contributor's patent claims that read directly on its contribution.

    //2. Grant of Rights

    //(A) Copyright Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free copyright license to reproduce its contribution, prepare derivative works of its contribution, and distribute its contribution or any derivative works that you create.

    //(B) Patent Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free license under its licensed patents to make, have made, use, sell, offer for sale, import, and/or otherwise dispose of its contribution in the software or derivative works of the contribution in the software.

    //3. Conditions and Limitations

    //(A) No Trademark License- This license does not grant you rights to use any contributors' name, logo, or trademarks.

    //(B) If you bring a patent claim against any contributor over patents that you claim are infringed by the software, your patent license from such contributor to the software ends automatically.

    //(C) If you distribute any portion of the software, you must retain all copyright, patent, trademark, and attribution notices that are present in the software.

    //(D) If you distribute any portion of the software in source code form, you may do so only under this license by including a complete copy of this license with your distribution. If you distribute any portion of the software in compiled or object code form, you may only do so under a license that complies with this license.

    //(E) The software is licensed "as-is." You bear the risk of using it. The contributors give no express warranties, guarantees or conditions. You may have additional consumer rights under your local laws which this license cannot change. To the extent permitted under your local laws, the contributors exclude the implied warranties of merchantability, fitness for a particular purpose and non-infringement.
    #endregion

    /// <summary>
    /// Class that allows comparison of two objects of the same type to each other.  Supports classes, lists, arrays, dictionaries, child comparison and more.
    /// </summary>
    /// <example>
    /// 
    /// CompareObjects compareObjects = new CompareObjects();
    /// 
    /// Person person1 = new Person();
    /// person1.DateCreated = DateTime.Now;
    /// person1.Name = "Greg";
    ///
    /// Person person2 = new Person();
    /// person2.Name = "John";
    /// person2.DateCreated = person1.DateCreated;
    ///
    /// if (!compareObjects.Compare(person1, person2))
    ///    Console.WriteLine(compareObjects.DifferencesString);
    /// 
    /// </example>
    public class CompareObjects
    {
        #region Class Variables

        /// <summary>
        /// Keep track of parent objects in the object hiearchy
        /// </summary>
        private readonly List<object> _parents = new List<object>();

        /// <summary>
        /// Reflection Cache for property info
        /// </summary>
        private readonly Dictionary<Type, PropertyInfo[]> _propertyCache = new Dictionary<Type, PropertyInfo[]>();

        /// <summary>
        /// Reflection Cache for field info
        /// </summary>
        private readonly Dictionary<Type, FieldInfo[]> _fieldCache = new Dictionary<Type, FieldInfo[]>();

        /// <summary>
        /// Reflection Cache for methods
        /// </summary>
        private static readonly Dictionary<Type, MethodInfo[]> MethodList = new Dictionary<Type, MethodInfo[]>();
        #endregion

        #region Properties

        /// <summary>
        /// Ignore classes, properties, or fields by name during the comparison.
        /// Case sensitive.
        /// </summary>
        /// <example>ElementsToIgnore.Add("CreditCardNumber")</example>
        private List<string> ElementsToIgnore { get; set; }

        //Security restriction in Silverlight prevents getting private properties and fields

        /// <summary>
        /// If true, private properties and fields will be compared. The default is false.
        /// </summary>
        private bool ComparePrivateProperties { get; set; }

        /// <summary>
        /// If true, private fields will be compared. The default is false.
        /// </summary>
        private bool ComparePrivateFields { get; set; }

        /// <summary>
        /// If true, static properties will be compared.  The default is true.
        /// </summary>
        private bool CompareStaticProperties { get; set; }

        /// <summary>
        /// If true, static fields will be compared.  The default is true.
        /// </summary>
        private bool CompareStaticFields { get; set; }

        /// <summary>
        /// If true, child objects will be compared. The default is true. 
        /// If false, and a list or array is compared list items will be compared but not their children.
        /// </summary>
        private bool CompareChildren { get; set; }

        /// <summary>
        /// If true, compare read only properties (only the getter is implemented).
        /// The default is true.
        /// </summary>
        private bool CompareReadOnly { get; set; }

        /// <summary>
        /// If true, compare fields of a class (see also CompareProperties).
        /// The default is true.
        /// </summary>
        private bool CompareFields { get; set; }

        /// <summary>
        /// If true, compare properties of a class (see also CompareFields).
        /// The default is true.
        /// </summary>
        private bool CompareProperties { get; set; }

        /// <summary>
        /// The maximum number of differences to detect
        /// </summary>
        /// <remarks>
        /// Default is 1 for performance reasons.
        /// </remarks>
        private int MaxDifferences { get; set; }

        /// <summary>
        /// The differences found during the compare
        /// </summary>
        private List<String> Differences { get; set; }

        /// <summary>
        /// The differences found in a string suitable for a textbox
        /// </summary>
        public string DifferencesString
        {
            get
            {
                var sb = new StringBuilder(4096);

                sb.Append("\r\nBegin Differences:\r\n");

                foreach (string item in Differences)
                {
                    sb.AppendFormat("{0}\r\n", item);
                }

                sb.AppendFormat("End Differences (Maximum of {0} differences shown).", MaxDifferences);

                return sb.ToString();
            }
        }

        /// <summary>
        /// Reflection properties and fields are cached. By default this cache is cleared after each compare.  Set to false to keep the cache for multiple compares.
        /// </summary>
        /// <seealso cref="Caching"/>
        /// <seealso cref="ClearCache"/>
        private bool AutoClearCache { get; set; }

        /// <summary>
        /// By default properties and fields for types are cached for each compare.  By default this cache is cleared after each compare.
        /// </summary>
        /// <seealso cref="AutoClearCache"/>
        /// <seealso cref="ClearCache"/>
        private bool Caching { get; set; }

        /// <summary>
        /// A list of attributes to ignore a class, property or field
        /// </summary>
        /// <example>AttributesToIgnore.Add(typeof(XmlIgnoreAttribute));</example>
        private List<Type> AttributesToIgnore { get; set; }

        /// <summary>
        /// If true, objects will be compared ignore their type diferences
        /// </summary>
        private bool IgnoreObjectTypes { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Set up defaults for the comparison
        /// </summary>
        public CompareObjects()
        {
            Differences = new List<string>();
            AttributesToIgnore = new List<Type>();
            ElementsToIgnore = new List<string>();
            CompareStaticFields = true;
            CompareStaticProperties = true;
            ComparePrivateProperties = false;
            ComparePrivateFields = false;
            CompareChildren = true;
            CompareReadOnly = true;
            CompareFields = true;
            CompareProperties = true;
            Caching = true;
            AutoClearCache = true;
            IgnoreObjectTypes = false;
            MaxDifferences = 1;


        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Compare two objects of the same type to each other.
        /// </summary>
        /// <remarks>
        /// Check the Differences or DifferencesString Properties for the differences.
        /// Default MaxDifferences is 1 for performance
        /// </remarks>
        /// <param name="object1"></param>
        /// <param name="object2"></param>
        /// <returns>True if they are equal</returns>
        public bool Compare(object object1, object object2)
        {
            string defaultBreadCrumb = string.Empty;

            Differences.Clear();
            Compare(object1, object2, defaultBreadCrumb);

            if (AutoClearCache)
                ClearCache();

            return Differences.Count == 0;
        }

        /// <summary>
        /// Reflection properties and fields are cached. By default this cache is cleared automatically after each compare.
        /// </summary>
        /// <seealso cref="AutoClearCache"/>
        /// <seealso cref="Caching"/>
        private void ClearCache()
        {
            _propertyCache.Clear();
            _fieldCache.Clear();
            MethodList.Clear();
        }

        #endregion

        #region Comparison Methods

        /// <summary>
        /// Compare two objects
        /// </summary>
        /// <param name="object1"></param>
        /// <param name="object2"></param>
        /// <param name="breadCrumb">Where we are in the object hiearchy</param>
        private void Compare(object object1, object object2, string breadCrumb)
        {
            //If both null return true
            if (object1 == null && object2 == null)
                return;

            //Check if one of them is null
            if (object1 == null)
            {
                Differences.Add(string.Format("object1{0} == null && object2{0} != null ((null),{1})", breadCrumb, NiceString(object2)));
                return;
            }

            if (object2 == null)
            {
                Differences.Add(string.Format("object1{0} != null && object2{0} == null ({1},(null))", breadCrumb, NiceString(object1)));
                return;
            }

            Type t1 = object1.GetType();
            Type t2 = object2.GetType();

            //Objects must be the same type
            if (t1 != t2 && !IgnoreObjectTypes)
            {
                Differences.Add(string.Format("Different Types:  object1{0}.GetType() != object2{0}.GetType() ({1},{2})", breadCrumb, t1.FullName, t2.FullName));
                return;
            }
            if (IsTypeOfType(t1))
            {
                CompareType(object1, object2, breadCrumb);
            }
            else if (IsIList(t1)) //This will do arrays, multi-dimensional arrays and generic lists
            {
                CompareIList(object1, object2, breadCrumb);
            }
            else if (IsHashSet(t1))
            {
                CompareHashSet(object1, object2, breadCrumb);
            }
            else if (IsIDictionary(t1))
            {
                CompareIDictionary(object1, object2, breadCrumb);
            }
            else if (IsEnum(t1))
            {
                CompareEnum(object1, object2, breadCrumb);
            }
            else if (IsPointer(t1))
            {
                ComparePointer(object1, object2, breadCrumb);
            }
            else if (IsUri(t1))
            {
                CompareUri(object1, object2, breadCrumb);
            }
            else if (IsString(t1))
            {
                CompareString(object1, object2, breadCrumb);
            }

            else if (IsSimpleType(t1))
            {
                CompareSimpleType(object1, object2, breadCrumb);
            }
            else if (IsClass(t1))
            {
                CompareClass(object1, object2, breadCrumb);
            }
            else if (IsTimespan(t1))
            {
                CompareTimespan(object1, object2, breadCrumb);
            }
            else if (IsStruct(t1))
            {
                CompareStruct(object1, object2, breadCrumb);
            }
            else
            {
                throw new NotImplementedException("Cannot compare object of type " + t1.Name);
            }

        }

        private void CompareUri(object object1, object object2, string breadCrumb)
        {
            var uri1 = object1 as Uri;
            var uri2 = object2 as Uri;

            if (uri1 == null) //This should never happen, null check happens one level up
                throw new ArgumentNullException("object1");

            if (uri2 == null) //This should never happen, null check happens one level up
                throw new ArgumentNullException("object2");

            if (uri1.OriginalString != uri2.OriginalString)
            {
                Differences.Add(string.Format("object1{0} != object2{0} ({1},{2})", breadCrumb, object1, object2));
            }
        }

        private void CompareType(object object1, object object2, string breadCrumb)
        {
            var t1 = (Type)object1;
            var t2 = (Type)object2;

            if (t1.FullName != t2.FullName)
            {
                Differences.Add(string.Format("object1{0} != object2{0} ({1},{2})", breadCrumb, t1.FullName, t2.FullName));
            }
        }

        private void CompareString(object object1, object object2, string breadCrumb)
        {
            if ((string) object1 != (string) object2)
            {
                Differences.Add(string.Format("object1{0} != object2{0}", breadCrumb));
            }
        }

        /// <summary>
        /// Compare a timespan struct
        /// </summary>
        /// <param name="object1"></param>
        /// <param name="object2"></param>
        /// <param name="breadCrumb"></param>
        private void CompareTimespan(object object1, object object2, string breadCrumb)
        {
            if (((TimeSpan)object1).Ticks != ((TimeSpan)object2).Ticks)
            {
                Differences.Add(string.Format("object1{0}.Ticks != object2{0}.Ticks", breadCrumb));
            }
        }

        /// <summary>
        /// Compare a pointer struct
        /// </summary>
        /// <param name="object1"></param>
        /// <param name="object2"></param>
        /// <param name="breadCrumb"></param>
        private void ComparePointer(object object1, object object2, string breadCrumb)
        {
            if (
                (object1 is IntPtr && object2 is IntPtr && ((IntPtr)object1) != ((IntPtr)object2)) ||
                (object1 is UIntPtr && object2 is UIntPtr && ((UIntPtr)object1) != ((UIntPtr)object2))
                )
            {
                Differences.Add(string.Format("object1{0} != object2{0}", breadCrumb));
            }
        }

        /// <summary>
        /// Compare an enumeration
        /// </summary>
        /// <param name="object1"></param>
        /// <param name="object2"></param>
        /// <param name="breadCrumb"></param>
        private void CompareEnum(object object1, object object2, string breadCrumb)
        {
            if (object1.ToString() != object2.ToString())
            {
                string currentBreadCrumb = AddBreadCrumb(breadCrumb, object1.GetType().Name, string.Empty, -1);
                Differences.Add(string.Format("object1{0} != object2{0} ({1},{2})", currentBreadCrumb, object1, object2));
            }
        }

        /// <summary>
        /// Compare a simple type
        /// </summary>
        /// <param name="object1"></param>
        /// <param name="object2"></param>
        /// <param name="breadCrumb"></param>
        private void CompareSimpleType(object object1, object object2, string breadCrumb)
        {
            if (object2 == null) //This should never happen, null check happens one level up
                throw new ArgumentNullException("object2");

            var valOne = object1 as IComparable;

            if (valOne == null) //This should never happen, null check happens one level up
                throw new ArgumentNullException("object1");

            if (valOne.CompareTo(object2) != 0)
            {
                Differences.Add(string.Format("object1{0} != object2{0} ({1},{2})", breadCrumb, object1, object2));
            }
        }



        /// <summary>
        /// Compare a struct
        /// </summary>
        /// <param name="object1"></param>
        /// <param name="object2"></param>
        /// <param name="breadCrumb"></param>
        private void CompareStruct(object object1, object object2, string breadCrumb)
        {
            try
            {
                _parents.Add(object1);
                _parents.Add(object2);

                Type t1 = object1.GetType();

                PerformCompareFields(t1, object1, object2, true, breadCrumb);
                PerformCompareProperties(t1, object1, object2, true, breadCrumb);
            }
            finally
            {
                _parents.Remove(object1);
                _parents.Remove(object2);
            }
        }

        /// <summary>
        /// Compare the properties, fields of a class
        /// </summary>
        /// <param name="object1"></param>
        /// <param name="object2"></param>
        /// <param name="breadCrumb"></param>
        private void CompareClass(object object1, object object2, string breadCrumb)
        {
            try
            {
                _parents.Add(object1);
                _parents.Add(object2);

                Type t1 = object1.GetType();

                //We ignore the class name
                if (ElementsToIgnore.Contains(t1.Name) || IgnoredByAttribute(t1))
                    return;

                //Compare the properties
                if (CompareProperties)
                    PerformCompareProperties(t1, object1, object2, false, breadCrumb);

                //Compare the fields
                if (CompareFields)
                    PerformCompareFields(t1, object1, object2, false, breadCrumb);
            }
            finally
            {
                _parents.Remove(object1);
                _parents.Remove(object2);
            }
        }


        /// <summary>
        /// Compare the fields of a class
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="object1"></param>
        /// <param name="object2"></param>
        /// <param name="structCompare"></param>
        /// <param name="breadCrumb"></param>
        private void PerformCompareFields(Type t1,
            object object1,
            object object2,
            bool structCompare,
            string breadCrumb)
        {
            IEnumerable<FieldInfo> currentFields = GetFieldInfo(t1);

            foreach (FieldInfo item in currentFields)
            {
                //Ignore invalid struct fields
                if (structCompare && !ValidStructSubType(item.FieldType))
                    continue;

                //Skip if this is a shallow compare
                if (!CompareChildren && IsChildType(item.FieldType))
                    continue;

                //If we should ignore it, skip it
                if (ElementsToIgnore.Contains(item.Name) || IgnoredByAttribute(item.FieldType))
                    continue;

                object objectValue1 = item.GetValue(object1);
                object objectValue2 = item.GetValue(object2);

                bool object1IsParent = objectValue1 != null && (objectValue1 == object1 || _parents.Contains(objectValue1));
                bool object2IsParent = objectValue2 != null && (objectValue2 == object2 || _parents.Contains(objectValue2));

                //Skip fields that point to the parent
                if (IsClass(item.FieldType)
                    && (object1IsParent || object2IsParent))
                {
                    continue;
                }

                string currentCrumb = AddBreadCrumb(breadCrumb, item.Name, string.Empty, -1);

                Compare(objectValue1, objectValue2, currentCrumb);

                if (Differences.Count >= MaxDifferences)
                    return;
            }
        }

        private IEnumerable<FieldInfo> GetFieldInfo(Type type)
        {
            if (Caching && _fieldCache.ContainsKey(type))
                return _fieldCache[type];

            FieldInfo[] currentFields;

            if (ComparePrivateFields && !CompareStaticFields)
                currentFields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            else if (ComparePrivateFields && CompareStaticFields)
                currentFields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
            else
                currentFields = type.GetFields(); //Default is public instance and static

            if (Caching)
                _fieldCache.Add(type, currentFields);

            return currentFields;
        }


        /// <summary>
        /// Compare the properties of a class
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="object1"></param>
        /// <param name="object2"></param>
        /// <param name="structCompare"></param>
        /// <param name="breadCrumb"></param>
        private void PerformCompareProperties(Type t1,
            object object1,
            object object2,
            bool structCompare,
            string breadCrumb)
        {
            IEnumerable<PropertyInfo> currentProperties = GetPropertyInfo(t1);

            foreach (PropertyInfo info in currentProperties)
            {
                //Ignore invalid struct fields
                if (structCompare && !ValidStructSubType(info.PropertyType))
                    continue;

                //If we can't read it, skip it
                if (info.CanRead == false)
                    continue;

                //Skip if this is a shallow compare
                if (!CompareChildren && IsChildType(info.PropertyType))
                    continue;

                //If we should ignore it, skip it
                if (ElementsToIgnore.Contains(info.Name) || IgnoredByAttribute(info.PropertyType))
                    continue;

                //If we should ignore read only, skip it
                if (!CompareReadOnly && info.CanWrite == false)
                    continue;

                //If we ignore types then we must get correct PropertyInfo object
                PropertyInfo secondObjectInfo = null;
                if (IgnoreObjectTypes)
                {
                    var secondObjectPropertyInfos = GetPropertyInfo(object2.GetType());

                    foreach (var propertyInfo in secondObjectPropertyInfos)
                    {
                        if (propertyInfo.Name != info.Name) continue;

                        secondObjectInfo = propertyInfo;
                        break;
                    }
                }
                else
                    secondObjectInfo = info;

                object objectValue1;
                object objectValue2;
                if (!IsValidIndexer(info, breadCrumb))
                {
                    objectValue1 = info.GetValue(object1, null);
                    objectValue2 = secondObjectInfo != null ? secondObjectInfo.GetValue(object2, null) : null;
                }
                else
                {
                    CompareIndexer(info, object1, object2, breadCrumb);
                    continue;
                }

                bool object1IsParent = objectValue1 != null && (objectValue1 == object1 || _parents.Contains(objectValue1));
                bool object2IsParent = objectValue2 != null && (objectValue2 == object2 || _parents.Contains(objectValue2));

                //Skip properties where both point to the corresponding parent
                if ((IsClass(info.PropertyType) || IsStruct(info.PropertyType)) && (object1IsParent && object2IsParent))
                {
                    continue;
                }

                string currentCrumb = AddBreadCrumb(breadCrumb, info.Name, string.Empty, -1);

                Compare(objectValue1, objectValue2, currentCrumb);

                if (Differences.Count >= MaxDifferences)
                    return;
            }
        }

        private IEnumerable<PropertyInfo> GetPropertyInfo(Type type)
        {
            if (Caching && _propertyCache.ContainsKey(type))
                return _propertyCache[type];

            PropertyInfo[] currentProperties;

            if (ComparePrivateProperties && !CompareStaticProperties)
                currentProperties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            else if (ComparePrivateProperties && CompareStaticProperties)
                currentProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
            else if (!CompareStaticProperties)
                currentProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            else
                currentProperties = type.GetProperties(); //Default is public instance and static

            if (Caching)
                _propertyCache.Add(type, currentProperties);

            return currentProperties;
        }

        private void CompareIndexer(PropertyInfo info, object object1, object object2, string breadCrumb)
        {
            if (info.ReflectedType != null)
            {
                var indexerCount1 = (int)info.ReflectedType.GetProperty("Count").GetGetMethod().Invoke(object1, new object[] { });
                var indexerCount2 = (int)info.ReflectedType.GetProperty("Count").GetGetMethod().Invoke(object2, new object[] { });

                //Indexers must be the same length
                string currentCrumb;
                if (indexerCount1 != indexerCount2)
                {
                    currentCrumb = AddBreadCrumb(breadCrumb, info.Name, string.Empty, -1);
                    Differences.Add(string.Format("object1{0}.Count != object2{0}.Count ({1},{2})", currentCrumb,
                        indexerCount1, indexerCount2));

                    if (Differences.Count >= MaxDifferences)
                        return;
                }

                // Run on indexer
                for (int i = 0; i < indexerCount1; i++)
                {
                    currentCrumb = AddBreadCrumb(breadCrumb, info.Name, string.Empty, i);
                    var objectValue1 = info.GetValue(object1, new object[] { i });
                    var objectValue2 = info.GetValue(object2, new object[] { i });
                    Compare(objectValue1, objectValue2, currentCrumb);

                    if (Differences.Count >= MaxDifferences)
                        return;
                }
            }
        }

        /// <summary>
        /// Compare a dictionary
        /// </summary>
        /// <param name="object1"></param>
        /// <param name="object2"></param>
        /// <param name="breadCrumb"></param>
        private void CompareIDictionary(object object1, object object2, string breadCrumb)
        {
            var iDict1 = object1 as IDictionary;
            var iDict2 = object2 as IDictionary;

            if (iDict1 == null) //This should never happen, null check happens one level up
                throw new ArgumentNullException("object1");

            if (iDict2 == null) //This should never happen, null check happens one level up
                throw new ArgumentNullException("object2");

            try
            {
                _parents.Add(object1);
                _parents.Add(object2);

                //Objects must be the same length
                if (iDict1.Count != iDict2.Count)
                {
                    Differences.Add(string.Format("object1{0}.Count != object2{0}.Count ({1},{2})", breadCrumb,
                                                  iDict1.Count, iDict2.Count));

                    if (Differences.Count >= MaxDifferences)
                        return;
                }

                IDictionaryEnumerator enumerator1 = iDict1.GetEnumerator();
                IDictionaryEnumerator enumerator2 = iDict2.GetEnumerator();

                while (enumerator1.MoveNext() && enumerator2.MoveNext())
                {
                    string currentBreadCrumb = AddBreadCrumb(breadCrumb, "Key", string.Empty, -1);

                    Compare(enumerator1.Key, enumerator2.Key, currentBreadCrumb);

                    if (Differences.Count >= MaxDifferences)
                        return;

                    currentBreadCrumb = AddBreadCrumb(breadCrumb, "Value", string.Empty, -1);

                    Compare(enumerator1.Value, enumerator2.Value, currentBreadCrumb);

                    if (Differences.Count >= MaxDifferences)
                        return;
                }
            }
            finally
            {
                _parents.Remove(object1);
                _parents.Remove(object2);
            }
        }

        /// <summary>
        /// Compare an array or something that implements IList
        /// </summary>
        /// <param name="object1"></param>
        /// <param name="object2"></param>
        /// <param name="breadCrumb"></param>
        private void CompareIList(object object1, object object2, string breadCrumb)
        {
            var ilist1 = object1 as IList;
            var ilist2 = object2 as IList;

            if (ilist1 == null) //This should never happen, null check happens one level up
                throw new ArgumentNullException("object1");

            if (ilist2 == null) //This should never happen, null check happens one level up
                throw new ArgumentNullException("object2");

            try
            {
                _parents.Add(object1);
                _parents.Add(object2);

                //Objects must be the same length
                if (ilist1.Count != ilist2.Count)
                {
                    Differences.Add(string.Format("object1{0}.Count != object2{0}.Count ({1},{2})", breadCrumb,
                                                  ilist1.Count, ilist2.Count));

                    if (Differences.Count >= MaxDifferences)
                        return;
                }

                IEnumerator enumerator1 = ilist1.GetEnumerator();
                IEnumerator enumerator2 = ilist2.GetEnumerator();
                int count = 0;

                while (enumerator1.MoveNext() && enumerator2.MoveNext())
                {
                    string currentBreadCrumb = AddBreadCrumb(breadCrumb, string.Empty, string.Empty, count);

                    Compare(enumerator1.Current, enumerator2.Current, currentBreadCrumb);

                    if (Differences.Count >= MaxDifferences)
                        return;

                    count++;
                }
            }
            finally
            {
                _parents.Remove(object1);
                _parents.Remove(object2);
            }
        }


        private void CompareHashSet(object object1, object object2, string breadCrumb)
        {
            try
            {
                _parents.Add(object1);
                _parents.Add(object2);

                Type t1 = object1.GetType();

                //Get count by reflection since we can't cast it to HashSet<>
                var hashSet1Count = (int)GetPropertyValue(t1, object1, "Count");
                var hashSet2Count = (int)GetPropertyValue(t1, object2, "Count");

                //Objects must be the same length
                if (hashSet1Count != hashSet2Count)
                {
                    Differences.Add(string.Format("object1{0}.Count != object2{0}.Count ({1},{2})", breadCrumb,
                                                  hashSet1Count, hashSet2Count));

                    if (Differences.Count >= MaxDifferences)
                        return;
                }

                //Get enumerators by reflection
                MethodInfo methodInfo = GetMethod(t1, "GetEnumerator");
                var enumerator1 = (IEnumerator)methodInfo.Invoke(object1, null);
                var enumerator2 = (IEnumerator)methodInfo.Invoke(object2, null);

                int count = 0;

                while (enumerator1.MoveNext() && enumerator2.MoveNext())
                {
                    string currentBreadCrumb = AddBreadCrumb(breadCrumb, string.Empty, string.Empty, count);

                    Compare(enumerator1.Current, enumerator2.Current, currentBreadCrumb);

                    if (Differences.Count >= MaxDifferences)
                        return;

                    count++;
                }
            }
            finally
            {
                _parents.Remove(object1);
                _parents.Remove(object2);
            }
        }
        #endregion

        #region IsType methods
        private bool IsTypeOfType(Type type)
        {
            return (typeof(Type).IsAssignableFrom(type));
        }

        /// <summary>
        /// Check if any type has attributes that should be bypassed
        /// </summary>
        /// <returns></returns>
        private bool IgnoredByAttribute(Type type)
        {
            return AttributesToIgnore.Any(attributeType => type.GetCustomAttributes(attributeType, false).Length > 0);
        }

        private bool IsTimespan(Type type)
        {
            return type == typeof(TimeSpan);
        }

        private bool IsPointer(Type type)
        {
            return type == typeof(IntPtr) || type == typeof(UIntPtr);
        }

        private bool IsEnum(Type type)
        {
            return type.IsEnum;
        }

        private bool IsStruct(Type type)
        {
            return type.IsValueType && !IsSimpleType(type);
        }

        private bool IsString(Type type)
        {
            return type == typeof(string);
        }

        private bool IsSimpleType(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                type = Nullable.GetUnderlyingType(type);
            }

            return type.IsPrimitive
                   || type == typeof(DateTime)
                   || type == typeof(decimal)
                   || type == typeof(string)
                   || type == typeof(Guid);

        }

        private bool ValidStructSubType(Type type)
        {
            return IsSimpleType(type)
                || IsEnum(type)
                || IsArray(type)
                || IsClass(type)
                || IsIDictionary(type)
                || IsTimespan(type)
                || IsIList(type);
        }

        private bool IsArray(Type type)
        {
            return type.IsArray;
        }

        private bool IsClass(Type type)
        {
            return type.IsClass;
        }

        private bool IsIDictionary(Type type)
        {
            return (typeof(IDictionary).IsAssignableFrom(type));
        }

        private bool IsIList(Type type)
        {
            return (typeof(IList).IsAssignableFrom(type));
        }

        private bool IsChildType(Type type)
        {
            return !IsSimpleType(type)
                && (IsClass(type)
                    || IsArray(type)
                    || IsIDictionary(type)
                    || IsIList(type)
                    || IsStruct(type)
                    || IsHashSet(type)
                    );
        }

        private bool IsUri(Type type)
        {
            return (typeof(Uri).IsAssignableFrom(type));
        }

        private bool IsHashSet(Type type)
        {
            return type.IsGenericType
                && type.GetGenericTypeDefinition() == typeof(HashSet<>);
        }

        #endregion

        #region Validity Checking
        private bool IsValidIndexer(PropertyInfo info, string breadCrumb)
        {
            ParameterInfo[] indexers = info.GetIndexParameters();

            if (indexers.Length == 0)
            {
                return false;
            }

            if (indexers.Length > 1)
            {
                throw new Exception("Cannot compare objects with more than one indexer for object " + breadCrumb);
            }

            if (indexers[0].ParameterType != typeof(Int32))
            {
                throw new Exception("Cannot compare objects with a non integer indexer for object " + breadCrumb);
            }

            if (info.ReflectedType != null && info.ReflectedType.GetProperty("Count") == null)
            {
                throw new Exception("Indexer must have a corresponding Count property for object " + breadCrumb);
            }

            if (info.ReflectedType != null && info.ReflectedType.GetProperty("Count").PropertyType != typeof(Int32))
            {
                throw new Exception("Indexer must have a corresponding Count property that is an integer for object " + breadCrumb);
            }

            return true;
        }
        #endregion

        #region Supporting Methods

        private object GetPropertyValue(Type type, object objectValue, string propertyName)
        {
            return GetPropertyInfo(type).First(o => o.Name == propertyName).GetValue(objectValue, null);
        }

        /// <summary>
        /// Get a method by name
        /// </summary>
        /// <param name="type"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        private MethodInfo GetMethod(Type type, string methodName)
        {
            return GetMethods(type).FirstOrDefault(m => m.Name == methodName);
        }

        /// <summary>
        /// Get the cached methods for a type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private IEnumerable<MethodInfo> GetMethods(Type type)
        {
            if (MethodList.ContainsKey(type))
                return MethodList[type];

            MethodInfo[] myMethodInfo = type.GetMethods();
            MethodList.Add(type, myMethodInfo);
            return myMethodInfo;
        }

        /// <summary>
        /// Convert an object to a nicely formatted string
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private string NiceString(object obj)
        {
            try
            {
                if (obj == null)
                    return "(null)";

                if (obj == DBNull.Value)
                    return "System.DBNull.Value";

                return obj.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Add a breadcrumb to an existing breadcrumb
        /// </summary>
        /// <param name="existing"></param>
        /// <param name="name"></param>
        /// <param name="extra"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private string AddBreadCrumb(string existing, string name, string extra, int index)
        {
            return AddBreadCrumb(existing, name, extra, index >= 0 ? index.ToString(CultureInfo.InvariantCulture) : null);
        }

        /// <summary>
        /// Add a breadcrumb to an existing breadcrumb
        /// </summary>
        /// <param name="existing"></param>
        /// <param name="name"></param>
        /// <param name="extra"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private string AddBreadCrumb(string existing, string name, string extra, string index)
        {
            bool useIndex = !String.IsNullOrEmpty(index);
            bool useName = name.Length > 0;
            var sb = new StringBuilder();

            sb.Append(existing);

            if (useName)
            {
                sb.AppendFormat(".");
                sb.Append(name);
            }

            sb.Append(extra);

            if (useIndex)
            {
                int result;
                sb.AppendFormat(Int32.TryParse(index, out result) ? "[{0}]" : "[\"{0}\"]", result);
            }
            return sb.ToString();
        }
        #endregion
    }
}
