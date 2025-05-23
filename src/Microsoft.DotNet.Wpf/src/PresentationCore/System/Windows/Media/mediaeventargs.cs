// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

//
//

namespace System.Windows.Media
{
    #region ExceptionEventArgs

    /// <summary>
    /// arguments for media failure event handlers
    /// </summary>
    public sealed class ExceptionEventArgs : EventArgs
    {
        /// <summary>
        /// Creates a new ExceptionEventArgs.
        /// </summary>
        /// <param name="errorException">Error Exception.</param>
        internal ExceptionEventArgs(Exception errorException)
            : base()
        {
            ArgumentNullException.ThrowIfNull(errorException);

            _errorException = errorException;
        }

        /// <summary>
        /// Error Exception
        /// </summary>
        public Exception ErrorException
        {
            get
            {
                return _errorException;
            }
        }

        /// <summary>
        /// Error exception data
        /// </summary>
        private Exception _errorException;
    };

    #endregion

    #region MediaScriptCommandEventArgs

    /// <summary>
    /// Arguments for any scripting commands associated with the media.
    /// </summary>
    public sealed class MediaScriptCommandEventArgs : EventArgs
    {
        /// <summary>
        /// Creates a new MediaScriptCommandEventArgs.
        /// </summary>
        internal
        MediaScriptCommandEventArgs(
            string      parameterType,
            string      parameterValue
            ) : base()
        {
            ArgumentNullException.ThrowIfNull(parameterType);

            ArgumentNullException.ThrowIfNull(parameterValue);

            _parameterType = parameterType;
            _parameterValue = parameterValue;
        }

        /// <summary>
        /// The type of the script command.
        /// </summary>
        public String ParameterType
        {
            get
            {
                return _parameterType;
            }
        }

        /// <summary>
        /// The parameter associated with the script command.
        /// </summary>
        public String ParameterValue
        {
            get
            {
                return _parameterValue;
            }
        }

        /// <summary>
        /// The type of scripting command
        /// </summary>
        private string _parameterType;

        /// <summary>
        /// The parameter associated with the script command.
        /// </summary>
        private string _parameterValue;
    }

    #endregion
};

