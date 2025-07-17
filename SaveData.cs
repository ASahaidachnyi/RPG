using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public string sceneName;
    public float[] playerPosition;
    public float currentHealth;
    public float currentMana;
    public float strength;
    public float agility;
    public float intelligence;
    public float cameraYaw;
    public float cameraPitch;
    // Інвентар, зброя тощо можна додати пізніше
}
