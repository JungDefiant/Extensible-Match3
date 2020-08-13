using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "New Monster Type", menuName = "Monster Type")]
public class MonsterType : ScriptableObject
{
    public Sprite Sprite;
    public int MatchID;
}
