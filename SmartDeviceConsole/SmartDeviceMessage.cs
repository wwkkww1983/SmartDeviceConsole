using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDeviceConsole
{

    class SmartDeviceBaseMessage
    {
        [JsonProperty(PropertyName = "id")]
        private string deviceId;

        [JsonProperty(PropertyName = "mtp")]
        private string msgType;

        [JsonProperty(PropertyName = "ch")]
        private string channelType;

        [JsonIgnore]
        public string DeviceId { get => deviceId; set => deviceId = value; }

        [JsonIgnore]
        public string MsgType { get => msgType; set => msgType = value; }

        [JsonIgnore]
        public string ChannelType { get => channelType; set => channelType = value; }
    }

    class SmartDeviceMessage<T> :SmartDeviceBaseMessage
    {
        [JsonProperty(PropertyName = "msg")]
        private T data;

        public T Data { get => data; set => data = value; }
    }

    
    class SmartDeviceMetaData<T>
    {
        [JsonProperty(PropertyName = "val")]
        private T value;

        [JsonProperty(PropertyName = "tp")]
        private long timestamp;

        [JsonIgnore]
        public T Value { get => value; set => this.value = value; }

        [JsonIgnore]
        public long Timestamp { get => timestamp; set => timestamp = value; }
    }

    class SmartDeviceNormalData
    {
        [JsonProperty(PropertyName = "cur_rt")]
        private SmartDeviceMetaData<Double> currentRealTimeValue;

        [JsonProperty(PropertyName = "cur_avg")]
        private SmartDeviceMetaData<Double> currentAvgValue;

        [JsonProperty(PropertyName = "cur_max")]
        private SmartDeviceMetaData<Double> currentMaxValue;

        [JsonProperty(PropertyName = "cur_min")]
        private SmartDeviceMetaData<Double> currentMinValue;

        [JsonProperty(PropertyName = "cur_elr_rt")]
        private SmartDeviceMetaData<Double> currentChargeRatio; //环流与电缆负荷比实时值

        [JsonProperty(PropertyName = "cur_mmr_rt")]
        private SmartDeviceMetaData<Double> currentMaxMinRatio; //环流最大值与最小值实时值

        [JsonProperty(PropertyName = "temp_rt")]
        private SmartDeviceMetaData<Double> temperatureRealtimeValue; //温度实时值

        [JsonProperty(PropertyName = "temp_avg")]
        private SmartDeviceMetaData<Double> temperatureAvgValue; //温度平均值

        [JsonProperty(PropertyName = "temp_max")]
        private SmartDeviceMetaData<Double> temperatureMaxValue; //温度最大值

        [JsonProperty(PropertyName = "temp_min")]
        private SmartDeviceMetaData<Double> temperatureMinValue; //温度最小值

        [JsonProperty(PropertyName = "vib")]
        private SmartDeviceMetaData<Int32> isVibration; //振动状态

        [JsonIgnore]
        public SmartDeviceMetaData<double> CurrentRealTimeValue { get => currentRealTimeValue; set => currentRealTimeValue = value; }
        [JsonIgnore]
        public SmartDeviceMetaData<double> CurrentAvgValue { get => currentAvgValue; set => currentAvgValue = value; }
        [JsonIgnore]
        public SmartDeviceMetaData<double> CurrentMaxValue { get => currentMaxValue; set => currentMaxValue = value; }
        [JsonIgnore]
        public SmartDeviceMetaData<double> CurrentMinValue { get => currentMinValue; set => currentMinValue = value; }
        [JsonIgnore]
        public SmartDeviceMetaData<double> CurrentChargeRatio { get => currentChargeRatio; set => currentChargeRatio = value; }
        [JsonIgnore]
        public SmartDeviceMetaData<double> CurrentMaxMinRatio { get => currentMaxMinRatio; set => currentMaxMinRatio = value; }
        [JsonIgnore]
        public SmartDeviceMetaData<double> TemperatureRealtimeValue { get => temperatureRealtimeValue; set => temperatureRealtimeValue = value; }
        [JsonIgnore]
        public SmartDeviceMetaData<double> TemperatureAvgValue { get => temperatureAvgValue; set => temperatureAvgValue = value; }
        [JsonIgnore]
        public SmartDeviceMetaData<double> TemperatureMaxValue { get => temperatureMaxValue; set => temperatureMaxValue = value; }
        [JsonIgnore]
        public SmartDeviceMetaData<double> TemperatureMinValue { get => temperatureMinValue; set => temperatureMinValue = value; }
        [JsonIgnore]
        public SmartDeviceMetaData<int> IsVibration { get => isVibration; set => isVibration = value; }
    }



}
