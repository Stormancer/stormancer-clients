using MessagePack;
using MessagePack.Formatters;
using System;
using System.Buffers;

namespace Stormancer
{
    /// <summary>
    /// Represents the id of a client session.
    /// </summary>
    public struct SessionId : IComparable
    {
        private Guid _value;

        /// <summary>
        /// Creates a byte array representation of the object.
        /// </summary>
        /// <returns></returns>
        public byte[] ToByteArray() => _value.ToByteArray();

        /// <summary>
        /// Tries writing the binary content of the session id to a <see cref="Span{T}"/>
        /// </summary>
        /// <param name="destination"></param>
        /// <returns></returns>
        public bool TryWriteBytes(Span<byte> destination)=> _value.TryWriteBytes(destination);
        private SessionId(Guid guid)
        {
            _value = guid;
        }
        private SessionId(byte[] rawValue)
        {
            _value = new Guid(rawValue);
        }
        private SessionId(ReadOnlySpan<byte> span)
        {
            _value = new Guid(span);
        }

        /// <summary>
        /// Returns true if the <see cref="SessionId"/> is empty.
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return _value == Guid.Empty;
        }

        /// <summary>
        /// An empty <see cref="SessionId"/>
        /// </summary>
        public static SessionId Empty
        {
            get
            {
                return new SessionId();
            }
        }

        /// <summary>
        /// Generates an hash code for the <see cref="SessionId"/>
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            
            return _value.GetHashCode();

        }

        public static bool operator==(SessionId sId1, SessionId sId2)
        {
            return sId1.Equals(sId2);
        }

        public static bool operator!=(SessionId sId1, SessionId sId2)
        {
            return !sId1.Equals(sId2);
        }

        /// <summary>
        /// Evaluates if a <see cref="SessionId"/> object is equal to the current one.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            if(obj == null)
            {
                return false;
            }

            if (obj is SessionId)
            {
                var other = (SessionId)obj;


                return other._value == _value;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a string representation of the session id.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return System.Convert.ToBase64String(ToByteArray()).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        /// <summary>
        /// Creates a new <see cref="SessionId"/>.
        /// </summary>
        /// <returns></returns>
        public static SessionId CreateNew()
        {
            return new SessionId(Guid.NewGuid());
        }

        /// <summary>
        /// Creates a <see cref="SessionId"/> from a string representation.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public static SessionId From(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                return new SessionId();
            }
            else
            {
                string incoming = sessionId.Replace('_', '/').Replace('-', '+');
                switch (sessionId.Length % 4)
                {
                    case 2: incoming += "=="; break;
                    case 3: incoming += "="; break;
                }
                return new SessionId(System.Convert.FromBase64String(incoming));
            }
        }
        /// <summary>
        /// Creates a <see cref="SessionId"/> object from a binary representation.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public static SessionId From(byte[] sessionId)
        {

            if (sessionId == null)
            {
                return new SessionId();
            }
            if (sessionId.Length != 16)
            {
                throw new ArgumentException("Session ids must be 16 bytes long");
            }
            return new SessionId(sessionId);
        }

        /// <summary>
        /// Creates a <see cref="SessionId"/> object from a binary representation.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public static SessionId From(ReadOnlySequence<byte> sessionId)
        {
            if (sessionId.Length != 16)
            {
                throw new ArgumentException("Session ids must be 16 bytes long");
            }
            if (sessionId.FirstSpan.Length == 16)
            {
                return new SessionId(sessionId.FirstSpan);
            }
            else
            {
                Span<byte> buffer = stackalloc byte[16];
                var reader = new SequenceReader<byte>(sessionId);
                reader.TryCopyTo(buffer);

                return new SessionId(buffer);
            }
        }

        public int CompareTo(object obj)
        {
            var sessionId = (SessionId)obj;

            return _value.CompareTo(sessionId._value);
        }
    }
    internal class SessionIdMessagePackFormatter : IMessagePackFormatter<SessionId>
    {
        public SessionId Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            var length = reader.ReadArrayHeader();
            
            return SessionId.From(reader.Sequence.Slice(length));
        }

        public void Serialize(ref MessagePackWriter writer, SessionId value, MessagePackSerializerOptions options)
        {
            writer.WriteBinHeader(16);
            value.TryWriteBytes(writer.GetSpan(16));
        }
    }
}
