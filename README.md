# DarkRiftLibrary-MLAPI

This library is focused on simplifing similar repetitive tasks that you have to deal with writing and reading messages from server to client and vice versa.

## Getting Started

In this section you will learn how to setup and use the library.

### Installing

To download it you can use [NuGet](https://www.nuget.org/packages/DarkRiftLibrary-MLAPI_Penca53/) or [Mediafire](http://www.mediafire.com/folder/9ch3qbqt30v4s/DarkRiftLibrary-MLAPI). Note: if you use Mediafire, you have to reference the .dll file in your project (If you're creating a C# app then you have to go in `Solution Explorer`/ Right click on `Reference`/`Add Reference`/`Browse` and search for it. If you're using Unity, then you have to create a folder called Plugins and move the .dll file to that Untiy folder.

### How to use

This library is simple and immediate... It provides you a new class called SyncObject that you can derive from as parent class in your classes that need to be synced. Once you derive from it, you can override two SyncObject's methods: `SerializeOptional` and `DeserializeOptional`. As the names suggest, the first one, `SerializeOptional`, serializes the variables you want to as you were already doing with `IDarkRiftSerilizable` `Serialize` and `Deserialize`. These methods, though, needs two different parameters: the `DarkRiftWriter` to write into and the `tag` you want. The `tag` is a way to recognize different situations that happens in your program: E.g. You have a `House` class that derives from `SyncObject` and when the `tag` is `1` then you want to serialize the `width` and `height` of the `House`. If it's `2` then you want to serialize the `inhabitants` of the `House`. Here there is a little example that shows all the functionalities of the library.

```cs
//Instance of the class to serialize
public House House = new House();

//Method that serializes only the Measurements of the House using the Tag class as tag
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

//Method that serializes only the Inhabitants of the House using the Tag class as tag
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

//Method that serializes all the variables of the House using a number as tag
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
}


//Class to serialize that derives from SyncObject
public class House : SyncObject
{
public float Width;
public float Height;
public int Inhabitants;

public override void SerializeOptional(DarkRiftWriter writer, int tag)
{
    //Depending on the tag, serialize something different
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
}

//Basic Tags class
public static class Tags
{
public static readonly ushort HouseMeasurements = 0;
public static readonly ushort HouseInhabitants = 1;
public static readonly ushort HouseAll = 2;
}
```

## Running the tests

How things work goes here.

## Built With

- .NET Standard 2.0
- [DarkRift 2](https://darkriftnetworking.com/DarkRift2)

## License

Apache-2.0.

## Authors

- **Penca53**
