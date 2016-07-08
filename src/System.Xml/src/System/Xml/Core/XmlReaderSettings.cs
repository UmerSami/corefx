// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Diagnostics;
#if !SILVERLIGHT
using Microsoft.Win32;
using System.Globalization;
using System.Security;
using System.Xml.Schema;
using System.Xml.XmlConfiguration;
#endif
using System.Runtime.Versioning;

namespace System.Xml
{
    // XmlReaderSettings class specifies basic features of an XmlReader.
    public sealed class XmlReaderSettings
    {
        //
        // Fields
        //

#if ASYNC || FEATURE_NETCORE
        private bool _useAsync;
#endif


        // Nametable
        private XmlNameTable _nameTable;

        // XmlResolver
        private XmlResolver _xmlResolver;

        // Text settings
        private int _lineNumberOffset;
        private int _linePositionOffset;

        // Conformance settings
        private ConformanceLevel _conformanceLevel;
        private bool _checkCharacters;

        private long _maxCharactersInDocument;
        private long _maxCharactersFromEntities;

        // Filtering settings
        private bool _ignoreWhitespace;
        private bool _ignorePIs;
        private bool _ignoreComments;

        // security settings
        private DtdProcessing _dtdProcessing;

#if !SILVERLIGHT
        //Validation settings
        private ValidationType _validationType;
        private XmlSchemaValidationFlags _validationFlags;
        private XmlSchemaSet _schemas;
        private ValidationEventHandler _valEventHandler;
#endif

        // other settings
        private bool _closeInput;

        // read-only flag
        private bool _isReadOnly;

        //
        // Constructor
        //
        public XmlReaderSettings()
        {
            Initialize();
        }

#if !FEATURE_LEGACYNETCF
        // introduced for supporting design-time loading of phone assemblies
        [Obsolete("This API supports the .NET Framework infrastructure and is not intended to be used directly from your code.", true)]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public XmlReaderSettings(XmlResolver resolver)
        {
            Initialize(resolver);
        }

        //
        // Properties
        //

#if ASYNC || FEATURE_NETCORE
        public bool Async
        {
            get
            {
                return _useAsync;
            }
            set
            {
                CheckReadOnly("Async");
                _useAsync = value;
            }
        }
#endif

        // Nametable
        public XmlNameTable NameTable
        {
            get
            {
                return _nameTable;
            }
            set
            {
                CheckReadOnly("NameTable");
                _nameTable = value;
            }
        }

#if !SILVERLIGHT
        // XmlResolver
        internal bool IsXmlResolverSet
        {
            get;
            set; // keep set internal as we need to call it from the schema validation code
        }
#endif

        public XmlResolver XmlResolver
        {
            set
            {
                CheckReadOnly("XmlResolver");
                _xmlResolver = value;
#if !SILVERLIGHT
                IsXmlResolverSet = true;
#endif
            }
        }

        internal XmlResolver GetXmlResolver()
        {
            return _xmlResolver;
        }

#if !SILVERLIGHT
        //This is used by get XmlResolver in Xsd.
        //Check if the config set to prohibit default resovler
        //notice we must keep GetXmlResolver() to avoid dead lock when init System.Config.ConfigurationManager
        internal XmlResolver GetXmlResolver_CheckConfig()
        {
            if (LocalAppContextSwitches.ProhibitDefaultUrlResolver && !IsXmlResolverSet)
                return null;
            else
                return _xmlResolver;
        }
#endif

        // Text settings
        public int LineNumberOffset
        {
            get
            {
                return _lineNumberOffset;
            }
            set
            {
                CheckReadOnly("LineNumberOffset");
                _lineNumberOffset = value;
            }
        }

        public int LinePositionOffset
        {
            get
            {
                return _linePositionOffset;
            }
            set
            {
                CheckReadOnly("LinePositionOffset");
                _linePositionOffset = value;
            }
        }

        // Conformance settings
        public ConformanceLevel ConformanceLevel
        {
            get
            {
                return _conformanceLevel;
            }
            set
            {
                CheckReadOnly("ConformanceLevel");

                if ((uint)value > (uint)ConformanceLevel.Document)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                _conformanceLevel = value;
            }
        }

        public bool CheckCharacters
        {
            get
            {
                return _checkCharacters;
            }
            set
            {
                CheckReadOnly("CheckCharacters");
                _checkCharacters = value;
            }
        }

        public long MaxCharactersInDocument
        {
            get
            {
                return _maxCharactersInDocument;
            }
            set
            {
                CheckReadOnly("MaxCharactersInDocument");
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                _maxCharactersInDocument = value;
            }
        }

        public long MaxCharactersFromEntities
        {
            get
            {
                return _maxCharactersFromEntities;
            }
            set
            {
                CheckReadOnly("MaxCharactersFromEntities");
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                _maxCharactersFromEntities = value;
            }
        }

        // Filtering settings
        public bool IgnoreWhitespace
        {
            get
            {
                return _ignoreWhitespace;
            }
            set
            {
                CheckReadOnly("IgnoreWhitespace");
                _ignoreWhitespace = value;
            }
        }

        public bool IgnoreProcessingInstructions
        {
            get
            {
                return _ignorePIs;
            }
            set
            {
                CheckReadOnly("IgnoreProcessingInstructions");
                _ignorePIs = value;
            }
        }

        public bool IgnoreComments
        {
            get
            {
                return _ignoreComments;
            }
            set
            {
                CheckReadOnly("IgnoreComments");
                _ignoreComments = value;
            }
        }

#if !SILVERLIGHT
        [Obsolete("Use XmlReaderSettings.DtdProcessing property instead.")]
        public bool ProhibitDtd
        {
            get
            {
                return _dtdProcessing == DtdProcessing.Prohibit;
            }
            set
            {
                CheckReadOnly("ProhibitDtd");
                _dtdProcessing = value ? DtdProcessing.Prohibit : DtdProcessing.Parse;
            }
        }
#endif

        public DtdProcessing DtdProcessing
        {
            get
            {
                return _dtdProcessing;
            }
            set
            {
                CheckReadOnly("DtdProcessing");

                if ((uint)value > (uint)DtdProcessing.Parse)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                _dtdProcessing = value;
            }
        }

        public bool CloseInput
        {
            get
            {
                return _closeInput;
            }
            set
            {
                CheckReadOnly("CloseInput");
                _closeInput = value;
            }
        }

#if !SILVERLIGHT
        public ValidationType ValidationType
        {
            get
            {
                return _validationType;
            }
            set
            {
                CheckReadOnly("ValidationType");

                if ((uint)value > (uint)ValidationType.Schema)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                _validationType = value;
            }
        }

        public XmlSchemaValidationFlags ValidationFlags
        {
            get
            {
                return _validationFlags;
            }
            set
            {
                CheckReadOnly("ValidationFlags");

                if ((uint)value > (uint)(XmlSchemaValidationFlags.ProcessInlineSchema | XmlSchemaValidationFlags.ProcessSchemaLocation |
                                           XmlSchemaValidationFlags.ReportValidationWarnings | XmlSchemaValidationFlags.ProcessIdentityConstraints |
                                           XmlSchemaValidationFlags.AllowXmlAttributes))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                _validationFlags = value;
            }
        }

        public XmlSchemaSet Schemas
        {
            get
            {
                if (_schemas == null)
                {
                    _schemas = new XmlSchemaSet();
                }
                return _schemas;
            }
            set
            {
                CheckReadOnly("Schemas");
                _schemas = value;
            }
        }

        public event ValidationEventHandler ValidationEventHandler
        {
            add
            {
                CheckReadOnly("ValidationEventHandler");
                _valEventHandler += value;
            }
            remove
            {
                CheckReadOnly("ValidationEventHandler");
                _valEventHandler -= value;
            }
        }
#endif

        //
        // Public methods
        //
        public void Reset()
        {
            CheckReadOnly("Reset");
            Initialize();
        }

        public XmlReaderSettings Clone()
        {
            XmlReaderSettings clonedSettings = this.MemberwiseClone() as XmlReaderSettings;
            clonedSettings.ReadOnly = false;
            return clonedSettings;
        }

        //
        // Internal methods
        //
#if !SILVERLIGHT
        internal ValidationEventHandler GetEventHandler()
        {
            return _valEventHandler;
        }
#endif

        internal XmlReader CreateReader(String inputUri, XmlParserContext inputContext)
        {
            if (inputUri == null)
            {
                throw new ArgumentNullException("inputUri");
            }
            if (inputUri.Length == 0)
            {
                throw new ArgumentException(Res.XmlConvert_BadUri, "inputUri");
            }

            // resolve and open the url
            XmlResolver tmpResolver = this.GetXmlResolver();
            if (tmpResolver == null)
            {
                tmpResolver = CreateDefaultResolver();
            }

            // create text XML reader
            XmlReader reader = new XmlTextReaderImpl(inputUri, this, inputContext, tmpResolver);

#if !SILVERLIGHT
            // wrap with validating reader
            if (this.ValidationType != ValidationType.None)
            {
                reader = AddValidation(reader);
            }
#endif

#if ASYNC
            if (_useAsync)
            {
                reader = XmlAsyncCheckReader.CreateAsyncCheckWrapper(reader);
            }
#endif
            return reader;
        }

        internal XmlReader CreateReader(Stream input, Uri baseUri, string baseUriString, XmlParserContext inputContext)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            if (baseUriString == null)
            {
                if (baseUri == null)
                {
                    baseUriString = string.Empty;
                }
                else
                {
                    baseUriString = baseUri.ToString();
                }
            }

            // create text XML reader
            XmlReader reader = new XmlTextReaderImpl(input, null, 0, this, baseUri, baseUriString, inputContext, _closeInput);

#if !SILVERLIGHT
            // wrap with validating reader
            if (this.ValidationType != ValidationType.None)
            {
                reader = AddValidation(reader);
            }
#endif

#if ASYNC
            if (_useAsync)
            {
                reader = XmlAsyncCheckReader.CreateAsyncCheckWrapper(reader);
            }
#endif

            return reader;
        }

        internal XmlReader CreateReader(TextReader input, string baseUriString, XmlParserContext inputContext)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            if (baseUriString == null)
            {
                baseUriString = string.Empty;
            }

            // create xml text reader
            XmlReader reader = new XmlTextReaderImpl(input, this, baseUriString, inputContext);

#if !SILVERLIGHT
            // wrap with validating reader
            if (this.ValidationType != ValidationType.None)
            {
                reader = AddValidation(reader);
            }
#endif

#if ASYNC
            if (_useAsync)
            {
                reader = XmlAsyncCheckReader.CreateAsyncCheckWrapper(reader);
            }
#endif

            return reader;
        }

        internal XmlReader CreateReader(XmlReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
#if !ASYNC || SILVERLIGHT || FEATURE_NETCORE
            // wrap with conformance layer (if needed)
            return AddConformanceWrapper(reader);
#else
            return AddValidationAndConformanceWrapper(reader);
#endif // !ASYNC || SILVERLIGHT || FEATURE_NETCORE
        }

        internal bool ReadOnly
        {
            get
            {
                return _isReadOnly;
            }
            set
            {
                _isReadOnly = value;
            }
        }

        private void CheckReadOnly(string propertyName)
        {
            if (_isReadOnly)
            {
                throw new XmlException(Res.Xml_ReadOnlyProperty, this.GetType().Name + '.' + propertyName);
            }
        }

        //
        // Private methods
        //
        private void Initialize()
        {
            Initialize(null);
        }

        private void Initialize(XmlResolver resolver)
        {
            _nameTable = null;
#if !SILVERLIGHT
            if (!EnableLegacyXmlSettings())
            {
                _xmlResolver = resolver;
                // limit the entity resolving to 10 million character. the caller can still
                // override it to any other value or set it to zero for unlimiting it
                _maxCharactersFromEntities = (long)1e7;
            }
            else
#endif            
            {
                _xmlResolver = (resolver == null ? CreateDefaultResolver() : resolver);
                _maxCharactersFromEntities = 0;
            }
            _lineNumberOffset = 0;
            _linePositionOffset = 0;
            _checkCharacters = true;
            _conformanceLevel = ConformanceLevel.Document;

            _ignoreWhitespace = false;
            _ignorePIs = false;
            _ignoreComments = false;
            _dtdProcessing = DtdProcessing.Prohibit;
            _closeInput = false;

            _maxCharactersInDocument = 0;

#if !SILVERLIGHT
            _schemas = null;
            _validationType = ValidationType.None;
            _validationFlags = XmlSchemaValidationFlags.ProcessIdentityConstraints;
            _validationFlags |= XmlSchemaValidationFlags.AllowXmlAttributes;
#endif

#if ASYNC || FEATURE_NETCORE
            _useAsync = false;
#endif

            _isReadOnly = false;
#if !SILVERLIGHT
            IsXmlResolverSet = false;
#endif
        }

        private static XmlResolver CreateDefaultResolver()
        {
            return new XmlSystemPathResolver();
        }

#if !SILVERLIGHT
        internal XmlReader AddValidation(XmlReader reader)
        {
            if (_validationType == ValidationType.Schema)
            {
                XmlResolver resolver = GetXmlResolver_CheckConfig();

                if (resolver == null &&
                    !this.IsXmlResolverSet &&
                    !EnableLegacyXmlSettings())
                {
                    resolver = new XmlUrlResolver();
                }
                reader = new XsdValidatingReader(reader, resolver, this);
            }
            else if (_validationType == ValidationType.DTD)
            {
                reader = CreateDtdValidatingReader(reader);
            }
            return reader;
        }

        private XmlReader AddValidationAndConformanceWrapper(XmlReader reader)
        {
            // wrap with DTD validating reader
            if (_validationType == ValidationType.DTD)
            {
                reader = CreateDtdValidatingReader(reader);
            }
            // add conformance checking (must go after DTD validation because XmlValidatingReader works only on XmlTextReader),
            // but before XSD validation because of typed value access
            reader = AddConformanceWrapper(reader);

            if (_validationType == ValidationType.Schema)
            {
                reader = new XsdValidatingReader(reader, GetXmlResolver_CheckConfig(), this);
            }
            return reader;
        }

        private XmlValidatingReaderImpl CreateDtdValidatingReader(XmlReader baseReader)
        {
            return new XmlValidatingReaderImpl(baseReader, this.GetEventHandler(), (this.ValidationFlags & XmlSchemaValidationFlags.ProcessIdentityConstraints) != 0);
        }
#endif // !SILVERLIGHT

        internal XmlReader AddConformanceWrapper(XmlReader baseReader)
        {
            XmlReaderSettings baseReaderSettings = baseReader.Settings;
            bool checkChars = false;
            bool noWhitespace = false;
            bool noComments = false;
            bool noPIs = false;
            DtdProcessing dtdProc = (DtdProcessing)(-1);
            bool needWrap = false;

            if (baseReaderSettings == null)
            {
#pragma warning disable 618

#if SILVERLIGHT
                // Starting from Windows phone 8.1 (TargetsAtLeast_Desktop_V4_5_1) we converge with the desktop behavior so we'll let the reader 
                // not throw exception if has different conformance level than Auto.
                if (BinaryCompatibility.TargetsAtLeast_Desktop_V4_5_1) {
                    if (this.conformanceLevel != ConformanceLevel.Auto && this.conformanceLevel != XmlReader.GetV1ConformanceLevel(baseReader)) {
                        throw new InvalidOperationException(string.Format(Res.Xml_IncompatibleConformanceLevel, this.conformanceLevel.ToString()));
                    }
                } else if (this.conformanceLevel != ConformanceLevel.Auto) {
                    throw new InvalidOperationException(string.Format(Res.Xml_IncompatibleConformanceLevel, this.conformanceLevel.ToString()));
                }
#else
                if (_conformanceLevel != ConformanceLevel.Auto && _conformanceLevel != XmlReader.GetV1ConformanceLevel(baseReader))
                {
                    throw new InvalidOperationException(string.Format(Res.Xml_IncompatibleConformanceLevel, _conformanceLevel.ToString()));
                }
#endif

#if !SILVERLIGHT
                // get the V1 XmlTextReader ref
                XmlTextReader v1XmlTextReader = baseReader as XmlTextReader;
                if (v1XmlTextReader == null)
                {
                    XmlValidatingReader vr = baseReader as XmlValidatingReader;
                    if (vr != null)
                    {
                        v1XmlTextReader = (XmlTextReader)vr.Reader;
                    }
                }
#endif

                // assume the V1 readers already do all conformance checking; 
                // wrap only if IgnoreWhitespace, IgnoreComments, IgnoreProcessingInstructions or ProhibitDtd is true;
                if (_ignoreWhitespace)
                {
                    WhitespaceHandling wh = WhitespaceHandling.All;
#if !SILVERLIGHT
                    // special-case our V1 readers to see if whey already filter whitespaces
                    if (v1XmlTextReader != null)
                    {
                        wh = v1XmlTextReader.WhitespaceHandling;
                    }
#endif
                    if (wh == WhitespaceHandling.All)
                    {
                        noWhitespace = true;
                        needWrap = true;
                    }
                }
                if (_ignoreComments)
                {
                    noComments = true;
                    needWrap = true;
                }
                if (_ignorePIs)
                {
                    noPIs = true;
                    needWrap = true;
                }
                // DTD processing
                DtdProcessing baseDtdProcessing = DtdProcessing.Parse;
#if !SILVERLIGHT
                if (v1XmlTextReader != null)
                {
                    baseDtdProcessing = v1XmlTextReader.DtdProcessing;
                }
#endif
                if ((_dtdProcessing == DtdProcessing.Prohibit && baseDtdProcessing != DtdProcessing.Prohibit) ||
                    (_dtdProcessing == DtdProcessing.Ignore && baseDtdProcessing == DtdProcessing.Parse))
                {
                    dtdProc = _dtdProcessing;
                    needWrap = true;
                }
#pragma warning restore 618
            }
            else
            {
                if (_conformanceLevel != baseReaderSettings.ConformanceLevel && _conformanceLevel != ConformanceLevel.Auto)
                {
                    throw new InvalidOperationException(string.Format(Res.Xml_IncompatibleConformanceLevel, _conformanceLevel.ToString()));
                }
                if (_checkCharacters && !baseReaderSettings.CheckCharacters)
                {
                    checkChars = true;
                    needWrap = true;
                }
                if (_ignoreWhitespace && !baseReaderSettings.IgnoreWhitespace)
                {
                    noWhitespace = true;
                    needWrap = true;
                }
                if (_ignoreComments && !baseReaderSettings.IgnoreComments)
                {
                    noComments = true;
                    needWrap = true;
                }
                if (_ignorePIs && !baseReaderSettings.IgnoreProcessingInstructions)
                {
                    noPIs = true;
                    needWrap = true;
                }

                if ((_dtdProcessing == DtdProcessing.Prohibit && baseReaderSettings.DtdProcessing != DtdProcessing.Prohibit) ||
                    (_dtdProcessing == DtdProcessing.Ignore && baseReaderSettings.DtdProcessing == DtdProcessing.Parse))
                {
                    dtdProc = _dtdProcessing;
                    needWrap = true;
                }
            }

            if (needWrap)
            {
                IXmlNamespaceResolver readerAsNSResolver = baseReader as IXmlNamespaceResolver;
                if (readerAsNSResolver != null)
                {
                    return new XmlCharCheckingReaderWithNS(baseReader, readerAsNSResolver, checkChars, noWhitespace, noComments, noPIs, dtdProc);
                }
                else
                {
                    return new XmlCharCheckingReader(baseReader, checkChars, noWhitespace, noComments, noPIs, dtdProc);
                }
            }
            else
            {
                return baseReader;
            }
        }

#if !SILVERLIGHT
        private static bool? s_enableLegacyXmlSettings = null;

        static internal bool EnableLegacyXmlSettings()
        {
            if (s_enableLegacyXmlSettings.HasValue)
            {
                return s_enableLegacyXmlSettings.Value;
            }

            if (!System.Xml.BinaryCompatibility.TargetsAtLeast_Desktop_V4_5_2)
            {
                s_enableLegacyXmlSettings = true;
                return s_enableLegacyXmlSettings.Value;
            }

            bool enableSettings = false; // default value
            if (!ReadSettingsFromRegistry(Registry.LocalMachine, ref enableSettings))
            {
                // still ok if this call return false too as we'll use the default value which is false
                ReadSettingsFromRegistry(Registry.CurrentUser, ref enableSettings);
            }

            s_enableLegacyXmlSettings = enableSettings;
            return s_enableLegacyXmlSettings.Value;
        }

        [SecuritySafeCritical]
        private static bool ReadSettingsFromRegistry(RegistryKey hive, ref bool value)
        {
            const string regValueName = "EnableLegacyXmlSettings";
            const string regValuePath = @"SOFTWARE\Microsoft\.NETFramework\XML";

            try
            {
                using (RegistryKey xmlRegKey = hive.OpenSubKey(regValuePath, false))
                {
                    if (xmlRegKey != null)
                    {
                        if (xmlRegKey.GetValueKind(regValueName) == RegistryValueKind.DWord)
                        {
                            value = ((int)xmlRegKey.GetValue(regValueName)) == 1;
                            return true;
                        }
                    }
                }
            }
            catch { /* use the default if we couldn't read the key */ }

            return false;
        }

#endif // SILVERLIGHT
    }
}