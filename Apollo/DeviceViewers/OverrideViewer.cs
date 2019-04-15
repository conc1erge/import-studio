﻿using Avalonia.Controls;
using Avalonia.Markup.Xaml;

using Apollo.Components;
using Apollo.Core;
using Apollo.Devices;

namespace Apollo.DeviceViewers {
    public class OverrideViewer: UserControl {
        public static readonly string DeviceIdentifier = "override";

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
        
        Override _override;
        Dial Target;

        private void Update_Target(int value) {
            Target.RawValue = value + 1;
        }

        private void Update_Maximum(int value) {
            Target.Enabled = (value != 1);
            if (Target.Enabled) Target.Maximum = value;
        } 

        public OverrideViewer(Override o) {
            InitializeComponent();
            
            _override = o;
            _override.TargetChanged += Update_Target;
            Program.Project.TrackCountChanged += Update_Maximum;

            Target = this.Get<Dial>("Target");
            Update_Maximum(Program.Project.Tracks.Count);
            Update_Target(_override.Target);
        }

        private void Target_Changed(double value) => _override.Target = (int)value - 1;
    }
}
