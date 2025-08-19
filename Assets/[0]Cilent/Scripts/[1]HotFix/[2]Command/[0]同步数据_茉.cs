using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using Cysharp.Threading.Tasks;
using LitJson;
// using 
public class 同数令_茉 : AbstractCommand
{
    public List<I记贞> 数据;
    string 录名;
    public 同数令_茉(List<I记贞> 数据, string 录名)
    {
        this.数据 = 数据;
        this.录名 = 录名;
    }
    protected override void OnExecute()
    {
#if UNITY_EDITOR ||UNITY_STANDALONE_WIN

#elif UNITY_ANDROID
    if (!BleManager.isConnected)
        {
            Debug.Log("[0]同步数据_茉 未连接蓝牙");
            return;
        }
        string bleAddress = PlayerPrefs.GetString("BLEAddress");
        string bleServiceUUID = PlayerPrefs.GetString("BLEServiceUUID");
        string bleCharacteristicUUID = PlayerPrefs.GetString("BLECharacteristicUUID");
        string jsonData = JsonMapper.ToJson(数据);
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(jsonData);
        sbyte[] sBytes = new sbyte[bytes.Length];
        for(int i = 0; i < bytes.Length; i++)
        {
            sBytes[i] = (sbyte)bytes[i];
        }
        BleManager.SendData(bleAddress, bleServiceUUID, bleCharacteristicUUID, sBytes);
#endif
    

    }
}
