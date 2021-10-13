using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace MyShortcuts {
    internal class SimpleConfigFile {
        Dictionary<string, string> dataMap;

        public SimpleConfigFile() { }
        public SimpleConfigFile(string filepath) {
            dataMap = Load(filepath);
        }

        public T Get<T>(string key, T defaultValue = default) {
            if (dataMap.TryGetValue(key, out var str)) {
                try {
                    var converter = TypeDescriptor.GetConverter(typeof(T));
                    if (converter != null) {
                        var ret = (T)converter.ConvertFromString(str);
                        if (!typeof(T).IsEnum || Enum.IsDefined(typeof(T), ret))
                            return ret;
                    }
                }
                catch { }
            }
            return defaultValue;
        }

        public void Get<T>(string key, ref T dest) {
            dest = Get(key, dest);
        }

        public void Set<T>(string key, T val) { dataMap[key] = val.ToString(); }

        public void Save(string filepath, params string[] desc) {
            using (var sw = new StringWriter()) {
                foreach (var kv in dataMap)
                    sw.WriteLine("{0}:{1}", kv.Key, kv.Value);
                if (desc.Length > 0) {
                    foreach (var v in desc)
                        sw.WriteLine(v);
                }
                try {
                    File.WriteAllText(filepath, sw.ToString(), Encoding.UTF8);
                }
                catch { }
            }
        }

        private static Dictionary<string, string> Load(string filepath) {
            var ret = new Dictionary<string, string>();
            try {
                var lines = File.ReadAllLines(filepath, Encoding.UTF8);
                foreach (var line in lines) {
                    if (!line.StartsWith("#")) {
                        var kv = line.Split(new char[] { ':' }, 2);
                        if (kv.Length > 1)
                            ret[kv[0]] = kv[1];
                    }
                }
            }
            catch { }
            return ret;
        }
    }
}
