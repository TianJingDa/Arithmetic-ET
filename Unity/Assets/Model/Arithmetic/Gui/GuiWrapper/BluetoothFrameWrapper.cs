using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BluetoothFrameWrapper : GuiFrameWrapper
{
	private const float 				advertisingTime = 30;
    private int                         delta;
    private bool   						isCentral;
    private bool                        scaning;
    private Dictionary<string, string>  peripheralDict;
    private List<Dropdown.OptionData>   digitDropdownOptionsList;

    private PatternID   curPatternID;
	private AmountID    curAmountID;
	private SymbolID    curSymbolID;
	private DigitID     curDigitID;
	private OperandID   curOperandID;

    private GameObject  bluetoothCategoryContent;
	private GameObject  bluetoothScanResultContent;
	private GameObject  bluetoothPeripheralDetailBg;
	private GameObject  bluetoothAdvertisingStopBtn;
	private GameObject  bluetoothConnectWaiting;
    private GameObject  bluetoothReScanBtn;
    private GameObject  bluetoothAndroidTip;
    private GameObject  bluetoothPeripheralBtn;
    private Transform   bluetoothScrollContent;
    private Text  		bluetoothConnectTime;
    private Dropdown    digitDropdown;



    void Start ()
    {
        BluetoothLEHardwareInterface.BluetoothEnable(true);
        id = GuiFrameID.BluetoothFrame;
        Init();
        delta = 0;
        scaning = false;
        curPatternID = PatternID.Number;
		curOperandID = OperandID.TwoNumbers;
		peripheralDict = new Dictionary<string, string> ();
        digitDropdownOptionsList = new List<Dropdown.OptionData>(digitDropdown.options);

#if UNITY_ANDROID
        bluetoothPeripheralBtn.SetActive(false);
        bluetoothAndroidTip.SetActive(true);
#elif UNITY_IOS
        bluetoothPeripheralBtn.SetActive(true);
        bluetoothAndroidTip.SetActive(false);
#endif
    }

    protected override void OnStart(Dictionary<string, GameObject> gameObjectDict)
    {
		bluetoothCategoryContent    	= gameObjectDict["BluetoothCategoryContent"];
		bluetoothScanResultContent    	= gameObjectDict["BluetoothScanResultContent"];
		bluetoothPeripheralDetailBg     = gameObjectDict["BluetoothPeripheralDetailBg"];
		bluetoothConnectWaiting 		= gameObjectDict["BluetoothConnectWaiting"];
        bluetoothAdvertisingStopBtn     = gameObjectDict["BluetoothAdvertisingStopBtn"];
        bluetoothReScanBtn              = gameObjectDict["BluetoothReScanBtn"];
        bluetoothAndroidTip             = gameObjectDict["BluetoothAndroidTip"];
        bluetoothPeripheralBtn          = gameObjectDict["BluetoothPeripheralBtn"];
        bluetoothConnectTime            = gameObjectDict["BluetoothConnectTime"].GetComponent<Text>();
        bluetoothScrollContent          = gameObjectDict["BluetoothScrollContent"].GetComponent<Transform>();
        digitDropdown                   = gameObjectDict["DigitDropdown"].GetComponent<Dropdown>();
    }

    protected override void OnButtonClick(Button btn)
    {
        base.OnButtonClick(btn);

        switch (btn.name)
        {
            case "Bluetooth2StartFrameBtn":
                GuiController.Instance.SwitchWrapperWithScale(GuiFrameID.StartFrame, false);
                break;
			case "BluetoothCentralBtn":
				InitializeBluetooth (true);
				break;
			case "BluetoothPeripheralBtn":
				InitializeBluetooth (false);	
                break;
			case "BackFromContentBtn":
                scaning = false;
                BluetoothLEHardwareInterface.RemoveCharacteristics();
                BluetoothLEHardwareInterface.RemoveServices();
                BluetoothLEHardwareInterface.DeInitialize(() =>
                {
                    MyDebug.LogGreen("DeInitialize Success!");
                });
                CommonTool.GuiHorizontalMove(bluetoothCategoryContent, Screen.width, MoveID.RightOrUp, canvasGroup, false);
                break;
			case "BackFromScanResultBtn":
			case "BluetoothAdvertisingStopBtn":
                bluetoothPeripheralDetailBg.SetActive(false);
                bluetoothConnectWaiting.SetActive(false);
                bluetoothAdvertisingStopBtn.SetActive(false);
                StopScan();
				break;
			case "BluetoothScanBtn":
				StartScan();
				break;
			case "ConnectCancelBtn":
				bluetoothPeripheralDetailBg.SetActive(false);
				break;
			case "BluetoothReScanBtn":
				ReScan();
				break;
            default:
                MyDebug.LogYellow("Can not find Button: " + btn.name);
                break;
        }
    }

	protected override void OnDropdownClick(Dropdown dpd)
	{
		base.OnDropdownClick(dpd);
		switch(dpd.name)
		{
			case "PatternDropdown":
			case "OperandDropdown":
				break;
			case "AmountDropdown":
				curAmountID = (AmountID)dpd.value;
				break;
			case "SymbolDropdown":
				curSymbolID = (SymbolID)dpd.value;
                RefreshDigitDropdown(dpd.value);
                break;
			case "DigitDropdown":
				curDigitID = (DigitID)(dpd.value + delta);
                break;
			default:
				MyDebug.LogYellow("Can not find Dropdown: " + dpd.name);
				break;
		}
	}

    private void RefreshDigitDropdown(int index)
    {
        switch (index)
        {
            case 0:
            case 1:
            case 2:
                digitDropdown.options = digitDropdownOptionsList.GetRange(0, 2);
                delta = 0;
                break;
            case 3:
                digitDropdown.options = digitDropdownOptionsList.GetRange(1, 2);
                delta = 1;
                break;
        }
        digitDropdown.value = 0;
        digitDropdown.RefreshShownValue();
        OnDropdownClick(digitDropdown);
    }


    /// <summary>
    /// 刷新Dropdown的状态
    /// </summary>
    private void RefreshCategoryContent()
	{
		Dropdown[] dropdownArray = GetComponentsInChildren<Dropdown>(true);
		for(int i = 0; i < dropdownArray.Length; i++)
		{
			for (int j = 0; j < dropdownArray[i].options.Count; j++)
			{
				dropdownArray[i].options[j].text = LanguageController.Instance.GetLanguage(dropdownArray[i].options[j].text);
			}
            dropdownArray[i].value = 0;
            dropdownArray[i].RefreshShownValue();
        }
        RefreshDigitDropdown(0);
        curAmountID = 0;
        curSymbolID = 0;
        curDigitID = 0;
    }

	private void ReScan()
	{
		if(isCentral)
		{
			BluetoothLEHardwareInterface.StopScan();
			RemovePeripherals();
			MyDebug.LogGreen("Start ReScaning!");
			BluetoothLEHardwareInterface.ScanForPeripheralsWithServices (new string[]{ BluetoothController.Instance.ServiceUUID}, 
				(address, name) => 
				{
					AddPeripheral (address, name);
				});
		}
	}

	private void RefreshScanResultContent()
	{
		RemovePeripherals ();
        bluetoothReScanBtn.SetActive(isCentral);
        bluetoothPeripheralDetailBg.SetActive(!isCentral);
		bluetoothConnectWaiting.SetActive(!isCentral);
		bluetoothAdvertisingStopBtn.SetActive(!isCentral);
	}

	private void AddPeripheral(string address, string name)
	{
		if(!peripheralDict.ContainsKey(address))
		{
			GameObject peripheral = GuiController.Instance.GetPrefabItem(GuiItemID.PeripheralItem);
			peripheral.name = "BluetoothItem" + peripheralDict.Count;
			peripheral.SendMessage("InitPrefabItem", new PeripheralInstance(address, name));
            peripheral.SendMessage("InitDetailWin", bluetoothPeripheralDetailBg);
			peripheral.transform.SetParent(bluetoothScrollContent);
			peripheral.transform.localScale = Vector3.one;

			peripheralDict[address] = name;
		}
	}

	private void RemovePeripherals()
	{
		for(int i = 0; i < bluetoothScrollContent.childCount; i++)
		{
			GameObject gameObject = bluetoothScrollContent.GetChild(i).gameObject;
			Destroy(gameObject);
		}

        if (peripheralDict != null) peripheralDict.Clear();
	}

	private void StartScan()
	{
        if (scaning)
        {
            MyDebug.LogYellow("Scaning!!!");
            return;
        }
        scaning = true;

        BluetoothLEHardwareInterface.RemoveCharacteristics();
        BluetoothLEHardwareInterface.RemoveServices();

        CategoryInstance curCategoryInstance = new CategoryInstance(curPatternID, curAmountID, curSymbolID, curDigitID, curOperandID);
		FightController.Instance.CurCategoryInstance = curCategoryInstance;

		string serviceUUID = (int)curAmountID + "" + (int)curSymbolID + "" + (int)curDigitID + "0";
        string readUUID    = (int)curAmountID + "" + (int)curSymbolID + "" + (int)curDigitID + "1";
        string writeUUID = (int)curAmountID + "" + (int)curSymbolID + "" + (int)curDigitID + "2";

        BluetoothController.Instance.ServiceUUID = BluetoothLEHardwareInterface.FullUUID(serviceUUID);
        BluetoothController.Instance.ReadUUID = BluetoothLEHardwareInterface.FullUUID(readUUID);
        BluetoothController.Instance.WriteUUID = BluetoothLEHardwareInterface.FullUUID(writeUUID);

        MyDebug.LogGreen("ServiceUUID:" + BluetoothController.Instance.ServiceUUID);
        MyDebug.LogGreen("ReadUUID:" + BluetoothController.Instance.ReadUUID);
        MyDebug.LogGreen("WriteUUID:" + BluetoothController.Instance.WriteUUID);

		if (isCentral) 
		{
			MyDebug.LogGreen("Central Start Scaning!");
			bluetoothScanResultContent.SetActive (true);
			RefreshScanResultContent ();
			CommonTool.GuiHorizontalMove (bluetoothScanResultContent, Screen.width, MoveID.RightOrUp, canvasGroup, true);
			BluetoothLEHardwareInterface.ScanForPeripheralsWithServices (new string[]{ BluetoothController.Instance.ServiceUUID}, 
				(address, name) => 
				{
					AddPeripheral (address, name);
				});
		} 
		else 
		{
            MyDebug.LogGreen("Peripheral Start Scaning!");
            BluetoothLEHardwareInterface.PeripheralName(PlayerController.Instance.PlayerName);
            MyDebug.LogGreen("PeripheralName:" + PlayerController.Instance.PlayerName);

            BluetoothLEHardwareInterface.CreateCharacteristic(BluetoothController.Instance.ReadUUID, 
				BluetoothLEHardwareInterface.CBCharacteristicProperties.CBCharacteristicPropertyRead |
				BluetoothLEHardwareInterface.CBCharacteristicProperties.CBCharacteristicPropertyNotify,
				BluetoothLEHardwareInterface.CBAttributePermissions.CBAttributePermissionsReadable, null, 0, null);
            MyDebug.LogGreen("CreateCharacteristic:Read!");

            BluetoothLEHardwareInterface.CreateCharacteristic(BluetoothController.Instance.WriteUUID, 
				BluetoothLEHardwareInterface.CBCharacteristicProperties.CBCharacteristicPropertyWrite,
				BluetoothLEHardwareInterface.CBAttributePermissions.CBAttributePermissionsWriteable, null, 0, 
				BluetoothController.Instance.PeripheralReceiveMessage);
            MyDebug.LogGreen("CreateCharacteristic:Write!");

            BluetoothLEHardwareInterface.CreateService(BluetoothController.Instance.ServiceUUID, true, (message)=>
				{
                    MyDebug.LogGreen("Create Service Success:" + message);
				});
            MyDebug.LogGreen("CreateService!");

            BluetoothLEHardwareInterface.StartAdvertising(() =>
            {
                MyDebug.LogGreen("Start Advertising!");
                bluetoothScanResultContent.SetActive(true);
                RefreshScanResultContent();
                StartCoroutine(AdvertisingCountDown());
                CommonTool.GuiHorizontalMove(bluetoothScanResultContent, Screen.width, MoveID.RightOrUp, canvasGroup, true);
            });

        }
    }

	private IEnumerator AdvertisingCountDown()
	{
		float time = advertisingTime;
		while(time > 0)
		{
			time -= Time.deltaTime;
			bluetoothConnectTime.text = Mathf.CeilToInt(time).ToString();
			yield return null;
		}
        bluetoothPeripheralDetailBg.SetActive(false);
        bluetoothConnectWaiting.SetActive(false);
        bluetoothAdvertisingStopBtn.SetActive(false);

        StopScan();
	}

	private void StopScan()
	{
        scaning = false;
		if (isCentral)
		{
			BluetoothLEHardwareInterface.StopScan ();
			CommonTool.GuiHorizontalMove(bluetoothScanResultContent, Screen.width, MoveID.RightOrUp, canvasGroup, false);
		} 
		else
		{
			StopAllCoroutines();
			BluetoothLEHardwareInterface.StopAdvertising (() => 
				{
					CommonTool.GuiHorizontalMove(bluetoothScanResultContent, Screen.width, MoveID.RightOrUp, canvasGroup, false);
				});
		}
	}
		
	private void InitializeBluetooth(bool isCentral)
    {
        this.isCentral = isCentral;
        BluetoothLEHardwareInterface.Initialize(isCentral, !isCentral,
            () =>
            {
                MyDebug.LogGreen("Initialize Success!");
                BluetoothLEHardwareInterface.RemoveCharacteristics();
                BluetoothLEHardwareInterface.RemoveServices();
                bluetoothCategoryContent.SetActive(true);
                RefreshCategoryContent();
                CommonTool.GuiHorizontalMove(bluetoothCategoryContent, Screen.width, MoveID.RightOrUp, canvasGroup, true);
            },
            (error) =>
            {
                string message = "";
                if (error.Contains("Not Supported"))
                {
                    MyDebug.LogYellow("Not Supported");
                    message = LanguageController.Instance.GetLanguage("Text_80009");
                }
                else if (error.Contains("Not Authorized"))
                {
                    MyDebug.LogYellow("Not Authorized");
                    message = LanguageController.Instance.GetLanguage("Text_80010");
                }
                else if (error.Contains("Powered Off"))
                {
                    MyDebug.LogYellow("Powered Off");
                    message = LanguageController.Instance.GetLanguage("Text_80011");
                }
                else if (error.Contains("Not Enabled"))
                {
                    MyDebug.LogYellow("Not Enabled");
                    message = LanguageController.Instance.GetLanguage("Text_80012");
                }
                else
                {
                    MyDebug.LogYellow("Central Initialize Fail: " + error);
                    message = LanguageController.Instance.GetLanguage("Text_80021"); 
                }
                GuiController.Instance.CurCommonTipInstance.SetInstance(CommonTipID.Single, message);
                GuiController.Instance.SwitchWrapper(GuiFrameID.CommonTipFrame, true);
            });

    }
}
