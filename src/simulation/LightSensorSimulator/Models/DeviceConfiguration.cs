﻿using System.Diagnostics.CodeAnalysis;

namespace LightSensorSimulator.Models
{
    [ExcludeFromCodeCoverage]
    public class DeviceConfiguration
    {
        public int DeviceId { get; set; }
        public string ServerUrl { get; set; }
        public int MeasurementInterval { get; set; }
        public int MeasurementsToSend { get; set; }
    }
}
