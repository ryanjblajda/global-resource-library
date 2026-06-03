using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace Axis
{
    public static class Utilities
    {
        public const string ContinuousPanTiltMoveAction = "continuouspantiltmove={1},{2}";
        public const string ContinuousZoomMoveAction = "continuouszoommove={1}";
        public const string RecallDevicePresetAction = "gotoserverpresetno={1}";
        public const string SaveDevicePresetAction = "setserverpresetno={1}";
        public const string SaveHomeAction = "setserverpresetno=99&home=yes";
        public const string MoveAction = "move={1}";
        public const string PanTiltZoomService = "com/ptz.cgi?";
        //public const string PanTiltZoomConfigureService = "com/ptzconfig.cgi?";
        public const string BaseURL = "http://{0}/axis-cgi/";

        public const string Home = "home";

        public enum CameraAction : ushort
        {
            Stop,
            Up,
            Down,
            Left,
            Right,
            In,
            Out,
            HomeGo,
            HomeSave,
            PresetRecall,
            PresetSave
        }
    }
}