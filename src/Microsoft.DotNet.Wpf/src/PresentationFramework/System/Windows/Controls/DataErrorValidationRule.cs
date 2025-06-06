﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

//
// Description:
//      DataErrorValidationRule is used when a ValidationError is the result of
//      a data error in the source item itself (e.g. as exposed by IDataErrorInfo).
//

using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using MS.Internal;

namespace System.Windows.Controls
{
    /// <summary>
    ///     DataErrorValidationRule can be added to the ValidationRulesCollection of a Binding
    ///     or MultiBinding to indicate that data errors in the source object should
    ///     be considered ValidationErrors
    /// </summary>
    public sealed class DataErrorValidationRule : ValidationRule
    {
        /// <summary>
        /// DataErrorValidationRule ctor.
        /// </summary>
        public DataErrorValidationRule() : base(ValidationStep.UpdatedValue, true)
        {
        }

        /// <summary>
        /// Validate is called when Data binding is updating
        /// </summary>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            // This rule is called during the CommittedValue step, so the value is the
            // owner of the rule collection - either a BindingGroup or an individual
            // binding expression.
            BindingGroup bindingGroup;
            BindingExpression bindingExpr;

            if ((bindingGroup = value as BindingGroup) != null)
            {
                // in a BindingGroup, check the item-level IDataErrorInfo for each
                // source item in the group
                IList items = bindingGroup.Items;
                for (int i=items.Count-1; i>=0; --i)
                {
                    IDataErrorInfo idei = items[i] as IDataErrorInfo;
                    if (idei != null)
                    {
                        string error = idei.Error;
                        if (!String.IsNullOrEmpty(error))
                        {
                            return new ValidationResult(false, error);
                        }
                    }
                }
            }
            else if ((bindingExpr = value as BindingExpression) != null)
            {
                // in a binding, check the error info for the binding's source
                // property
                IDataErrorInfo idei = bindingExpr.SourceItem as IDataErrorInfo;
                string name = (idei != null) ? bindingExpr.SourcePropertyName : null;

                if (!String.IsNullOrEmpty(name))
                {
                    // get the data error information, if any, by calling idie[name].
                    // We do this in a paranoid way, even though indexers with
                    // string-valued arguments are not supposed to throw exceptions.

                    string error;
                    try
                    {
                        error = idei[name];
                    }
                    catch (Exception ex)
                    {
                        if (CriticalExceptions.IsCriticalApplicationException(ex))
                            throw;

                        error = null;

                        if (TraceData.IsEnabled)
                        {
                            TraceData.TraceAndNotify(TraceEventType.Error,
                                            TraceData.DataErrorInfoFailed(
                                                name,
                                                idei.GetType().FullName,
                                                ex.GetType().FullName,
                                                ex.Message),
                                            bindingExpr);
                        }
                    }

                    if (!String.IsNullOrEmpty(error))
                    {
                        return new ValidationResult(false, error);
                    }
                }
            }
            else
                throw new InvalidOperationException(SR.Format(SR.ValidationRule_UnexpectedValue, this, value));

            return ValidationResult.ValidResult;
        }

        internal static readonly DataErrorValidationRule Instance = new DataErrorValidationRule();
    }
}

