using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class LevelProperty
{
    public int Stt;
    public int KeyLock;
    public int Star;
}
public class Levemapmanager
{
    public List<LevelProperty> LevelMaps = new List<LevelProperty>();

    public class RootObject
    {
        public List<LevelProperty> LevelMaps { get; set; }

    }
}



