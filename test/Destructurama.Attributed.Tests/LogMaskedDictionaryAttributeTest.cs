using Destructurama.Attributed.Tests.Support;
using NUnit.Framework;
using Serilog;
using Serilog.Events;
using System.Collections.Generic;
using System.Linq;

namespace Destructurama.Attributed.Tests
{
    public class CustomizedDictionaryMaskedLogs
    {
        [LogMaskedDictionary(MaskKeys = new[] { "Test1", "Test2", "Test3" })]
        public Dictionary<string, string>? DictionaryMasked { get; set; }
    }

    [TestFixture]
    public class LogMaskedDictionaryAttributeTest
    {
        [Test]
        public void LogMaskedDictionaryAttribute()
        {
            // [LogMaskedDictionary(MaskKeys = new[] { "Test1", "Test2", "Test3" })]
            // {"Test1": "111", "Test2": "222", "Test3": "333"} -> {"Test1": "***", "Test2": "***", "Test3": "***"}

            LogEvent evt = null!;

            var log = new LoggerConfiguration()
                .Destructure.UsingAttributes()
                .WriteTo.Sink(new DelegatingSink(e => evt = e))
                .CreateLogger();

            var customized = new CustomizedDictionaryMaskedLogs
            {
                DictionaryMasked = new Dictionary<string, string>
                {
                    {"Test1", "111" },
                    {"Test2", "222" },
                    {"Test3", "333" },
                }
            };

            log.Information("Here is {@Customized}", customized);

            var sv = (StructureValue)evt.Properties["Customized"];
            var props = sv.Properties.ToDictionary(p => p.Name, p => p.Value);

            Assert.IsTrue(props.ContainsKey("DictionaryMasked"));
            var seq = props["DictionaryMasked"] as SequenceValue;
            foreach (var elem in seq!.Elements)
            {
                Assert.AreEqual("***", ((KeyValuePair<string, string>)elem.LiteralValue()).Value);
            }
        }
    }
}
