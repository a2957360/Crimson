using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

[XmlType("Level")]
public class Level
{
    private int _id;
    public int id
    {
        get { return _id; }
    }
    public string name { get; set; }
    public int width { get; set; }
    public int height { get; set; }

    [XmlArray("Heroes")]
    [XmlArrayItem("Unit")]
    public List<Unit> heroPositions { get; set; }

    [XmlArray("Enemies")]
    [XmlArrayItem("Unit")]
    public List<Unit> enemyPositions { get; set; }

    [XmlArray("Tiles")]
    [XmlArrayItem("Tile")]
    public List<Tile> Stiles { get; set; }

    [XmlIgnore]
    public Tile[,] tiles { get; set; }

    public Level()
    {
        _id = 0;
        name = "new Name";
        width = 10;
        height = 10;

        heroPositions = new List<Unit>();
        enemyPositions = new List<Unit>();
        Stiles = new List<Tile>();
        tiles = new Tile[width, height];


        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                tiles[i, j] = new Tile(i, j, 0, 0);
            }
        }
    }

    public Level(int newId, string newName, int _width, int _height)
    {
        _id = newId;
        name = newName;
        width = _width;
        height = _height;

        heroPositions = new List<Unit>();
        enemyPositions = new List<Unit>();
        Stiles = new List<Tile>();
        tiles = new Tile[width, height];


        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                tiles[i, j] = new Tile(i, j, 0, 0);
            }
        }
    }

    public void serializeTile()
    {
        Stiles.Clear();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Stiles.Add(tiles[i, j]);
            }
        }
    }

    public void deSerializeTile()
    {

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                tiles[i, j] = Stiles[i+j];
            }
        }
    }


    //public void STileToTile()
    //{
    //    for (int i = 0; i < Stiles.Count; i++)
    //    {
    //        tiles[Stiles[i]._x, Stiles[i]._y] = Stiles[i];
    //    }
    //}
}
