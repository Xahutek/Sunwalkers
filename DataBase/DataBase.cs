using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataBase : MonoBehaviour
{
    public static EntityDataBase Entities;
    public static KeywordDataBase Keywords;

    public EntityDataBase EntityData;
    public KeywordDataBase KeywordData;

    private void Awake()
    {
        Entities = EntityData;
        Keywords = KeywordData;
    }
}
