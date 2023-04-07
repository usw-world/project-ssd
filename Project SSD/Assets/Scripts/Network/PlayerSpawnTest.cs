using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerSpawnTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public struct CreateMMOCharacterMessage : NetworkMessage
{
    public Race race;
    public string name;
    public Color hairColor;
    public Color eyeColor;
}

public enum Race
{
    None,
    Elvish,
    Dwarvish,
    Human
}