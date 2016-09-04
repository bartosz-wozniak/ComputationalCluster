using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Linq;
using System.Xml;
using CCluster.Common;
using CCluster.Messages;
using CCluster.Common.Communication.Exceptions;
using log4net;

namespace CCluster.Common.Communication.Messaging
{
    /// <remarks>
    /// Works only with types from CCluster.Messages namespace (subnamespaces aren't supported for now), any other type
    /// won't be supported.
    /// 
    /// This is rather fast and loose implementation that only satisfies the basic contract. SRP is broken here because
    /// of the shared dictionary of serializers.
    /// </remarks>
    public class SimpleMessageSerializer : IMessageSerializer, IMessageDeserializer
    {
        private static readonly ConcurrentDictionary<Type, DataContractSerializer> Serializers
            = new ConcurrentDictionary<Type, DataContractSerializer>();
        private static readonly IReadOnlyDictionary<string, Type> AvailableTypes;

        private static readonly ILog log = LogProvider.GetCurrentClassLogger();

        static SimpleMessageSerializer()
        {
            log.Debug("Create Message Serializer");
            var msgType = typeof(IMessage);
            AvailableTypes =
                msgType.Assembly.GetExportedTypes()
                    .Where(t => msgType.IsAssignableFrom(t))
                    .ToDictionary(t => t.Name, t => t);
        }

        public byte[] Serialize(IMessage message)
        {
            log.Debug("Serialize Message");
            var serializer = GetSerializer(message.GetType());
            using (var ms = new MemoryStream())
            {
                using (var writer = new XmlTextWriter(ms, Encoding.UTF8))
                {
                    serializer.WriteObject(writer, message);
                }
                return ms.ToArray();
            }
        }

        public IMessage Deserialize(byte[] data, int offset, int count)
        {
            log.Debug("Deserialize Message");
            try
            {
                using (var dictReader = XmlDictionaryReader.CreateTextReader(data, offset, count, Encoding.UTF8, new XmlDictionaryReaderQuotas(), _ => { }))
                {
                    dictReader.MoveToStartElement();
                    var type = AvailableTypes[dictReader.Name];
                    var serializer = GetSerializer(type);
                    return serializer.ReadObject(dictReader) as IMessage;
                }
            }
            catch (Exception ex)
            {
                //TODO Change log type
                log.Debug("Deserialization failed, throwing CannotDeserializeMessageException, exception was caught: " + ex.Message);
                throw new CannotDeserializeMessageException(ex);
            }
        }

        private static DataContractSerializer GetSerializer(Type messageType)
        {
            return Serializers.GetOrAdd(messageType, t => new DataContractSerializer(t));
        }
    }
}
