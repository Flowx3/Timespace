using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.ComponentModel;

public class AnimatedPanel : Panel
{
    private float currentHeight = 0;
    private float targetHeight = 0;
    private System.Windows.Forms.Timer animationTimer;
    private const float ANIMATION_SPEED = 15;

    public AnimatedPanel()
    {
        this.DoubleBuffered = true;
        this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer, true);

        animationTimer = new System.Windows.Forms.Timer { Interval = 16 }; // ~60fps
        animationTimer.Tick += AnimationTimer_Tick;
    }

    public void ShowAnimated()
    {
        targetHeight = this.Height;
        this.Height = 0;
        currentHeight = 0;
        this.Visible = true;
        animationTimer.Start();
    }

    public void HideAnimated(Action onComplete = null)
    {
        targetHeight = 0;
        animationTimer.Start();
    }

    private void AnimationTimer_Tick(object sender, EventArgs e)
    {
        if (targetHeight > currentHeight)
        {
            currentHeight = Math.Min(currentHeight + ANIMATION_SPEED, targetHeight);
            this.Height = (int)currentHeight;
        }
        else if (targetHeight < currentHeight)
        {
            currentHeight = Math.Max(currentHeight - ANIMATION_SPEED, targetHeight);
            this.Height = (int)currentHeight;
        }

        if (currentHeight == targetHeight)
        {
            animationTimer.Stop();
            if (targetHeight == 0)
                this.Visible = false;
        }

        this.Invalidate();
    }
}