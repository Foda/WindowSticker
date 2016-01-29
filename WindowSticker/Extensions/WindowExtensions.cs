﻿using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace WindowSticker
{
    /// <summary>
    /// Used from EarTrumped
    /// </summary>
    internal static class WindowExtensions
    {
        private static bool _windowVisible;

        public static bool IsWindowVisible(this Window window)
        {
            return _windowVisible;
        }

        public static void HideWithAnimation(this Window window)
        {
            var hideAnimation = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(0.2)),
                FillBehavior = FillBehavior.Stop,
                EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseInOut }
            };

            var taskbarPosition = TaskbarService.TaskbarPosition;
            switch (taskbarPosition)
            {
                case TaskbarPosition.Left:
                case TaskbarPosition.Right:
                    hideAnimation.From = window.Left;
                    break;
                default:
                    hideAnimation.From = window.Top;
                    break;
            }
            hideAnimation.To = (taskbarPosition == TaskbarPosition.Top || taskbarPosition == TaskbarPosition.Left) ? hideAnimation.From - 10 : hideAnimation.From + 10;
            hideAnimation.Completed += (s, e) =>
            {
                window.Visibility = Visibility.Hidden;
            };

            switch (taskbarPosition)
            {
                case TaskbarPosition.Left:
                case TaskbarPosition.Right:
                    window.ApplyAnimationClock(Window.LeftProperty, hideAnimation.CreateClock());
                    break;
                default:
                    window.ApplyAnimationClock(Window.TopProperty, hideAnimation.CreateClock());
                    break;
            }
            _windowVisible = false;
        }

        public static void ShowwithAnimation(this Window window)
        {
            window.Visibility = Visibility.Visible;
            window.Topmost = false;
            window.Activate();

            var showAnimation = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(0.3)),
                FillBehavior = FillBehavior.Stop,
                EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
            };
            var showAnimationOpacity = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(0.1)),
                FillBehavior = FillBehavior.Stop,
                EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
            };

            var taskbarPosition = TaskbarService.TaskbarPosition;
            switch (taskbarPosition)
            {
                case TaskbarPosition.Left:
                case TaskbarPosition.Right:
                    showAnimation.To = window.Left;
                    break;
                default:
                    showAnimation.To = window.Top;
                    break;
            }
            showAnimation.From = (taskbarPosition == TaskbarPosition.Top || taskbarPosition == TaskbarPosition.Left) ? showAnimation.To - 25 : showAnimation.To + 25;
            showAnimation.Completed += (s, e) =>
            {
                window.Topmost = true;
                window.Focus();
            };

            showAnimationOpacity.From = 0;
            showAnimationOpacity.To = 1;

            switch (taskbarPosition)
            {
                case TaskbarPosition.Left:
                case TaskbarPosition.Right:
                    window.ApplyAnimationClock(Window.LeftProperty, showAnimation.CreateClock());
                    break;
                default:
                    window.ApplyAnimationClock(Window.TopProperty, showAnimation.CreateClock());
                    break;
            }

            window.ApplyAnimationClock(Window.OpacityProperty, showAnimationOpacity.CreateClock());
            _windowVisible = true;
        }

        public static Matrix CalculateDpiFactors(this Window window)
        {
            var mainWindowPresentationSource = PresentationSource.FromVisual(window);
            return mainWindowPresentationSource == null ? new Matrix() { M11 = 1, M22 = 1 } : mainWindowPresentationSource.CompositionTarget.TransformToDevice;
        }

        public static double DpiHeightFactor(this Window window)
        {
            var m = CalculateDpiFactors(window);
            return m.M22;
        }

        public static double DpiWidthFactor(this Window window)
        {
            var m = CalculateDpiFactors(window);
            return m.M11;
        }
    }
}
