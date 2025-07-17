using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EncryptedSaveLoadManager : MonoBehaviour
{
    public static EncryptedSaveLoadManager Instance;

    private string saveFilePath;

    [Header("Encryption")]
    [SerializeField] private string encryptionKey = "mySuperSecretKey"; // 16/24/32 Symbols
    private readonly string iv = "1234567890123456"; // AES requires a 16-byte IV

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        // Guarantee the key is exactly 16 ASCII characters
        encryptionKey = "mySecretKey123456";
        saveFilePath = Path.Combine(Application.persistentDataPath, "encrypted_save.dat");
    }

    public void Save(PlayerStats stats, Vector3 position, float yaw, float pitch)
    {
        SaveData data = new SaveData
        {
            sceneName = SceneManager.GetActiveScene().name,
            playerPosition = new float[] { position.x, position.y, position.z },
            currentHealth = stats.CurrentHealth,
            currentMana = stats.CurrentMana,
            strength = stats.strength,
            agility = stats.agility,
            intelligence = stats.intelligence,
            cameraYaw = yaw,
            cameraPitch = pitch
        };

        string json = JsonUtility.ToJson(data);
        string encrypted = Encrypt(json);
        File.WriteAllText(saveFilePath, encrypted);
        Debug.Log($"Encrypted save created at: {saveFilePath}");
    }

    public SaveData Load()
    {
        if (!File.Exists(saveFilePath)) return null;
        string encrypted = File.ReadAllText(saveFilePath);
        string json = Decrypt(encrypted);
        return JsonUtility.FromJson<SaveData>(json);
    }

    public void DeleteSave()
    {
        if (File.Exists(saveFilePath)) File.Delete(saveFilePath);
    }

    private string Encrypt(string plainText)
    {
        using Aes aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(encryptionKey);
        aes.IV = Encoding.UTF8.GetBytes(iv);

        ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using MemoryStream ms = new();
        using CryptoStream cs = new(ms, encryptor, CryptoStreamMode.Write);
        using StreamWriter sw = new(cs);
        sw.Write(plainText);
        return Convert.ToBase64String(ms.ToArray());
    }

    private string Decrypt(string cipherText)
    {
        byte[] buffer = Convert.FromBase64String(cipherText);
        using Aes aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(encryptionKey);
        aes.IV = Encoding.UTF8.GetBytes(iv);

        ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using MemoryStream ms = new(buffer);
        using CryptoStream cs = new(ms, decryptor, CryptoStreamMode.Read);
        using StreamReader sr = new(cs);
        return sr.ReadToEnd();
    }
}
