using EventFlow.Core;
using EventFlow.ValueObjects;
using System.Text.Json.Serialization;

namespace Trsys.BackOffice.Domain
{
    [JsonConverter(typeof(SingleValueObjectConverter))]
    public class PublisherId : Identity<PublisherId>
    {
        public PublisherId(string value) : base(value)
        {
        }
    }
}