using GalaSoft.MvvmLight.Messaging;
using ScriptLib;
using System.Reflection;
using ScriptLib.Types;

namespace ScriptViz.ViewModel
{
    public class MoveListDesignViewModel : VMBase
    {
        public static MoveListDesignViewModel Instance => new MoveListDesignViewModel();

        #region Variables

        public MoveList SelectedMoveList { get; set; }

        public Move SelectedMove { get; set; }

        int _selectedMoveIndex = 611;
        public int SelectedMoveIndex
        {
            get => _selectedMoveIndex;
            set
            {
                if (value < 0) return;

                _selectedMoveIndex = value;
                RaisePropertyChanged(nameof(SelectedMoveIndex)); // Notifies connected UI elements that SelectedMoveIndex has changed
                RaisePropertyChanged(nameof(SelectedMove));

                //if (SelectedMoveList == null) SelectedMove = null;

                //int numberOfMoves = SelectedMoveList.Moves.Length;
                //if (numberOfMoves > 0 && SelectedMoveIndex.IsBetween(0, numberOfMoves - 1)) // "if index is valid"
                //    SelectedMove = SelectedMoveList.Moves[SelectedMoveIndex];
                //else
                //    SelectedMove = null;

                Messenger.Default.Send(SelectedMove);
            }
        }

        // PROPERTY
        public PropertyInfo SelectedProperty => (SelectedPropertyIndex <= 0) ? null : SelectedMove.GetAllProperties()[SelectedPropertyIndex];

        int _selectedPropertyIndex;
        public int SelectedPropertyIndex
        {
            get => _selectedPropertyIndex;
            set
            {
                _selectedPropertyIndex = (value <= 0) ? value : (value + SelectedMove.GetGeneralPropertiesOffset());
                RaisePropertyChanged(nameof(SelectedPropertyIndex));
                RaisePropertyChanged(nameof(SelectedProperty));
            }
        }

        #endregion // Variables

        #region Constructor

        public MoveListDesignViewModel()
        {
            SelectedMoveList = new MoveList();
            SelectedMove = new Move
            {
                Name="2MK",
                Index=611,
                FirstHitboxFrame = 6,
                LastHitboxFrame = 8,
                InterruptFrame = 24,
                TotalTicks = 45,
                ReturnToOriginalPosition = 7,
                Slide = 0.0F,
                unk3 = 0.0F,
                unk4 = 0.0F,
                unk5 = 0.0F,
                unk6 = 0.0F,
                unk7 = 0.0F,
                Flag = 65537,
                unk9 = 0,
                numberOfTypes = 8,
                unk13 = 0,
                HeaderSize = 64,
                Unknown12 = 0,
                Unknown13 = 0,
                Unknown14 = 0,
                Unknown15 = 0,
                Unknown16 = 0,
                Unknown17 = 0,
                Unknown18 = 0.0F,
                Unknown19 = 0,
                Unknown20 = 0,
                Unknown21 = 0,
                Unknown22 = 0,
                AutoCancels = null,
                Type1s = new []
                {
                    new Type1
                    {
                        TickStart = 0,
                        TickEnd = 6,
                        Flag1 = 290,
                        Flag2 = 0
                    },
                    new Type1
                    {
                        TickStart = 6,
                        TickEnd = 45,
                        Flag1 = 258,
                        Flag2 = 0
                    }
                },
                Forces = null,
                Cancels = new []
                {
                    new Cancel
                    {
                        TickStart = 4,
                        TickEnd = 12,
                        CancelList = 45,
                        Type = 3
                    },
                    new Cancel
                    {
                        TickStart = 8,
                        TickEnd = 12,
                        CancelList = 45,
                        Type = 8
                    },
                    new Cancel
                    {
                        TickStart = 1,
                        TickEnd = 8,
                        CancelList = 37,
                        Type = 3
                    },
                    new Cancel
                    {
                        TickStart = 6,
                        TickEnd = 8,
                        CancelList = 37,
                        Type = 8
                    },
                    new Cancel
                    {
                        TickStart = 1,
                        TickEnd = 8,
                        CancelList = 70,
                        Type = 3
                    },
                    new Cancel
                    {
                        TickStart = 6,
                        TickEnd = 8,
                        CancelList = 70,
                        Type = 8
                    }
                },
                Others = null,
                Hitboxes = new []
                {
                    new Hitbox
                    {
                        TickStart = 0,
                        TickEnd = 8,
                        X = 0.4F,
                        Y = 0.35F,
                        Z = 0.0F,
                        Width = 2.0F,
                        Height = 0.6F,
                        Unknown1 = 0,
                        Unknown2 = 25,
                        Unknown3 = 25,
                        Unknown4 = 25,
                        Unknown5 = 0,
                        Unknown6 = 72,
                        Unknown7 = 0,
                        Unknown8 = 0,
                        NumberOfHits = 1,
                        HitType = 4,
                        JuggleLimit = 1,
                        JuggleIncrease = 1,
                        Flag4 = 0,
                        HitboxEffectIndex = -1,
                        Unknown10 = 16,
                        Unknown11 = 0,
                        Unknown12 = 0
                    },
                    new Hitbox
                    {
                        TickStart = 6,
                        TickEnd = 7,
                        X = 0.65F,
                        Y = 0.55F,
                        Z = 0.0F,
                        Width = 0.5F,
                        Height = 0.37F,
                        Unknown1 = 0,
                        Unknown2 = 25,
                        Unknown3 = 25,
                        Unknown4 = 25,
                        Unknown5 = 0,
                        Unknown6 = 8,
                        Unknown7 = 128,
                        Unknown8 = 7680,
                        NumberOfHits = 513,
                        HitType = 0,
                        JuggleLimit = 0,
                        JuggleIncrease = 1,
                        Flag4 = 0,
                        HitboxEffectIndex = 22,
                        Unknown10 = 16,
                        Unknown11 = 0,
                        Unknown12 = 0
                    },
                    new Hitbox
                    {
                        TickStart = 7,
                        TickEnd = 8,
                        X = 0.65F,
                        Y = 0.43F,
                        Z = 0.0F,
                        Width = 0.5F,
                        Height = 0.4F,
                        Unknown1 = 0,
                        Unknown2 = 25,
                        Unknown3 = 25,
                        Unknown4 = 25,
                        Unknown5 = 0,
                        Unknown6 = 8,
                        Unknown7 = 128,
                        Unknown8 = 7680,
                        NumberOfHits = 513,
                        HitType = 0,
                        JuggleLimit = 0,
                        JuggleIncrease = 1,
                        Flag4 = 0,
                        HitboxEffectIndex = 22,
                        Unknown10 = 16,
                        Unknown11 = 0,
                        Unknown12 = 0
                    }
                },
                Hurtboxes = new []
                {
                    new Hurtbox
                    {
                        TickStart = 0,
                        TickEnd = 25,
                        X = -0.4F,
                        Y = 0.9F,
                        Z = 0.0F,
                        Width = 1.0F,
                        Height = 0.4F,
                        Unknown1 = 0,
                        Unknown2 = 25,
                        Unknown3 = 25,
                        Unknown4 = 25,
                        Unknown5 = 0,
                        Unknown6 = 0,
                        Unknown7 = 0,
                        Unknown8 = 0,
                        Unknown9 = 1,
                        Flag1 = 3,
                        Flag2 = 0,
                        Flag3 = 0,
                        Flag4 = 0,
                        HitEffect = 0,
                        Unknown10 = 3,
                        Unknown11 = 0,
                        Unknown12 = 1.0F,
                        Unknown13 = 0
                    },
                    new Hurtbox
                    {
                        TickStart = 0,
                        TickEnd = 25,
                        X = -0.4F,
                        Y = 0.0F,
                        Z = 0.0F,
                        Width = 1.0F,
                        Height = 0.9F,
                        Unknown1 = 0,
                        Unknown2 = 25,
                        Unknown3 = 25,
                        Unknown4 = 25,
                        Unknown5 = 0,
                        Unknown6 = 0,
                        Unknown7 = 0,
                        Unknown8 = 0,
                        Unknown9 = 1,
                        Flag1 = 3,
                        Flag2 = 0,
                        Flag3 = 0,
                        Flag4 = 0,
                        HitEffect = 0,
                        Unknown10 = 3,
                        Unknown11 = 0,
                        Unknown12 = 1.0F,
                        Unknown13 = 0
                    },
                    new Hurtbox
                    {
                        TickStart = 0,
                        TickEnd = 25,
                        X = -0.15F,
                        Y = 0.0F,
                        Z = 0.0F,
                        Width = 0.6F,
                        Height = 1.0F,
                        Unknown1 = 0,
                        Unknown2 = 25,
                        Unknown3 = 25,
                        Unknown4 = 25,
                        Unknown5 = 0,
                        Unknown6 = 0,
                        Unknown7 = 0,
                        Unknown8 = 0,
                        Unknown9 = 1,
                        Flag1 = 4,
                        Flag2 = 0,
                        Flag3 = 0,
                        Flag4 = 0,
                        HitEffect = 0,
                        Unknown10 = 3,
                        Unknown11 = 0,
                        Unknown12 = 1.0F,
                        Unknown13 = 0
                    },
                    new Hurtbox
                    {
                        TickStart = 5,
                        TickEnd = 6,
                        X = 0.6F,
                        Y = 0.32F,
                        Z = 0.0F,
                        Width = 0.35F,
                        Height = 0.88F,
                        Unknown1 = 0,
                        Unknown2 = 25,
                        Unknown3 = 25,
                        Unknown4 = 25,
                        Unknown5 = 0,
                        Unknown6 = 0,
                        Unknown7 = 0,
                        Unknown8 = 0,
                        Unknown9 = 1,
                        Flag1 = 3,
                        Flag2 = 0,
                        Flag3 = 0,
                        Flag4 = 0,
                        HitEffect = 0,
                        Unknown10 = 3,
                        Unknown11 = 0,
                        Unknown12 = 1.0F,
                        Unknown13 = 0
                    },
                    new Hurtbox
                    {
                        TickStart = 6,
                        TickEnd = 8,
                        X = 0.6F,
                        Y = 0.32F,
                        Z = 0.0F,
                        Width = 0.65F,
                        Height = 0.88F,
                        Unknown1 = 0,
                        Unknown2 = 25,
                        Unknown3 = 25,
                        Unknown4 = 25,
                        Unknown5 = 0,
                        Unknown6 = 0,
                        Unknown7 = 0,
                        Unknown8 = 0,
                        Unknown9 = 1,
                        Flag1 = 3,
                        Flag2 = 0,
                        Flag3 = 0,
                        Flag4 = 0,
                        HitEffect = 0,
                        Unknown10 = 3,
                        Unknown11 = 0,
                        Unknown12 = 1.0F,
                        Unknown13 = 0
                    },
                    new Hurtbox
                    {
                        TickStart = 8,
                        TickEnd = 10,
                        X = 0.6F,
                        Y = 0.0F,
                        Z = 0.0F,
                        Width = 0.45F,
                        Height = 1.3F,
                        Unknown1 = 0,
                        Unknown2 = 25,
                        Unknown3 = 25,
                        Unknown4 = 25,
                        Unknown5 = 0,
                        Unknown6 = 0,
                        Unknown7 = 0,
                        Unknown8 = 0,
                        Unknown9 = 1,
                        Flag1 = 3,
                        Flag2 = 0,
                        Flag3 = 0,
                        Flag4 = 0,
                        HitEffect = 0,
                        Unknown10 = 3,
                        Unknown11 = 0,
                        Unknown12 = 1.0F,
                        Unknown13 = 0
                    },
                    new Hurtbox
                    {
                        TickStart = 25,
                        TickEnd = 45,
                        X = -0.4F,
                        Y = 0.9F,
                        Z = 0.0F,
                        Width = 0.9F,
                        Height = 0.4F,
                        Unknown1 = 0,
                        Unknown2 = 25,
                        Unknown3 = 25,
                        Unknown4 = 25,
                        Unknown5 = 0,
                        Unknown6 = 0,
                        Unknown7 = 0,
                        Unknown8 = 0,
                        Unknown9 = 1,
                        Flag1 = 3,
                        Flag2 = 0,
                        Flag3 = 0,
                        Flag4 = 0,
                        HitEffect = 0,
                        Unknown10 = 3,
                        Unknown11 = 0,
                        Unknown12 = 1.0F,
                        Unknown13 = 0
                    },
                    new Hurtbox
                    {
                        TickStart = 25,
                        TickEnd = 45,
                        X = -0.4F,
                        Y = 0.0F,
                        Z = 0.0F,
                        Width = 0.9F,
                        Height = 0.9F,
                        Unknown1 = 0,
                        Unknown2 = 25,
                        Unknown3 = 25,
                        Unknown4 = 25,
                        Unknown5 = 0,
                        Unknown6 = 0,
                        Unknown7 = 0,
                        Unknown8 = 0,
                        Unknown9 = 1,
                        Flag1 = 3,
                        Flag2 = 0,
                        Flag3 = 0,
                        Flag4 = 0,
                        HitEffect = 0,
                        Unknown10 = 3,
                        Unknown11 = 0,
                        Unknown12 = 1.0F,
                        Unknown13 = 0
                    },
                    new Hurtbox
                    {
                        TickStart = 25,
                        TickEnd = 45,
                        X = -0.15F,
                        Y = 0.0F,
                        Z = 0.0F,
                        Width = 0.5F,
                        Height = 1.0F,
                        Unknown1 = 0,
                        Unknown2 = 25,
                        Unknown3 = 25,
                        Unknown4 = 25,
                        Unknown5 = 0,
                        Unknown6 = 0,
                        Unknown7 = 0,
                        Unknown8 = 0,
                        Unknown9 = 1,
                        Flag1 = 4,
                        Flag2 = 0,
                        Flag3 = 0,
                        Flag4 = 0,
                        HitEffect = 0,
                        Unknown10 = 3,
                        Unknown11 = 0,
                        Unknown12 = 1.0F,
                        Unknown13 = 0
                    }
                },
                PhysicsBoxes = new []
                {
                    new PhysicsBox
                    {
                        TickStart = 0,
                        TickEnd = 25,
                        X = -0.15F,
                        Y = 0.0F,
                        Z = 0.0F,
                        Width = 0.5F,
                        Height = 1.0F,
                        Unknown1 = 0,
                        Unknown2 = 25,
                        Unknown3 = 25,
                        Unknown4 = 25,
                        Unknown5 = 0,
                        Unknown6 = 1
                    },
                    new PhysicsBox
                    {
                        TickStart = 25,
                        TickEnd = 45,
                        X = -0.15F,
                        Y = 0.0F,
                        Z = 0.0F,
                        Width = 0.5F,
                        Height = 1.0F,
                        Unknown1 = 0,
                        Unknown2 = 25,
                        Unknown3 = 25,
                        Unknown4 = 25,
                        Unknown5 = 0,
                        Unknown6 = 1
                    }
                },
                Animations = new[]
                {
                    new Animation
                    {
                        TickStart = 0,
                        TickEnd = 6,
                        Index = 56,
                        Type = "Regular",
                        FrameStart = 0,
                        FrameEnd = 6,
                        Unknown1 = 0,
                        Unknown2 = 96
                    },
                    new Animation
                    {
                        TickStart = 0,
                        TickEnd = 4,
                        Index = 28,
                        Type = "Face",
                        FrameStart = 0,
                        FrameEnd = 0,
                        Unknown1 = 0,
                        Unknown2 = 80
                    },
                    new Animation
                    {
                        TickStart = 6,
                        TickEnd = 45,
                        Index = 56,
                        Type = "Regular",
                        FrameStart = 6,
                        FrameEnd = 45,
                        Unknown1 = 0,
                        Unknown2 = 64
                    },
                    new Animation
                    {
                        TickStart = 4,
                        TickEnd = 17,
                        Index = 11,
                        Type = "Face",
                        FrameStart = 0,
                        FrameEnd = 0,
                        Unknown1 = 0,
                        Unknown2 = 48
                    },
                    new Animation
                    {
                        TickStart = 17,
                        TickEnd = 25,
                        Index = 28,
                        Type = "Face",
                        FrameStart = 0,
                        FrameEnd = 0,
                        Unknown1 = 0,
                        Unknown2 = 32
                    },
                    new Animation
                    {
                        TickStart = 25,
                        TickEnd = 45,
                        Index = 0,
                        Type = "Face",
                        FrameStart = 0,
                        FrameEnd = 0,
                        Unknown1 = 0,
                        Unknown2 = 16
                    }
                },
                Type9s = null,
                SoundEffects = new[]
                {
                    new SoundEffect
                    {
                        TickStart = 3,
                        TickEnd = 4,
                        Unknown1 = 0,
                        Unknown2 = 101,
                        Unknown3 = 1,
                        Unknown4 = 0,
                        Unknown5 = 0,
                        Unknown6 = 0
                    },
                    new SoundEffect
                    {
                        TickStart = 4,
                        TickEnd = 5,
                        Unknown1 = 0,
                        Unknown2 = 1101,
                        Unknown3 = 2,
                        Unknown4 = 0,
                        Unknown5 = 0,
                        Unknown6 = 0
                    },
                    new SoundEffect
                    {
                        TickStart = 5,
                        TickEnd = 6,
                        Unknown1 = 0,
                        Unknown2 = 10,
                        Unknown3 = 3,
                        Unknown4 = 0,
                        Unknown5 = 0,
                        Unknown6 = 0
                    },
                    new SoundEffect
                    {
                        TickStart = 5,
                        TickEnd = 6,
                        Unknown1 = 0,
                        Unknown2 = 20,
                        Unknown3 = 3,
                        Unknown4 = 0,
                        Unknown5 = 0,
                        Unknown6 = 0
                    }
                },
                VisualEffects = null,
                Positions = new[]
                {
                    new Position
                    {
                        TickStart = 6,
                        TickEnd = 7,
                        Movement = 0.420376F,
                        Flag = 32768
                    },
                    new Position
                    {
                        TickStart = 7,
                        TickEnd = 8,
                        Movement = 0.446445F,
                        Flag = 32768
                    },
                    new Position
                    {
                        TickStart = 8,
                        TickEnd = 9,
                        Movement = 0.465011F,
                        Flag = 32768
                    },
                    new Position
                    {
                        TickStart = 9,
                        TickEnd = 10,
                        Movement = 0.483134F,
                        Flag = 32768
                    },
                    new Position
                    {
                        TickStart = 10,
                        TickEnd = 11,
                        Movement = 0.496938F,
                        Flag = 32768
                    },
                    new Position
                    {
                        TickStart = 11,
                        TickEnd = 12,
                        Movement = 0.504828F,
                        Flag = 32768
                    },
                    new Position
                    {
                        TickStart = 12,
                        TickEnd = 13,
                        Movement = 0.507428F,
                        Flag = 32768
                    },
                    new Position
                    {
                        TickStart = 13,
                        TickEnd = 14,
                        Movement = 0.507424F,
                        Flag = 32768
                    },
                    new Position
                    {
                        TickStart = 14,
                        TickEnd = 15,
                        Movement = 0.507343F,
                        Flag = 32768
                    },
                    new Position
                    {
                        TickStart = 15,
                        TickEnd = 16,
                        Movement = 0.505097F,
                        Flag = 32768
                    },
                    new Position
                    {
                        TickStart = 16,
                        TickEnd = 17,
                        Movement = 0.491164F,
                        Flag = 32768
                    },
                    new Position
                    {
                        TickStart = 17,
                        TickEnd = 18,
                        Movement = 0.454741F,
                        Flag = 32768
                    },
                    new Position
                    {
                        TickStart = 18,
                        TickEnd = 19,
                        Movement = 0.395846F,
                        Flag = 32768
                    },
                    new Position
                    {
                        TickStart = 19,
                        TickEnd = 20,
                        Movement = 0.272889F,
                        Flag = 32768
                    },
                    new Position
                    {
                        TickStart = 20,
                        TickEnd = 21,
                        Movement = 0.122928F,
                        Flag = 32768
                    },
                    new Position
                    {
                        TickStart = 21,
                        TickEnd = 22,
                        Movement = 0.026637F,
                        Flag = 32768
                    },
                    new Position
                    {
                        TickStart = 22,
                        TickEnd = 23,
                        Movement = 0.0F,
                        Flag = 32768
                    },
                    new Position
                    {
                        TickStart = 23,
                        TickEnd = 24,
                        Movement = 0.0F,
                        Flag = 32768
                    },
                    new Position
                    {
                        TickStart = 24,
                        TickEnd = 25,
                        Movement = 0.0F,
                        Flag = 32768
                    },
                    new Position
                    {
                        TickStart = 25,
                        TickEnd = 26,
                        Movement = 0.0F,
                        Flag = 32768
                    },
                    new Position
                    {
                        TickStart = 26,
                        TickEnd = 27,
                        Movement = 0.0F,
                        Flag = 32768
                    },
                    new Position
                    {
                        TickStart = 27,
                        TickEnd = 28,
                        Movement = 0.0F,
                        Flag = 32768
                    },
                    new Position
                    {
                        TickStart = 28,
                        TickEnd = 29,
                        Movement = 0.0F,
                        Flag = 32768
                    },
                    new Position
                    {
                        TickStart = 29,
                        TickEnd = 30,
                        Movement = 0.0F,
                        Flag = 32768
                    },
                    new Position
                    {
                        TickStart = 30,
                        TickEnd = 31,
                        Movement = 0.0F,
                        Flag = 32768
                    },
                    new Position
                    {
                        TickStart = 31,
                        TickEnd = 32,
                        Movement = 0.0F,
                        Flag = 32768
                    },
                    new Position
                    {
                        TickStart = 32,
                        TickEnd = 33,
                        Movement = 0.0F,
                        Flag = 32768
                    },
                    new Position
                    {
                        TickStart = 33,
                        TickEnd = 34,
                        Movement = 0.0F,
                        Flag = 32768
                    },
                    new Position
                    {
                        TickStart = 34,
                        TickEnd = 35,
                        Movement = 0.0F,
                        Flag = 32768
                    },
                    new Position
                    {
                        TickStart = 35,
                        TickEnd = 36,
                        Movement = 0.0F,
                        Flag = 32768
                    },
                    new Position
                    {
                        TickStart = 36,
                        TickEnd = 37,
                        Movement = 0.0F,
                        Flag = 32768
                    },
                    new Position
                    {
                        TickStart = 37,
                        TickEnd = 38,
                        Movement = 0.0F,
                        Flag = 32768
                    },
                    new Position
                    {
                        TickStart = 38,
                        TickEnd = 39,
                        Movement = 0.0F,
                        Flag = 32768
                    },
                    new Position
                    {
                        TickStart = 39,
                        TickEnd = 40,
                        Movement = 0.0F,
                        Flag = 32768
                    },
                    new Position
                    {
                        TickStart = 40,
                        TickEnd = 41,
                        Movement = 0.0F,
                        Flag = 32768
                    },
                    new Position
                    {
                        TickStart = 41,
                        TickEnd = 42,
                        Movement = 0.0F,
                        Flag = 32768
                    },
                    new Position
                    {
                        TickStart = 42,
                        TickEnd = 43,
                        Movement = 0.0F,
                        Flag = 32768
                    },
                    new Position
                    {
                        TickStart = 43,
                        TickEnd = 44,
                        Movement = 0.0F,
                        Flag = 32768
                    },
                    new Position
                    {
                        TickStart = 44,
                        TickEnd = 45,
                        Movement = 0.0F,
                        Flag = 32768
                    },
                    new Position
                    {
                        TickStart = 0,
                        TickEnd = 1,
                        Movement = 0.000844F,
                        Flag = 32768
                    },
                    new Position
                    {
                        TickStart = 1,
                        TickEnd = 2,
                        Movement = 0.030805F,
                        Flag = 32768
                    },
                    new Position
                    {
                        TickStart = 2,
                        TickEnd = 3,
                        Movement = 0.120384F,
                        Flag = 32768
                    },
                    new Position
                    {
                        TickStart = 3,
                        TickEnd = 4,
                        Movement = 0.227335F,
                        Flag = 32768
                    },
                    new Position
                    {
                        TickStart = 4,
                        TickEnd = 5,
                        Movement = 0.298741F,
                        Flag = 32768
                    },
                    new Position
                    {
                        TickStart = 5,
                        TickEnd = 6,
                        Movement = 0.362065F,
                        Flag = 32768
                    }
                }
            };

            SelectedMoveList.Moves = new [] { SelectedMove, SelectedMove, SelectedMove };
            SelectedMoveList.Unknown1 = 0;

            SelectedPropertyIndex = 4;
        }

        #endregion

        #region Load MoveList



        #endregion

    }
}
