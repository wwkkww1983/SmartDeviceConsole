using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;

namespace SmartDeviceConsole
{
    class Program
    {
        static String accessKeyId = "LTAI2I8tyVCRUgdu";
        static String accessKeySecret = "Ae2aYDErGq6Tdb60PTzbz0ZxSsMFpX";

        //static string serverUrl = "http://localhost:8080";
        static string serverUrl = "http://120.24.14.110:8080";
        static string productKey = "a1E6VCCsule";
        static AliIotMqttService mqttService = null;

        static string normalDataTableName = "imdevice_normal_data";

        static SmartDeviceServiceEvent eventHandler = new SmartDeviceServiceEvent();

        class SmartDeviceServiceEvent : IAliIotMqttServiceEvent
        {
            public void ConnectionClosed()
            {
                Console.WriteLine("已从IoT平台断开，并退出登录");
                LoginUtils.Instance.LoginStatus = false;
            }

            public void MessageReceived(string msg)
            {
                //Console.WriteLine("收到: " + msg);
                /*
                try {
                    SmartDeviceBaseMessage baseMsg = JsonConvert.DeserializeObject<SmartDeviceBaseMessage>(msg);

                    if (baseMsg != null)
                    {
                        if (baseMsg.MsgType != null && baseMsg.MsgType.Equals("normal"))
                        {
                            SmartDeviceMessage<SmartDeviceNormalData> dataMsg = JsonConvert.DeserializeObject<SmartDeviceMessage<SmartDeviceNormalData>>(msg);
                            displayNormalData(dataMsg);

                        }
                    }
                }catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                */
                
            }
        }

        static void Main(string[] args)
        {
            
            mqttService = new AliIotMqttService();
            mqttService.EventHandler = eventHandler;
            
            String cmdLine;
           
            while(!(cmdLine = Console.ReadLine()).Equals("exit"))
            {
                if (cmdLine.Equals("login"))
                {
                    LoginHandler();
                }
                else if (cmdLine.Equals("bind device"))
                {
                    BindDeviceHandler();
                }
                else if (cmdLine.Equals("unbind device"))
                {
                    UnbindDeviceHandler();
                }
                else if (cmdLine.Equals("logout"))
                {
                    LogoutHandler();
                }
                else if(cmdLine.Equals("register user"))
                {
                    RegisterUserHandler();
                }
                else if(cmdLine.Equals("register device"))
                {
                    RegisterDeviceHandler();
                }else if(cmdLine.Equals("query bound device"))
                {
                    //查询已绑定通信设备信息
                    QueryBoundDeviceHandler();
                }else if(cmdLine.Equals("edit device"))
                {
                    //编辑已绑定通信设备信息
                    EditBoundDeviceHandler();
                }else if(cmdLine.Equals("add sensor"))
                {
                    //添加测量单元 修改和删除接口后续增加
                    AddSensorHandler();
                }

            }
            
        }

        static void LoginHandler()
        {
           
            Console.WriteLine("请输入用户名");
            string userName = Console.ReadLine();
            Console.WriteLine("请输入密码");
            string password = Console.ReadLine();

            string url = serverUrl + "/login?" + Constant.productKeyParam + "=" + productKey + "&"
                                               + Constant.userNameParam + "=" + userName + "&" 
                                               + Constant.passwordParam + "=" + password;

            string statusCode;
            string responsStr = HttpUtils.Instance.CallGet(url, out statusCode);

            if (!statusCode.Equals("OK"))
            {
                Console.WriteLine("登录失败，请确认网络连接");
                return;
            }

            ValueWrapper<AliIotDeviceInfo> wrapper = JsonConvert.DeserializeObject<ValueWrapper<AliIotDeviceInfo>>(responsStr);


            if (!wrapper.STATUS)
            {
                Console.WriteLine("登录失败： " + wrapper.ErrorMsg);
                return;
            }

            AliIotDeviceInfo deviceInfo = wrapper.VALUE;

            try
            {
                mqttService.mqttConnect(userName, productKey, deviceInfo);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("连接Iot平台失败");
                return;
            }

            LoginUtils.Instance.LoginStatus = true;
            LoginUtils.Instance.DeviceInfo = deviceInfo;
            LoginUtils.Instance.UserName = userName;

            Console.WriteLine("登录成功");

        }

        static void BindDeviceHandler()
        {
            if(!LoginUtils.Instance.LoginStatus)
            {
                Console.WriteLine("请先登录");
                return;
            }

            Console.WriteLine("请输入产品名称：");
            string deviceName = Console.ReadLine();

            string url = serverUrl + "/bind_device?" + Constant.productKeyParam + "=" + productKey + "&"
                                               + Constant.userNameParam + "=" + LoginUtils.Instance.UserName + "&"
                                               + Constant.deviceIdParam + "=" + deviceName;
            string statusCode;
            string responsStr = HttpUtils.Instance.CallGet(url, out statusCode);

            if (!statusCode.Equals("OK"))
            {
                Console.WriteLine("绑定失败，请确认网络连接");
                return;
            }

            StatusWrapper wrapper = JsonConvert.DeserializeObject<StatusWrapper>(responsStr);
            if (!wrapper.STATUS)
            {
                Console.WriteLine("绑定失败： " + wrapper.ErrorMsg);
                return;
            }

            Console.WriteLine("绑定成功");
            return;
        }

        static void UnbindDeviceHandler()
        {
            if (!LoginUtils.Instance.LoginStatus)
            {
                Console.WriteLine("请先登录");
                return;
            }

            Console.WriteLine("请输入产品名称：");
            string deviceName = Console.ReadLine();

            string url = serverUrl + "/unbind_device?" + Constant.productKeyParam + "=" + productKey + "&"
                                               + Constant.userNameParam + "=" + LoginUtils.Instance.UserName + "&"
                                               + Constant.deviceIdParam + "=" + deviceName;
            string statusCode;
            string responsStr = HttpUtils.Instance.CallGet(url, out statusCode);

            if (!statusCode.Equals("OK"))
            {
                Console.WriteLine("解除绑定失败，请确认网络连接");
                return;
            }

            StatusWrapper wrapper = JsonConvert.DeserializeObject<StatusWrapper>(responsStr);
            if (!wrapper.STATUS)
            {
                Console.WriteLine("解除绑定失败： " + wrapper.ErrorMsg);
                return;
            }

            Console.WriteLine("解除绑定成功");
            return;
        }

        static void LogoutHandler()
        {
            mqttService.mqttDisconnect();
        }

        static void RegisterUserHandler()
        {
            Console.WriteLine("请输入新用户名：");
            string userName = Console.ReadLine();

            if(userName == string.Empty)
            {
                Console.WriteLine("输入用户名为空");
                return;
            }

            Console.WriteLine("请输入新用户名密码：");
            string password = Console.ReadLine();

            if (password == string.Empty)
            {
                Console.WriteLine("输入用户密码为空");
                return;
            }

            string url = serverUrl + "/register_user?" + Constant.productKeyParam + "=" + productKey + "&"
                                               + Constant.userNameParam + "=" + userName + "&"
                                               + Constant.passwordParam + "=" + password;

            string statusCode;
            string responsStr = HttpUtils.Instance.CallGet(url, out statusCode);

            if (!statusCode.Equals("OK"))
            {
                Console.WriteLine("注册失败，请确认网络连接");
                return;
            }

            StatusWrapper wrapper = JsonConvert.DeserializeObject<StatusWrapper>(responsStr);

            if (!wrapper.STATUS)
            {
                Console.WriteLine("注册失败： " + wrapper.ErrorMsg);
                return;
            }

            Console.WriteLine("注册成功");
        }

        static void RegisterDeviceHandler()
        {
            Console.WriteLine("请输入设备的唯一名称：");
            string deviceName = Console.ReadLine();

            string url = serverUrl + "/register_device?" + Constant.productKeyParam + "=" + productKey + "&"
                                               + Constant.deviceIdParam + "=" + deviceName;

            string statusCode;
            string responsStr = HttpUtils.Instance.CallGet(url, out statusCode);

            if (!statusCode.Equals("OK"))
            {
                Console.WriteLine("注册失败，请确认网络连接");
                return;
            }

            ValueWrapper<AliIotDeviceInfo> wrapper = JsonConvert.DeserializeObject<ValueWrapper<AliIotDeviceInfo> >(responsStr);

            if (!wrapper.STATUS)
            {
                Console.WriteLine("注册失败： " + wrapper.ErrorMsg);
                return;
            }

            Console.WriteLine("注册成功");
            Console.WriteLine("Iot平台设备名称： " + wrapper.VALUE.NAME);
            Console.WriteLine("Iot平台设备秘钥(Secret)：" + wrapper.VALUE.SECRET);
        }

        static void QueryBoundDeviceHandler()
        {
            if (!LoginUtils.Instance.LoginStatus)
            {
                Console.WriteLine("请先登录");
                return;
            }

            string url = serverUrl + "/query_bound_device?" + Constant.productKeyParam + "=" + productKey + "&"
                                               + Constant.userNameParam + "=" + LoginUtils.Instance.UserName;

            string statusCode;
            string responsStr = HttpUtils.Instance.CallGet(url, out statusCode);

            if (!statusCode.Equals("OK"))
            {
                Console.WriteLine("查询失败，请确认网络连接");
                return;
            }

            ValueWrapper<Dictionary<string, CableDevice>> valueWrapper = JsonConvert.DeserializeObject<ValueWrapper<Dictionary<string, CableDevice>>>(responsStr);

            if(!valueWrapper.STATUS)
            {
                Console.WriteLine("查询失败： " + valueWrapper.ErrorMsg);
                return;
            }

            Dictionary<string, CableDevice> deviceMap = valueWrapper.VALUE;
            if(deviceMap == null || deviceMap.Count == 0)
            {
                Console.WriteLine("该用户暂未绑定设备");
                return;
            }

            string[] deviceIdArray = deviceMap.Keys.ToArray<string>();

            foreach(string deviceId in deviceIdArray)
            {
                CableDevice cableDevice = null;
                deviceMap.TryGetValue(deviceId, out cableDevice);

                if(cableDevice != null)
                {
                    Console.WriteLine("+++++++++++++++++++++++++++++++++++");
                    Console.WriteLine("通信设备ID号： " + deviceId);
                    string aliasName = cableDevice.AliasName;
                    string comment = cableDevice.Comment;

                    if(aliasName == null)
                    {
                        aliasName = "";
                    }

                    if(comment == null)
                    {
                        comment = "";
                    }

                    Console.WriteLine("设备别名：" + aliasName);
                    Console.WriteLine("设备备注：" + comment);

                    AliIotDeviceInfo iotDeviceInfo = cableDevice.IotDeviceInfo;
                    if(iotDeviceInfo != null)
                    {
                        Console.WriteLine("设备在线状态：" + iotDeviceInfo.STATUS);
                    }
                    Console.WriteLine("+++++++++++++++++++++++++++++++++++");
                }

                
            }

        }

        static void EditBoundDeviceHandler()
        {
            //这个接口可能后续会修改

            Console.WriteLine("请输入要编辑的设备ID：");
            string deviceId = Console.ReadLine();

            Console.WriteLine("输入设备别名：");
            string aliasName = Console.ReadLine();

            Console.WriteLine("输入设备备注信息");
            string comment = Console.ReadLine();

            string url = serverUrl + "/edit_device?" + Constant.deviceIdParam + "=" + deviceId;
            

            Dictionary<String, String> postBodyParams = new Dictionary<string, string>();
            postBodyParams.Add(Constant.deviceAliasNameParam , aliasName);
            postBodyParams.Add(Constant.deviceCommentParam , comment);

            string postBodyStr = JsonConvert.SerializeObject(postBodyParams);

            string statusCode;
            string responsStr = HttpUtils.Instance.CallPost(url, out statusCode, postBodyStr);

            if (!statusCode.Equals("OK"))
            {
                Console.WriteLine("编辑失败，请确认网络连接");
                return;
            }

            StatusWrapper wrapper = JsonConvert.DeserializeObject<StatusWrapper>(responsStr);
            if (!wrapper.STATUS)
            {
                Console.WriteLine("编辑失败： " + wrapper.ErrorMsg);
                return;
            }

            Console.WriteLine("编辑成功");
            return;


        }

        static void AddSensorHandler()
        {
            if (!LoginUtils.Instance.LoginStatus)
            {
                Console.WriteLine("请先登录");
                return;
            }

            Console.WriteLine("请输入要添加的测量单元的ID：");
            string sensorId = Console.ReadLine();

            Console.WriteLine("请输入要传感器所在的通信单元ID：");
            string deviceId = Console.ReadLine(); //此处务必自己控制一下 是否是已绑定的通信设备，API暂时没有这项检测，以后再加上

            string url = serverUrl + "/add_sensor?" + Constant.productKeyParam + "=" + productKey + "&"
                                               + Constant.userNameParam + "=" + LoginUtils.Instance.UserName + "&"
                                               + Constant.sensorIdParam + "=" + sensorId + "&"
                                               + Constant.deviceIdParam + "=" + deviceId;


            string statusCode;
            string responsStr = HttpUtils.Instance.CallGet(url, out statusCode);

            if (!statusCode.Equals("OK"))
            {
                Console.WriteLine("添加失败，请确认网络连接");
                return;
            }

            StatusWrapper wrapper = JsonConvert.DeserializeObject<StatusWrapper>(responsStr);
            if (!wrapper.STATUS)
            {
                Console.WriteLine("添加失败： " + wrapper.ErrorMsg);
                return;
            }

            Console.WriteLine("添加成功");
            return;
        }

        static void displayNormalData(SmartDeviceMessage<SmartDeviceNormalData> dataMsg)
        {
            Console.WriteLine("---------------------------------------");
            if (dataMsg == null)
                return;

            if (dataMsg.DeviceId != null && dataMsg.ChannelType != null && dataMsg.Data != null)
            {
                Console.WriteLine("DeviceId: " + dataMsg.DeviceId + ", " + "Channel: " + dataMsg.ChannelType);

                SmartDeviceNormalData nmData = dataMsg.Data;

                String dataTitle = "";

                dataTitle = "电流实时值：";
                if(nmData.CurrentRealTimeValue != null)
                {
                    Console.WriteLine(dataTitle + nmData.CurrentRealTimeValue.Value);
                }
                else
                {
                    Console.WriteLine(dataTitle);
                }

                dataTitle = "电流平均值：";
                if (nmData.CurrentAvgValue != null)
                {
                    Console.WriteLine(dataTitle + nmData.CurrentAvgValue.Value);
                }
                else
                {
                    Console.WriteLine(dataTitle);
                }


                dataTitle = "电流最大值：";
                if (nmData.CurrentMaxValue != null)
                {
                    Console.WriteLine(dataTitle + nmData.CurrentMaxValue.Value);
                }
                else
                {
                    Console.WriteLine(dataTitle);
                }


                dataTitle = "电流最小值：";
                if (nmData.CurrentMinValue != null)
                {
                    Console.WriteLine(dataTitle + nmData.CurrentMinValue.Value);
                }
                else
                {
                    Console.WriteLine(dataTitle);
                }


                dataTitle = "环流与主缆负荷比实时值：";
                if (nmData.CurrentChargeRatio != null)
                {
                    Console.WriteLine(dataTitle + nmData.CurrentChargeRatio.Value);
                }
                else
                {
                    Console.WriteLine(dataTitle);
                }


                dataTitle = "环流最大值与最小值比实时值：";
                if (nmData.CurrentMaxMinRatio != null)
                {
                    Console.WriteLine(dataTitle + nmData.CurrentMaxMinRatio.Value);
                }
                else
                {
                    Console.WriteLine(dataTitle);
                }


                dataTitle = "温度实时值：";
                if (nmData.TemperatureRealtimeValue != null)
                {
                    Console.WriteLine(dataTitle + nmData.TemperatureRealtimeValue.Value);
                }
                else
                {
                    Console.WriteLine(dataTitle);
                }


                dataTitle = "温度平均值：";
                if (nmData.TemperatureAvgValue != null)
                {
                    Console.WriteLine(dataTitle + nmData.TemperatureAvgValue.Value);
                }
                else
                {
                    Console.WriteLine(dataTitle);
                }


                dataTitle = "温度最大值：";
                if (nmData.TemperatureMaxValue != null)
                {
                    Console.WriteLine(dataTitle + nmData.TemperatureMaxValue.Value);
                }
                else
                {
                    Console.WriteLine(dataTitle);
                }


                dataTitle = "温度最小值：";
                if (nmData.TemperatureMinValue != null)
                {
                    Console.WriteLine(dataTitle + nmData.TemperatureMinValue.Value);
                }
                else
                {
                    Console.WriteLine(dataTitle);
                }
            }


            Console.WriteLine("---------------------------------------");
        }
    }
}
