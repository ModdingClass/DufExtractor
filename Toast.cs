using System;
using System.Drawing;
using System.Windows.Forms;

internal enum ToastKind
{
    Success,
    Info,
    Warning,
    Error
}

internal static class Toast
{
    public static void Show(string title, string message, ToastKind kind)
    {
        using var form = new ToastForm(title, message, kind);
        Application.Run(form);
    }

    private sealed class ToastForm : Form
    {
        private const int DisplayMs = 2600;
        private const int FadeStepMs = 20;
        private const double FadeStep = 0.08;

        private readonly System.Windows.Forms.Timer _displayTimer = new();
        private readonly System.Windows.Forms.Timer _fadeTimer = new();

        public ToastForm(string title, string message, ToastKind kind)
        {
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;
            TopMost = true;
            BackColor = Color.FromArgb(30, 30, 34);
            Width = 360;
            Height = 88;
            Opacity = 0;

            var accent = new Panel
            {
                Dock = DockStyle.Left,
                Width = 6,
                BackColor = AccentColor(kind)
            };

            var titleLabel = new Label
            {
                Text = title,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                AutoSize = false,
                Location = new Point(22, 14),
                Size = new Size(Width - 40, 22),
                BackColor = Color.Transparent
            };

            var messageLabel = new Label
            {
                Text = message,
                ForeColor = Color.FromArgb(225, 225, 225),
                Font = new Font("Segoe UI", 9.5f),
                AutoSize = false,
                Location = new Point(22, 38),
                Size = new Size(Width - 40, 40),
                BackColor = Color.Transparent
            };

            Controls.Add(messageLabel);
            Controls.Add(titleLabel);
            Controls.Add(accent);

            var workArea = Screen.PrimaryScreen!.WorkingArea;
            Location = new Point(workArea.Right - Width - 16, workArea.Bottom - Height - 16);

            Click += (_, _) => BeginFade();
            titleLabel.Click += (_, _) => BeginFade();
            messageLabel.Click += (_, _) => BeginFade();

            Shown += (_, _) =>
            {
                _displayTimer.Interval = DisplayMs;
                _displayTimer.Tick += (_, _) =>
                {
                    _displayTimer.Stop();
                    BeginFade();
                };
                _displayTimer.Start();
                FadeIn();
            };
        }

        private void FadeIn()
        {
            var fadeInTimer = new System.Windows.Forms.Timer { Interval = FadeStepMs };
            fadeInTimer.Tick += (_, _) =>
            {
                if (Opacity >= 0.95)
                {
                    Opacity = 0.95;
                    fadeInTimer.Stop();
                    fadeInTimer.Dispose();
                    return;
                }
                Opacity += FadeStep;
            };
            fadeInTimer.Start();
        }

        private void BeginFade()
        {
            _fadeTimer.Interval = FadeStepMs;
            _fadeTimer.Tick += (_, _) =>
            {
                if (Opacity <= FadeStep)
                {
                    _fadeTimer.Stop();
                    Close();
                    return;
                }
                Opacity -= FadeStep;
            };
            _fadeTimer.Start();
        }

        private static Color AccentColor(ToastKind kind) => kind switch
        {
            ToastKind.Success => Color.FromArgb(120, 200, 120),
            ToastKind.Info => Color.FromArgb(90, 160, 220),
            ToastKind.Warning => Color.FromArgb(230, 180, 70),
            ToastKind.Error => Color.FromArgb(220, 90, 90),
            _ => Color.Gray
        };
    }
}
