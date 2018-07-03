using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System;
using System.IO;

[XmlRoot("Game")]
[XmlInclude(typeof(Level))]
public class Game
{
    public int maxLv { get; set; }

    [XmlArray("Levels")]
    [XmlArrayItem("Level")]
    public List<Level> levels { get; set; }

    public Game()
    {
        maxLv = 5;
        levels = new List<Level>();
    }

    public void WriteXML(string file, Game _game)
    {
        for (int i = 0; i < levels.Count; i++)
        {
            levels[i].serializeTile();
        }

        //Type[] extra = { typeof(Level), typeof(Unit), typeof(Tile), typeof(Position) };

        //XmlSerializer x = new XmlSerializer(typeof(Game), extra);

        XmlSerializer x = new XmlSerializer(typeof(Game));

        TextWriter writer = new StreamWriter(file);
        x.Serialize(writer, _game);
        writer.Close();
    }

    public Game LoadXML(string file)
    {
        FileStream fs = new FileStream(file, FileMode.OpenOrCreate);
        if (fs != null)
        {
            TextReader reader = new StreamReader(fs);
            Type[] extra = { typeof(Level), typeof(Unit), typeof(Tile) };
            XmlSerializer x = new XmlSerializer(typeof(Game), extra);
            //XmlSerializer x = new XmlSerializer(typeof(Game));

            Game _game = (Game)x.Deserialize(reader);
            //for (int i = 0; i < _game.levels.Count; i++)
            //{
            //    _game.levels[i].deSerializeTile();
            //}
            //object j = x.Deserialize(reader);
            reader.Close();
            //Game _game = new Game();
            return _game;
        }
        return null;
    }
}
