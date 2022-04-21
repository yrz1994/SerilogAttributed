// Copyright 2015-2018 Destructurama Contributors, Serilog Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Destructurama.Attributed
{
    /// <summary>
    /// Apply to a property to apply a mask to the logged value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class LogMaskedDictionaryAttribute : LogMaskedAttribute, IPropertyDestructuringAttribute
    {
        /// <summary>
        /// The keys of the dictionary that need to be masked.
        /// </summary>
        public string[] MaskKeys { get; set; } = new string[] { };

        /// <inheritdoc/>
        public new bool TryCreateLogEventProperty(string name, object? value, ILogEventPropertyValueFactory propertyValueFactory, out LogEventProperty? property)
        {
            property = new LogEventProperty(name, CreateValue(value));
            return true;
        }

        private object FormatMaskedKeyValue(KeyValuePair<string, string> keyValue)
        {
            if (!MaskKeys.Contains(keyValue.Key, StringComparer.Ordinal) || string.IsNullOrEmpty(keyValue.Value)) return keyValue;
            return new KeyValuePair<string, string>(keyValue.Key, (string)FormatMaskedValue(keyValue.Value));

        }

        private LogEventPropertyValue CreateValue(object? value)
        {
            return value switch
            {
                Dictionary<string, string> dict => new SequenceValue(dict.Select(keyValuePair => new ScalarValue(FormatMaskedKeyValue(keyValuePair)))),
                _ => new ScalarValue(null)
            };

        }
    }
}
