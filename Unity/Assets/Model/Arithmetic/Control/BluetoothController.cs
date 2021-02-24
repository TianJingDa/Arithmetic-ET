using System.Collections;
using System.Collections.Generic;

public sealed class BluetoothController : Controller
{
    #region C#单例
    private static BluetoothController instance = null;
    private BluetoothController()
    {
        base.id = ControllerID.BluetoothController;

        MyDebug.LogWhite("Loading Controller:" + id.ToString());
    }
    public static BluetoothController Instance
    {
        get { return instance ?? (instance = new BluetoothController()); }
    }
    #endregion

    public string ServiceUUID { get; set; }
    public string ReadUUID { get; set; }
    public string WriteUUID { get; set; }
    public PeripheralInstance CurPeripheralInstance { get; set; }
    public System.Action<BluetoothMessage> BLESendMessage { get; set; }
    public System.Action<BluetoothMessage> BLEReceiveMessage { get; set; }

    private bool isCentral;

    private void InitBluetoothData()
    {

    }

    private string FullUUID(string uuid)
    {
        return "0000" + uuid + "-0000-1000-8000-00805f9b34fb";
    }

    public bool IsEqualUUID(string uuid1, string uuid2)
    {
        if (uuid1.Length == 4) uuid1 = FullUUID(uuid1);
        if (uuid2.Length == 4) uuid2 = FullUUID(uuid2);

        return (uuid1.ToUpper().CompareTo(uuid2.ToUpper()) == 0);
    }

    public void CentralReceiveMessage(string address, string characteristic, byte[] bytes)
    {
        MyDebug.LogGreen("CentralReceiveMessage");

        BluetoothMessage msg = new BluetoothMessage(bytes);
        if (msg == null)
        {
            MyDebug.LogYellow("CentralReceiveMessage: Message is NULL!");
            return;
        }

        MyDebug.LogGreen("Index:" + msg.index + ", Result:" + msg.result + ", name:" + msg.name);

        if (msg.index == 0)
        {
            UnityEngine.Random.InitState(msg.result);
            GuiController.Instance.CompetitionGUI = GuiFrameID.BluetoothFrame;
            GuiController.Instance.SwitchWrapper(GuiFrameID.BluetoothFightFrame);
        }
        else
        {
            if (BLEReceiveMessage != null) BLEReceiveMessage(msg);
        }
    }

    public void PeripheralReceiveMessage(string UUID, byte[] bytes)
    {
        MyDebug.LogGreen("PeripheralReceiveMessage");
        BluetoothMessage msg = new BluetoothMessage(bytes);
        if (msg == null)
        {
            MyDebug.LogYellow("PeripheralReceiveMessage: Message is NULL!");
            return;
        }

        MyDebug.LogGreen("Index:" + msg.index + ", Result:" + msg.result + ", name:" + msg.name);

        if (msg.index == 0)
        {
            UnityEngine.Random.InitState(msg.result);
            CurPeripheralInstance = new PeripheralInstance("", msg.name);
            SetSendMessageFunc(false);
            BLESendMessage(msg);
            GuiController.Instance.CompetitionGUI = GuiFrameID.BluetoothFrame;
            GuiController.Instance.SwitchWrapper(GuiFrameID.BluetoothFightFrame);
        }
        else
        {
            if (BLEReceiveMessage != null) BLEReceiveMessage(msg);
        }
    }

    public void SetSendMessageFunc(bool isCentral)
    {
        this.isCentral = isCentral;
        if (isCentral) BLESendMessage = CentralSendMessage;
        else BLESendMessage = PeripheralSendMessage;
    }

    public void OnBluetoothFightFinish()
    {
        MyDebug.LogGreen("OnBluetoothFightFinish");
        if (isCentral)
        {
            MyDebug.LogGreen("OnBluetoothFightFinish:Central");
            BluetoothLEHardwareInterface.UnSubscribeCharacteristic(CurPeripheralInstance.address,
                                                                   ServiceUUID,
                                                                   ReadUUID,
                                                                   (characteristic) =>
                                                                   {
                                                                       MyDebug.LogGreen("UnSubscribeCharacteristic Success :" + characteristic);
                                                                   });

            BluetoothLEHardwareInterface.DisconnectPeripheral(CurPeripheralInstance.address,
                (disconnectAddress) =>
                {
                    MyDebug.LogGreen("DisconnectPeripheral Success:" + disconnectAddress);
                });

        }
        else
        {
            MyDebug.LogGreen("OnBluetoothFightFinish:Peripheral");
            BluetoothLEHardwareInterface.StopAdvertising(() =>
            {
                MyDebug.LogGreen("Stop Advertising!");
            });
        }

        BluetoothLEHardwareInterface.RemoveCharacteristics();
        BluetoothLEHardwareInterface.RemoveServices();

        BluetoothLEHardwareInterface.DeInitialize(() =>
        {
            MyDebug.LogGreen("DeInitialize Success!");
        });
        BluetoothLEHardwareInterface.BluetoothEnable(false);
    }

    private void CentralSendMessage(BluetoothMessage message)
    {
        MyDebug.LogGreen("CentralSendMessage");
        MyDebug.LogGreen("index:" + message.index);
        MyDebug.LogGreen("result:" + message.result);
        MyDebug.LogGreen("name:" + message.name);
        MyDebug.LogGreen("Length:" + message.data.Length);
        BluetoothLEHardwareInterface.WriteCharacteristic(CurPeripheralInstance.address, ServiceUUID, WriteUUID, message.data, message.data.Length, true, (characteristicUUID) =>
        {
            BluetoothLEHardwareInterface.Log("Write Succeeded");
        });
    }

    private void PeripheralSendMessage(BluetoothMessage message)
    {
        MyDebug.LogGreen("PeripheralSendMessage");
        MyDebug.LogGreen("index:" + message.index);
        MyDebug.LogGreen("result:" + message.result);
        MyDebug.LogGreen("name:" + message.name);
        MyDebug.LogGreen("Length:" + message.data.Length);
        BluetoothLEHardwareInterface.UpdateCharacteristicValue(ReadUUID, message.data, message.data.Length);
    }


}
