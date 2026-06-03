from extronlib.interface import SerialInterface, EthernetClientInterface
from extronlib.system import ProgramLog
import re


class DeviceClass:
    def __init__(self):

        self.Unidirectional = 'False'
        self.connectionCounter = 15
        self.DefaultResponseTimeout = 0.3
        self._compile_list = {}
        self.Subscription = {}
        self.ReceiveData = self.__ReceiveData
        self.__receiveBuffer = b''
        self.__maxBufferSize = 2048
        self.__matchStringDict = {}
        self.counter = 0
        self.connectionFlag = True
        self.initializationChk = True
        self.Debug = False
        self.Models = {}

        self.Commands = {
            'ConnectionStatus': {'Status': {}},
            'AutoFocus': {'Parameters': ['Camera'], 'Status': {}},
            'AutoIris': {'Parameters': ['Camera'], 'Status': {}},
            'AutoWhiteBalance': {'Parameters': ['Camera'], 'Status': {}},
            'Focus': {'Parameters': ['Camera', 'Speed'], 'Status': {}},
            'Home': {'Parameters': ['Camera'], 'Status': {}},
            'LineInMute': {'Parameters': ['Channel'], 'Status': {}},
            'LineInVolume': {'Parameters': ['Channel'], 'Status': {}},
            'LineOutMute': {'Parameters': ['Channel'], 'Status': {}},
            'LineOutVolume': {'Parameters': ['Channel'], 'Status': {}},
            'MasterMute': {'Status': {}},
            'MasterVolume': {'Status': {}},
            'Pan': {'Parameters': ['Camera', 'Speed'], 'Status': {}},
            'PresetRecall': {'Parameters': ['Camera'], 'Status': {}},
            'PresetSave': {'Parameters': ['Camera'], 'Status': {}},
            'Standby': {'Parameters': ['Camera'], 'Status': {}},
            'Tilt': {'Parameters': ['Camera', 'Speed'], 'Status': {}},
            'VideoMute': {'Parameters': ['Input'], 'Status': {}},
            'VideoMuteAll': {'Status': {}},
            'VideoPIP': {'Parameters': ['Channel'], 'Status': {}},
            'VideoPIPOff': {'Status': {}},
            'VideoSource': {'Parameters': ['Channel'], 'Status': {}},
            'Zoom': {'Parameters': ['Camera', 'Speed'], 'Status': {}},
        }

        self.deviceUsername = 'admin'
        self.devicePassword = 'password'

        if self.Unidirectional == 'False':
            self.AddMatchString(re.compile(b'login:'), self.__MatchUsername, None)
            self.AddMatchString(re.compile(b'Password:'), self.__MatchPassword, None)

        self.AllResRegex = re.compile(b'[\w: ]+\r\n[\w: ]+\r\n|[\w: ]+\r\nvolume:[ ]{0,9}-?\d+\.\d dB\r\n|login:|Password:|ERROR\r\n')

    def __MatchUsername(self, match, tag):
        if self.deviceUsername is not None:
            self.Send(self.deviceUsername + '\r\n')
        else:
            self.MissingCredentialsLog('Username')

    def __MatchPassword(self, match, tag):
        if self.devicePassword is not None:
            self.Send(self.devicePassword + '\r\n')
        else:
            self.MissingCredentialsLog('Password')

    def SetAutoFocus(self, value, qualifier):

        AutoFocusState = {
            'Auto': 'auto',
            'Manual': 'manual'
        }

        camera = int(qualifier['Camera'])

        if 1 <= camera <= 8:
            AutoFocusCmdString = 'camera {0} focus mode {1}\r'.format(camera, AutoFocusState[value])
            self.__SetHelper('AutoFocus', AutoFocusCmdString, value, qualifier)
        else:
            self.Discard('Invalid Command for SetAutoFocus')

    def UpdateAutoFocus(self, value, qualifier):

        AutoFocusState = {
            'auto': 'Auto',
            'manual': 'Manual'
        }

        camera = qualifier['Camera']
        if 1 <= int(camera) <= 8:
            AutoFocusCmdString = 'camera {0} focus mode get\r'.format(camera)
            res = self.__UpdateHelper('AutoFocus', AutoFocusCmdString, value, qualifier)
            if res:
                match = re.search('auto_focus:[ ]{0,9}(auto|manual)\r', res)
                try:
                    value = AutoFocusState[match.group(1)]
                except (KeyError, IndexError, AttributeError):
                    self.Error(['Auto Focus : Invalid/Unexpected Response'])
                else:
                    self.WriteStatus('AutoFocus', value, qualifier)
        else:
            self.Discard('Invalid Command for UpdateAutoFocus')

    def SetAutoIris(self, value, qualifier):

        AutoIrisState = {
            'On': 'on',
            'Off': 'off'
        }

        camera = int(qualifier['Camera'])

        if 1 <= camera <= 8:
            AutoIrisCmdString = 'camera {0} ccu set auto_iris {1}\r'.format(camera, AutoIrisState[value])
            self.__SetHelper('AutoIris', AutoIrisCmdString, value, qualifier)
        else:
            self.Discard('Invalid Command for SetAutoIris')

    def UpdateAutoIris(self, value, qualifier):  # page 52 of the protocol manual

        AutoIrisState = {
            'on': 'On',
            'off': 'Off'
        }
        camera = qualifier['Camera']
        if 1 <= int(camera) <= 8:
            AutoIrisCmdString = 'camera {0} ccu get iris\r'.format(camera)
            res = self.__UpdateHelper('AutoIris', AutoIrisCmdString, value, qualifier)
            if res:
                match = re.search('iris:[ ]{0,9}(on|off)\r', res)
                try:
                    value = AutoIrisState[match.group(1)]
                except (KeyError, IndexError, AttributeError):
                    self.Error(['Auto Iris : Invalid/Unexpected Response'])
                else:
                    self.WriteStatus('AutoIris', value, qualifier)
        else:
            self.Discard('Invalid Command for UpdateAutoIris')

    def SetAutoWhiteBalance(self, value, qualifier):

        AutoWhiteBalanceState = {
            'On': 'on',
            'Off': 'off'
        }

        camera = int(qualifier['Camera'])

        if 1 <= camera <= 8:
            AutoWhiteBalanceCmdString = 'camera {0} ccu set auto_white_balance {1}\r'.format(camera, AutoWhiteBalanceState[value])
            self.__SetHelper('AutoWhiteBalance', AutoWhiteBalanceCmdString, value, qualifier)
        else:
            self.Discard('Invalid Command for SetAutoWhiteBalance')

    def UpdateAutoWhiteBalance(self, value, qualifier):  # page 52 of the protocol manual

        AutoWhiteBalanceState = {
            'on': 'On',
            'off': 'Off'
        }

        camera = qualifier['Camera']
        if 1 <= int(camera) <= 8:
            AutoWhiteBalanceCmdString = 'camera {0} ccu get auto_white_balance\r'.format(camera)
            res = self.__UpdateHelper('AutoWhiteBalance', AutoWhiteBalanceCmdString, value, qualifier)
            if res:
                match = re.search('auto_white_balance:[ ]{0,9}(on|off)\r', res)
                try:
                    value = AutoWhiteBalanceState[match.group(1)]
                except (KeyError, IndexError, AttributeError):
                    self.Error(['Auto White Balance : Invalid/Unexpected Response'])
                else:
                    self.WriteStatus('AutoWhiteBalance', value, qualifier)
        else:
            self.Discard('Invalid Command for UpdateAutoWhiteBalance')

    def SetFocus(self, value, qualifier):

        FocusState = {
            'Far': 'far',
            'Near': 'near',
            'Stop': 'stop'
        }

        camera = int(qualifier['Camera'])
        focusspeed = int(qualifier['Speed'])

        if 1 <= camera <= 8 and 1 <= focusspeed <= 8:
            if value == 'Stop':
                FocusCmdString = 'camera {0} focus stop\r'.format(camera)
            else:
                FocusCmdString = 'camera {0} focus {1} {2}\r'.format(camera, FocusState[value], focusspeed)
            self.__SetHelper('Focus', FocusCmdString, value, qualifier)
        else:
            self.Discard('Invalid Command for SetFocus')

    def SetHome(self, value, qualifier):

        camera = int(qualifier['Camera'])

        if 1 <= camera <= 8:
            HomeCmdString = 'camera {0} home\r'.format(camera)
            self.__SetHelper('Home', HomeCmdString, value, qualifier)
        else:
            self.Discard('Invalid Command for SetHome')

    def SetLineInMute(self, value, qualifier):

        ChannelStates = {
            '1': 'line_in_1',
            '2': 'line_in_2'
        }

        LineInMuteState = {
            'On': 'on',
            'Off': 'off'
        }

        channel = qualifier['Channel']
        if 1 <= int(channel) <= 2:
            channel = ChannelStates[channel]
            LineInMuteCmdString = 'audio {0} mute {1}\r'.format(channel, LineInMuteState[value])
            self.__SetHelper('LineInMute', LineInMuteCmdString, value, qualifier)
        else:
            self.Discard('Invalid Command for SetLineInMute')

    def UpdateLineInMute(self, value, qualifier):

        ChannelStates = {
            '1': 'line_in_1',
            '2': 'line_in_2',
        }

        LineInMuteState = {
            'on': 'On',
            'off': 'Off'
        }

        channel = qualifier['Channel']
        if 1 <= int(channel) <= 2:
            channel = ChannelStates[channel]
            LineInMuteCmdString = 'audio {0} mute get\r'.format(channel)
            res = self.__UpdateHelper('LineInMute', LineInMuteCmdString, value, qualifier)
            if res:
                match = re.search('mute:[ ]{0,9}(on|off)\r', res)
                try:
                    value = LineInMuteState[match.group(1)]
                except (KeyError, IndexError, AttributeError):
                    self.Error(['Line In Mute : Invalid/Unexpected Response'])
                else:
                    self.WriteStatus('LineInMute', value, qualifier)
        else:
            self.Discard('Invalid Command for UpdateLineInMute')

    def SetLineInVolume(self, value, qualifier):

        ChannelStates = {
            '1': 'line_in_1',
            '2': 'line_in_2'
        }

        ValueConstraints = {
            'Min': -50,
            'Max': 20
        }

        channel = qualifier['Channel']
        if 1 <= int(channel) <= 2 and ValueConstraints['Min'] <= value <= ValueConstraints['Max']:
            channel = ChannelStates[channel]
            LineInVolumeCmdString = 'audio {0} volume set {1}\r'.format(channel, value)
            self.__SetHelper('LineInVolume', LineInVolumeCmdString, value, qualifier)
        else:
            self.Discard('Invalid Command for SetLineInVolume')

    def UpdateLineInVolume(self, value, qualifier):

        ChannelStates = {
            '1': 'line_in_1',
            '2': 'line_in_2',
        }

        channel = qualifier['Channel']
        if 1 <= int(channel) <= 2:
            channel = ChannelStates[channel]
            LineInVolumeCmdString = 'audio {0} volume get\r'.format(channel)
            res = self.__UpdateHelper('LineInVolume', LineInVolumeCmdString, value, qualifier)
            if res:
                match = re.search('audio line_in_[12] volume get\r\nvolume:[ ]{0,9}(-?\d+)\.\d dB\r\n', res)
                try:
                    value = int(match.group(1))
                except (ValueError, IndexError, AttributeError):
                    self.Error(['Line In Volume : Invalid/Unexpected Response'])
                else:
                    self.WriteStatus('LineInVolume', value, qualifier)
        else:
            self.Discard('Invalid Command for UpdateLineInVolume')

    def SetLineOutMute(self, value, qualifier):

        ChannelStates = {
            '1': 'line_out_1',
            '2': 'line_out_2'
        }

        LineOutMuteState = {
            'On': 'on',
            'Off': 'off'
        }

        channel = qualifier['Channel']
        if 1 <= int(channel) <= 2:
            channel = ChannelStates[channel]
            LineOutMuteCmdString = 'audio {0} mute {1}\r'.format(channel, LineOutMuteState[value])
            self.__SetHelper('LineOutMute', LineOutMuteCmdString, value, qualifier)
        else:
            self.Discard('Invalid Command for SetLineOutMute')

    def UpdateLineOutMute(self, value, qualifier):

        ChannelStates = {
            '1': 'line_out_1',
            '2': 'line_out_2',
        }

        LineOutMuteState = {
            'on': 'On',
            'off': 'Off'
        }

        channel = qualifier['Channel']
        if 1 <= int(channel) <= 2:
            channel = ChannelStates[channel]
            LineOutMuteCmdString = 'audio {0} mute get\r'.format(channel)
            res = self.__UpdateHelper('LineOutMute', LineOutMuteCmdString, value, qualifier)
            if res:
                match = re.search('mute:[ ]{0,9}(on|off)\r', res)
                try:
                    value = LineOutMuteState[match.group(1)]
                except (KeyError, IndexError, AttributeError):
                    self.Error(['Line Out Mute : Invalid/Unexpected Response'])
                else:
                    self.WriteStatus('LineOutMute', value, qualifier)
        else:
            self.Discard('Invalid Command for UpdateLineOutMute')

    def SetLineOutVolume(self, value, qualifier):

        ChannelStates = {
            '1': 'line_out_1',
            '2': 'line_out_2'
        }

        ValueConstraints = {
            'Min': -50,
            'Max': 20
        }

        channel = qualifier['Channel']
        if 1 <= int(channel) <= 2 and ValueConstraints['Min'] <= value <= ValueConstraints['Max']:
            channel = ChannelStates[channel]
            LineOutVolumeCmdString = 'audio {0} volume set {1}\r'.format(channel, value)
            self.__SetHelper('LineOutVolume', LineOutVolumeCmdString, value, qualifier)
        else:
            self.Discard('Invalid Command for SetLineOutVolume')

    def UpdateLineOutVolume(self, value, qualifier):

        ChannelStates = {
            '1': 'line_out_1',
            '2': 'line_out_2',
        }

        channel = qualifier['Channel']
        if 1 <= int(channel) <= 2:
            channel = ChannelStates[channel]
            LineOutVolumeCmdString = 'audio {0} volume get\r'.format(channel)
            res = self.__UpdateHelper('LineOutVolume', LineOutVolumeCmdString, value, qualifier)
            if res:
                match = re.search('audio line_out_[12] volume get\r\nvolume:[ ]{0,9}(-?\d+)\.\d dB\r\n', res)
                try:
                    value = int(match.group(1))
                except (ValueError, IndexError, AttributeError):
                    self.Error(['Line Out Volume : Invalid/Unexpected Response'])
                else:
                    self.WriteStatus('LineOutVolume', value, qualifier)
        else:
            self.Discard('Invalid Command for UpdateLineOutVolume')

    def SetMasterMute(self, value, qualifier):

        MasterMuteState = {
            'On': 'on',
            'Off': 'off'
        }

        MasterMuteCmdString = 'audio master mute {0}\r'.format(MasterMuteState[value])
        self.__SetHelper('MasterMute', MasterMuteCmdString, value, qualifier)

    def UpdateMasterMute(self, value, qualifier):

        MasterMuteState = {
            'on': 'On',
            'off': 'Off'
        }

        MasterMuteCmdString = 'audio master mute get\r'
        res = self.__UpdateHelper('MasterMute', MasterMuteCmdString, value, qualifier)
        if res:
            match = re.search('mute:[ ]{0,9}(on|off)\r', res)
            try:
                value = MasterMuteState[match.group(1)]
            except (KeyError, IndexError, AttributeError):
                self.Error(['Master Mute : Invalid/Unexpected Response'])
            else:
                self.WriteStatus('MasterMute', value, qualifier)

    def SetMasterVolume(self, value, qualifier):

        ValueConstraints = {
            'Min': -50,
            'Max': 20
        }

        if ValueConstraints['Min'] <= value <= ValueConstraints['Max']:
            MasterVolumeCmdString = 'audio master volume set {0}\r'.format(value)
            self.__SetHelper('MasterVolume', MasterVolumeCmdString, value, qualifier)
        else:
            self.Discard('Invalid Command for SetMasterVolume')

    def UpdateMasterVolume(self, value, qualifier):

        MasterVolumeCmdString = 'audio master volume get\r'
        res = self.__UpdateHelper('MasterVolume', MasterVolumeCmdString, value, qualifier)
        if res:
            match = re.search('audio master volume get\r\nvolume:[ ]{0,9}(-?\d+)\.\d dB\r\n', res)
            try:
                value = int(match.group(1))
            except (ValueError, IndexError, AttributeError):
                self.Error(['Master Volume : Invalid/Unexpected Response'])
            else:
                self.WriteStatus('MasterVolume', value, qualifier)

    def SetPan(self, value, qualifier):

        PanSpeedConstraints = {
            'Min': 1,
            'Max': 24
        }

        PanState = {
            'Left': 'left',
            'Right': 'right',
            'Stop': 'stop'
        }

        camera = int(qualifier['Camera'])
        panspeed = int(qualifier['Speed'])

        if PanSpeedConstraints['Min'] <= panspeed <= PanSpeedConstraints['Max'] and 1 <= camera <= 8:
            if value == 'Stop':
                PanCmdString = 'camera {0} pan stop\r'.format(camera)
            else:
                PanCmdString = 'camera {0} pan {1} {2}\r'.format(camera, PanState[value], panspeed)
            self.__SetHelper('Pan', PanCmdString, value, qualifier)
        else:
            self.Discard('Invalid Command for SetPan')

    def SetPresetRecall(self, value, qualifier):

        ValueConstraints = {
            'Min': 1,
            'Max': 16
        }

        camera = int(qualifier['Camera'])

        if ValueConstraints['Min'] <= int(value) <= ValueConstraints['Max'] and 1 <= camera <= 8:
            PresetRecallCmdString = 'camera {0} preset recall {1}\r'.format(camera, value)
            self.__SetHelper('PresetRecall', PresetRecallCmdString, value, qualifier)
        else:
            self.Discard('Invalid Command for SetPresetRecall')

    def SetPresetSave(self, value, qualifier):

        ValueConstraints = {
            'Min': 1,
            'Max': 16
        }

        camera = int(qualifier['Camera'])

        if ValueConstraints['Min'] <= int(value) <= ValueConstraints['Max'] and 1 <= camera <= 8:
            PresetSaveCmdString = 'camera {0} preset store {1}\r'.format(camera, value)
            self.__SetHelper('PresetSave', PresetSaveCmdString, value, qualifier)
        else:
            self.Discard('Invalid Command for SetPresetSave')

    def SetStandby(self, value, qualifier):

        StandbyState = {
            'On': 'on',
            'Off': 'off'
        }

        camera = int(qualifier['Camera'])

        if 1 <= camera <= 8:
            StandbyCmdString = 'camera {0} standby {1}\r'.format(camera, StandbyState[value])
            self.__SetHelper('Standby', StandbyCmdString, value, qualifier)
        else:
            self.Discard('Invalid Command for SetStandby')

    def UpdateStandby(self, value, qualifier):

        StandbyState = {
            'on': 'On',
            'off': 'Off'
        }

        camera = qualifier['Camera']
        if 1 <= int(camera) <= 8:
            StandbyCmdString = 'camera {0} standby get\r'.format(camera)
            res = self.__UpdateHelper('Standby', StandbyCmdString, value, qualifier)
            if res:
                match = re.search('standby:[ ]{0,9}(on|off)\r', res)
                try:
                    value = StandbyState[match.group(1)]
                except (KeyError, IndexError, AttributeError):
                    self.Error(['Standby : Invalid/Unexpected Response'])
                else:
                    self.WriteStatus('Standby', value, qualifier)
        else:
            self.Discard('Invalid Command for UpdateStandby')

    def SetTilt(self, value, qualifier):

        TiltState = {
            'Up': 'up',
            'Down': 'down',
            'Stop': 'stop'
        }

        camera = int(qualifier['Camera'])
        tiltspeed = int(qualifier['Speed'])

        if 1 <= camera <= 8 and 1 <= tiltspeed <= 20:
            if value == 'Stop':
                TiltCmdString = 'camera {0} tilt stop\r'.format(camera)
            else:
                TiltCmdString = 'camera {0} tilt {1} {2}\r'.format(camera, TiltState[value], tiltspeed)
            self.__SetHelper('Tilt', TiltCmdString, value, qualifier)
        else:
            self.Discard('Invalid Command for SetTilt')

    def SetVideoMute(self, value, qualifier):

        InputState = {
            '1': 'input1',
            '2': 'input2',
            '3': 'input3',
            '4': 'input4',
            '5': 'input5',
            '6': 'input6',
            '7': 'input7',
            '8': 'input8',
            'Program': 'program',
            'Preview': 'preview',
            'USB Stream': 'usb_stream',
            'IP Stream': 'ip_stream'
        }

        VideoMuteState = {
            'On': 'on',
            'Off': 'off'
        }

        VideoInput = qualifier['Input']
        if 1 <= int(VideoInput) <= 8:
            VideoInput = InputState[VideoInput]
            VideoMuteCmdString = 'video {0} mute {1}\r'.format(VideoInput, VideoMuteState[value])
            self.__SetHelper('VideoMute', VideoMuteCmdString, value, qualifier)
        else:
            self.Discard('Invalid Command for SetVideoMute')

    def UpdateVideoMute(self, value, qualifier):

        InputState = {
            '1': 'input1',
            '2': 'input2',
            '3': 'input3',
            '4': 'input4',
            '5': 'input5',
            '6': 'input6',
            '7': 'input7',
            '8': 'input8',
            'Program': 'program',
            'Preview': 'preview',
            'USB Stream': 'usb_stream',
            'IP Stream': 'ip_stream',
        }

        VideoMuteState = {
            'on': 'On',
            'off': 'Off'
        }

        VideoInput = qualifier['Input']

        VideoInput = InputState[VideoInput]
        VideoMuteCmdString = 'video {0} mute get\r'.format(VideoInput)
        res = self.__UpdateHelper('VideoMute', VideoMuteCmdString, value, qualifier)
        if res:
            match = re.search('mute:[ ]{0,9}(on|off)\r', res)
            try:
                value = VideoMuteState[match.group(1)]
            except (KeyError, IndexError, AttributeError):
                self.Error(['Video Mute : Invalid/Unexpected Response'])
            else:
                self.WriteStatus('VideoMute', value, qualifier)

    def SetVideoMuteAll(self, value, qualifier):

        VideoMuteAllState = {
            'On': 'on',
            'Off': 'off'
        }

        VideoMuteAllCmdString = 'video master mute {0}\r'.format(VideoMuteAllState[value])
        self.__SetHelper('VideoMuteAll', VideoMuteAllCmdString, value, qualifier)

    def UpdateVideoMuteAll(self, value, qualifier):

        VideoMuteAllState = {
            'on': 'On',
            'off': 'Off'
        }

        VideoMuteAllCmdString = 'video master mute get\r'
        res = self.__UpdateHelper('VideoMuteAll', VideoMuteAllCmdString, value, qualifier)
        if res:
            match = re.search('mute:[ ]{0,9}(on|off)\r', res)
            try:
                value = VideoMuteAllState[match.group(1)]
            except (KeyError, IndexError, AttributeError):
                self.Error(['Video Mute All : Invalid/Unexpected Response'])
            else:
                self.WriteStatus('VideoMuteAll', value, qualifier)

    def SetVideoPIP(self, value, qualifier):

        ChannelStates = {
            'Program': 'program',
            'Preview': 'preview'
        }

        VideoPIPState = {
            '1': 'input1',
            '2': 'input2',
            '3': 'input3',
            '4': 'input4',
            '5': 'input5',
            '6': 'input6',
            '7': 'input7',
            '8': 'input8'
        }

        channel = qualifier['Channel']
        if channel in ChannelStates:
            channel = ChannelStates[channel]
            VideoPIPCmdString = 'video {0} pip source {1}\r'.format(channel, VideoPIPState[value])
            self.__SetHelper('VideoPIP', VideoPIPCmdString, value, qualifier)
        else:
            self.Discard('Invalid Command for SetVideoPIP')

    def UpdateVideoPIP(self, value, qualifier):

        ChannelStates = {
            'Program': 'program',
            'Preview': 'preview',
        }

        VideoPIPState = {
            'input1': '1',
            'input2': '2',
            'input3': '3',
            'input4': '4',
            'input5': '5',
            'input6': '6',
            'input7': '7',
            'input8': '8'
        }

        channel = qualifier['Channel']
        if channel in ChannelStates:
            channel = ChannelStates[channel]
            VideoPIPCmdString = 'video {0} pip get\r'.format(channel)
            res = self.__UpdateHelper('VideoPIP', VideoPIPCmdString, value, qualifier)
            if res:
                match = re.search('source:[ ]{0,9}(input[1-8])\r', res)
                try:
                    value = VideoPIPState[match.group(1)]
                except (KeyError, IndexError, AttributeError):
                    self.Error(['Video PIP : Invalid/Unexpected Response'])
                else:
                    self.WriteStatus('VideoPIP', value, qualifier)
        else:
            self.Discard('Invalid Command for UpdateVideoPIP')

    def SetVideoPIPOff(self, value, qualifier):

        VideoPIPOffState = {
            'Program': 'program',
            'Preview': 'preview'
        }

        VideoPIPOffCmdString = 'video {0} pip off\r'.format(VideoPIPOffState[value])
        self.__SetHelper('VideoPIPOff', VideoPIPOffCmdString, value, qualifier)

    def SetVideoSource(self, value, qualifier):

        ChannelStates = {
            'Program': 'program',
            'Preview': 'preview',
            'USB Stream': 'usb_stream',
            'IP Stream': 'ip_stream'
        }

        VideoSourceState = {
            '1': 'input1',
            '2': 'input2',
            '3': 'input3',
            '4': 'input4',
            '5': 'input5',
            '6': 'input6',
            '7': 'input7',
            '8': 'input8',
            'Program': 'program',
            'Preview': 'preview',
            'Multiviewer': 'multiviewer'
        }

        channel = qualifier['Channel']
        if channel in ChannelStates:
            channel = ChannelStates[channel]
            VideoSourceCmdString = 'video {0} source set {1}\r'.format(channel, VideoSourceState[value])
            self.__SetHelper('VideoSource', VideoSourceCmdString, value, qualifier)
        else:
            self.Discard('Invalid Command in SetVideoSource')

    def UpdateVideoSource(self, value, qualifier):

        ChannelStates = {
            'Program': 'program',
            'Preview': 'preview',
            'USB Stream': 'usb_stream',
            'IP Stream': 'ip_stream',
        }

        VideoSourceState = {
            'input1': '1',
            'input2': '2',
            'input3': '3',
            'input4': '4',
            'input5': '5',
            'input6': '6',
            'input7': '7',
            'input8': '8',
            'program': 'Program',
            'preview': 'Preview',
            'multiviewer': 'Multiviewer'
        }

        channel = qualifier['Channel']
        if channel in ChannelStates:
            channel = ChannelStates[channel]
            VideoSourceCmdString = 'video {0} source get\r'.format(channel)
            res = self.__UpdateHelper('VideoSource', VideoSourceCmdString, value, qualifier)
            if res:
                match = re.search('source:[ ]{0,9}(input[1-8]|program|preview|multiviewer)\r', res)
                try:
                    value = VideoSourceState[match.group(1)]
                except (KeyError, IndexError, AttributeError):
                    self.Error(['Video Source : Invalid/Unexpected Response'])
                else:
                    self.WriteStatus('VideoSource', value, qualifier)
        else:
            self.Discard('Invalid Command for UpdateVideoSource')

    def SetZoom(self, value, qualifier):

        ZoomState = {
            'In': 'in',
            'Out': 'out',
            'Stop': 'stop'
        }

        camera = int(qualifier['Camera'])
        zoomspeed = int(qualifier['Speed'])

        if 1 <= camera <= 8 and 1 <= zoomspeed <= 7:
            if value == 'Stop':
                ZoomCmdString = 'camera {0} zoom stop\r'.format(camera)
            else:
                ZoomCmdString = 'camera {0} zoom {1} {2}\r'.format(camera, ZoomState[value], zoomspeed)
            self.__SetHelper('Zoom', ZoomCmdString, value, qualifier)
        else:
            self.Discard('Invalid Command for SetZoom')

    def __CheckResponseForErrors(self, sourceCmdName, response):

        if 'login:' in response:
            self.Send(self.deviceUsername + '\r\n')
        elif 'Password:' in response:
            self.Send(self.devicePassword + '\r\n')
        elif 'ERROR\r\n' in response:
            self.Error(['Invalid/Unexpected Response: {0}'.format(sourceCmdName)])
        else:
            return response

    def __SetHelper(self, command, commandstring, value, qualifier):
        self.Debug = True

        if self.Unidirectional == 'True':
            self.Send(commandstring)
        else:
            res = self.SendAndWait(commandstring, self.DefaultResponseTimeout, deliTag=b'\r')
            if not res:
                self.Error(['{0} : Invalid/Unexpected Response'.format(command)])
            else:
                res = self.__CheckResponseForErrors(command, res.decode())

    def __UpdateHelper(self, command, commandstring, value, qualifier):

        if self.Unidirectional == 'True':
            self.Discard('Inappropriate Command ' + command)
            return ''
        else:
            if self.initializationChk:
                self.OnConnected()
                self.initializationChk = False

            self.counter = self.counter + 1
            if self.counter > self.connectionCounter and self.connectionFlag:
                self.OnDisconnected()

            res = self.SendAndWait(commandstring, self.DefaultResponseTimeout, deliRex=self.AllResRegex)
            if not res:
                return ''
            else:
                return self.__CheckResponseForErrors(command, res.decode())

    def OnConnected(self):
        self.connectionFlag = True
        self.WriteStatus('ConnectionStatus', 'Connected')
        self.counter = 0

    def OnDisconnected(self):
        self.WriteStatus('ConnectionStatus', 'Disconnected')
        self.connectionFlag = False

    ######################################################
    # RECOMMENDED not to modify the code below this point
    ######################################################

    # Send Control Commands
    def Set(self, command, value, qualifier=None):
        method = getattr(self, 'Set%s' % command)
        if method is not None and callable(method):
            method(value, qualifier)
        else:
            print(command, 'does not support Set.')

    # Send Update Commands

    def Update(self, command, qualifier=None):
        method = getattr(self, 'Update%s' % command)
        if method is not None and callable(method):
            method(None, qualifier)
        else:
            print(command, 'does not support Update.')

    # This method is to tie an specific command with a parameter to a call back method
    # when its value is updated. It sets how often the command will be query, if the command
    # have the update method.
    # If the command doesn't have the update feature then that command is only used for feedback
    def SubscribeStatus(self, command, qualifier, callback):
        Command = self.Commands.get(command)
        if Command:
            if command not in self.Subscription:
                self.Subscription[command] = {'method': {}}

            Subscribe = self.Subscription[command]
            Method = Subscribe['method']

            if qualifier:
                for Parameter in Command['Parameters']:
                    try:
                        Method = Method[qualifier[Parameter]]
                    except BaseException:
                        if Parameter in qualifier:
                            Method[qualifier[Parameter]] = {}
                            Method = Method[qualifier[Parameter]]
                        else:
                            return

            Method['callback'] = callback
            Method['qualifier'] = qualifier
        else:
            print(command, 'does not exist in the module')

    # This method is to check the command with new status have a callback method then trigger the callback
    def NewStatus(self, command, value, qualifier):
        if command in self.Subscription:
            Subscribe = self.Subscription[command]
            Method = Subscribe['method']
            Command = self.Commands[command]
            if qualifier:
                for Parameter in Command['Parameters']:
                    try:
                        Method = Method[qualifier[Parameter]]
                    except BaseException:
                        break
            if 'callback' in Method and Method['callback']:
                Method['callback'](command, value, qualifier)

    # Save new status to the command
    def WriteStatus(self, command, value, qualifier=None):
        self.counter = 0
        if not self.connectionFlag:
            self.OnConnected()
        Command = self.Commands[command]
        Status = Command['Status']
        if qualifier:
            for Parameter in Command['Parameters']:
                try:
                    Status = Status[qualifier[Parameter]]
                except KeyError:
                    if Parameter in qualifier:
                        Status[qualifier[Parameter]] = {}
                        Status = Status[qualifier[Parameter]]
                    else:
                        return
        try:
            if Status['Live'] != value:
                Status['Live'] = value
                self.NewStatus(command, value, qualifier)
        except BaseException:
            Status['Live'] = value
            self.NewStatus(command, value, qualifier)

    # Read the value from a command.
    def ReadStatus(self, command, qualifier=None):
        Command = self.Commands[command]
        Status = Command['Status']
        if qualifier:
            for Parameter in Command['Parameters']:
                try:
                    Status = Status[qualifier[Parameter]]
                except KeyError:
                    return None
        try:
            return Status['Live']
        except BaseException:
            return None

    def __ReceiveData(self, interface, data):
        # Handle incoming data
        self.__receiveBuffer += data
        index = 0  # Start of possible good data

        # check incoming data if it matched any expected data from device module
        for regexString, CurrentMatch in self.__matchStringDict.items():
            while True:
                result = re.search(regexString, self.__receiveBuffer)
                if result:
                    index = result.start()
                    CurrentMatch['callback'](result, CurrentMatch['para'])
                    self.__receiveBuffer = self.__receiveBuffer[:result.start()] + self.__receiveBuffer[result.end():]
                else:
                    break

        if index:
            # Clear out any junk data that came in before any good matches.
            self.__receiveBuffer = self.__receiveBuffer[index:]
        else:
            # In rare cases, the buffer could be filled with garbage quickly.
            # Make sure the buffer is capped.  Max buffer size set in init.
            self.__receiveBuffer = self.__receiveBuffer[-self.__maxBufferSize:]

    # Add regular expression so that it can be check on incoming data from device.
    def AddMatchString(self, regex_string, callback, arg):
        if regex_string not in self.__matchStringDict:
            self.__matchStringDict[regex_string] = {'callback': callback, 'para': arg}

    def MissingCredentialsLog(self, credential_type):
        if isinstance(self, EthernetClientInterface):
            port_info = 'IP Address: {0}:{1}'.format(self.IPAddress, self.IPPort)
        elif isinstance(self, SerialInterface):
            port_info = 'Host Alias: {0}\r\nPort: {1}'.format(self.Host.DeviceAlias, self.Port)
        else:
            return
        ProgramLog("{0} module received a request from the device for a {1}, "
                   "but device{1} was not provided.\n Please provide a device{1} "
                   "and attempt again.\n Ex: dvInterface.device{1} = '{1}'\n Please "
                   "review the communication sheet.\n {2}"
                   .format(__name__, credential_type, port_info), 'warning')


class SerialClass(SerialInterface, DeviceClass):

    def __init__(self, Host, Port, Baud=38400, Data=8, Parity='None', Stop=1, FlowControl='Off', CharDelay=0, Mode='RS232', Model=None):
        SerialInterface.__init__(self, Host, Port, Baud, Data, Parity, Stop, FlowControl, CharDelay, Mode)
        self.ConnectionType = 'Serial'
        DeviceClass.__init__(self)
        # Check if Model belongs to a subclass
        if len(self.Models) > 0:
            if Model not in self.Models:
                print('Model mismatch')
            else:
                self.Models[Model]()

    def Error(self, message):
        portInfo = 'Host Alias: {0}, Port: {1}'.format(self.Host.DeviceAlias, self.Port)
        print('Module: {}'.format(__name__), portInfo, 'Error Message: {}'.format(message[0]), sep='\r\n')

    def Discard(self, message):
        self.Error([message])


class SerialOverEthernetClass(EthernetClientInterface, DeviceClass):

    def __init__(self, Hostname, IPPort, Protocol='TCP', ServicePort=0, Model=None):
        EthernetClientInterface.__init__(self, Hostname, IPPort, Protocol, ServicePort)
        self.ConnectionType = 'Serial'
        DeviceClass.__init__(self)
        # Check if Model belongs to a subclass
        if len(self.Models) > 0:
            if Model not in self.Models:
                print('Model mismatch')
            else:
                self.Models[Model]()

    def Error(self, message):
        portInfo = 'IP Address/Host: {0}:{1}'.format(self.Hostname, self.IPPort)
        print('Module: {}'.format(__name__), portInfo, 'Error Message: {}'.format(message[0]), sep='\r\n')

    def Discard(self, message):
        self.Error([message])

    def Disconnect(self):
        EthernetClientInterface.Disconnect(self)
        self.OnDisconnected()


class EthernetClass(EthernetClientInterface, DeviceClass):

    def __init__(self, Hostname, IPPort, Protocol='TCP', ServicePort=0, Model=None):
        EthernetClientInterface.__init__(self, Hostname, IPPort, Protocol, ServicePort)
        self.ConnectionType = 'Ethernet'
        DeviceClass.__init__(self)
        # Check if Model belongs to a subclass
        if len(self.Models) > 0:
            if Model not in self.Models:
                print('Model mismatch')
            else:
                self.Models[Model]()

    def Error(self, message):
        portInfo = 'IP Address/Host: {0}:{1}'.format(self.Hostname, self.IPPort)
        print('Module: {}'.format(__name__), portInfo, 'Error Message: {}'.format(message[0]), sep='\r\n')

    def Discard(self, message):
        self.Error([message])

    def Disconnect(self):
        EthernetClientInterface.Disconnect(self)
        self.OnDisconnected()
