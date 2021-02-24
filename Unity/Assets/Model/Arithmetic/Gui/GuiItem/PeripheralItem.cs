using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class PeripheralItem : Item, IPointerClickHandler
{
	private const float connectTime = 10f;
    private bool receiveReadID;
    private bool receiveWriteID;

	private PeripheralInstance content;
	private GameObject detailWin;
	private GameObject bluetoothConnectWaiting;
	private Text periphralName;
	private Text periphralAddress;
	private Text bluetoothConnectTime;

	protected override void InitPrefabItem(object data)
	{
		content = data as PeripheralInstance;
		if (content == null)
		{
			MyDebug.LogYellow("BluetoothInstance is null!!");
			return;
		}

        Init();
        receiveReadID = false;
        receiveWriteID = false;
        periphralName.text = content.name;
		periphralAddress.text = content.address;
	}

	protected override void OnStart(Dictionary<string, GameObject> gameObjectDict)
	{
		periphralName = gameObjectDict["PeriphralName"].GetComponent<Text>();
		periphralAddress = gameObjectDict["PeriphralAddress"].GetComponent<Text>();
	}

	protected override void InitDetailWin(GameObject detailWin)
	{
		this.detailWin = detailWin;
	}
		
	public void OnPointerClick(PointerEventData eventData)
	{
        if (detailWin)
		{
			detailWin.SetActive(true);
			Dictionary<string, GameObject> detailWinDict = CommonTool.InitGameObjectDict(detailWin);
			bluetoothConnectWaiting = detailWinDict["BluetoothConnectWaiting"];
			GameObject connectConfirmBtn = detailWinDict["ConnectConfirmBtn"];
			bluetoothConnectTime = detailWinDict["BluetoothConnectTime"].GetComponent<Text>();
			Text bluetoothPeripheralDetailTitle_Text = detailWinDict["BluetoothPeripheralDetailTitle_Text"].GetComponent<Text>();

			bluetoothConnectWaiting.SetActive(false);
			string tip = LanguageController.Instance.GetLanguage(bluetoothPeripheralDetailTitle_Text.GetIndex());
            bluetoothPeripheralDetailTitle_Text.text = string.Format(tip, content.name);
			CommonTool.AddEventTriggerListener(connectConfirmBtn, EventTriggerType.PointerClick, ConnectToPeripheral);
		}
	}

	private void ConnectToPeripheral(BaseEventData evenData)
	{
        MyDebug.LogGreen("ConnectToPeripheral");
        BluetoothLEHardwareInterface.StopScan();
        BluetoothController.Instance.CurPeripheralInstance = content;
		bluetoothConnectWaiting.SetActive(true);
		StartCoroutine(ConnectCountDown());
        BluetoothLEHardwareInterface.ConnectToPeripheral (BluetoothController.Instance.CurPeripheralInstance.address, null, null,
			(address, serviceUUID, characteristicUUID) => 
				{
                    MyDebug.LogGreen("_Address:" + address);
                    MyDebug.LogGreen("_ServiceUUID:" + serviceUUID);
                    MyDebug.LogGreen("_CharacteristicUUID:" + characteristicUUID);

                    if (BluetoothController.Instance.IsEqualUUID(serviceUUID, BluetoothController.Instance.ServiceUUID))
					{
                        MyDebug.LogGreen("Address:" + address);
                        MyDebug.LogGreen("ServiceUUID:" + serviceUUID);

                        if (BluetoothController.Instance.IsEqualUUID(characteristicUUID, BluetoothController.Instance.ReadUUID))
                        {
                            MyDebug.LogGreen("CharacteristicUUID:" + characteristicUUID);
                            receiveReadID = true;
                            if (receiveWriteID)
                            {
                                receiveReadID = false;
                                receiveWriteID = false;
                                StartCoroutine(SubscribeCharacteristic());
                            }
                        }
                        else if (BluetoothController.Instance.IsEqualUUID(characteristicUUID, BluetoothController.Instance.WriteUUID))
                        {
                            MyDebug.LogGreen("CharacteristicUUID:" + characteristicUUID);
                            receiveWriteID = true;
                            if (receiveReadID)
                            {
                                receiveReadID = false;
                                receiveWriteID = false;
                                StartCoroutine(SubscribeCharacteristic());
                            }
                        }
					}
				}, 
			(address) =>
				{
                    // this will get called when the device disconnects
                    // be aware that this will also get called when the disconnect
                    // is called above. both methods get call for the same action
                    // this is for backwards compatibility
                    if(GuiController.Instance.CurGUI == GuiFrameID.BluetoothFrame || GuiController.Instance.CurGUI == GuiFrameID.BluetoothFightFrame)
                    {
                        MyDebug.LogWhite("Peripheral Disconnect!");
                        string tip = LanguageController.Instance.GetLanguage("Text_80019");
                        GuiController.Instance.CurCommonTipInstance = new CommonTipInstance(CommonTipID.Single, tip);
                        GuiController.Instance.SwitchWrapper(GuiFrameID.CommonTipFrame, true);
                        if(GuiController.Instance.CurGUI == GuiFrameID.BluetoothFrame)
                        {
                            StopAllCoroutines();
                            detailWin.SetActive(false);
                        }
                    }
                });
	}

    private IEnumerator SubscribeCharacteristic()
    {
        MyDebug.LogGreen("Subscribe Characteristic!");
        yield return new WaitForSeconds(1f);
        BluetoothLEHardwareInterface.SubscribeCharacteristicWithDeviceAddress(BluetoothController.Instance.CurPeripheralInstance.address,
                                                                       BluetoothController.Instance.ServiceUUID,
                                                                       BluetoothController.Instance.ReadUUID, NotificationAction,
                                                                       BluetoothController.Instance.CentralReceiveMessage);
    }

    private void NotificationAction(string address, string characteristicUUID)
    {
        MyDebug.LogGreen("Subscribe NotificationAction!");
        MyDebug.LogGreen("Address:" + address);
        MyDebug.LogGreen("CharacteristicUUID:" + characteristicUUID);
        StartCoroutine(FirstWrite());
    }

    private IEnumerator FirstWrite()
    {
        MyDebug.LogGreen("First Write!");
        yield return new WaitForSeconds(1f);
        int seed = UnityEngine.Random.Range(1, int.MaxValue);
        BluetoothMessage message = new BluetoothMessage(0, seed, GameManager.Instance.UserName);
        BluetoothController.Instance.SetSendMessageFunc(true);
        BluetoothController.Instance.BLESendMessage(message);
    }

    private IEnumerator ConnectCountDown()
	{
		float time = connectTime;
		while(time > 0)
		{
			time -= Time.deltaTime;
			bluetoothConnectTime.text = Mathf.CeilToInt(time).ToString();
			yield return null;
		}
        detailWin.SetActive(false);
    }
}

[Serializable]
public class PeripheralInstance
{
	public string address;
	public string name;

    public PeripheralInstance() { }

    public PeripheralInstance(string address, string name)
	{
		this.address = address;
		this.name = name;
	}
}