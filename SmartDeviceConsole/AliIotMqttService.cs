using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace SmartDeviceConsole
{
    interface IAliIotMqttServiceEvent
    {
        void MessageReceived(string msg);
        void ConnectionClosed();
    }


    class AliIotMqttService
    {
        public readonly static string aliIotAddress = "iot-as-mqtt.cn-shanghai.aliyuncs.com";
        private MqttClient mqttClient = null;
        private IAliIotMqttServiceEvent eventHandler = null;

        public IAliIotMqttServiceEvent EventHandler
        {
            set { eventHandler = value; }
            get { return eventHandler; }
        }
        
        public void mqttConnect(string clientId ,string productKey , AliIotDeviceInfo deviceInfo)
        {
            string brokerAddress = productKey + "." + aliIotAddress;
            try
            {
                mqttClient = new MqttClient(brokerAddress);

                mqttClient.MqttMsgPublishReceived += mqttMsgReceived;
                mqttClient.ConnectionClosed += mqttConnectionClosed;

                string mqttClientId = clientId + "|securemode=3,signmethod=hmacsha1|";
                string mqttUsername = deviceInfo.NAME + "&" + productKey;

                Dictionary<string, string> signParams = new Dictionary<string, string>();
                signParams.Add("productKey", productKey);
                signParams.Add("deviceName", deviceInfo.NAME);
                signParams.Add("clientId", clientId);

                string mqttPassword = SignUtil.sign(signParams, deviceInfo.SECRET);
                mqttClient.Connect(mqttClientId, mqttUsername, mqttPassword);

                string subTopic = "/" + productKey + "/" + deviceInfo.NAME + "/user/get";

                mqttClient.Subscribe(new string[] { subTopic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }


        public void mqttDisconnect()
        {
            if (mqttClient == null)
                return;

            mqttClient.Disconnect();
        }

        public void mqttMsgReceived(Object sender, MqttMsgPublishEventArgs e)
        {
            string msgReceived = System.Text.Encoding.UTF8.GetString(e.Message);        
            if(eventHandler != null)
            {
                eventHandler.MessageReceived(msgReceived);
            }
        }

        public void mqttConnectionClosed(object sender, EventArgs e)
        {
            if(eventHandler != null)
            {
                eventHandler.ConnectionClosed();
            }
        }
    }



    class AliIotDeviceInfo
    {
        private string id;
        private string name;
        private string secret;
        private string status;

        public string ID
        {
            get { return id; }
            set { id = value; }
        }

        public string NAME
        {
            get { return name; }
            set { name = value; }
        }


        public string SECRET
        {
            get { return secret; }
            set { secret = value; }
        }

        public string STATUS
        {
            get { return status; }
            set { status = value; }
        }

    }

    class CableDevice
    {
        //通信单元Id

        private string deviceId;

        //通信单元别名
        private string aliasName;

        //备注
        private string comment;

        //IoT平台相关信息
        AliIotDeviceInfo iotDeviceInfo;

        public string DeviceId
        {
            set { deviceId = value; }
            get { return deviceId; }
        }

        public string AliasName
        {
            set { aliasName = value; }
            get { return aliasName; }
        }

        public string Comment
        {
            set { comment = value; }
            get { return comment; }
        }

        public AliIotDeviceInfo IotDeviceInfo
        {
            set { iotDeviceInfo = value; }
            get { return iotDeviceInfo; }
        }
    }
}
