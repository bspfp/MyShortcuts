using System;
using System.IO;
using System.Text;

namespace MyShortcuts {
    public enum DeactiveBehavior {
        None,
        Minimize,
        MoveToBack,
    }

    public enum PinMethods {
        None,
        Pin,
        Unpin,
    }

    public class ConfigFile {
        public string Folder = "";
        public double Left = 0;
        public double Top = 0;
        public double Width = 0;
        public double Height = 0;
        public bool Maximized = false;
        public DeactiveBehavior DeactiveBehavior = DeactiveBehavior.MoveToBack;
        public PinMethods PinMethods = PinMethods.None;
        public bool KeepFolder = false;

        public bool Valid => Width > 0 && Height > 0 && Folder.Length > 0;

        private string[] Description { get; } = {
            "#",
            "### 아래는 설명 ###",
            "#",
            "# Folder: <string>, 보여줄 폴더를 지정",
            "# Left, Top, Width, Height: <double>, 창의 위치, 단위는 1/96 inch 논리 픽셀",
            "# Maximized: <boolean>, 최대화",
            "# DeactiveBehavior: <enum>, 비활성화 동작 설정, None, Minimize, MoveToBack",
            "# PinMethods: <enum>, 창 고정 방법, None, Pin, Unpin",
            "# KeepFolder: <boolean>, 활성화 될 때 지정된 폴더로 다시 보여 주려면 true",
        };

        private const string ConfigFileName = "MyShortcuts.config";
        private const string DefaultFolderName = "Shortcuts";

        private string configFilePath = "";

        private readonly DeactiveBehavior[] deactiveBehaviors = new DeactiveBehavior[] { DeactiveBehavior.Minimize, DeactiveBehavior.MoveToBack, DeactiveBehavior.None };
        private int currentDeactiveBehavior = 0;
        private readonly PinMethods[] pinMethods = new PinMethods[] { PinMethods.Pin, PinMethods.None, PinMethods.Unpin, PinMethods.None };
        private int currentPinMethod = 0;

        public static ConfigFile Load() {
            var userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var defaultFolder = Path.Combine(userProfilePath, DefaultFolderName);
            var configFilePath = Path.Combine(userProfilePath, ConfigFileName);

            var data = new SimpleConfigFile(configFilePath);
            ConfigFile ret = new ConfigFile {
                Folder = defaultFolder,
                configFilePath = configFilePath
            };

            data.Get("Folder", ref ret.Folder);
            data.Get("Left", ref ret.Left);
            data.Get("Top", ref ret.Top);
            data.Get("Width", ref ret.Width);
            data.Get("Height", ref ret.Height);
            data.Get("Maximized", ref ret.Maximized);
            data.Get("DeactiveBehavior", ref ret.DeactiveBehavior);
            data.Get("DeactiveBehavior", ref ret.DeactiveBehavior);
            data.Get("PinMethods", ref ret.PinMethods);
            data.Get("KeepFolder", ref ret.KeepFolder);

            for (ret.currentDeactiveBehavior = 0; ret.currentDeactiveBehavior < ret.deactiveBehaviors.Length; ret.currentDeactiveBehavior++) {
                if (ret.deactiveBehaviors[ret.currentDeactiveBehavior] == ret.DeactiveBehavior)
                    break;
            }
            for (ret.currentPinMethod = 0; ret.currentPinMethod < ret.pinMethods.Length; ret.currentPinMethod++) {
                if (ret.pinMethods[ret.currentPinMethod] == ret.PinMethods)
                    break;
            }

            return ret;
        }
        public void Save() {
            var data = new SimpleConfigFile(configFilePath);

            data.Set("Folder", Folder);
            data.Set("Left", Left);
            data.Set("Top", Top);
            data.Set("Width", Width);
            data.Set("Height", Height);
            data.Set("Maximized", Maximized);
            data.Set("DeactiveBehavior", DeactiveBehavior);
            data.Set("DeactiveBehavior", DeactiveBehavior);
            data.Set("PinMethods", PinMethods);
            data.Set("KeepFolder", KeepFolder);

            data.Save(configFilePath, Description);
        }

        public DeactiveBehavior NextDeactiveBehavior() {
            currentDeactiveBehavior = (currentDeactiveBehavior + 1) % deactiveBehaviors.Length;
            DeactiveBehavior = deactiveBehaviors[currentDeactiveBehavior];
            return DeactiveBehavior;
        }

        public PinMethods NextPinMethod() {
            currentPinMethod = (currentPinMethod + 1) % pinMethods.Length;
            PinMethods = pinMethods[currentPinMethod];
            return PinMethods;
        }

        public bool NextKeepFolder() {
            KeepFolder = !KeepFolder;
            return KeepFolder;
        }
    }
}
