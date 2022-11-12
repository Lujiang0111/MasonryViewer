using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prism.Mvvm;
using System;
using System.IO;
using System.Text;

namespace MasonryViewer.Extensions
{
    public class SettingManager : BindableBase
    {
        private int imageCntPerLine = 4;
        public int ImageCntPerLine
        {
            get { return imageCntPerLine; }
            private set { SetProperty(ref imageCntPerLine, value); }
        }

        private double mouseWheelPrecision = 1;
        public double MouseWheelPrecision
        {
            get { return mouseWheelPrecision; }
            private set { SetProperty(ref mouseWheelPrecision, value); }
        }

        private static readonly string commonSettingsFileName = "Settings.json";

        public void SetImageCntPerLine(int imageCntPerLine)
        {
            ImageCntPerLine = imageCntPerLine;
            SaveCommonSetting();
        }

        public void SetMouseWheelPrecision(double mouseWheelPrecision)
        {
            MouseWheelPrecision = mouseWheelPrecision;
            SaveCommonSetting();
        }

        private SettingManager()
        {
            LoadCommonSettings();
        }

        private void LoadCommonSettings()
        {
            if (!File.Exists(commonSettingsFileName))
            {
                return;
            }

            using (StreamReader streamReader = File.OpenText(commonSettingsFileName))
            using (JsonTextReader reader = new JsonTextReader(streamReader))
            {
                JObject settingJObject = null;
                try
                {
                    settingJObject = (JObject)JToken.ReadFrom(reader);
                }
                catch (Exception)
                {
                    return;
                }

                if (null != settingJObject["image_cnt_per_line"])
                {
                    ImageCntPerLine = (int)settingJObject["image_cnt_per_line"];
                }

                if (null != settingJObject["mouse_wheel_precision"])
                {
                    MouseWheelPrecision = (double)settingJObject["mouse_wheel_precision"];
                }
            }
        }

        private void SaveCommonSetting()
        {
            JObject settingJObject = new JObject();
            settingJObject["image_cnt_per_line"] = ImageCntPerLine;
            settingJObject["mouse_wheel_precision"] = MouseWheelPrecision;

            using (StreamWriter streamWriter = new StreamWriter(commonSettingsFileName, false, Encoding.UTF8))
            using (JsonTextWriter writer = new JsonTextWriter(streamWriter))
            {
                settingJObject.WriteTo(writer);
            }
        }

        public static SettingManager Instance { get; private set; }
        static SettingManager()
        {
            Instance = new SettingManager();
        }
    }
}
