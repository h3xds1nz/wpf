// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

//
// Description: Extension methods for use with MsSpellCheckLib.RCW interfaces
//

using System.Runtime.InteropServices;
using IEnumString = System.Windows.Documents.MsSpellCheckLib.RCW.IEnumString;
using IEnumSpellingError = System.Windows.Documents.MsSpellCheckLib.RCW.IEnumSpellingError;
using ISpellingError = System.Windows.Documents.MsSpellCheckLib.RCW.ISpellingError;

using SpellingError = System.Windows.Documents.MsSpellCheckLib.SpellChecker.SpellingError;
using CorrectiveAction = System.Windows.Documents.MsSpellCheckLib.SpellChecker.CorrectiveAction;

namespace System.Windows.Documents.MsSpellCheckLib
{
    /// <summary>
    /// Extension methods for use with MsSpellCheckLib.RCW interfaces
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// Extracts a list of strings from an RCW.IEnumString instance.
        /// </summary>
        internal static List<string> ToList(
            this IEnumString enumString,
            bool shouldSuppressCOMExceptions = true,
            bool shouldReleaseCOMObject = true)
        {
            var result = new List<string>();

            ArgumentNullException.ThrowIfNull(enumString);

            try
            {
                uint fetched = 0;
                string str = string.Empty;

                do
                {
                    enumString.RemoteNext(1, out str, out fetched);
                    if (fetched > 0)
                    {
                        result.Add(str);
                    }
                }
                while (fetched > 0);
            }
            catch (COMException) when (shouldSuppressCOMExceptions)
            {
                // do nothing here
                // the exception filter does it all
            }
            finally
            {
                if (shouldReleaseCOMObject)
                {
                    Marshal.ReleaseComObject(enumString);
                }
            }

            return result;
        }


        /// <summary>
        /// Extracts a list of SpellingError's from an RCW.IEnumSpellingError instance.
        /// </summary>
        internal static List<SpellingError> ToList(
            this IEnumSpellingError spellingErrors,
            SpellChecker spellChecker,
            string text,
            bool shouldSuppressCOMExceptions = true,
            bool shouldReleaseCOMObject = true)
        {
            ArgumentNullException.ThrowIfNull(spellingErrors);

            var result = new List<SpellingError>();

            try
            {
                while (true)
                {
                    ISpellingError iSpellingError = spellingErrors.Next();

                    if (iSpellingError == null)
                    {
                        // no more ISpellingError objects left in the enum
                        break;
                    }

                    var error = new SpellingError(iSpellingError, spellChecker, text, shouldSuppressCOMExceptions, true);
                    result.Add(error);
                }
            }
            catch (COMException) when (shouldSuppressCOMExceptions)
            {
                // do nothing here
                // the exception filter does it all.
            }
            finally
            {
                if (shouldReleaseCOMObject)
                {
                    Marshal.ReleaseComObject(spellingErrors);
                }
            }

            return result;
        }

        /// <summary>
        /// Determines whether a collection of SpellingError instances
        /// has any actual errors, or whether they represent a 'clean'
        /// result.
        /// </summary>
        internal static bool IsClean(this List<SpellingError> errors)
        {
            ArgumentNullException.ThrowIfNull(errors);

            bool isClean = true;
            foreach (var error in errors)
            {
                if (error.CorrectiveAction != CorrectiveAction.None)
                {
                    isClean = false;
                    break;
                }
            }

            return isClean;
        }

        /// <summary>
        /// Determines whether an RCW.IEnumSpellingError instance has any errors,
        /// without asking for expensive details.
        /// </summary>
        internal static bool HasErrors(
            this IEnumSpellingError spellingErrors,
            bool shouldSuppressCOMExceptions = true,
            bool shouldReleaseCOMObject = true)
        {
            ArgumentNullException.ThrowIfNull(spellingErrors);

            bool result = false;

            try
            {
                while (!result)
                {
                    ISpellingError iSpellingError = spellingErrors.Next();

                    if (iSpellingError == null)
                    {
                        // no more ISpellingError objects left in the enum
                        break;
                    }

                    if ((CorrectiveAction)iSpellingError.CorrectiveAction != CorrectiveAction.None)
                    {
                        result = true;
                    }
                    Marshal.ReleaseComObject(iSpellingError);
                }
            }
            catch (COMException) when (shouldSuppressCOMExceptions)
            {
                // do nothing here
                // the exception filter does it all.
            }
            finally
            {
                if (shouldReleaseCOMObject)
                {
                    Marshal.ReleaseComObject(spellingErrors);
                }
            }

            return result;
        }
    }
}
