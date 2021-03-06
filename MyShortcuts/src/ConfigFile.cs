using System;
using System.IO;
using System.Text;

namespace MyShortcuts {
    public enum DeactiveBehavior {
        None,
        Minimize,
        MoveToBack,
    }

    public class ConfigFile {
        public string Folder = "";
        public double Left = 0;
        public double Top = 0;
        public double Width = 0;
        public double Height = 0;
        public bool Maximized = false;
        public DeactiveBehavior DeactiveBehavior = DeactiveBehavior.MoveToBack;
        public bool KeepFolder = false;
        public bool UseSingleClick = false;

        public bool Valid => Width > 0 && Height > 0 && Folder.Length > 0;

        private string[] Description { get; } = {
            "#",
            "### 아래는 설명 ###",
            "#",
            "# Folder: <string>, 보여줄 폴더를 지정",
            "# Left, Top, Width, Height: <double>, 창의 위치, 단위는 1/96 inch 논리 픽셀",
            "# Maximized: <boolean>, 최대화",
            "# DeactiveBehavior: <enum>, 비활성화 동작 설정, None, Minimize, MoveToBack",
            "# KeepFolder: <boolean>, 활성화 될 때 지정된 폴더로 다시 보여 주려면 true",
            "# UseSingleClick: <boolean>, 더블클릭 대신 클릭으로 아이템을 실행하려면 true",
        };

        private const string ConfigFileName = "MyShortcuts.config";
        private const string DefaultFolderName = "Shortcuts";

        private string configFilePath = "";

        private readonly DeactiveBehavior[] deactiveBehaviors = new DeactiveBehavior[] { DeactiveBehavior.Minimize, DeactiveBehavior.MoveToBack, DeactiveBehavior.None };
        private int currentDeactiveBehavior = 0;

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
            data.Get("KeepFolder", ref ret.KeepFolder);
            data.Get("UseSingleClick", ref ret.UseSingleClick);

            for (ret.currentDeactiveBehavior = 0; ret.currentDeactiveBehavior < ret.deactiveBehaviors.Length; ret.currentDeactiveBehavior++) {
                if (ret.deactiveBehaviors[ret.currentDeactiveBehavior] == ret.DeactiveBehavior)
                    break;
            }

            Directory.CreateDirectory(ret.Folder);

            return ret;
        }
        private void Save(bool withWindowPosSize) {
            var data = new SimpleConfigFile(configFilePath);

            data.Set("Folder", Folder);
            if (withWindowPosSize) {
                data.Set("Left", Left);
                data.Set("Top", Top);
                data.Set("Width", Width);
                data.Set("Height", Height);
                data.Set("Maximized", Maximized);
            }

            data.Set("DeactiveBehavior", DeactiveBehavior);
            data.Set("KeepFolder", KeepFolder);
            data.Set("UseSingleClick", UseSingleClick);

            data.Save(configFilePath, Description);
        }

        public DeactiveBehavior NextDeactiveBehavior() {
            currentDeactiveBehavior = (currentDeactiveBehavior + 1) % deactiveBehaviors.Length;
            DeactiveBehavior = deactiveBehaviors[currentDeactiveBehavior];
            Save(false);
            return DeactiveBehavior;
        }

        public bool NextKeepFolder() {
            KeepFolder = !KeepFolder;
            Save(false);
            return KeepFolder;
        }

        public void ChangeWindowPosSize() {
            // TODO
            Save(true);
        }
    }
}
