using System;
using System.Collections.Generic;
using DarkRift;

namespace Penca53.DarkRift.MLAPI
{
    public static class MLAPI
    {
        /// <summary>
        /// Stores all the types that implements the ISync interface and needs to assign an int ID to their type (used to deserialize without knowing the type)
        /// </summary>
        public static Dictionary<int, Type> SyncTypes = new Dictionary<int, Type>();
    }

    public interface ISync : IDarkRiftSerializable
    {
        /// <summary>
        /// ID of the type of the class that implements ISync
        /// </summary>
        int TypeID { get; set; }

        /// <summary>
        /// Serializes data depending on the tag condition
        /// </summary>
        /// <param name="writer">The DarkRiftWriter to write in</param>
        /// <param name="tag">The tag condition</param>
        void SerializeOptional(DarkRiftWriter writer, int tag);
        /// <summary>
        /// Deserializes data depending on the tag condition
        /// </summary>
        /// <param name="reader">The DarkRiftReader to read from</param>
        /// <param name="tag">The tag condition</param>
        void DeserializeOptional(DarkRiftReader reader, int tag);
    }

    /// <summary>
    /// Specifies the extra data written in the DarkRiftWriter
    /// </summary>
    public enum ExtraSyncData : byte
    {
        Nothing,
        TypeID,
        Tag,
        TypeIDANDTag,
    }

    public static class DarkRiftExtensions
    {
        #region Write

        /// <summary>
        /// Writes in a DarkRiftWriter the given ISync instance and the typeID, if wanted. Calls SerializeOptional using -1 (default)
        /// </summary>
        /// <param name="writer">The DarRiftWriter to write in</param>
        /// <param name="sync">The ISync instance to write</param>
        /// <param name="sendTypeID">If you want to send the TypeID of the ISync instance</param>
        public static void WriteSmart(this DarkRiftWriter writer, ISync sync, bool sendTypeID = false)
        {
            if (!sendTypeID)
            {
                writer.Write((byte)0);
            }
            else
            {
                writer.Write((byte)1);
                writer.Write(sync.TypeID);
            }

            sync.SerializeOptional(writer, -1);
        }

        /// <summary>
        /// Writes in a DarkRiftWriter the given ISync instance, the typeID, if wanted and the tag, if wanted. Calls SerializeOptional using tag
        /// </summary>
        /// <param name="writer">The DarRiftWriter to write in</param>
        /// <param name="sync">The ISync instance to write</param>
        /// <param name="tag">The tag to use in SerializeOptional (and to send if you want)</param>
        /// <param name="sendTag">If you want to send the tag</param>
        /// <param name="sendTypeID">If you want to send the TypeID of the ISync instance</param>
        public static void WriteSmart(this DarkRiftWriter writer, ISync sync, int tag, bool sendTag = false, bool sendTypeID = false)
        {
            if (!sendTypeID)
            {
                if (!sendTag)
                {
                    writer.Write((byte)0);
                }
                else
                {
                    writer.Write((byte)2);
                    writer.Write(tag);
                }
            }
            else
            {
                if (!sendTag)
                {
                    writer.Write((byte)1);
                    writer.Write(sync.TypeID);
                }
                else
                {
                    writer.Write((byte)3);
                    writer.Write(sync.TypeID);
                    writer.Write(tag);
                }
            }

            sync.SerializeOptional(writer, tag);
        }

        /// <summary>
        /// Writes in a DarkRiftWriter the given ISync instance, the typeID, if wanted and the tag, if wanted. Calls SerializeOptional using tag (default = -1)
        /// </summary>
        /// <param name="writer">The DarRiftWriter to write in</param>
        /// <param name="sync">The ISync instance to write</param>
        /// <param name="extraSyncData">The way to handle extra data to send</param>
        /// <param name="tag">The tag to use in SerializeOptional (and to send if you want)</param>
        public static void WriteSmart(this DarkRiftWriter writer, ISync sync, ExtraSyncData extraSyncData, int tag = -1)
        {
            writer.Write((byte)extraSyncData);

            switch (extraSyncData)
            {
                case ExtraSyncData.TypeID:
                    writer.Write(sync.TypeID);
                    break;
                case ExtraSyncData.Tag:
                    writer.Write(tag);
                    break;
                case ExtraSyncData.TypeIDANDTag:
                    writer.Write(sync.TypeID);
                    writer.Write(tag);
                    break;
            }

            sync.SerializeOptional(writer, tag);
        }

        #endregion

        #region ReadSerializable DarkRiftReader

        /// <summary>
        /// Reads from a DarkRift reader and creates a new instance of the type written in the buffer (which has to implement ISync) and as tag it uses the one written in the buffer or -1 if there isn't (default)
        /// </summary>
        /// <param name="reader">The DarkRift reader to read from</param>
        /// <returns></returns>
        public static ISync ReadSerializableSmart(this DarkRiftReader reader)
        {
            ExtraSyncData _extraSyncData = (ExtraSyncData)reader.ReadByte();
            ISync _syncObject = default;
            int _typeID;
            int _tag;

            switch (_extraSyncData)
            {
                case ExtraSyncData.Nothing:
                    Console.WriteLine("No TypeID provided!");
                    break;
                case ExtraSyncData.TypeID:
                    _typeID = reader.ReadInt32();
                    _syncObject = (ISync)Activator.CreateInstance(MLAPI.SyncTypes[_typeID]);
                    _syncObject.DeserializeOptional(reader, -1);
                    break;
                case ExtraSyncData.Tag:
                    Console.WriteLine("No TypeID provided!");
                    break;
                case ExtraSyncData.TypeIDANDTag:
                    _typeID = reader.ReadInt32();
                    _tag = reader.ReadInt32();
                    _syncObject = (ISync)Activator.CreateInstance(MLAPI.SyncTypes[_typeID]);
                    _syncObject.DeserializeOptional(reader, _tag);
                    break;
            }

            return _syncObject;
        }

        /// <summary>
        /// Reads from a DarkRift reader and creates a new instance of a given type T (which has to implement ISync) and as tag it uses the one written in the buffer or -1 if there isn't (default)
        /// </summary>
        /// <typeparam name="T">The I/O type</typeparam>
        /// <param name="reader">The DarkRift reader to read from</param>
        /// <returns></returns>
        public static T ReadSerializableSmart<T>(this DarkRiftReader reader) where T : ISync, new()
        {
            ExtraSyncData _extraSyncData = (ExtraSyncData)reader.ReadByte();
            T _syncObject = default;
            int _typeID;
            int _tag;

            switch (_extraSyncData)
            {
                case ExtraSyncData.Nothing:
                    _syncObject = (T)Activator.CreateInstance(typeof(T));
                    _syncObject.DeserializeOptional(reader, -1);
                    break;
                case ExtraSyncData.TypeID:
                    _typeID = reader.ReadInt32();
                    _syncObject = (T)Activator.CreateInstance(typeof(T));
                    _syncObject.DeserializeOptional(reader, -1);
                    break;
                case ExtraSyncData.Tag:
                    _tag = reader.ReadInt32();
                    _syncObject = (T)Activator.CreateInstance(typeof(T));
                    _syncObject.DeserializeOptional(reader, _tag);
                    break;
                case ExtraSyncData.TypeIDANDTag:
                    _typeID = reader.ReadInt32();
                    _tag = reader.ReadInt32();
                    _syncObject = (T)Activator.CreateInstance(typeof(T));
                    _syncObject.DeserializeOptional(reader, _tag);
                    break;
            }

            return _syncObject;
        }

        /// <summary>
        /// Reads from a DarkRift reader and creates a new instance of the type written in the buffer (which has to implement ISync) and as tag it uses the parameter tag
        /// </summary>
        /// <param name="reader">The DarkRift reader to read from</param>
        /// <param name="tag">The tag to use in DeserializeOptional</param>
        /// <returns></returns>
        public static ISync ReadSerializableSmart(this DarkRiftReader reader, int tag)
        {
            ExtraSyncData _extraSyncData = (ExtraSyncData)reader.ReadByte();
            ISync _syncObject = default;
            int _typeID;
            int _tag;

            switch (_extraSyncData)
            {
                case ExtraSyncData.Nothing:
                    Console.WriteLine("No TypeID provided!");
                    break;
                case ExtraSyncData.TypeID:
                    _typeID = reader.ReadInt32();
                    _syncObject = (ISync)Activator.CreateInstance(MLAPI.SyncTypes[_typeID]);
                    _syncObject.DeserializeOptional(reader, tag);
                    break;
                case ExtraSyncData.Tag:
                    Console.WriteLine("No TypeID provided!");
                    break;
                case ExtraSyncData.TypeIDANDTag:
                    _typeID = reader.ReadInt32();
                    _tag = reader.ReadInt32();
                    _syncObject = (ISync)Activator.CreateInstance(MLAPI.SyncTypes[_typeID]);
                    _syncObject.DeserializeOptional(reader, tag);
                    break;
            }

            return _syncObject;
        }

        /// <summary>
        /// Reads from a DarkRift reader and creates a new instance of a given type T (which has to implement ISync) and as tag it uses the parameter tag
        /// </summary>
        /// <typeparam name="T">The I/O type</typeparam>
        /// <param name="reader">The DarkRift reader to read from</param>
        /// <param name="tag">The tag to use in DeserializeOptional</param>
        /// <returns></returns>
        public static T ReadSerializableSmart<T>(this DarkRiftReader reader, int tag) where T : ISync, new()
        {
            ExtraSyncData _extraSyncData = (ExtraSyncData)reader.ReadByte();
            T _syncObject = default;
            int _typeID;
            int _tag;

            switch (_extraSyncData)
            {
                case ExtraSyncData.Nothing:
                    _syncObject = (T)Activator.CreateInstance(typeof(T));
                    _syncObject.DeserializeOptional(reader, tag);
                    break;
                case ExtraSyncData.TypeID:
                    _typeID = reader.ReadInt32();
                    _syncObject = (T)Activator.CreateInstance(typeof(T));
                    _syncObject.DeserializeOptional(reader, tag);
                    break;
                case ExtraSyncData.Tag:
                    _tag = reader.ReadInt32();
                    _syncObject = (T)Activator.CreateInstance(typeof(T));
                    _syncObject.DeserializeOptional(reader, tag);
                    break;
                case ExtraSyncData.TypeIDANDTag:
                    _typeID = reader.ReadInt32();
                    _tag = reader.ReadInt32();
                    _syncObject = (T)Activator.CreateInstance(typeof(T));
                    _syncObject.DeserializeOptional(reader, tag);
                    break;
            }

            return _syncObject;
        }

        #endregion

        #region ReadSerializable ISync
        /// <summary>
        /// Reads from a DarkRift reader and updates the given instance (which has to implement ISync) and as tag it uses the one written in the buffer or -1 if there isn't (default)
        /// </summary>
        /// <param name="sync">The instance to update</param>
        /// <param name="reader">The DarkRift reader to read from</param>
        public static void ReadSerializableSmart(this ISync sync, DarkRiftReader reader)
        {
            ExtraSyncData _extraSyncData = (ExtraSyncData)reader.ReadByte();
            int _typeID;
            int _tag;

            switch (_extraSyncData)
            {
                case ExtraSyncData.Nothing:
                    sync.DeserializeOptional(reader, -1);
                    break;
                case ExtraSyncData.TypeID:
                    _typeID = reader.ReadInt32();
                    sync.DeserializeOptional(reader, -1);
                    break;
                case ExtraSyncData.Tag:
                    _tag = reader.ReadInt32();
                    sync.DeserializeOptional(reader, _tag);
                    break;
                case ExtraSyncData.TypeIDANDTag:
                    _typeID = reader.ReadInt32();
                    _tag = reader.ReadInt32();
                    sync.DeserializeOptional(reader, _tag);
                    break;
            }
        }

        /// <summary>
        /// Reads from a DarkRift reader and updates the given instance (which has to implement ISync) and as tag it uses the parameter tag
        /// </summary>
        /// <param name="sync"></param>
        /// <param name="reader"></param>
        /// <param name="tag"></param>
        public static void ReadSerializableSmart(this ISync sync, DarkRiftReader reader, int tag)
        {
            ExtraSyncData _extraSyncData = (ExtraSyncData)reader.ReadByte();
            int _typeID;
            int _tag;

            switch (_extraSyncData)
            {
                case ExtraSyncData.Nothing:
                    sync.DeserializeOptional(reader, tag);
                    break;
                case ExtraSyncData.TypeID:
                    _typeID = reader.ReadInt32();
                    sync.DeserializeOptional(reader, tag);
                    break;
                case ExtraSyncData.Tag:
                    _tag = reader.ReadInt32();
                    sync.DeserializeOptional(reader, tag);
                    break;
                case ExtraSyncData.TypeIDANDTag:
                    _typeID = reader.ReadInt32();
                    _tag = reader.ReadInt32();
                    sync.DeserializeOptional(reader, tag);
                    break;
            }
        }

        #endregion
    }
}