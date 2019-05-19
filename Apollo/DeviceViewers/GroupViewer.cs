﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.VisualTree;

using Apollo.Binary;
using Apollo.Components;
using Apollo.Core;
using Apollo.Devices;
using Apollo.Elements;
using Apollo.Helpers;
using Apollo.Viewers;

namespace Apollo.DeviceViewers {
    public class GroupViewer: UserControl, IMultipleChainParentViewer, ISelectParentViewer {
        public static readonly string DeviceIdentifier = "group";

        public int? IExpanded {
            get => _group.Expanded;
        }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
        
        Group _group;
        DeviceViewer _parent;
        Controls _root;

        ContextMenu ChainContextMenu;

        Controls Contents;
        VerticalAdd ChainAdd;

        private void SetAlwaysShowing() {
            ChainAdd.AlwaysShowing = (Contents.Count == 1);

            for (int i = 1; i < Contents.Count; i++)
                ((ChainInfo)Contents[i]).ChainAdd.AlwaysShowing = false;

            if (Contents.Count > 1) ((ChainInfo)Contents.Last()).ChainAdd.AlwaysShowing = true;
        }

        public void Contents_Insert(int index, Chain chain) {
            ChainInfo viewer = new ChainInfo(chain);
            viewer.ChainAdded += Chain_Insert;
            viewer.ChainRemoved += Chain_Remove;
            viewer.ChainExpanded += Expand;
            chain.Info = viewer;

            Contents.Insert(index + 1, viewer);
            SetAlwaysShowing();

            if (IsArrangeValid && _group.Expanded != null && index <= _group.Expanded) _group.Expanded++;
        }

        public void Contents_Remove(int index) {
            if (IsArrangeValid && _group.Expanded != null) {
                if (index < _group.Expanded) _group.Expanded--;
                else if (index == _group.Expanded) Expand(null);
            }

            _group[index].Info = null;
            Contents.RemoveAt(index + 1);
            SetAlwaysShowing();
        }

        public GroupViewer(Group group, DeviceViewer parent) {
            InitializeComponent();

            _group = group;
            _parent = parent;
            _root = _parent.Root.Children;

            ChainContextMenu = (ContextMenu)this.Resources["ChainContextMenu"];
            ChainContextMenu.AddHandler(MenuItem.ClickEvent, new EventHandler(ChainContextMenu_Click));

            this.AddHandler(DragDrop.DropEvent, Drop);
            this.AddHandler(DragDrop.DragOverEvent, DragOver);

            Contents = this.Get<StackPanel>("Contents").Children;
            ChainAdd = this.Get<VerticalAdd>("ChainAdd");
            
            for (int i = 0; i < _group.Count; i++) {
                _group[i].ClearParentIndexChanged();
                Contents_Insert(i, _group[i]);
            }

            if (_group.Expanded != null) Expand_Insert(_group.Expanded.Value);
        }

        private void Expand_Insert(int index) {
            _root.Insert(1, new ChainViewer(_group[index], true));
            _root.Insert(2, new DeviceTail(_parent));

            _parent.Border.CornerRadius = new CornerRadius(5, 0, 0, 5);
            _parent.Header.CornerRadius = new CornerRadius(5, 0, 0, 0);
            ((ChainInfo)Contents[index + 1]).Get<TextBlock>("Name").FontWeight = FontWeight.Bold;
        }

        private void Expand_Remove() {
            _root.RemoveAt(2);
            _root.RemoveAt(1);

            _parent.Border.CornerRadius = new CornerRadius(5);
            _parent.Header.CornerRadius = new CornerRadius(5, 5, 0, 0);
            ((ChainInfo)Contents[_group.Expanded.Value + 1]).Get<TextBlock>("Name").FontWeight = FontWeight.Normal;
        }

        public void Expand(int? index) {
            if (_group.Expanded != null) {
                Expand_Remove();

                if (index == _group.Expanded) {
                    _group.Expanded = null;
                    return;
                }
            }

            if (index != null) Expand_Insert(index.Value);
            
            _group.Expanded = index;
        }

        private void Chain_Insert(int index) {
            Chain chain = new Chain();
            if (Preferences.AutoCreatePageFilter) chain.Add(new PageFilter());
            if (Preferences.AutoCreateKeyFilter) chain.Add(new KeyFilter());

            Chain_Insert(index, chain);
        }

        private void Chain_InsertStart() => Chain_Insert(0);

        private void Chain_Insert(int index, Chain chain) {
            Chain r = chain.Clone();
            List<int> path = Track.GetPath(_group);

            Program.Project.Undo.Add($"Chain Added", () => {
                ((Group)Track.TraversePath(path)).Remove(index);
            }, () => {
                ((Group)Track.TraversePath(path)).Insert(index, r.Clone());
            });

            _group.Insert(index, chain);
        }

        private void Chain_Remove(int index) {
            Chain u = _group[index].Clone();
            List<int> path = Track.GetPath(_group);

            Program.Project.Undo.Add($"Chain Deleted", () => {
                ((Group)Track.TraversePath(path)).Insert(index, u.Clone());
            }, () => {
                ((Group)Track.TraversePath(path)).Remove(index);
            });

            _group.Remove(index);
        }

        private void Chain_Action(string action) => Chain_Action(action, false);
        private void Chain_Action(string action, bool right) => Track.Get(_group).Window?.Selection.Action(action, _group, (right? _group.Count : 0) - 1);

        private void ChainContextMenu_Click(object sender, EventArgs e) {
            ((Window)this.GetVisualRoot()).Focus();
            IInteractive item = ((RoutedEventArgs)e).Source;

            if (item.GetType() == typeof(MenuItem))
                Chain_Action((string)((MenuItem)item).Header, true);
        }

        private void Click(object sender, PointerReleasedEventArgs e) {
            if (e.MouseButton == MouseButton.Right)
                ChainContextMenu.Open((Control)sender);

            e.Handled = true;
        }

        private void DragOver(object sender, DragEventArgs e) {
            e.Handled = true;
            if (!e.Data.Contains("chain") && !e.Data.Contains("device")) e.DragEffects = DragDropEffects.None; 
        }

        private void Drop(object sender, DragEventArgs e) {
            e.Handled = true;
            
            IControl source = (IControl)e.Source;
            while (source.Name != "DropZoneAfter" && source.Name != "ChainAdd")
                source = source.Parent;

            bool copy = e.Modifiers.HasFlag(InputModifiers.Control);
            bool result;

            if (e.Data.Contains("chain")) {
                List<Chain> moving = ((List<ISelect>)e.Data.Get("chain")).Select(i => (Chain)i).ToList();

                IMultipleChainParent source_parent = (IMultipleChainParent)moving[0].Parent;

                int before = moving[0].IParentIndex.Value - 1;
                int after = (source.Name == "DropZoneAfter")? _group.Count - 1 : -1;

                if (result = Chain.Move(moving, _group, after, copy)) {
                    int before_pos = before;
                    int after_pos = moving[0].IParentIndex.Value - 1;
                    int count = moving.Count;

                    if (source_parent == _group && after < before)
                        before_pos += count;
                    
                    List<int> sourcepath = Track.GetPath((ISelect)source_parent);
                    List<int> targetpath = Track.GetPath((ISelect)_group);
                    
                    Program.Project.Undo.Add(copy? $"Chain Copied" : $"Chain Moved", copy
                        ? new Action(() => {
                            IMultipleChainParent targetdevice = ((IMultipleChainParent)Track.TraversePath(targetpath));

                            for (int i = after + count; i > after; i--)
                                targetdevice.Remove(i);

                        }) : new Action(() => {
                            IMultipleChainParent sourcedevice = ((IMultipleChainParent)Track.TraversePath(sourcepath));
                            IMultipleChainParent targetdevice = ((IMultipleChainParent)Track.TraversePath(targetpath));

                            List<Chain> umoving = (from i in Enumerable.Range(after_pos + 1, count) select targetdevice[i]).ToList();

                            Chain.Move(umoving, sourcedevice, before_pos, copy);

                    }), () => {
                        IMultipleChainParent sourcedevice = ((IMultipleChainParent)Track.TraversePath(sourcepath));
                        IMultipleChainParent targetdevice = ((IMultipleChainParent)Track.TraversePath(targetpath));

                        List<Chain> rmoving = (from i in Enumerable.Range(before + 1, count) select sourcedevice[i]).ToList();

                        Chain.Move(rmoving, targetdevice, after);
                    });
                }
            
            } else if (e.Data.Contains("device")) {
                List<Device> moving = ((List<ISelect>)e.Data.Get("device")).Select(i => (Device)i).ToList();

                Chain source_chain = moving[0].Parent;
                Chain target_chain;

                int before = moving[0].IParentIndex.Value - 1;
                int after = -1;

                int? remove = null;

                if (source.Name != "DropZoneAfter") {
                    _group.Insert((remove = 0).Value);
                    target_chain = _group[0];
                } else {
                    _group.Insert((remove = _group.Count).Value);
                    target_chain = _group.Chains.Last();
                }

                if (result = Device.Move(moving, target_chain, after = target_chain.Count - 1, copy)) {
                    int before_pos = before;
                    int after_pos = moving[0].IParentIndex.Value - 1;
                    int count = moving.Count;

                    if (source_chain == target_chain && after < before)
                        before_pos += count;
                    
                    List<int> sourcepath = Track.GetPath(source_chain);
                    List<int> targetpath = Track.GetPath(target_chain);
                    
                    Program.Project.Undo.Add(copy? $"Device Copied" : $"Device Moved", copy
                        ? new Action(() => {
                            Chain targetchain = ((Chain)Track.TraversePath(targetpath));

                            for (int i = after + count; i > after; i--)
                                targetchain.Remove(i);
                            
                            if (remove != null)
                                ((IMultipleChainParent)targetchain.Parent).Remove(remove.Value);

                        }) : new Action(() => {
                            Chain sourcechain = ((Chain)Track.TraversePath(sourcepath));
                            Chain targetchain = ((Chain)Track.TraversePath(targetpath));

                            List<Device> umoving = (from i in Enumerable.Range(after_pos + 1, count) select targetchain[i]).ToList();

                            Device.Move(umoving, sourcechain, before_pos);

                            if (remove != null)
                                ((IMultipleChainParent)targetchain.Parent).Remove(remove.Value);

                    }), () => {
                        Chain sourcechain = ((Chain)Track.TraversePath(sourcepath));
                        Chain targetchain;

                        if (remove != null) {
                            IMultipleChainParent target = ((IMultipleChainParent)Track.TraversePath(targetpath.Skip(1).ToList()));
                            target.Insert(remove.Value);
                            targetchain = target[remove.Value];
                        
                        } else targetchain = ((Chain)Track.TraversePath(targetpath));

                        List<Device> rmoving = (from i in Enumerable.Range(before + 1, count) select sourcechain[i]).ToList();

                        Device.Move(rmoving, targetchain, after, copy);
                    });
                }

            } else return;

            if (!result) e.DragEffects = DragDropEffects.None;
        }

        public async void Copy(int left, int right, bool cut = false) {
            Copyable copy = new Copyable();
            
            for (int i = left; i <= right; i++)
                copy.Contents.Add(_group[i]);

            string b64 = Convert.ToBase64String(Encoder.Encode(copy).ToArray());

            if (cut) Delete(left, right);
            
            await Application.Current.Clipboard.SetTextAsync(b64);
        }

        public async void Paste(int right) {
            string b64 = await Application.Current.Clipboard.GetTextAsync();
            
            Copyable paste = Decoder.Decode(new MemoryStream(Convert.FromBase64String(b64)), typeof(Copyable));

            List<int> path = Track.GetPath(_group);

            Program.Project.Undo.Add($"Chain Pasted", () => {
                for (int i = 0; i < paste.Contents.Count; i++)
                    ((Group)Track.TraversePath(path)).Remove(right + i + 1);

            }, () => {
                for (int i = 0; i < paste.Contents.Count; i++)
                    ((Group)Track.TraversePath(path)).Insert(right + i + 1, ((Chain)paste.Contents[i]).Clone());
            });

            for (int i = 0; i < paste.Contents.Count; i++)
                _group.Insert(right + i + 1, ((Chain)paste.Contents[i]).Clone());
        }

        public void Duplicate(int left, int right) {
            List<int> path = Track.GetPath(_group);

            Program.Project.Undo.Add($"Chain Duplicated", () => {
                for (int i = 0; i <= right - left; i++)
                    ((Group)Track.TraversePath(path)).Remove(right + i + 1);

            }, () => {
                Group group = ((Group)Track.TraversePath(path));

                for (int i = 0; i <= right - left; i++)
                    group.Insert(right + i + 1, group[left + i].Clone());
            });

            for (int i = 0; i <= right - left; i++)
                _group.Insert(right + i + 1, _group[left + i].Clone());
        }

        public void Delete(int left, int right) {
            List<Chain> u = (from i in Enumerable.Range(left, right - left + 1) select _group[i].Clone()).ToList();

            List<int> path = Track.GetPath(_group);

            Program.Project.Undo.Add($"Chain Deleted", () => {
                for (int i = left; i <= right; i++)
                    ((Group)Track.TraversePath(path)).Insert(i, u[i - left].Clone());

            }, () => {
                for (int i = right; i >= left; i--)
                    ((Group)Track.TraversePath(path)).Remove(i);
            });

            for (int i = right; i >= left; i--)
                _group.Remove(i);
        }

        public void Group(int left, int right) => throw new InvalidOperationException("A Chain cannot be grouped.");

        public void Ungroup(int index) => throw new InvalidOperationException("A Chain cannot be ungrouped.");
        
        public void Rename(int left, int right) => ((ChainInfo)Contents[left + 1]).StartInput(left, right);
    }
}
