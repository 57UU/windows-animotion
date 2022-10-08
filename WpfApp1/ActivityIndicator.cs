using System;
using System.Collections.Generic;
using System.Linq;

using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace WpfApp1;

/// <summary>
/// Page1.xaml 的交互逻辑
/// </summary>
public partial class ActivityIndicator 
{
    //可调参数

    /// <summary>
    /// 小球的直径
    /// </summary>
    public int r = 5;
    /// <summary>
    /// 每一次计算间隔的时长
    /// </summary>
    public int Interval;

    /// <summary>
    /// 虚拟大球旋转的速度
    /// </summary>
    public float speed { get; set; } = 0.015f;

    /// <summary>
    /// 坐标系的伸缩变换
    /// </summary>
    public float scale = 150;
    /// <summary>
    /// 是否启用DEBUG
    /// </summary>
    public bool isEnableDebug = false;


    public ActivityIndicator(Brush color,int freshRate = 160, int point = 12)
    {
        Brush = color;
        InitializeComponent();
        this.MinWidth = 800;
        this.MinHeight = 20;



        this.pointNumber = point;
        Interval = 1000 / freshRate;
        degree = Pi2 / pointNumber;
        values = new float[pointNumber];
        ellipses = new Ellipse[pointNumber];
        for(int i = 0; i < pointNumber; i++)
        {
            Ellipse e = new();
            e.Fill = Brushes.DarkCyan;
            e.Width = e.Height = r;
            ellipses[i] = e;
            layout.Children.Add(e);
        }
        halfBallHeight = r / 2;


        if (isEnableDebug)
        {
            //坐标轴
            drawXY();

            actualBalls = new Ellipse[pointNumber];
            for (int i = 0; i < pointNumber; i++)
            {
                Ellipse e = new();
                e.Fill = Brushes.Black;
                e.Width = e.Height = r;
                actualBalls[i] = e;
                layout.Children.Add(e);
            }
            actualBallsValus=new float[pointNumber*2];

            actualBallsLine = new Line[pointNumber];
            for (int i = 0; i < pointNumber; i++)
            {
                Line e = new();
                layout.Children.Add(e);
                e.Stroke = Brushes.Red;
                e.StrokeThickness = 3;
                this.SizeChanged += adjustLocation;
                void adjustLocation(object s, EventArgs ea)
                {
                    e.X1 = this.ActualWidth / 2;
                    e.Y1 = this.ActualHeight / 2 + scale;
                }


                actualBallsLine[i] = e;
                
                adjustLocation(null,null);
            }
        }







        Running();
    }
    private float[] actualBallsValus;
    private Ellipse[] actualBalls ;
    private Line[] actualBallsLine;

    private void drawXY()
    {
        Line x = new();
        Line y = new();

        y.Stroke = x.Stroke = Brushes.Black;
        x.StrokeThickness=y.StrokeThickness = 1;

        layout.Children.Add(x);
        layout.Children.Add(y);
        this.SizeChanged += adjustLocation;
        void adjustLocation(object sender,EventArgs args)
        {
            x.X1 = 0;
            x.X2 = this.ActualWidth;
            x.Y1 = x.Y2 = this.ActualHeight / 2;
            y.X1 = y.X2 = this.ActualWidth / 2;
            y.Y1 = this.ActualHeight;
            y.Y2 = 0;
        }
        adjustLocation(null,null);
        

    }



    /// <summary>
    /// 小球颜色
    /// </summary>
    public Brush Brush { get; set; }

    //小球个数,Ps:小球坐标:即虚拟大球上的点到(0,-1)连线与x轴交点
    private int pointNumber;
    /// <summary>
    /// array to store the X data
    /// </summary>
    private float[] values;
    /// <summary>
    /// the degree between each point
    /// </summary>
    private float degree;
    /// <summary>
    /// 相位
    /// </summary>
    private float φ0 = 0;
    /// <summary>
    /// 计算位置，保存在values
    /// </summary>
    private void calculatePoint()
    {
        
        float θ = φ0;
        for (int i = 0; i < pointNumber; i++, θ += degree)
        {
            //从(0,-1)到点(用θ表示)的连线与X轴交点
            //float v = (float)Math.Sin(θ) / ((float)Math.Cos(θ) + 1);
            float v = (float)MathF.Sin(θ) / (5*(float)MathF.Cos(θ) + 5);
            //convert: scale
            v *= scale;
 
            values[i] = v;
            if (isEnableDebug)
            {
                actualBallsValus[i*2] = MathF.Sin(θ) * scale;
                actualBallsValus[i * 2+1] = MathF.Cos(θ) * scale;

                if (v < this.ActualWidth*10 && v > -(this.ActualWidth*10))
                {
                    actualBallsLine[i].X2 = actualBallsValus[i * 2 + 1]  + this.ActualWidth / 2;
                    actualBallsLine[i].Y2 = actualBallsValus[i * 2] + this.ActualHeight / 2;
                }
            }
        }
    }
    /// <summary>
    /// the ball on  the screen
    /// </summary>
    private Ellipse[] ellipses;
    private float halfBallHeight;
    private void putElement(in Shape control, in float x)
    {
            
            putElement(control, x, 0);

    }
    private void putElement(in Shape control, in float x,in float y)
    {
        var halfWidth= (float)(layout.ActualWidth / 2);
        if (-halfWidth - 100 < x && x < halfWidth + 100)//argument is valid
        {
            Canvas.SetLeft(control, x+ halfWidth - halfBallHeight);
            float Height = (float)(y - halfBallHeight+ (layout.ActualHeight / 2));
            Canvas.SetBottom(control, Height);
        }
    }
    private void show()
    {

        for(int i = 0; i < pointNumber; i++)
        {
            var e = ellipses[i];
            putElement(e,values[i]);
        }
        if (isEnableDebug)
        {
            for (int i = 0; i < pointNumber; i++)
            {
                var e = actualBalls[i];
                putElement(e, actualBallsValus[i * 2], actualBallsValus[i*2+1]);
            }
        }
    }
    /// <summary>
    /// 开始计算
    /// </summary>
    /// <returns></returns>
    
    private async Task Running()
    {
        while (true)
        {
            await Task.Delay(Interval);
            //先计算结果
            calculatePoint();
            //再显示结果
            show();
            //相位递增，以表示大球旋转
            φ0 += speed;
            if (φ0 > Pi2)
            {
                φ0 -= Pi2;
            }
        }
    }

    float Pi2 = (float)(2 * Math.PI);
}
