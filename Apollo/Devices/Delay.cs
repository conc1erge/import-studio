using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;

using Newtonsoft.Json;

using Apollo.Elements;
using Apollo.Structures;

namespace Apollo.Devices {
    public class Delay: Device {
        public static readonly new string DeviceIdentifier = "delay";

        public bool Mode; // true uses Length
        public Length Length;
        private int _time;
        private Decimal _gate;

        private object locker = new object();

        private Queue<Timer> _timers = new Queue<Timer>();
        private TimerCallback _timerexit;

        public int Time {
            get {
                return _time;
            }
            set {
                if (10 <= value && value <= 30000)
                    _time = value;
            }
        }

        public Decimal Gate {
            get {
                return _gate;
            }
            set {
                if (0 <= value && value <= 4)
                    _gate = value;
            }
        }

        public override Device Clone() {
            return new Delay(Mode, Length, _time, _gate);
        }

        public Delay(bool mode = false, Length length = null, int time = 500, Decimal gate = 1): base(DeviceIdentifier) {
            _timerexit = new TimerCallback(Tick);

            if (length == null) length = new Length();

            Mode = mode;
            Time = time;
            Length = length;
            Gate = gate;
        }

        private void Tick(object info) {
            if (info.GetType() == typeof(Signal)) {
                Signal n = (Signal)info;

                lock (locker) {
                    MIDIExit?.Invoke(n);

                    _timers.Dequeue().Dispose();
                }
            }
        }

        public override void MIDIEnter(Signal n) {
            _timers.Enqueue(new Timer(_timerexit, n.Clone(), Convert.ToInt32((Mode? (int)Length : _time) * _gate), Timeout.Infinite));
        }

        public static Device DecodeSpecific(string jsonString) {
            Dictionary<string, object> json = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);
            if (json["device"].ToString() != DeviceIdentifier) return null;

            Dictionary<string, object> data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json["data"].ToString());

            return new Delay(Convert.ToBoolean(data["mode"]), new Length(Convert.ToInt32(data["length"])), Convert.ToInt32(data["time"]), Convert.ToDecimal(data["gate"]));
        }

        public override string EncodeSpecific() {
            StringBuilder json = new StringBuilder();

            using (JsonWriter writer = new JsonTextWriter(new StringWriter(json))) {
                writer.WriteStartObject();

                    writer.WritePropertyName("device");
                    writer.WriteValue(DeviceIdentifier);

                    writer.WritePropertyName("data");
                    writer.WriteStartObject();

                        writer.WritePropertyName("mode");
                        writer.WriteValue(Mode);

                        writer.WritePropertyName("length");
                        writer.WriteValue(Convert.ToInt32(Math.Log(Convert.ToDouble(Length.Value), 2)) + 7);

                        writer.WritePropertyName("time");
                        writer.WriteValue(_time);

                        writer.WritePropertyName("gate");
                        writer.WriteValue(_gate);

                    writer.WriteEndObject();

                writer.WriteEndObject();
            }
            
            return json.ToString();
        }
    }
}