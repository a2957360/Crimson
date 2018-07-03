using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureMeshLayer : MonoBehaviour
{
    public int _X;
    public int _Y;
    public bool _isSet = false;
    TextMesh tMesh;

    private void Awake()
    {
        tMesh = GetComponent<TextMesh>();
    }

    void Start()
    {
        GetComponent<Renderer>().sortingLayerName = "Debug";
        InvokeRepeating("SetTerrainType", 2, 2);
    }

    void SetTerrainType()
    {
        if (_isSet && tMesh != null)
        {
            Cell newCell;
            if (GameManager.Instance._cells.TryGetValue(new Vector2(_X, _Y), out newCell))
            {
                switch (newCell._terrian)
                {
                    case Cell.Terrians.Walkable:
                        {
                            tMesh.text = "W/";
                        }
                        break;
                    case Cell.Terrians.NonAccess:
                        {
                            tMesh.text = "X/";
                        }
                        break;
                    case Cell.Terrians.Forest:
                        {
                            tMesh.text = "S/";
                        }
                        break;
                    case Cell.Terrians.Fort:
                        {
                            tMesh.text = "D/";
                        }
                        break;
                    case Cell.Terrians.AttackingSpot:
                        {
                            tMesh.text = "A/";
                        }
                        break;
                    case Cell.Terrians.Danger:
                        {
                            tMesh.text = "Q/";
                        }
                        break;

                }

                string crystal = "";
                switch (newCell._crystal_spawn)
                {
                    case Cell.Crystal_Spawns.None:
                        {
                            crystal = "(n)";
                        }
                        break;
                    case Cell.Crystal_Spawns.Small:
                        {
                            crystal = "(s)";
                        }
                        break;
                    case Cell.Crystal_Spawns.Med:
                        {
                            crystal = "(m)";
                        }
                        break;
                    case Cell.Crystal_Spawns.Large:
                        {
                            crystal = "(l)";
                        }
                        break;
                }
                //tMesh.text = newCell._cost.ToString() + "/" + newCell._playerWeight.ToString() + "/" + newCell._enemyWeight.ToString() + "/" + newCell._ZoD;
                tMesh.text += newCell._cost.ToString() + "/" + (newCell.isPlayer ? "T" : "F") + "\nZ:" + newCell._ZoD + "\nC" + crystal + ":" + newCell.CrystalScoreGet();
                //tMesh.text += newCell._cost.ToString() + "/" + newCell._ZoD + "/" + newCell._crimsonScore;
            }

        }
    }

}
