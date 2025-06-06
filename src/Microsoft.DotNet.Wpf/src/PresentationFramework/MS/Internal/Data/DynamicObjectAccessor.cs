// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

//
// Description: Accessors for dynamic objects
//

using SW = System.Windows;              // SR

namespace MS.Internal.Data
{
    #region DynamicObjectAccessor

    internal class DynamicObjectAccessor
    {
        protected DynamicObjectAccessor(Type ownerType, string propertyName)
        {
            _ownerType = ownerType;
            _propertyName = propertyName;
        }

        public Type OwnerType { get { return _ownerType; } }
        public string PropertyName { get { return _propertyName; } }
        public bool IsReadOnly { get { return false; } }
        public Type PropertyType { get { return typeof(object); } }

        public static string MissingMemberErrorString(object target, string name)
        {
            return SW.SR.Format(SW.SR.PropertyPathNoProperty, target, "Items");
        }

        private Type _ownerType;
        private string _propertyName;
    }

    #endregion DynamicObjectAccessor

    #region DynamicPropertyAccessor

    internal abstract class DynamicPropertyAccessor : DynamicObjectAccessor
    {
        protected DynamicPropertyAccessor(Type ownerType, string propertyName)
            : base(ownerType, propertyName)
        {
        }

        public abstract object GetValue(object component);

        public abstract void SetValue(object component, object value);
    }

    #endregion DynamicPropertyAccessor

    #region DynamicIndexerAccessor

    internal abstract class DynamicIndexerAccessor : DynamicObjectAccessor
    {
        protected DynamicIndexerAccessor(Type ownerType, string propertyName)
            : base(ownerType, propertyName)
        {
        }

        public abstract object GetValue(object component, object[] args);

        public abstract void SetValue(object component, object[] args, object value);
    }

    #endregion DynamicIndexerAccessor
}
