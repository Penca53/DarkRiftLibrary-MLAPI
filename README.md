# DarkRiftLibrary-MLAPI

This library is focused on simplifing similar repetitive tasks that you have to deal with writing and reading messages from server to client and vice versa.

## Getting Started

In this section you will learn how to setup and use the library.

### Installing

To download it you have to go in the GitHub's release section and download the lastest release. Then you just have to add to the **references** `DarkRiftLibrary-MLAPI.dll`: Go in `Solution Explorer`/ Right click on `Reference`/`Add Reference`/`Browse` and search for it. If you're using **Unity**, then you have to create a folder called **Plugins** and move the `DarkRiftLibrary-MLAPI.dll` file to the folder.

### How to use

This library is simple and immediate... It provides you a new interface called `ISync` that you can implement in your classes that need to be synced. Once you derive from it, you have to implement a property called `TypeID`, which stores the ID of the current class. It is used to unpack a received message into one specific type. Then there are 2 methods which you should already know: `Serialize` and `Deserialize`. They can be used to **serialize** and **deserialize** **all** the object. Finally there are 2 new methods called `SerializeOptional` and `DeserializeOptional`. As the name suggests, they are used to **serialize** and **deserialize** data depending on a **condition**, which is the `tag` **parameter**. You can use the `tag` to know in which situation you're in and, depending on it, you can **serialize** and **deserialize** one field instead of another. Here it is a little **example** that shows all the functionalities of the library.

```cs
public class Example
{
    //Instance of the class to serialize
    public House House = new House();


    // SERIALIZATION

    // Method that serializes only the Measurements of the House using the Tags class as tag
    public void SendHouseMeasurements()
    {
        using (DarkRiftWriter _writer = DarkRiftWriter.Create())
        {
            _writer.Write(House, Tags.HouseMeasurements);

            using (Message _message = Message.Create(Tags.HouseMeasurements, _writer))
            {
                //Send Message
            }
        }
    }

    // Method that serializes only the Inhabitants of the House using the Tags class as tag
    public void SendHouseInhabitants()
    {
        using (DarkRiftWriter _writer = DarkRiftWriter.Create())
        {
            _writer.Write(House, Tags.HouseInhabitants);

            using (Message _message = Message.Create(Tags.HouseInhabitants, _writer))
            {
                //Send Message
            }
        }
    }

    // Method that serializes all the variables of the House using a number as tag
    public void SendHouseAll()
    {
        using (DarkRiftWriter _writer = DarkRiftWriter.Create())
        {
            _writer.Write(House, 2);

            using (Message _message = Message.Create(2, _writer))
            {
                //Send Message
            }
        }
    }



    // DESERIALIZATION

    // Receives the messages
    public void ReceiveMessage(object sender, MessageReceivedEventArgs e)
    {
        if (e.Tag == Tags.HouseMeasurements)
        {
            ReceiveHouseMeasurements(sender, e);
        }
        else if (e.Tag == Tags.HouseInhabitants)
        {
            ReceiveHouseInhabitants(sender, e);
        }
        else if (e.Tag == Tags.HouseAll)
        {
            ReceiveHouseAll(sender, e);
        }
    }

    // Method that deserializes only the Measurements of the House using the Message tag as tag
    public void ReceiveHouseMeasurements(object sender, MessageReceivedEventArgs e)
    {
        using (Message _message = e.GetMessage())
        {
            using (DarkRiftReader _reader = _message.GetReader())
            {
                _reader.ReadSerializable<House>(e.Tag);
            }
        }
    }

    // Method that deserializes only the Inhabitants of the House using the Tags class as tag
    public void ReceiveHouseInhabitants(object sender, MessageReceivedEventArgs e)
    {
        using (Message _message = e.GetMessage())
        {
            using (DarkRiftReader _reader = _message.GetReader())
            {
                _reader.ReadSerializable<House>(Tags.HouseInhabitants);
            }
        }
    }

    // Method that deserializes all the variables of the House using a number as tag
    public void ReceiveHouseAll(object sender, MessageReceivedEventArgs e)
    {
        using (Message _message = e.GetMessage())
        {
            using (DarkRiftReader _reader = _message.GetReader())
            {
                _reader.ReadSerializable<House>(2);
            }
        }
    }
}


// Class to serialize that derives from SyncObject
public class House : ISync
{
    public float Width;
    public float Height;
    public int Inhabitants;

    // Method that serializes different variables depending on the tag
    public override void SerializeOptional(DarkRiftWriter writer, int tag)
    {
        if (tag == 0)
        {
            writer.Write(Width);
            writer.Write(Height);
        }
        else if (tag == 1)
        {
            writer.Write(Inhabitants);
        }
        else if (tag == 2)
        {
            writer.Write(Width);
            writer.Write(Height);
            writer.Write(Inhabitants);
        }
    }

    // Method that deserializes different variables depending on the tag
    public override void DeserializeOptional(DarkRiftReader reader, int tag)
    {
        if (tag == 0)
        {
            Width = reader.ReadSingle();
            Height = reader.ReadSingle();
        }
        else if (tag == 1)
        {
            Inhabitants = reader.ReadInt32();
        }
        else if (tag == 2)
        {
            Width = reader.ReadSingle();
            Height = reader.ReadSingle();
            Inhabitants = reader.ReadInt32();
        }
    }
}

// Basic Tags class
public static class Tags
{
    public static readonly ushort House = 0;
}
```

## Documentation

**Serialization**
  - `SerializeOptional(DarkRiftWriter, int)` Used to serialize data depending on the `tag` condition.
  - *Extensions* (which call `SerializeOptional(DarkRiftWriter, int)`)
    - `DarkRiftWriter.Write(ISync sync, bool sendTypeID = false)` Serializes `sync` using the default tag (-1) and it has the condition to send the `TypeID`.
    - `DarkRiftWriter.Write(ISync sync, int tag, bool sendTag = false, bool sendTypeID = false)` Serializes `sync` using the `tag`, it has the condition to send the `Tag` and has the condition to send the `TypeID`.
    - `DarkRiftWriter.Write(ISync sync, ExtraSyncData extraSyncData, int tag = -1)` Serializes `sync` using the `tag`, it has the condition **enum** `extraSyncData` where you can choose the extra data to send.

**Deserialization**
 - `DeserializeOptional(DarkRiftReader, int)` Used to deserialize data depending on the `tag` condition.
 - *Extenstions* (which call `DeserializeOptional(DarkRiftReader, int)`)
   - `ISync ReadSerializable()` Deserializes the message into a new instance of a type written in the message (which has to implement ISync) and as tag it uses the one written in the message or -1 if there isn't (default).
   - `T ReadSerializable<T>()` Deserializes the message into a new instance of the type given (which has to implement ISync) and as tag it uses the one written in the message or -1 if there isn't (default).
   - `ISync ReadSerializable(int tag)` Deserializes the message into a new instance of a type written in the message (which has to implement ISync) and as tag it uses the parameter `tag`.
    `T ReadSerializable<T>(int tag)` Deserializes the message into a new instance of a type given (which has to implement ISync) and as tag it uses the parameter `tag`.
    
   - `ReadSerializable(DarkRiftReader reader)` Deserializes the message and updates the given instance (which has to implement ISync) and as tag it uses the one written in the message or -1 if there isn't (default).
   - `ReadSerializable(DarkRiftReader reader, int tag)` Deserializes the message and updates the given instance (which has to implement ISync) and as tag it uses the parameter `tag`.
        
**Extras**
`ISync` has 1 property: `TypeID` which stores the ID of the type of the class. You have to assign one unique number to each class that implements `ISync` so that when you will read a message without knowing the type to cast to, the library will use the `TypeID` written in the message to return the correct **Type**. **NOTE**: if you know the type everytime you receive a message, then you can don't use `TypeID` and choose to not send it to slightly reduce the bandwidth!

## Built With

- .NET Framework 4.7.2
- [DarkRift 2](https://darkriftnetworking.com/DarkRift2)

## License

Apache-2.0

## Authors

- **Penca53**
